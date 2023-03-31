using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Entities.Orders;
using core.Entities.Tasks;
using core.Interfaces;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace infra.Services
{
     public class OrderAssignmentService : IOrderAssignmentService
    {
        private readonly ATSContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskService _taskService;
        private readonly ITaskControlledService _taskControlledService;
        private readonly ICommonServices _commonServices;
        //private readonly int _targetDaysForHRExecutivesToSourceCVs = 5;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmployeeService _empService;
        public OrderAssignmentService(ATSContext context, IUnitOfWork unitOfWork, 
            IEmployeeService empService, ITaskService taskService, ITaskControlledService taskControlledService,
            ICommonServices commonServices, UserManager<AppUser> userManager)
        {
            _taskControlledService = taskControlledService;
            _empService = empService;
            _userManager = userManager;
            _commonServices = commonServices;
            _taskService = taskService;
            _unitOfWork = unitOfWork;
            _context = context;
        }


        public async Task<ICollection<EmailMessage>> DesignOrderAssessmentQs(int orderId, int loggedInEmployeeId)
        {
            if (!await OrderItemsNeedAssessment(orderId)) throw new Exception("None of the order items need assessment");

            var orderDto = await _context.Orders.Where(x => x.Id == orderId).Select(x => new {x.OrderNo, x.OrderDate, x.ProjectManagerId, x.Customer.CustomerName}).FirstOrDefaultAsync();

            var task = new ApplicationTask((int)EnumTaskType.OrderCategoryAssessmentQDesign, DateTime.Now, loggedInEmployeeId,
                orderDto.ProjectManagerId, orderId, orderDto.OrderNo, 0, "Design Assessment Questions for Order No. " +
                orderDto.OrderNo + " dated " + orderDto.OrderDate + " for " + orderDto.CustomerName,
                DateTime.Now.AddDays(1), "Open",0, null);

            //emails are composed in ComposeServices and sent in TaskServices
            var msgs = await _taskControlledService.CreateNewTaskAndMsgs(task, loggedInEmployeeId);

            return msgs.emailMessages;

        }


        public async Task<bool> DeleteHRExecAssignment(int id)
        {
            //TODO - if in process, do not allow deletion

            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _unitOfWork.Repository<ApplicationTask>().Delete(task);
                if (await _unitOfWork.Complete() > 0) { return true; } else { return false; }
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> OrderItemsNeedAssessment(int orderId)
        {
            var ItemsNeedAssessment = await _context.OrderItems.Where(x => x.OrderId == orderId
                && x.RequireAssess == true).ToListAsync();

            return (ItemsNeedAssessment != null && ItemsNeedAssessment.Count > 0);
        }

          public Task<bool> EditOrderAssignment(ApplicationTask task)
          {
               throw new NotImplementedException();
          }

          public Task<bool> SetTaskAsCompleted(int id, string remarks)
          {
               throw new NotImplementedException();
          }

          public Task<bool> DeleteApplicationTask(int id, string remarks)
          {
               throw new NotImplementedException();
          }
     }
}