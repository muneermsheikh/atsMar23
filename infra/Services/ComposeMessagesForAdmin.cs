using core.Dtos;
using core.Entities;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Orders;
using core.Interfaces;
using infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class ComposeMessagesForAdmin : IComposeMessagesForAdmin
    {
            private const string _smsNewLine = "<smsbr>";
            private readonly IEmployeeService _empService;
            private readonly ATSContext _context;
            private readonly IComposeMessages _commonMessages;
            private readonly IConfiguration _confg;
            private readonly ICommonServices _commonServices;
            private readonly int _empId_HRSup=12;

        public ComposeMessagesForAdmin( IEmployeeService empService, ATSContext context, IComposeMessages commonMessages, IConfiguration confg, ICommonServices commonServices )
        {
               _commonServices = commonServices;
               _confg = confg;
               _commonMessages = commonMessages;
               _context = context;
               _empService = empService;
        }
    
        
        public async Task<List<EmailMessage>> AdviseSelectionStatusToCandidateByEmail(ICollection<SelectionMessageDto> selectionsDto, string senderUsername, DateTime datetimenow, string senderEmail, int senderEmployeeId)
          {
                var msgs = new List<EmailMessage>();
                string subject = "";
                string subjectInBody="";
                string msgBody = "";

               foreach(var sel in selectionsDto)
               {
                    subject = "Your selection as " + sel.CategoryName + " for " + sel.CustomerCity;
                    subjectInBody = "<b><u>Subject: </b>Your selection as " + sel.CategoryName + " for " + sel.CustomerName + "</u>";
                    msgBody = string.Format("{0: dd-MMMM-yyyy}", DateTime.Today) + "<br><br>" + 
                         sel.CandidateTitle + " " + sel.CandidateName + "email: " + sel.CandidateEmail + "<br><br>" + 
                         "copy: " + sel.HRExecName + ", email: " + sel.HRExecEmail + "<br><br>Dear " + sel.CandidateTitle + " " + sel.CandidateName + ":" +
                         "<br><br>" + subject + "<br><br>";

                    msgBody += await GetEmailMessageBodyContents("selectionadvisetocandidate", sel.CandidateName, sel.ApplicationNo, sel.CustomerName, sel.CategoryName, sel.employment);
                    msgBody += "<br>Best Regards<br>HR Supervisor";

                    var emailMessage = new EmailMessage
                    {
                        MessageGroup="Selection",
                        MessageComposedOn=DateTime.Now,
                        SenderUserName=senderUsername,
                        SenderEmailAddress=senderEmail,
                        SenderId=senderEmployeeId,
                        RecipientId = sel.CandidateId,
                        RecipientUserName = sel.CandidateName,
                        RecipientEmailAddress = sel.CandidateEmail + "; " + sel.HRExecEmail,       //TODO - HRExecEmail included in Recipient, as CC and BCC not working
                        CcEmailAddress = sel.HRExecEmail,
                        BccEmailAddress = "",
                        Subject = subject,
                        Content = msgBody,
                        MessageTypeId = (int)EnumMessageType.SelectionAdvisebyemail,
                        SenderDeleted=false,
                        RecipientDeleted=false
                    };

                    msgs.Add(emailMessage);

               }

               if (msgs.Count > 0) return msgs;
               return null;
          }

        private async Task<string> GetEmailMessageBodyContents(string messageType, string candidatename, int applicationno, string customername, string categoryname, Employment employment)
        {
            //MessageComposeSources contains collection of static text lines for each type of message.
            var msgLines = await _context.MessageComposeSources
                .Where(x => x.MessageType.ToLower() == messageType.ToLower()  && x.Mode == "mail")
                .OrderBy(x => x.SrNo).ToListAsync();
            
            var mailbody="";
            foreach (var m in msgLines)
            {
                    switch(messageType) {
                        case "selectionadvisetocandidate":
                            //if m.LineText equals "<tableofselectiondetails>", then it is a dynamic data, to be
                            //retreived from SelectionDecision, else accept the static data m.LineText
                            if(employment != null) {
                                mailbody += m.LineText == "<tableofselectiondetails>" 
                                ? _commonMessages.GetSelectionDetails(candidatename, applicationno, 
                                    customername, categoryname, employment)
                                : m.LineText;
                            }
                            break;
                        
                        default:
                            break;    
                    }
            }

            return mailbody;
        }         
        public async Task<SMSMessage> AdviseSelectionStatusToCandidateBySMS(SelectionDecisionMessageParamDto selectionParam)
          {
               var selection = selectionParam.SelectionDecision;
               var candidate = await (from cvref in _context.CVRefs
                        where cvref.Id == selection.CVRefId
                        join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                        select cand).FirstOrDefaultAsync();

               var candidateName = candidate.KnownAs;
               var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

               string msg = "";

               var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
               if (string.IsNullOrEmpty(mobileNo)) return null;

               msg = "Dear " + title + " " + candidateName + _smsNewLine + _smsNewLine;
               var msgssms = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "selectionadvisetocandidate" && x.Mode == "sms")
                    .OrderBy(x => x.SrNo).ToListAsync();
               foreach (var m in msgssms)
               {
                    msg += m.LineText == "<tableofselectiondetailssms>" ? 
                        _commonMessages.GetSelectionDetailsBySMS(selection) : m.LineText;
               }
               msg += msg + "<br><br>HR Supervisor";

               var smsMessage = new SMSMessage
               {
                    UserId = candidate.Id,
                    PhoneNo = mobileNo,
                    SMSText = msg
               };

               return smsMessage;
          }

        public List<EmailMessage> AdviseRejectionStatusToCandidateByEmail(ICollection<SelectionMessageDto> rejectionsDto,
            string loggedInUserName, DateTime dateTimeNow, string senderEmailAddresss, int loggedInEmployeeId)
        {
            var msgs = new List<EmailMessage>();
            var subject = "";

            
            string subjectInBody="";
            string msgBody = "";

            foreach(var rej in rejectionsDto)
            {
                subject = "Your cadidature as " + rej.CategoryName + " for " + rej.CustomerCity + " is NOT approved";
                subjectInBody = "<b><u>Subject: </b>Your candidature for " + rej.CategoryName + " for " + rej.CustomerName + "</u>";
                msgBody = string.Format("{0: dd-MMMM-yyyy}", DateTime.Today) + "<br><br>" + 
                        rej.CandidateTitle + " " + rej.CandidateName + "email: " + rej.CandidateEmail + "<br><br>" + 
                        "copy: " + rej.HRExecName + ", email: " + rej.HRExecEmail + "<br><br>Dear " + rej.CandidateTitle + " " + rej.CandidateName + ":" +
                        "<br><br>" + subject + "<br><br>";

                msgBody += "We regret to inform you that M/S " + rej.CustomerName + " have not approved of your candidature for the position " +
                        "of " + rej.CategoryName + " giving following reason:<br><ul><li>" + rej.RejectionReason + "</li></ul><br>";
                msgBody += "The rejection by the Customer is indicative of only their specific needs and does not reflect your suitabiity for the position in general. " +
                        "We will therefore be continuing to look for " +
                        "alternate opportunities for you and hope to revert to you as soon as possible.<br><br>" + 
                        "In case you do not want us to continue looking for opportunities for you, please do mark yourself as unavailable by cicking here " + 
                            "so as not to include your profile in our future searches<br><br>Best regards/HR Supervisor";
                msgBody += "<br><br>This is a system generated message";
                
                msgBody += "<br>Best Regards<br>HR Supervisor";

                var emailMessage = new EmailMessage
                {
                    MessageGroup="Rejection",
                    MessageComposedOn=DateTime.Now,
                    SenderUserName=loggedInUserName,
                    SenderEmailAddress=senderEmailAddresss,
                    SenderId=loggedInEmployeeId,
                    RecipientId = rej.CandidateId,
                    RecipientUserName = rej.CandidateName,
                    RecipientEmailAddress = rej.CandidateEmail + "; " + rej.HRExecEmail,       //TODO - HRExecEmail included in Recipient, as CC and BCC not working
                    CcEmailAddress = rej.HRExecEmail,
                    BccEmailAddress = "",
                    Subject = subject,
                    Content = msgBody,
                    MessageTypeId = (int)EnumMessageType.SelectionAdvisebyemail,
                    SenderDeleted=false,
                    RecipientDeleted=false
                };

                msgs.Add(emailMessage);
            }

            if (msgs.Count > 0) return msgs;
            return null;
        }
        public async Task<SMSMessage> AdviseRejectionStatusToCandidateBySMS(SelectionDecisionMessageParamDto selectionParam)
        {
            var selection = selectionParam.SelectionDecision;
            var candidate = await (from cvref in _context.CVRefs
                                where cvref.Id == selection.CVRefId
                                join cand in _context.Candidates on cvref.CandidateId equals cand.Id
                                select cand).FirstOrDefaultAsync();

            var candidateName = candidate.KnownAs;
            var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

            string msg = "";

            var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
            if (string.IsNullOrEmpty(mobileNo)) return null;

            msg = "Yr candidature for the position of " + selection.CategoryName + _smsNewLine;
            var msgssms = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "rejectionadvisetocandidate" && x.Mode == "sms")
                .OrderBy(x => x.SrNo).ToListAsync();
            foreach (var m in msgssms)
            {
                msg += m.LineText;
            }
            msg = msg + "<br><br>HR Supervisor";

            var smsMessage = new SMSMessage
            {
                UserId = candidate.Id,
                PhoneNo = mobileNo,
                SMSText = msg
            };

            return smsMessage;
        }

        public async Task<EmailMessage> AckEnquiryToCustomer(OrderMessageParamDto orderMessageDto)
        {
            var order = orderMessageDto.Order;
            var customer = await _context.Customers.Where(x => x.Id == orderMessageDto.Order.CustomerId)
                .Include(x => x.CustomerOfficials).FirstOrDefaultAsync();
            if (customer==null) throw new Exception("failed to retrieve customer data for customer no. " + orderMessageDto.Order.CustomerId);
            var OrderItems = orderMessageDto.Order.OrderItems.OrderBy(x => x.SrNo).ToList();
            var projectManagerId = order.ProjectManagerId == 0 ? 8 : order.ProjectManagerId;
            EmployeeDto projManager = await _empService.GetEmployeeFromIdAsync(projectManagerId);

            string[] officialDepts = { "main contact", "hr", "accounts", "logistics" };
            CustomerOfficial official = null;
            foreach (var off in officialDepts)
            {
                official = customer.CustomerOfficials.Where(x => x.Divn?.ToLower() == off).FirstOrDefault();
                if (official != null) break;
            }
            bool HasException = false;
            var msg = DateTime.Now.Date + "<br><br>M/S" + customer.CustomerName;
            if (!string.IsNullOrEmpty(customer.Add)) msg += "<br>" + customer.Add;
            if (!string.IsNullOrEmpty(customer.Add2)) msg += "<br>" + customer.Add2;
            msg += "<br>" + customer.City + ", " + customer.Country + "<br><br>";
            msg += official == null ? "" : "Kind Attn : " + official.Title + official.OfficialName + ", " + official.Designation + "<br><br>";
            msg += "Dear " + official?.Gender == "F" ? "Madam:" : "Sir:" + "<br><br>";
            msg += "Thank you very much for your manpower enquiry dated " + order.OrderDate.Date + " for following personnel: ";
            msg += "<br><br>" + _commonMessages.ComposeOrderItems(order.OrderNo, OrderItems, HasException) + "<br><br>";
            msg += HasException == true
                ? "Please note the exceptions mentioned under the column <i>Exceptions</i> and respond ASAP.  " +
                        "We will initiate execution of the wroks at this end on receipt of your clarificatins.<br><br>"
                : "We have initiated the works, and will revert to you soon with our delivery plan.<br><br>";
            msg += "Your point of contact for this order execution shall be the undersigned<br><br>";
            msg += "Please feel free to reach me for any clarification.<br><br>Best regards<br><br>" +
                projManager.EmployeeName + "<br>" + projManager.Position + "<br>" + _confg.GetSection("IdealUserName").Value;
            msg += string.IsNullOrEmpty(projManager.OfficialPhoneNo) == true ? "" : "<br>Phone: " + projManager.OfficialPhoneNo;
            msg += string.IsNullOrEmpty(projManager.OfficialMobileNo) == true ? "" : "<br>Mobile: " + projManager.OfficialMobileNo;
            msg += string.IsNullOrEmpty(projManager.OfficialEmailAddress) == true ? "" : "<br>Email: " + projManager.OfficialEmailAddress;

            var senderEmailAddress = _confg["EmailSenderEmailId"] ?? "";
            var senderUserName = _confg["EmailSenderDisplayName"] ?? "";
            var recipientUserName = customer.CustomerName ?? "";
            var recipientEmailAddress = official?.Email ?? "";
            var ccEmailAddress = _confg["EmailCCandAck"] ?? "";
            var bccEmailAddress = _confg["EmailBCCandAck"] ?? "";
            var subject = "Your enquiry dated " + order.OrderDate.Date + " is registered by us under Serial No. " + order.OrderNo;
            var messageTypeId = (int)EnumMessageType.OrderAcknowledgement;
            
            var emailMessage = new EmailMessage
            {
                SenderEmailAddress = senderEmailAddress,
                SenderUserName = senderUserName,
                RecipientUserName = recipientUserName,
                RecipientEmailAddress = recipientEmailAddress,
                CcEmailAddress = ccEmailAddress,
                BccEmailAddress = bccEmailAddress,
                Subject = subject,
                Content = msg,
                MessageTypeId = messageTypeId
            };

            return emailMessage;
        }

        public async Task<EmailMessage> ForwardEnquiryToHRDept(Order order)
        {
            string msg = "";
            var HRSup = _confg.GetSection("EmpHRSupervisorId").Value;
            int HRSupId = HRSup == null ? 0 : Convert.ToInt32(HRSup);
            var hrObj = await _empService.GetEmployeeFromIdAsync(HRSupId);
            int projMgrId = order.ProjectManagerId == 0 ? 1 : order.ProjectManagerId;       //todo - correct this
            var projMgr = await _empService.GetEmployeeFromIdAsync(projMgrId);
            var cust = await _commonServices.CustomerBriefDetailsFromCustomerId(order.CustomerId);

            msg = DateTime.Now.Date + "<br><br>" + hrObj.EmployeeName + ", " + hrObj.Position + "<br>" +
                "<br>HR Supervisor<br>Email: " + hrObj.OfficialEmailAddress + "<br><br>";
            if (order.ForwardedToHRDeptOn == null || ((DateTime)(order.ForwardedToHRDeptOn)).Date.Year < 200)
            {
                msg += "Following requirement is forwarded to you for execution within time periods shown:<br><br>";
            }
            else
            {
                msg += "Following requirement forwarded to you on " +
                        ((DateTime)order.ForwardedToHRDeptOn).Date + " <b><font color='blue'>is revised</font></b>as follows:<br><br>";
            }
            msg += "Order No.:" + order.OrderNo + " dated " + order.OrderDate +
                "<br>Customer:" + cust.CustomerName + ", " + cust.City + ", Place of work: " + order.CityOfWorking;
            msg += "<br><br>Overall Project Completion target: " + order.CompleteBy.Date + "<br><br>";
            msg += "Requirement details are as follows.  For Job Description, click the relevant link<br>";

            var itemids = await _context.OrderItems.Where(x => x.OrderId == order.Id).Select(x => x.Id).ToListAsync();
            var tbl = _commonMessages.TableOfOrderItemsContractReviewedAndApproved(itemids);
            msg += tbl;
            msg += "<br><br>Task for this requirement is also assigned to you.<br><br>" + projMgr.KnownAs +
                "<br>Project Manager-Order" + order.OrderNo;

            var emailMsg = new EmailMessage("forwardToHR", projMgr.EmployeeId, hrObj.EmployeeId, projMgr.OfficialEmailAddress,
                projMgr.UserName, hrObj.UserName, hrObj.OfficialEmailAddress, "", "", "New Requirement No. " + order.OrderNo,
                msg, (int)EnumMessageType.RequirementForwardToHRDept, 3);
            return emailMsg;
        }
    
        public ICollection<EmailMessage> ComposeCVFwdMessagesToClient(ICollection<CVFwdMsgDto> cvfwddtos, LoggedInUserDto loggedIn)
        {
            DateTime dateTimeNow = DateTime.Now;
            var emails = new List<EmailMessage>();

            int lastOfficialId=0;
            string msg="";
            int counter=0;

            string concludingMsg ="</table><br>Please review the CVs and provide us your feedback at the very earliest.<br><br>" +
                    "While we try to retain candidates as long as possible, due to the dynamic market conditions, " +
                    "candidates availability becomes volatile, and it is always preferable to keep candidates positively " +
                    "engaged.  While you may take a little longer to make up your minds for selections, the candidates " +
                    "that you are not interested in can be advised to us, so that they may be released for other opportunities.";

            concludingMsg += "While rejecting a profile, if you also advise us reasons for the rejection, it will help us " +
                    "adjust our criteria for shortlistings, which will ultimately help in minimizing rejections at your end.";
            
            concludingMsg +="<br><br>Regards<br><br>This is system generated message";

            var lastCVRef = new CVFwdMsgDto();

            var email = new EmailMessage();

            foreach(var cvref in cvfwddtos) // result)
            {
                if(cvref.OfficialId==0) continue;   //failed to get a custmer official
                if(lastOfficialId != cvref.OfficialId) {
                    if (lastOfficialId != 0) {
                        msg += concludingMsg;
                        email = new EmailMessage("cv forward", loggedIn.LoggedInEmployeeId, lastCVRef.OfficialId, 
                            loggedIn.LoggedInAppUserEmail, loggedIn.LoggedIAppUsername, lastCVRef.OfficialName, lastCVRef.OfficialEmail, "", "", 
                            counter + " CVs forwarded against your requirement", msg, (int)EnumMessageType.CVForwardingToClient, 3);
                        emails.Add(email);
                    }
                    msg = dateTimeNow.Date.ToString("dd-MMM-yy") + "<br><br>"+  cvref.OfficialTitle + " " + cvref.OfficialName + ", " + 
                        cvref.Designation + "<br>M/S " + cvref.CustomerName + ", " + cvref.City + "<br>Email:" + cvref.OfficialEmail +
                        "<br><br>Dear Sir:<br><br>We are pleased to enclose following CVs for your consideration against your requirements mentioned:<br>" +
                        "<table border='1'><tr><th width=10%>Order ref and dated</th><th width=20%>Category</th><th width=5%>Application<br>No</th>" + 
                        "<th width=20%>Candidate Name</th><th width=5%>Passport No</th><th width=15%>Attachments</th><th width=5%>Forwarded<br>so far</th>" + 
                        "<th width=10%>Our assessment<br>Grade</th><th width=10%>Salary Expectation</th></tr>";
                    counter=0;
                }

                msg += "<tr><td>" + cvref.OrderNo +"-"+ cvref.ItemSrNo + "/<br>" + cvref.OrderDated.ToString("ddMMMyy") + "</td><td>" +
                    cvref.CategoryName + "</td><td>" + cvref.ApplicationNo + "</td><td>" + cvref.CandidateName + 
                    "</td><td>"+ cvref.PPNo + "</td><td></td><td>" + cvref.CumulativeSentSoFar + "</td><td>" + 
                    cvref.AssessmentGrade + "</td></tr>";
                counter+=1;
                lastOfficialId=cvref.OfficialId;
                lastCVRef=cvref;
            }
            
            msg += concludingMsg;
            email = new EmailMessage("cv forward", loggedIn.LoggedInEmployeeId, lastCVRef.OfficialId, 
                loggedIn.LoggedInAppUserEmail, loggedIn.LoggedIAppUsername, lastCVRef.OfficialName, lastCVRef.OfficialEmail, "", "", 
                counter + " CVs forwarded against your requirement", msg, (int)EnumMessageType.CVForwardingToClient, 3);
            emails.Add(email);

            return emails;
        
        }
   }
}