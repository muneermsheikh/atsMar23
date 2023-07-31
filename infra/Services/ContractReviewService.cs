using AutoMapper;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using core.Dtos;
using core.Specifications;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class ContractReviewService : IContractReviewService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly ATSContext _context;
          private readonly IMapper _mapper;
          private readonly IComposeMessagesForHR _composeMsg;
          private readonly int _OperationsManagementId;
          private readonly IEmailService _emailService;
          private readonly IComposeMessagesForAdmin _msgForAdmin;
          public ContractReviewService(IUnitOfWork unitOfWork, ATSContext context, IMapper mapper, IComposeMessagesForAdmin msgForAdmin,
               IComposeMessagesForHR composeMsg, IConfiguration config, IEmailService emailService)
          {
               _msgForAdmin = msgForAdmin;
               _emailService = emailService;
               _OperationsManagementId = Convert.ToInt32(config.GetSection("OperationsManagementId").Value);
               _composeMsg = composeMsg;
               _mapper = mapper;
               _context = context;
               _unitOfWork = unitOfWork;
          }

          public void AddReviewItemStatus(string reviewItemStatusName)
          {
               var status = new ReviewItemStatus(reviewItemStatusName);
               _unitOfWork.Repository<ReviewItemStatus>().Add(status);
          }

          public void AddReviewStatus(string reviewStatusName)
          {
               var status = new ReviewStatus { Status = reviewStatusName };
               _unitOfWork.Repository<ReviewStatus>().Add(status);
          }

          public async Task<EmailMessageDto> EditContractReview(ContractReview model)
          {
               // thanks to @slauma of stackoverflow
               var existingObj = await _context.ContractReviews
               .Where(p => p.Id == model.Id)
               .Include(x => x.ContractReviewItems).ThenInclude(x => x.ReviewItems)
               //.AsNoTracking()
               .SingleOrDefaultAsync();

               if (existingObj == null) throw new Exception("The Contract Review model does not exist in the database");
               if (existingObj.ContractReviewItems == null) throw new Exception("The Contract Review Items collection does not exist in the database");
               if (existingObj.ContractReviewItems.Any(x => x.ReviewItems == null)) throw new Exception("Review Parameters in one of the items do not exist");

               _context.Entry(existingObj).CurrentValues.SetValues(model);   //saves only the parent, not children

               //Delete children that exist in existing record, but not in the new model order
               foreach (var existingItem in existingObj.ContractReviewItems.ToList())
               {
                    if (!model.ContractReviewItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.ContractReviewItems.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               //children that are not deleted, are either updated or new ones to be added
               foreach (var itemModel in model.ContractReviewItems)
               {
                    //work on the contractReviewItem
                    var existingItem = existingObj.ContractReviewItems.Where(c => c.Id == itemModel.Id && c.Id != default(int)).SingleOrDefault();
                    if (existingItem != null)       // record exists, update it
                    {
                         _context.Entry(existingItem).CurrentValues.SetValues(itemModel);
                         _context.Entry(existingItem).State = EntityState.Modified;
                    }
                    else            //record does not exist, insert a new record
                    {
                         var newItem = new ContractReviewItem(itemModel.OrderItemId, itemModel.OrderId, itemModel.CategoryName, itemModel.Quantity);
                         existingObj.ContractReviewItems.Add(newItem);
                         _context.Entry(newItem).State = EntityState.Added;
                    }

                    //work on ContractReviewItem.ReviewItems
                    //check if records to be deleted - if exists in DB but not in model
                    foreach (var existingRvwItem in existingItem.ReviewItems.ToList())
                    {
                         if (!itemModel.ReviewItems.Any(c => c.Id == existingRvwItem.Id && c.Id != default(int)))
                         {
                              _context.ReviewItems.Remove(existingRvwItem);
                              _context.Entry(existingRvwItem).State = EntityState.Deleted;
                         }
                    }

                    foreach (var reviewItemModel in itemModel.ReviewItems.ToList())
                    {
                         var existingRvwItem = existingItem.ReviewItems.Where(c => c.Id == reviewItemModel.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingRvwItem != null)
                         {     //record exists
                              _context.Entry(existingRvwItem).CurrentValues.SetValues(reviewItemModel);
                              _context.Entry(existingRvwItem).State = EntityState.Modified;
                         }
                         else
                         {            // add new
                              var newRvw = new ReviewItem
                              {
                                   SrNo = reviewItemModel.SrNo,
                                   ReviewParameter = reviewItemModel.ReviewParameter,
                                   Response = reviewItemModel.Response,
                                   IsMandatoryTrue = reviewItemModel.IsMandatoryTrue,
                                   Remarks = reviewItemModel.Remarks
                              };
                              existingItem.ReviewItems.Add(newRvw);
                              _context.Entry(newRvw).State = EntityState.Added;
                         }
                    }
               }

               //update ContractReview.ReviewStatus
               if (existingObj.ContractReviewItems.Any(x => x.ReviewItemStatus == (int)EnumReviewItemStatus.NotReviewed))
               {
                    existingObj.RvwStatusId = (int)EnumReviewStatus.NotReviewed;
               }
               else if (existingObj.ContractReviewItems.Any(x => x.ReviewItemStatus != (int)EnumReviewItemStatus.Accepted)
               && existingObj.ContractReviewItems.Any(x => x.ReviewItemStatus == (int)EnumReviewItemStatus.Accepted))
               {   //atleast one categry accepted and atleast 1 category not accepted
                    existingObj.RvwStatusId = (int)EnumReviewStatus.AcceptedWithSomeRegrets;
               }
               else {
                    existingObj.RvwStatusId = (int)EnumReviewStatus.Accepted;
               }

               _context.Entry(existingObj).State = EntityState.Modified;

               if (await _context.SaveChangesAsync() > 0)
               {
                    //if reviewed and accepted, forward requirement to HR Dept
                    if ((existingObj.RvwStatusId == (int)EnumReviewStatus.Accepted ||
                         existingObj.RvwStatusId == (int)EnumReviewStatus.AcceptedWithSomeRegrets) &&
                         existingObj.ReleasedForProduction)
                    {
                         //FORWARD REQUIREMENT TO HR DEPT
                         var order = await _context.Orders.Where(x => x.Id == existingObj.OrderId)
                              .Include(x => x.OrderItems).FirstOrDefaultAsync();
                         var emailMsg = await _msgForAdmin.ForwardEnquiryToHRDept(order);

                         order.ForwardedToHRDeptOn = DateTime.Now;
                         _unitOfWork.Repository<Order>().Update(order);
                         await _unitOfWork.Complete();

                         //create task in the name of the project manager
                         var newTask = new ApplicationTask((int)EnumTaskType.OrderAssignmentToProjectManager,
                              existingObj.ReviewedOn, _OperationsManagementId, order.ProjectManagerId, order.Id,
                              order.OrderNo,  0, "You are assigned to be totally responsible to execute Order No. " + 
                              order.OrderNo + " dt " + order.OrderDate.Date + 
                              ", task for the same is created in your name.  Check your Task Dashboard for details.",
                              ((DateTime)order.ForwardedToHRDeptOn).Date.AddDays(8), "Not Started",0, null);
                         
                         return new EmailMessageDto { EmailMessage = emailMsg, ErrorMessage = "" };
                    }
                    else
                    {
                         return new EmailMessageDto { EmailMessage = null, ErrorMessage = "" };
                    }
               }
               throw new Exception("Failed to save the changes");
          }


          public async Task<ContractReviewItemReturnValueDto> EditContractReviewItem(ContractReviewItemDto model, int loggedInEmployeeId)
          {
               // thanks to @slauma of stackoverflow
               var existingObj = await _context.ContractReviewItems
                    .Where(p => p.Id == model.Id)
                    .Include(x => x.ReviewItems)
                    .SingleOrDefaultAsync();

               if (existingObj == null || existingObj.ReviewItems == null) return null;

               _context.Entry(existingObj).CurrentValues.SetValues(model);   //saves only the parent, not children

               //Delete children that exist in existing record, but not in the new model order
               foreach (var existingItem in existingObj.ReviewItems.ToList())
               {
                    if (!model.ReviewItems.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                    {
                         _context.ReviewItems.Remove(existingItem);
                         _context.Entry(existingItem).State = EntityState.Deleted;
                    }
               }

               //children that are not deleted, are either updated or new ones to be added
               foreach (var itemModel in model.ReviewItems)
               {
                    //work on the contractReviewItem
                    var existingItem = existingObj.ReviewItems.Where(c => c.Id == itemModel.Id && c.Id != default(int)).SingleOrDefault();
                    if (existingItem != null)       // record exists, update it
                    {
                         _context.Entry(existingItem).CurrentValues.SetValues(itemModel);
                         _context.Entry(existingItem).State = EntityState.Modified;
                    }
                    else            //record does not exist, insert a new record
                    {
                         int srno = model.ReviewItems.Max(x => x.SrNo)+1;
                         var newItem = new ReviewItem {
                              OrderItemId=itemModel.OrderItemId, 
                              ContractReviewItemId= itemModel.ContractReviewItemId, 
                              SrNo = srno, ReviewParameter = itemModel.ReviewParameter,
                              Response = itemModel.Response, ResponseText = itemModel.ResponseText, 
                              IsResponseBoolean=itemModel.IsResponseBoolean, 
                              IsMandatoryTrue=itemModel.IsMandatoryTrue, 
                              Remarks=itemModel.Remarks
                         };
                         existingObj.ReviewItems.Add(newItem);
                         _context.Entry(newItem).State = EntityState.Added;
                    }
               }

               _context.Entry(existingObj).State = EntityState.Modified;

               //update orderitem.reviewitemstatusId                    
               var orderitem  = await _context.OrderItems.FindAsync(model.OrderItemId);
               orderitem.ReviewItemStatusId = model.ReviewItemStatus;
               _context.Entry(orderitem).State = EntityState.Modified;

               var updated = new ContractReviewItemReturnValueDto();
               updated.ReviewItemStatusId=(int)orderitem.ReviewItemStatusId;
               updated.ContractReviewStatusId=(int) EnumReviewStatus.NotReviewed;               
               if(await _context.SaveChangesAsync() > 0) {
                    //update order.contractReviewStatus
                    var order = await _context.Orders.Include(x => x.OrderItems).Where(x => x.Id == model.OrderId).FirstOrDefaultAsync();
                    updated.ContractReviewStatusId = await UpdateOrderReviewStatusBasedOnOrderItemReviewStatus(order, loggedInEmployeeId);
               }

               return updated;
          }


          public async Task<ContractReview> GetContractReviewDtoByOrderIdAsync(int orderId)
          {
               var crvw = await _context.ContractReviews.Where(x => x.OrderId == orderId)
                    .Include(x => x.ContractReviewItems)
                    .ThenInclude(x => x.ReviewItems)
                    .FirstOrDefaultAsync();
               return crvw;
          }

          public async Task<ICollection<ContractReviewItemDto>> GetContractReviewItemsWithOrderDetails(ContractReviewItemSpecParams cParams)
          {

               //var ReviewItemSpecParams = new ContractReviewItemSpecParams{OrderItemIds = (ICollection<int?>)lst };
               //var ReviewItemSpecParams = new ContractReviewItemSpecParams(cParams);
               var specs = new ContractReviewItemSpecs(cParams);
               var countSpecs = new ContractReviewItemForCountSpecs(cParams);

               var rvwItems = await _unitOfWork.Repository<ContractReviewItem>().ListAsync(specs);
               //var orderNo = await _context.Orders.Where(x => x.Id == orderItem.OrderId).Select(x => x.OrderNo).FirstOrDefaultAsync();

               var listDto = new List<ContractReviewItemDto>();
               int srno=1;
               foreach(var item in rvwItems) 
               {
                    listDto.Add(new ContractReviewItemDto(
                         item.Id, item.ContractReviewId, item.OrderId,
                         item.OrderItemId, srno++, item.CategoryName, item.SourceFrom, item.Quantity,
                         item.Ecnr, item.RequireAssess, item.ReviewItems, item.ReviewItemStatus
                    ));
               };

               return listDto;
          }

          public async Task<bool> DeleteContractReview(int orderid)
          {

               var contractReview = await _context.ContractReviews.Where(x => x.OrderId == orderid).FirstOrDefaultAsync();
               if (contractReview == null) throw new Exception("the object to delete does not exist");
               _unitOfWork.Repository<ContractReview>().Delete(contractReview);

               /*
               //contractReview is configured for contractREviewItems to be cascade deleted
               //contractReviewItem is configured for REviewItems to be cascade deleted

               var items = await _context.ContractReviewItems.Where(x => x.OrderId == orderid).ToListAsync();
               foreach (var item in items)
               {
                    _unitOfWork.Repository<ContractReviewItem>().Delete(item);
               }
               */
               if (await _unitOfWork.Complete() == 0) throw new Exception("failed to delete the object");
               return true;
          }

          public async Task<bool> DeleteContractReviewItem(int orderitemid)
          {
               //contractReviewItem is configured to cascade delete ReviewItems
               var item = await _context.ContractReviewItems.Where(x => x.OrderItemId == orderitemid).FirstOrDefaultAsync();
               if (item == null) throw new Exception("the object to delete does not exist");
               _unitOfWork.Repository<ContractReviewItem>().Delete(item);
               if (await _unitOfWork.Complete() == 0) throw new Exception("failed to delete the object");
               return true;
          }

          public async Task<bool> DeleteReviewReviewItem(int id)
          {
               var reviewItem = await _context.ReviewItems.FindAsync(id);
               if (reviewItem == null) throw new Exception("the object to delete does not exist");
               _unitOfWork.Repository<ReviewItem>().Delete(reviewItem);
               if (await _unitOfWork.Complete() == 0) throw new Exception("Failed to delete the review item object");
               return true;
          }

          public async Task<ICollection<ReviewStatus>> GetReviewStatus()
          {
               return await _context.ReviewStatuses.ToListAsync();
          }

          public async Task<ICollection<ReviewItemStatus>> GetReviewItemStatus()
          {
               //return await _unitOfWork.Repository<ReviewItemStatus>().ListAllAsync();
               return await _context.ReviewItemStatuses.OrderBy(x => x.ItemStatus).ToListAsync();
          }

          public async Task<ContractReview> CreateContractReviewObject(int orderId, string LoggedInAppUserId)
          {
               //check if Remuneration for all the order items exist
               var itemIds = await _context.OrderItems
                    .Where(x => x.OrderId == orderId)
                    .Select(x => x.Id)
                    .ToListAsync();
               if (itemIds.Count() == 0) throw new Exception("Order Items not created");
               var orderitems = await _context.OrderItems.Where(x => itemIds.Contains(x.Id))
                    .Include(x => x.Remuneration)
                    .ToListAsync();
               if (orderitems == null || orderitems.Count() == 0) throw new Exception("Remunerations need to be defined for all the items before the contract review");

               //check if the object exists
               var contractReview = await _context.ContractReviews.Where(x => x.OrderId == orderId).Include(x => x.ContractReviewItems).FirstOrDefaultAsync();
               if (contractReview != null) throw new System.Exception("Contract Review Object already exists");

               int loggedInEmployeeId = await _context.Employees.Where(x => x.AppUserId == LoggedInAppUserId).Select(x => x.Id).FirstOrDefaultAsync();

               var order = await _context.Orders.Where(x => x.Id == orderId)
                    .Include(x => x.OrderItems).FirstOrDefaultAsync();
               var contractReviewItems = new List<ContractReviewItem>();
               var reviewData = await _context.ReviewItemDatas.OrderBy(x => x.SrNo).ToListAsync();

               foreach (var item in order.OrderItems)
               {
                    if (string.IsNullOrEmpty(item.CategoryName)) item.CategoryName = await _context.Categories.Where(x => x.Id == item.CategoryId).Select(x => x.Name).FirstOrDefaultAsync();

                    var itemData = new List<ReviewItem>();
                    foreach (var data in reviewData)
                    {
                         itemData.Add(new ReviewItem
                         {
                              SrNo = data.SrNo,
                              OrderItemId = item.Id,
                              ReviewParameter = data.ReviewParameter,
                              IsMandatoryTrue = data.IsMandatoryTrue
                         });
                    }
                    contractReviewItems.Add(new ContractReviewItem
                    {
                         OrderItemId = item.Id,
                         OrderId = item.OrderId,
                         Quantity = item.Quantity,
                         CategoryName = item.CategoryName,
                         Ecnr = item.Ecnr,
                         RequireAssess = item.RequireAssess,
                         SourceFrom = item.SourceFrom,
                         ReviewItems = itemData
                    });
               }

               if (string.IsNullOrEmpty(order.CustomerName)) order.CustomerName = await _context.Customers.Where(x => x.Id == order.CustomerId).Select(x => x.CustomerName).FirstOrDefaultAsync();
               contractReview = new ContractReview
               {
                    OrderNo = order.OrderNo,
                    OrderId = orderId,
                    OrderDate = order.OrderDate.Date,
                    CustomerId = order.CustomerId,
                    CustomerName = order.CustomerName,
                    ReviewedBy = loggedInEmployeeId,
                    ReviewedOn = System.DateTime.Now,
                    ContractReviewItems = contractReviewItems
               };

               _unitOfWork.Repository<ContractReview>().Add(contractReview);

               if (await _unitOfWork.Complete() > 0) return contractReview;

               throw new Exception("failed to create the Contract Review object");
          }

          public  ICollection<int> ConvertCSVToAray(string csv) {
               bool isParsingOk = true;
               int[] results = Array.ConvertAll<string,int>(csv.Split(','), 
               new Converter<string,int>(
               delegate(string num)
               {
                    int r;
                    isParsingOk &= int.TryParse(num, out r);
                    return r;
               }));
               return results;

           }
          public async Task<Pagination<ContractReview>> GetContractReviews(ContractReviewSpecParams cParams)
          {
               if (!string.IsNullOrEmpty(cParams.OrderIds)) {
                    cParams.OrderIdInts = ConvertCSVToAray(cParams.OrderIds);
               }

               //check order Ids to ascertain values are valid; 
               //if not present records in contractReviews and contractReviewItems, then insert
               var orderIds = await _context.Orders.Where(x => cParams.OrderIdInts.Contains(x.Id)).Include(x => x.OrderItems).ToListAsync();
               foreach(var order in orderIds) {
                    var crvw = await _context.ContractReviews.Where(x => x.OrderId == order.Id).FirstOrDefaultAsync();
                    var rvwItems = new List<ContractReviewItem>();
                         
                         foreach(var item in order.OrderItems) {
                              var rvwItem = await _context.ContractReviewItems.Where(x => x.OrderItemId == item.Id).FirstOrDefaultAsync();
                              if (rvwItem == null) {
                                   rvwItem = new ContractReviewItem(item.Id, item.OrderId, item.CategoryName, item.Quantity);
                                   rvwItems.Add(rvwItem);
                                   //_unitOfWork.Repository<ContractReviewItem>().Add(rvwItem);
                              }
                         }
                    if (crvw == null) {
                         crvw = new ContractReview(order.Id, order.OrderNo, order.OrderDate, order.CustomerId, order.CustomerName, rvwItems);
                         _unitOfWork.Repository<ContractReview>().Add(crvw);
                    } else if (rvwItems.Count() > 0) {
                         crvw.ContractReviewItems = rvwItems;
                         _unitOfWork.Repository<ContractReview>().Update(crvw);
                    }
                         
               }
               await _unitOfWork.Complete();

               var spec = new ContractReviewSpecs(cParams);
               var countSpec = new ContractReviewForCountSpecs(cParams);
               var totalItems = await _unitOfWork.Repository<ContractReview>().CountAsync(countSpec);
               var data = await _unitOfWork.Repository<ContractReview>().ListAsync(spec);

               return new Pagination<ContractReview>(cParams.PageIndex, cParams.PageSize, totalItems, data);

          }
     
          public async Task<ContractReview> GetOrAddContractReview(int id, int loggedInEmployeeId)
          {
               var employee = await _context.Employees.FindAsync(loggedInEmployeeId);
               var appuserid = employee.AppUserId;
               //check order Ids to ascertain values are valid; 
               //if not present records in contractReviews and contractReviewItems, then insert
               var rvw = await _context.ContractReviews
                    .Include(x => x.ContractReviewItems)
                    .ThenInclude(x => x.ReviewItems)
                    .FirstOrDefaultAsync();
               
               if(rvw==null) {
                    rvw = await CreateContractReviewObject(id, appuserid);
                    if (rvw ==null) return null;
                    return rvw;
               }
               
               var reviewQs = await _context.ReviewItemDatas.OrderBy(x => x.SrNo).ToListAsync();
               if (rvw.ContractReviewItems == null || rvw.ContractReviewItems.Count == 0) {
                    var orderitems = await _context.OrderItems.Where(x => x.OrderId == id).ToListAsync();
                    int srNo=0;
                    var reviewitems = new List<ReviewItem>();
                    foreach(var item in orderitems)
                    {    
                         foreach(var q in reviewQs) {
                              var itemQ = new ReviewItem(item.Id, srNo++,q.ReviewParameter, q.Response,q.IsResponseBoolean, q.IsMandatoryTrue);
                              reviewitems.Add(itemQ);
                              _context.Entry(itemQ).State = EntityState.Added;
                         }
                         var items = new ContractReviewItem(item.Id, item.OrderId, item.CategoryName, item.Quantity, reviewitems);
                         _context.Entry(items).State = EntityState.Added;
                    }
               } else{
                    int srNo=0;
                    foreach(var item in rvw.ContractReviewItems) {
                         if (item.ReviewItems == null || item.ReviewItems.Count == 0) {
                              var reviewitems = new List<ReviewItem>();
                              foreach(var q in reviewQs) {
                                   var reviewitem = new ReviewItem(item.Id, srNo++,q.ReviewParameter, q.Response,q.IsResponseBoolean, q.IsMandatoryTrue);
                                   item.ReviewItems.Add(reviewitem);
                                   _context.Entry(reviewitem).State = EntityState.Added;
                              }
                         }
                    }
               }

               _context.Entry(rvw).State = EntityState.Modified;
               
               if (await _context.SaveChangesAsync() > 0) return rvw;

               return null;

          }
     
          private async Task<List<ReviewItem>> GetReviewItemData(int orderitemid) 
          {
               var results = new List<ReviewItem>();
               var rvwdata = await _context.ReviewItemDatas.OrderBy(x => x.SrNo).ToListAsync();
               foreach(var data in rvwdata) {
                    results.Add (new ReviewItem{
                         OrderItemId = orderitemid, SrNo = data.SrNo, ReviewParameter = data.ReviewParameter, 
                         Response = false, IsMandatoryTrue = data.IsMandatoryTrue, IsResponseBoolean = data.IsResponseBoolean});
               }
               return results;
          }
          public async Task<ContractReviewItemDto> GetOrAddReviewResults(int orderitemid)
          {
               var results = new List<ReviewItem>();
               
               //check if contractReview exists
               var contractReviewItem = await _context.ContractReviewItems.Where(x => x.OrderItemId == orderitemid)
                    .Include(x => x.ReviewItems).FirstOrDefaultAsync();
               var orderitemObj = await (from i in _context.OrderItems where(i.Id == orderitemid)
                         join c in _context.Categories on i.CategoryId equals c.Id 
                         join o in _context.Orders on i.OrderId equals o.Id 
                         join cust in _context.Customers on o.CustomerId equals cust.Id
                         select new {CustomerName=cust.CustomerName, CategoryName=c.Name, OrderId=i.OrderId,
                              OrderNo = o.OrderNo, OrderDate = o.OrderDate, Quantity = i.Quantity, CustomerId = o.CustomerId }
                         ).FirstOrDefaultAsync();

               if (contractReviewItem == null) {
                    results = await GetReviewItemData(orderitemid);
                    contractReviewItem = new ContractReviewItem{
                         OrderItemId = orderitemid, OrderId = orderitemObj.OrderId, CustomerName=orderitemObj.CustomerName,
                         CategoryName = orderitemObj.CategoryName, Quantity = orderitemObj.Quantity, ReviewItems=results};

                    var creview = await _context.ContractReviews.Where(x => x.OrderId == orderitemObj.OrderId).FirstOrDefaultAsync();
                    var cReviewItems =new List<ContractReviewItem>();
                    cReviewItems.Add(contractReviewItem);

                    if (creview == null) {
                         creview = new ContractReview(orderitemObj.OrderId, orderitemObj.OrderNo, orderitemObj.OrderDate,
                              orderitemObj.CustomerId, orderitemObj.CustomerName, cReviewItems );
                         _unitOfWork.Repository<ContractReview>().Add(creview);
                         await _unitOfWork.Complete();
                    } else {
                         contractReviewItem.ContractReviewId = creview.Id;
                         _unitOfWork.Repository<ContractReviewItem>().Add(contractReviewItem);
                         await _unitOfWork.Complete();
                    }
               } else if (contractReviewItem.ReviewItems==null || contractReviewItem.ReviewItems.Count ==0) {
                    contractReviewItem.ReviewItems = await GetReviewItemData(orderitemid);
                    _unitOfWork.Repository<ContractReviewItem>().Update(contractReviewItem);
               }
               
               var dto = _mapper.Map<ContractReviewItem, ContractReviewItemDto>(contractReviewItem);
               if (dto != null) {
                    if(string.IsNullOrEmpty(contractReviewItem?.CategoryName) 
                         || string.IsNullOrEmpty(contractReviewItem?.CustomerName)
                         || contractReviewItem.OrderId == 0
                         || contractReviewItem.OrderNo == 0
                         || contractReviewItem.OrderDate.Year < 2000 ) 
                    {
                         if(string.IsNullOrEmpty(dto.CategoryName)) dto.CategoryName=orderitemObj.CategoryName;
                         if (string.IsNullOrEmpty(dto.CustomerName)) dto.CustomerName=orderitemObj.CustomerName;
                         if (dto.OrderId == 0) dto.OrderId=orderitemObj.OrderId;
                         if (dto.OrderDate.Year < 2000) dto.OrderDate=orderitemObj.OrderDate;
                         if (dto.OrderNo==0) dto.OrderNo=orderitemObj.OrderNo;
                    }
               }
               return dto;
          }

          public async Task<ICollection<ReviewItemData>> GetReviewData()
          {
               var data = await _context.ReviewItemDatas.OrderBy(x => x.SrNo).ToListAsync();
               return data;
          }

		//based on status of ALL orderItem.ReviewItemStatusId, update OrderReviewStatus field
          public async Task<int> UpdateOrderReviewStatusBasedOnOrderItemReviewStatus(Order order, int loggedInEmployeeId)
		{
               
               int orderReviewStatusIdUpdated=(int)EnumReviewStatus.NotReviewed;       //default
               
               //if any item is not reviewed, return false
               var orderitems = order.OrderItems.ToList();
               int orderReviewStatus=0;
               var orderitemids = orderitems.Select(x => x.Id).ToList();

               var reviewitems = await _context.ContractReviewItems
                    .Where(x => orderitemids.Contains(x.OrderItemId))
                    .Include(x => x.ReviewItems.Where(x => x.ReviewParameter=="Service Charges in INR"))
                    .ToListAsync();
               
               if(reviewitems == null || reviewitems.Count == 0 || orderitems.Count > reviewitems.Count) {
                    if(order.Status != "Awaiting Review") {
                         order.Status="Awaiting Review";
                         await UpdateOrderReviewStatusAndContractReview(1, order, loggedInEmployeeId);
                         return 0;
                    }
               }

              /* 
               //copy reviewItem.Charges to orderitem.
               foreach(var item in orderitems) {
                    var reviewitem = reviewitems .Where(x => x.OrderItemId==item.Id).FirstOrDefault();
                    if(reviewitem==null) {
                         order.Status="Awaiting Review";
                         await UpdateOrderReviewStatusAndContractReview(1, order, loggedInEmployeeId);
                         return 0;
                    }
                    if(reviewitem.ReviewItemStatus==(int)EnumReviewItemStatus.Accepted) {
                         var serviceCharge = reviewitem.
                    }
                    var chargeRecord = reviewitem.ReviewItems.Select(x => x.ResponseText).FirstOrDefault();
                    if(chargeRecord==null) return 0;
                    int charges = Convert.ToInt32(chargeRecord);
                    if(item.ReviewItemStatusId != reviewitem.ReviewItemStatus || item.Charges != charges) {
                         item.ReviewItemStatusId = reviewitem.ReviewItemStatus;
                         item.Charges = charges;
                    }

               }
               */

               if(!orderitems.Any(x => x.ReviewItemStatusId==7)) {     //atleast 1 item is regretted
                    if(orderitems.Any(x => x.ReviewItemStatusId==7)) {
                         //atleast 1 item is approed, and 1 items is regretted
                         orderReviewStatus=3;          //accepted with regrets
                    } else {
                         orderReviewStatus=2;
                    }
               } else {  //all items accepted
                    orderReviewStatus=4;     //accepte
               }

               //update order, contract review
               
               orderReviewStatusIdUpdated = await UpdateOrderReviewStatusAndContractReview(orderReviewStatus, order, loggedInEmployeeId);
               //this is saved by calling program
               return orderReviewStatusIdUpdated;
		}

          //UPDATES  Order -Status, ContractReviewStatusId, ReviewStatusId
          //UPDATES  ContractReview - ReviewedBy, ReviewedOn, ReviewStatusId (based on ContractReviewItems ReviewItemStatus values)
          //returns Order.ReviewStatusId
          private async Task<int> UpdateOrderReviewStatusAndContractReview(int reviewStatusId, Order order, int loggedInEmployeeId) {
               
               string currentStatus="";
               int ContractReviewStatusId=0;
               //update Order table

               //update Order.Status field - enumOrderReviewStatus
               switch(reviewStatusId) {
                    case 4:        //Accepted:
                         currentStatus="Accepted";
                         ContractReviewStatusId=4;
                         break;
                    case 2:        //AcceptedWithSomeRegrets:
                         currentStatus="Accepted With some regrets";  
                         ContractReviewStatusId=3;
                         break;
                    case 3:        //Regretted:
                         currentStatus="Regretted";         //Regretted;
                         ContractReviewStatusId=2;
                         break;
                    default:
                         currentStatus="Awaiting Review";
                         ContractReviewStatusId=1;
                         break;
               }
               order.ContractReviewStatusId=reviewStatusId;
               order.Status=currentStatus;
               order.ContractReviewStatusId=ContractReviewStatusId;

               _context.Entry(order).State=EntityState.Modified;

               //update ContractReview table
               var contractReview = await _context.ContractReviews.Where(x => x.OrderId==order.Id).FirstOrDefaultAsync();
               contractReview.ReviewedBy=loggedInEmployeeId;
               contractReview.ReviewedOn=DateTime.Now;
               contractReview.RvwStatusId=(int)reviewStatusId;

               _context.Entry(contractReview).State = EntityState.Modified;

               return reviewStatusId;
          }
	}
}