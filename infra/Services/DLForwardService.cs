using AutoMapper;
using AutoMapper.QueryableExtensions;
using core.Entities.Admin;
using core.Entities.EmailandSMS;
using core.Interfaces;
using core.Dtos;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class DLForwardService : IDLForwardService
     {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ATSContext _context;
        private readonly ICommonServices _commonService;
        private readonly IComposeMessagesForHR _composeMsgHR;
        private readonly IMapper _mapper;
        private readonly ITaskService _taskService;
        public DLForwardService(IUnitOfWork unitOfWork, 
            ATSContext context, 
            ITaskService taskService,
            ICommonServices commonService, 
            IComposeMessagesForHR composeMsgHR, 
            IMapper mapper)
        {
            _mapper = mapper;
            _composeMsgHR = composeMsgHR;
            _context = context;
            _unitOfWork = unitOfWork;
            _commonService = commonService;
            _taskService = taskService;
        }

        public async Task<ICollection<DLForwardToAgent>> DLForwardsForActiveDLs()
        {
            var forwards = await (from fwd in _context.DLForwardToAgents
                join o in _context.DLForwardCategories on fwd.OrderId equals o.OrderId
                join i in _context.OrderItems on o.OrderItemId equals i.Id where i.Status=="Under Process"
                join d in _context.DLForwardCategoryOfficials on  o.OrderItemId equals d.OrderItemId
                select new {fwd, o, d}).ToListAsync();
            
            return (ICollection<DLForwardToAgent>)forwards;
        }

        public async Task<ICollection<AssociatesWithDLForwardedDto>> AssociatesWithDLForwarded()
        {
            var associates =  _context.Customers.Where(X => X.CustomerType=="Associate").Select(x => new {x.CustomerName, x.City, x.CustomerOfficials})
                .Include(c => c.CustomerOfficials)  //.Select(x => new {x.Id, x.CustomerId, x.OfficialName, x.Email, x.Designation, x.PhoneNo, x.Mobile, x.IsValid, x.DLForwards}).Where(x => x.IsValid) )
                .ThenInclude(c => c.DLForwardToAgents)
                .AsQueryable();

            var qry = await associates.ProjectTo<AssociatesWithDLForwardedDto>(_mapper.ConfigurationProvider).ToListAsync();
            
            return qry;
                
        }

        public async Task<DLForwardToAgent> DLForwardsForDL(int orderid)
        {
            var forward = await _context.DLForwardToAgents.Where(x => x.OrderId == orderid)
                .Include(x => x.DlForwardCategories).ThenInclude(x => x.DlForwardCategoryOfficials)
                .FirstOrDefaultAsync();
            
            return forward;
        }
        
        public async Task<string> ForwardDLToAgents(DLForwardToAgent dlfwd, int LoggedInEmpId, string LoggedInEmpKnownAs, string LoggedInEmpEmail)
        {
            //get DLForardId
            var dlForward=await _context.DLForwardToAgents.Where(x => x.OrderId ==dlfwd.OrderId)
                .Include(x => x.DlForwardCategories).ThenInclude(x => x.DlForwardCategoryOfficials).FirstOrDefaultAsync();

            var dlForwardItems = new List<DLForwardCategory>();
            if(dlForward==null) {
                dlForward = new DLForwardToAgent();
                dlForward.CustomerId=dlfwd.CustomerId;
                dlForward.OrderDate=dlfwd.OrderDate;
                dlForward.OrderId=dlfwd.OrderId;
                dlForward.OrderNo=dlfwd.OrderNo;

                _context.Entry(dlForward).State = EntityState.Added;
            } 

            foreach(var modelItem in dlfwd.DlForwardCategories)
            {
                var existingItem = dlForward?.DlForwardCategories?.Where(c => c.OrderItemId == modelItem.OrderItemId && c.OrderItemId != default(int)).SingleOrDefault();
                if(existingItem == null)    //add to existingItem
                {
                    existingItem = new DLForwardCategory(dlForward.OrderId, modelItem.OrderItemId, modelItem.CategoryId, modelItem.CategoryName);
                    var items = new List<DLForwardCategory>();
                    if(dlForward.DlForwardCategories==null || dlForward.DlForwardCategories.Count==0) dlForward.DlForwardCategories = items;
                    dlForward.DlForwardCategories.Add(existingItem);
                    _context.Entry(existingItem).State = EntityState.Added;
                    
                }
                //DLFwdDAtes
                foreach(var dt in modelItem.DlForwardCategoryOfficials)
                {
                    if (existingItem.DlForwardCategoryOfficials==null) {
                        var dlforwarddates = new List<DLForwardCategoryOfficial>();
                        existingItem.DlForwardCategoryOfficials = dlforwarddates;
                    }
                    existingItem.DlForwardCategoryOfficials.Add(dt);
                }
            }

            
            //dlFwd State already set at the top
            int recordsAffected=0;
            try {
                recordsAffected = await _context.SaveChangesAsync();
            } catch(Exception ex) {
                return ex.Message;
            } finally {
                var msgs = await _composeMsgHR.ComposeMsgsToForwardOrdersToAgents(dlfwd, LoggedInEmpId, LoggedInEmpKnownAs, LoggedInEmpEmail);
                
                if (msgs.EmailMessages != null & msgs.EmailMessages?.Count() > 0 ) {
                    foreach(var msg in msgs.EmailMessages) {
                        _unitOfWork.Repository<EmailMessage>().Add(msg);
                    }
                    
                    if (await _unitOfWork.Complete() > 0) {
                        //send email messages
                    } else {
                        throw new Exception("failed to send email messages");
                    }
                }
                if (msgs.SMSMessages != null & msgs.SMSMessages?.Count() > 0 ) {
                    foreach(var msg in msgs.SMSMessages) {
                        _unitOfWork.Repository<SMSMessage>().Add(msg);
                    }
                    if (await _unitOfWork.Complete() > 0) {
                        //send SMS messages
                    } else {
                        throw new Exception("failed to send SMS Messages");
                    }
                }
                if (msgs.WhatsAppMessages != null & msgs.WhatsAppMessages?.Count() > 0 ) {
                    foreach(var msg in msgs.WhatsAppMessages) {
                        _unitOfWork.Repository<SMSMessage>().Add(msg);
                    }
                    if (await _unitOfWork.Complete() > 0) {
                        //send WhatsApp messages
                    } else {
                        throw new Exception ("failed to send WhatsApp messages");
                    }
                }
            }

            return "";
            
        }

        
        public async Task<ICollection<DLForwardCategory>> OrderItemForwardedToStats (int OrderId)
        {
           var qry  = await _context.DLForwardCategories.Where(x => x.OrderId == OrderId)
                .Include(x => x.DlForwardCategoryOfficials).ToListAsync();
            return qry;
        }

        
    }
}