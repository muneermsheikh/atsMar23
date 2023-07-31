using System.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using core.Dtos;
using core.Entities;
using core.Entities.Admin;
using core.Entities.Identity;
using core.Interfaces;
using core.Specifications;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace infra.Services
{
     public class CustomerService : ICustomerService
     {
          private readonly IUnitOfWork _unitOfWork;
          private readonly IMapper _mapper;
		private readonly ILogger<CustomerService> _logger;
		private readonly UserManager<AppUser> _userManager;
          private readonly IUserService _userService;
          private readonly ATSContext _context;
          public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerService> logger,
               UserManager<AppUser> userManager, IUserService userService,
               ATSContext context)
          {
               _context = context;
               _userService = userService;
               _userManager = userManager;
               _mapper = mapper;
			_logger = logger;
			_unitOfWork = unitOfWork;
          }

          public async Task<ICollection<CustomerDto>> AddCustomers(ICollection<RegisterCustomerDto> dtos)
          {
               var customers = new List<Customer>();
               //using (var scope = new TransactionScope()) 
               //{
                    foreach(var dto in dtos)
                    {
                         var custIndustries = new List<CustomerIndustry>();
                         var custOfficials = new List<CustomerOfficial>();
                         var agencySpecialties = new List<AgencySpecialty>();

                         if (dto.CustomerIndustries != null && dto.CustomerIndustries.Count > 0) 
                         {
                              foreach (var ind in dto.CustomerIndustries)
                              {
                                   custIndustries.Add(new CustomerIndustry { IndustryId = ind.IndustryId });
                              }
                              custIndustries = custIndustries.Count() > 0 ? custIndustries : null;
                         }

                         if (dto.CustomerOfficials != null && dto.CustomerOfficials.Count >0)
                         {
                              //UserManager.CreateAsync never fails, so it will be added after customer official is added succesfully
                              //create identity users for each customer official
                              foreach (var off in dto.CustomerOfficials)
                              {
                                   custOfficials.Add(new CustomerOfficial(off.Gender, off.Title,
                                        off.OfficialName, off.Designation, off.PhoneNo, off.Mobile, 
                                        off.Email, off.ImageUrl, off.LogInCredential));
                              }
                              custOfficials = custOfficials.Count() > 0 ? custOfficials : null;
                         }
                         
                         if (dto.AgencySpecialties != null && dto.AgencySpecialties.Count > 0 )
                         {
                              if(dto.AgencySpecialties!=null && dto.AgencySpecialties.Count() > 0)
                              {
                                   foreach (var sp in dto.AgencySpecialties)
                                   {
                                        agencySpecialties.Add(new AgencySpecialty { IndustryId = sp.IndustryId, ProfessionId = sp.ProfessionId });
                                   }
                              }
                              agencySpecialties = agencySpecialties.Count() > 0 ? agencySpecialties : null;
                         }
                         
                         //add the customer
                         var customer = new Customer(dto.CustomerType, dto.CustomerName, dto.KnownAs, dto.Add,
                              dto.Add2, dto.City, dto.Pin, dto.District, dto.State, dto.Country, dto.Email,
                              dto.Website, dto.Phone, dto.Phone2, custIndustries, custOfficials, agencySpecialties);

                         _unitOfWork.Repository<Customer>().Add(customer);
                         customers.Add(customer);
                    }
                    
                    var result = await _unitOfWork.Complete();

                    //now create identity users for each customer official, and update customer official table
                    //this could have been done before adding the customer official, but if Usermanager.CreateAsync succeeds
                    //(which always succeeds) and customer official insert fails, then we are left with user identity without
                    //corresponding customer officials.  So next time the customer official is to be added, it will nto succeed
                    //because its email Id already exists in AppUser
                    if (result > 0) {
                         foreach(var customer in customers)
                         {
                              foreach(var off in customer.CustomerOfficials)
                              {
                                   if (!string.IsNullOrEmpty(off.Email) && !CheckEmailExistsAsync(off.Email).Result) {
                                        if (off.LogInCredential) 
                                        {
                                              var dtoCust = dtos.Where(x => x.CustomerName.ToLower() == customer.CustomerName.ToLower() && x.City.ToLower() == customer.City.ToLower()).FirstOrDefault();
                                             var dtoOff = dtoCust.CustomerOfficials.Where(x => 
                                                  x.OfficialName.ToLower()  
                                                  == off.OfficialName.ToLower())
                                                  .Select(y => new {y.KnownAs, y.Password})
                                                  .FirstOrDefault();
                                             
                                             var appuser = new AppUser
                                             {
                                                  UserType = "official",
                                                  Gender = off.Gender,
                                                  DisplayName = off.OfficialName,
                                                  Email = off.Email,
                                                  UserName = off.Email,
                                                  PhoneNumber = off.PhoneNo,
                                                  KnownAs = dtoOff.KnownAs,
                                                 /* Address = new Address{AddressType="O", Gender=off.Gender,
                                                            FirstName=dtoOff.FirstName,
                                                            Add = dtoOff.Add, StreetAdd=dtoOff.StreetAdd, 
                                                            City=dtoOff.City, Country=dtoOff.Country}
                                                  */
                                             };

                                             var added = await _userManager.CreateAsync(appuser,  dtoOff.Password);
                                             if (added.Succeeded) {
                                                  off.AppUserId = appuser.Id;
                                                  _unitOfWork.Repository<CustomerOfficial>().Update(off);
                                             }
                                        }
                                   }
                              }
                         }

                         await _unitOfWork.Complete();
                         return _mapper.Map<ICollection<Customer>, ICollection<CustomerDto>>(customers);
                    }
                    return null;
               //}
          }

     
          public async Task<CustomerDto> AddCustomer(RegisterCustomerDto dto)
          {

               var offs = new List<CustomerOfficial>();
               if(dto.CustomerOfficials.Count() > 0 ) {
                    foreach(var off in dto.CustomerOfficials) {
                         offs.Add(new CustomerOfficial(off.Gender, off.Title, off.OfficialName, off.Designation,
                              off.PhoneNo, off.Mobile, off.Email, off.ImageUrl, off.LogInCredential));
                    }
               }
               var CustomerToAdd = new Customer(dto.CustomerType,dto.CustomerName, dto.KnownAs, dto.Add,
                    dto.Add2, dto.City, dto.Pin, dto.District, dto.State, dto.Country, dto.Email, dto.Website,
                    dto.Phone, dto.Phone2, dto.Introduction, dto.CustomerIndustries, dto.AgencySpecialties);
               
               _unitOfWork.Repository<Customer>().Add(CustomerToAdd);

               int RecordsInserted=0;
               try {
                    RecordsInserted = await _unitOfWork.Complete();
               } catch (Exception ex) {
                    _logger.LogError(ex, ex.Message);
                    return null;
               } 

               if (RecordsInserted > 0) {
                    foreach(var off in offs) {
                         off.CustomerId=CustomerToAdd.Id;
                         _unitOfWork.Repository<CustomerOfficial>().Add(off);
                    }
                    await _unitOfWork.Complete();
                    foreach (var off in dto.CustomerOfficials)
                    {
                         if (off.LogInCredential)
                         {
                              var appuser = new AppUser
                              {
                                   UserType = "official",
                                   Gender = off.Gender,
                                   DisplayName = off.KnownAs,
                                   Email = off.Email,
                                   UserName = off.Email,
                                   PhoneNumber = off.PhoneNo
                              };
                              var added = await _userManager.CreateAsync(appuser, off.Password);
                         }
                    
                    }
               }
               return _mapper.Map<Customer, CustomerDto>(CustomerToAdd);
               
          }

          public Task<bool> CustomerExistsByIdAsync(int id)
          {
               throw new System.NotImplementedException();
          }

          public void DeleteCustomer(Customer customer)
          {
               throw new System.NotImplementedException();
          }

          public async Task<bool> EditCustomer(Customer model)
          {
               var existingObject = await _context.Customers.Where(x => x.Id == model.Id)
                    .Include(x => x.CustomerOfficials)
                    .Include(x => x.CustomerIndustries)
                    .Include(x => x.AgencySpecialties)
                    .Include(x => x.VendorSpecialties)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
               
               //updates parent. not the collections
               _context.Entry(existingObject).CurrentValues.SetValues(model);

               //CustomerOfficials
               if(model.CustomerOfficials==null || model.CustomerOfficials?.Count==0)
               {
                    foreach(var item in existingObject.CustomerOfficials.ToList())
                    {
                         _context.CustomerOfficials.Remove(item);
                         _context.Entry(item).State = EntityState.Deleted;
                    }
               } else {
                    //Delete that exist in DB but not in the model
                    foreach(var existingItem in existingObject.CustomerOfficials)
                    {
                         if (!model.CustomerOfficials.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                         {
                              _context.CustomerOfficials.Remove(existingItem);
                              _context.Entry(existingItem).State = EntityState.Deleted;
                         }
                    }

                    string lastName="";
                    //others are eitehr to be added or updated
                    foreach(var item in model.CustomerOfficials)
                    {
                         var existingItem = existingObject.CustomerOfficials.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingItem != null)     //replace DB Object, i.e. the existingItem, with values from the model
                         {
                              _context.Entry(existingItem).CurrentValues.SetValues(item);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {
                              if (lastName == item.OfficialName + item.Email) continue;        //it is creating 2 new records instead of 1

                              var newOff = new CustomerOfficial(model.Id, item.Gender, 
                                   item.Title, item.OfficialName, item.Designation, item.PhoneNo,
                                   item.Mobile, item.Email, item.ImageUrl, item.LogInCredential, item.IsValid);
                              if(existingObject.CustomerOfficials==null) existingObject.CustomerOfficials=new List<CustomerOfficial>();
                              existingObject.CustomerOfficials.Add(newOff);
                              _context.Entry(newOff).State = EntityState.Added;

                              lastName=item.OfficialName + item.Email;
                         }
                         
                    }

               }

               //CustomerIndustries
               if(model.CustomerIndustries==null || model.CustomerIndustries?.Count == 0) {
                    foreach(var item in existingObject.CustomerIndustries.ToList()) {
                         _context.CustomerIndustries.Remove(item);
                         _context.Entry(item).State = EntityState.Deleted;
                    }
               } else {
                    //Delete that exist in DB but not in the model
                    if(model.CustomerIndustries==null || model.CustomerIndustries?.Count==0)
                    {
                         foreach(var item in existingObject.CustomerIndustries)
                         {
                              _context.CustomerIndustries.Remove(item);
                              _context.Entry(item).State = EntityState.Deleted;
                         }
                    } else {                      //others are eitehr to be added or updated
                         int lastItem=0;
                         foreach(var item in model.CustomerIndustries)
                         {
                              var existingItem = existingObject.CustomerIndustries.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                              if (existingItem != null)     //replace DB Object, i.e. the existingItem, with values from the model
                              {
                                   _context.Entry(existingItem).CurrentValues.SetValues(item);
                                   _context.Entry(existingItem).State = EntityState.Modified;
                              } else {
                                   if(lastItem==item.IndustryId) continue;
                                   var newInd = new CustomerIndustry(model.Id, item.IndustryId, item.Name);
                                   if(existingObject.CustomerIndustries==null) existingObject.CustomerIndustries=new List<CustomerIndustry>();
                                   existingObject.CustomerIndustries.Add(newInd);
                                   _context.Entry(newInd).State = EntityState.Added;
                                   lastItem = item.IndustryId;
                              }
                         }
                    }
               }
               
               //AGencyOfficials
               if(model.AgencySpecialties==null || model.AgencySpecialties?.Count ==0) {
                    foreach(var item in existingObject.AgencySpecialties.ToList()) {
                         _context.AgencySpecialties.Remove(item);
                         _context.Entry(item).State = EntityState.Deleted;
                    }
               } else {
                    //Delete that exist in DB but not in the model
                    foreach(var existingItem in existingObject.AgencySpecialties)
                    {
                         if (!model.AgencySpecialties.Any(c => c.Id == existingItem.Id && c.Id != default(int)))
                         {
                              _context.AgencySpecialties.Remove(existingItem);
                              _context.Entry(existingItem).State = EntityState.Deleted;
                         }
                    }

                    //others are eitehr to be added or updated
                    int lastItem=0;
                    foreach(var item in model.AgencySpecialties)
                    {
                         var existingItem = existingObject.AgencySpecialties.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                         if (existingItem != null)     //replace DB Object, i.e. the existingItem, with values from the model
                         {
                              _context.Entry(existingItem).CurrentValues.SetValues(item);
                              _context.Entry(existingItem).State = EntityState.Modified;
                         } else {
                              if (lastItem==item.ProfessionId + item.IndustryId) continue;
                              var newInd = new AgencySpecialty(item.Id, model.Id, item.ProfessionId, item.IndustryId, item.Name);
                              if(existingObject.CustomerIndustries==null) existingObject.AgencySpecialties=new List<AgencySpecialty>();
                              existingObject.AgencySpecialties.Add(newInd);
                              _context.Entry(newInd).State = EntityState.Added;
                              lastItem = item.ProfessionId + item.IndustryId;
                         }
                    }
               }

               //VendorSpecialties
               if(model.VendorSpecialties==null || model.VendorSpecialties?.Count == 0) {
                    foreach(var item in existingObject.VendorSpecialties.ToList()) {
                         _context.VendorSpecialties.Remove(item);
                         _context.Entry(item).State = EntityState.Deleted;
                    }
               } else {
                    //Delete that exist in DB but not in the model
                    if(model.VendorSpecialties==null || model.VendorSpecialties?.Count==0)
                    {
                         foreach(var item in existingObject.VendorSpecialties)
                         {
                              _context.VendorSpecialties.Remove(item);
                              _context.Entry(item).State = EntityState.Deleted;
                         }
                    } else {                      //others are eitehr to be added or updated
                         int lastItem=0;
                         foreach(var item in model.VendorSpecialties)
                         {
                              var existingItem = existingObject.VendorSpecialties.Where(c => c.Id == item.Id && c.Id != default(int)).SingleOrDefault();
                              if (existingItem != null)     //replace DB Object, i.e. the existingItem, with values from the model
                              {
                                   _context.Entry(existingItem).CurrentValues.SetValues(item);
                                   _context.Entry(existingItem).State = EntityState.Modified;
                              } else {
                                   if(lastItem==item.VendorFacilityId) continue;
                                   var newInd = new VendorSpecialty(model.Id, item.VendorFacilityId, item.Name);
                                   if(existingObject.VendorSpecialties==null) existingObject.VendorSpecialties=new List<VendorSpecialty>();
                                   existingObject.VendorSpecialties.Add(newInd);
                                   _context.Entry(newInd).State = EntityState.Added;
                                   lastItem = item.VendorFacilityId;
                              }
                         }
                    }
               }
               

               _context.Entry(existingObject).State = EntityState.Modified;

               return await _context.SaveChangesAsync() > 0;

          }

          public async Task<CustomerDto> GetCustomerByIdAsync(int id)
          {
               var cust = await _context.Customers 
                    .Where(x => x.Id == id)
                    .Include(x => x.CustomerOfficials)
                    .Include(x => x.CustomerIndustries)
                    .Include(x => x.AgencySpecialties)
                    .Include(x => x.VendorSpecialties)
                    .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
               //var offs = await _context.CustomerOfficials.Where(x => x.CustomerId == id).ToListAsync();
               //if (cust.CustomerOfficials.Count() ==0 && offs.Count() > 0) cust.CustomerOfficials=offs;
               return cust;
          }

          public async Task<CustomerBriefDto> GetCustomerBriefById(int id)
          {
               var cust = await _context.Customers 
                    .Where(x => x.Id == id)
                    .ProjectTo<CustomerBriefDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
               return cust;
          }

          public Task<CustomerDto> GetCustomerByUserNameAsync(string username)
          {
               throw new System.NotImplementedException();
          }

          public async Task<ICollection<ClientIdAndNameDto>> GetClientIdAndName()
          {
               return  await _context.Customers.Select(x => new ClientIdAndNameDto{
                    CustomerId=x.Id, CustomerName = x.CustomerName, KnownAs = x.KnownAs
               }).OrderBy(x => x.CustomerId)
               .ToListAsync();
          }

          public async Task<ICollection<CustomerIdAndNameDto>> GetCustomerIdAndName(string customerType)
          {
               var qry = await (from c in _context.Customers where c.CustomerType.ToLower() == customerType.ToLower()
                    select new CustomerIdAndNameDto {
                         Id=c.Id, CustomerName = c.CustomerName, Name = c.CustomerName, City = c.City, Country=c.Country
                    }) .ToListAsync();

               return qry;
          }

          public Task<ICollection<CustomerIdAndNameDto>> GetCustomerIdAndNames(ICollection<int> customerIds)
          {
               throw new System.NotImplementedException();
          }

          public Task<ICollection<CustomerDto>> GetCustomersAsync(string userType)
          {
               throw new System.NotImplementedException();
          }

          public Task<Pagination<CustomerDto>> GetCustomersPaginatedAsync(CustomerParams custParam)
          {
               throw new System.NotImplementedException();
          }

          public void UpdateCustomer(Customer customer)
          {
               throw new System.NotImplementedException();
          }
          private async Task<bool> CheckEmailExistsAsync(string email)
          {
               return await _userManager.FindByEmailAsync(email) != null;
          }

          public async Task<ICollection<CustomerCity>> GetCustomerCityNames(string customerType)
          {
               var c = await _context.Customers.Where(x => x.CustomerType.ToLower() == customerType.ToLower())
                    .Select(x => x.City).Distinct().ToListAsync();
               var lsts = new List<CustomerCity>();
               foreach(var lst in c)
               {
                    lsts.Add(new CustomerCity{CityName = lst});
               }
               return lsts;
          }

          public Task<ICollection<string>> GetCustomerIndustryTypes(string customerType)
          {
               throw new NotImplementedException();
          }

          public async Task<ICollection<CustomerOfficialDto>> GetOfficialDetails()
          {
               var offs = await (from c in _context.Customers
                         .Where(x => x.CustomerType == "associate" && x.CustomerStatus==EnumCustomerStatus.Active )
                         join o in _context.CustomerOfficials on c.Id equals o.CustomerId where o.IsValid
                         select new CustomerOfficialDto(o.Id, c.Id,  c.CustomerName, c.City, c.Country, o.Title, 
                              o.OfficialName, o.Designation, o.Email, o.Mobile, c.Introduction, c.KnownAs, 
                              Convert.ToInt32(c.CustomerStatus)==300 )
                    ).ToListAsync();
               
               return offs;
          }

          public async Task<CustomerOfficialDto> GetCustomerOfficialDetail(int CustomerOfficialId)
          {
               var off = await (from c in _context.Customers
                         .Where(x => x.CustomerType == "customer"  && x.CustomerStatus==EnumCustomerStatus.Active )
                         join o in _context.CustomerOfficials on c.Id equals o.CustomerId where o.IsValid && o.Id == CustomerOfficialId
                         select new CustomerOfficialDto(o.Id, c.Id,  c.CustomerName, c.City, c.Country, o.Title, 
                              o.OfficialName, o.Designation, o.Email, o.Mobile, c.Introduction, c.KnownAs,
                              Convert.ToInt32(c.CustomerStatus)==300)
                    ).FirstOrDefaultAsync();
               
               return off;
          }

          public async Task<Pagination<CustomerBriefDto>> GetCustomersBriefAsync(CustomerSpecParams custParam)
          {
               var qry = (from c in _context.Customers.Where(x => x.CustomerType==custParam.CustomerType)
                    orderby c.CustomerName
                    select new CustomerBriefDto{Id = c.Id,
                         CustomerName = c.CustomerName, KnownAs = c.KnownAs, 
                         City = c.City, Country = c.Country, CustomerType = c.CustomerType}
               ).AsQueryable();
               var test = await qry.ToListAsync();
               if(!string.IsNullOrEmpty(custParam.CustomerCityName)) qry = qry.Where(x => x.City==custParam.CustomerCityName);
               
               var count = await qry.CountAsync();

               var data = await qry.Skip((custParam.PageIndex-1)*custParam.PageSize).Take(custParam.PageSize).ToListAsync();

               return new Pagination<CustomerBriefDto>(custParam.PageIndex, custParam.PageSize, count, data);
          }
          
          public async Task<string> GetCustomerNameFromId(int officialId)
          {
               var custId = await _context.CustomerOfficials.FindAsync(officialId);

               var nmm = await _context.Customers.Where(x => x.Id==custId.CustomerId).Select(x => x.KnownAs).FirstOrDefaultAsync();

               return nmm;

          }
          
     }
}