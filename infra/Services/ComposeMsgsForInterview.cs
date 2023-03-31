using core.Entities.EmailandSMS;
using core.Entities.Tasks;
using core.Interfaces;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class ComposeMsgsForInterview : IComposeMsgsForInterview
	{
            private readonly ATSContext _context;
            private readonly IEmployeeService _empService;
            private readonly ICommonServices _commonServices;
            private readonly string HRDeptEmail = "hr@afreenintl.in";
		public ComposeMsgsForInterview(ATSContext context, ICommonServices commonServices, IConfiguration confg, IEmployeeService empService)
		{
                  this._commonServices = commonServices;
                  this._empService = empService;
                  this._context = context;
		}

		async Task<EmailMessage> IComposeMsgsForInterview.ComposeEmailMsgToCandidateForInterviewInfo(int interviewItemId, int candidateid)
		{
			    
            var urlToAccept="url.in";
            var urlToDeny="url.in";

            var customer = await (from it in _context.InterviewItems where it.Id==interviewItemId
                join i in _context.Interviews on it.InterviewId equals i.Id
                join o in _context.Orders on i.OrderId equals o.Id
                join c in _context.Customers on o.CustomerId equals c.Id
                join e in _context.Employees on o.ProjectManagerId equals e.Id
                select new {
                    customerName = c.CustomerName, customerCity = c.City, customerDescription = c.Introduction, 
                    ProjectManagerName=e.FirstName + " " + e.FamilyName, empPhones = e.EmployeePhones, 
                    ProjManagerPosition = e.Position, projManagerAppUserId=e.AppUserId, projManagerEmail=e.Email,
                    projManagerEmployeeId = e.Id
                }).FirstOrDefaultAsync();
            var empPhone = customer.empPhones.Where(x => x.IsOfficial).Select(x => x.MobileNo).FirstOrDefault();
            
            var candidate = await _context.Candidates.Where(x => x.Id == candidateid)
                .Select(x => new{x.FullName, x.ApplicationNo, x.PpNo, x.Email, x.Gender, x.ReferredBy, x.UserPhones}).FirstOrDefaultAsync();
            var phone = candidate.UserPhones.Where(x => x.IsMain).Select(x => x.MobileNo).FirstOrDefault();
            var interviewitem = await _context.InterviewItems.FindAsync(interviewItemId);

            string emailSubject = "Your interview for the position of " + interviewitem.CategoryName + 
                " at " + customer.customerCity + " on " + interviewitem.Venue + " on " + interviewitem.CategoryName;

            var title = candidate.Gender=="M" ? "Mr." : "Ms.";
            var address = candidate.Gender=="M" ? "Sir" : "Madam";
            
            var msgBody = "Dear " + title + candidate.FullName + "<br>Application No.: " +
                candidate.ApplicationNo + "<br>Passport No.: " + candidate.PpNo + "<br>Phone No." + phone + "<br>eMail:" + candidate.Email;
            msgBody +="<br><br><br>Dear " + address + ":<br><br>";
            msgBody +="Referring to our discussions, pleased to advise you have been schedued for interview as follows:<br><br>";
            msgBody +="<pre>";
            msgBody +="<b>Company</b>:" + customer.customerDescription + "<br><b>City of Employment</b>" + customer.customerCity +
                "<br><b>Interview for Position</b>:" + interviewitem.CategoryName +"<br><b>Venue</b>:" + interviewitem.Venue +
                "<br><b>Date and Time</b>" + Convert.ToDateTime(interviewitem.InterviewDateFrom).ToString("dd-mmm-yy HH:mm") +
                "</pre>";
            msgBody += "<br><br>Please bring all your testimonials in originals, along with your Passport.<br><br>Kindly click " +
                urlToAccept + " to confirm your attendance at the interview, or " + urlToDeny + " to signify you will not be attending the interview";
            msgBody +="<br><br>Look forward to see you at the interviews.<br><br>Best regards<br><br>" + customer.ProjectManagerName +
                "<br>" + customer.ProjManagerPosition + "<br>Mobile: " + empPhone;

            var emailMessage = new EmailMessage
                {
                    MessageTypeId=(int)EnumMessageType.InterviewAdviseToCandidatebyEmail,
                    MessageGroup="Interview",
                    MessageComposedOn=DateTime.Now,
                    SenderUserName=customer.ProjectManagerName,     //Project Manager is the sender
                    SenderEmailAddress=customer.projManagerEmail,
                    SenderId=customer.projManagerEmployeeId,
                    RecipientId = candidateid,
                    RecipientUserName = candidate.FullName,
                    RecipientEmailAddress = candidate.Email,       //TODO - HRExecEmail included in Recipient, as CC and BCC not working
                    CcEmailAddress = HRDeptEmail,
                    BccEmailAddress = "",
                    Subject = emailSubject,
                    Content = msgBody,
                    SenderDeleted=false,
                    RecipientDeleted=false,
                    PostAction=(int)EnumPostTaskAction.OnlyComposeEmailMessage
                };

            _context.EmailMessages.Add(emailMessage);

            if(await _context.SaveChangesAsync() > 0) return emailMessage;

            return null;
        }

		async Task<SMSMessage> IComposeMsgsForInterview.ComposeSMSToCandidateForInterviewInfo(int interviewItemId, int candidateid)
		{
			
            var urlToAccept="url.in";

            string _smsNewLine="";

            var customer = await (from it in _context.InterviewItems where it.Id==interviewItemId
                join i in _context.Interviews on it.InterviewId equals i.Id
                join o in _context.Orders on i.OrderId equals o.Id
                join c in _context.Customers on o.CustomerId equals c.Id
                join e in _context.Employees on o.ProjectManagerId equals e.Id
                select new {
                    customerName = c.CustomerName, customerCity = c.City, customerDescription = c.Introduction, 
                    ProjectManagerName=e.FirstName + " " + e.FamilyName, empPhones = e.EmployeePhones, 
                    ProjManagerPosition = e.Position, projManagerAppUserId=e.AppUserId, projManagerEmail=e.Email,
                    projManagerEmployeeId = e.Id
                }).FirstOrDefaultAsync();
            
            var candidate = await _context.Candidates.Where(x => x.Id == candidateid)
                .Select(x => new{x.Id, x.KnownAs, x.ApplicationNo, x.PpNo, x.UserPhones}).FirstOrDefaultAsync();
            var phone = candidate.UserPhones.Where(x => x.IsMain).Select(x => x.MobileNo).FirstOrDefault();
            var interviewitem = await _context.InterviewItems.FindAsync(interviewItemId);
            
            var msgBody ="Pleased to advise yr interview schedule as follows:" +_smsNewLine;
            msgBody +="City of Employment" + customer.customerCity +
                _smsNewLine + "Position:" + interviewitem.CategoryName + _smsNewLine + "Venue:" + interviewitem.Venue +
                " on " + Convert.ToDateTime(interviewitem.InterviewDateFrom).ToString("dd-mmm-yy HH:mm");
                
            msgBody += _smsNewLine + "Pl bring all yr testimonials and yr Passport. " + _smsNewLine +
                "Pl send msg ACCEPTED from this Mobile No, to confirm u will attend the interview, or " + 
                "send msg NOT ATTENDING from this mobile No, to" + urlToAccept;
            msgBody += _smsNewLine + "Rgds/" + customer.ProjectManagerName +
                _smsNewLine +  customer.ProjManagerPosition;

            var smsMessage = new SMSMessage
               {
                    UserId = candidate.Id,
                    PhoneNo =  phone,
                    SMSText = msgBody
               };

               return smsMessage;

		}
	}
}