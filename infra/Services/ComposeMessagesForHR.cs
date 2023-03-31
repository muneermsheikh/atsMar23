using AutoMapper;
using AutoMapper.QueryableExtensions;
using core.Dtos;
using core.Entities.Admin;
using core.Entities.EmailandSMS;
using core.Entities.Identity;
using core.Interfaces;
using infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infra.Services
{
     public class ComposeMessagesForHR: IComposeMessagesForHR
{
        private const string _smsNewLine = "<smsbr>";
        private const string _WAPNewLine = "<smsbr>";
        private readonly IEmployeeService _empService;
        private readonly ATSContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICommonServices _commonServices;
        private readonly IComposeMessages _composeMessages;
        private readonly IConfiguration _confg;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderItemService _OrderItemService;
        private string CCEmail="hr@afreenintl.in";
        private string BCCEmail = "operations@afreenintl.in";
        private readonly IMapper _mapper;
        private readonly ICustomerOfficialServices _customerOfficialServices;

        public ComposeMessagesForHR(IEmployeeService empService, IOrderItemService OrderItemService, ATSContext context, 
            UserManager<AppUser> userManager, ICommonServices commonServices, ICustomerOfficialServices customerOfficialServices,
            IComposeMessages composeMessages, IConfiguration confg, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _customerOfficialServices = customerOfficialServices;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _confg = confg;
            _OrderItemService = OrderItemService;
            _composeMessages = composeMessages;
            _commonServices = commonServices;
            _userManager = userManager;
            _context = context;
            _empService = empService;
            CCEmail = _confg.GetSection("Email_CC_DLForward").Value;
            BCCEmail = _confg.GetSection("Email_BCC_DLForward").Value;
        }
        
        public Task<ICollection<EmailMessage>> ComposeMessagesToDocControllerAdminToForwardCVs(ICollection<CVsSubmittedDto> cvsSubmitted, LoggedInUserDto loggedIn)
        {
            throw new NotImplementedException();
        }
        public async Task<EmailMessage> ComposeHTMLToAckToCandidateByEmail(CandidateMessageParamDto candidateParam)
        {
            var candidate = candidateParam.Candidate;
            string msg = "";
            var candidateName = candidate.FullName;
            var email = candidate.Email;
            var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

            string subject = "";

            subject = "Your Application is registered with us under Sr. No. " + candidate.ApplicationNo;
            msg = DateTime.Today + "<br><br>" + title + " " + candidateName + "<br><br>Dear " + title + " " + candidateName;
            msg += "Dear " + title + " " + candidateName + "<br><br>" + subject + "<br><br>";
            var msgs = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "acknowledgementtocandidate" && x.Mode == "mail")
                .OrderBy(x => x.SrNo).ToListAsync();
            foreach (var m in msgs)
            {
                msg += m.LineText == "tableofrelevantopenings"
                        ? await _composeMessages.TableOfRelevantOpenings(
                            candidate.UserProfessions.Select(x => x.CategoryId).ToList())
                        : m.LineText;
            }
            msg = msg + "<br><br>HR Supervisor";

            var emailMessage = new EmailMessage
            {
                SenderEmailAddress = _confg["EmailSenderEmailId"],
                SenderUserName = _confg["EmailSenderDisplayName"],
                RecipientUserName = candidate.KnownAs,
                RecipientEmailAddress = email,
                CcEmailAddress = _confg["EmailCCandAck"],
                BccEmailAddress = _confg["EmailBCCandAck"],
                Subject = subject,
                Content = msg,
                MessageTypeId = (int)EnumMessageType.CVAcknowledgementByEMail
            };

            return emailMessage;
        }

        public async Task<SMSMessage> ComposeMsgToAckToCandidateBySMS(CandidateMessageParamDto candidateParam)
          {
               var candidate = candidateParam.Candidate;
               string msg = "";
               var candidateName = candidate.KnownAs;
               var title = candidate.Gender.ToLower() == "m" ? "Mr." : "Ms.";

               string subject = "";

               var mobileNo = candidate.UserPhones.Where(x => x.IsMain && x.IsValid).Select(x => x.MobileNo).FirstOrDefault();
               if (string.IsNullOrEmpty(mobileNo)) return null;

               subject = "Your Application registered with us under Sr. No. " + candidate.ApplicationNo;
               msg = DateTime.Today + _smsNewLine + _smsNewLine + title + " " + candidateName + _smsNewLine + _smsNewLine + "Dear " + title + " " + candidateName;
               var sms = await _context.MessageComposeSources.Where(x => x.MessageType.ToLower() == "acknowledgementtocandidate" && x.Mode == "sms")
                    .OrderBy(x => x.SrNo).ToListAsync();
               foreach (var m in sms)
               {
                    msg += m;
               }
               msg += _smsNewLine + _smsNewLine + "HR Supervisor";

               var smsMessage = new SMSMessage
               {
                    UserId = candidate.Id,
                    PhoneNo = mobileNo,
                    SMSText = msg
               };

               return smsMessage;
          }

        public async Task<EmailMessage> ComposeHTMLToPublish_CVReadiedToForwardToClient(ICollection<CommonDataDto> commonDataDtos, LoggedInUserDto loggedInDto, int recipientId)
          {
               var emp = await _empService.GetEmployeeBriefAsyncFromAppUserId(loggedInDto.LoggedInAppUserId);
               var recipient = await _empService.GetEmployeeBriefAsyncFromEmployeeId(recipientId);

               var msg = DateTime.Now.Date + "<br><br>"+ emp.EmployeeName + "<br>" + emp.Position + "<br>email:"+ emp.Email +
                    "Following Applications are cleared for forwarding to respective clients by the Document Controller:<br><br>" +
                    "<table border='1'><tr><th>App No</th><th>Candidate Name</th><th>Category Ref</th><th>Customer</th><th>Submitted by</th></tr>";
               foreach(var item in commonDataDtos)
               {
                    msg += "<tr><td>" + item.ApplicationNo + "</td><td>" + item.CandidateName + "</td><td>" + 
                         item.CategoryName+"</td><td>" + item.CustomerName + "</td><td>" + emp.EmployeeName + "</td></tr>";
               }

               msg +="</table>Regards<br><br>This is system generated message";

               var email = new EmailMessage("advisory", loggedInDto.LoggedInEmployeeId, recipient.EmployeeId, loggedInDto.LoggedInAppUserEmail, 
                    loggedInDto.LoggedIAppUsername, recipient.KnownAs, recipient.Email, "", "", 
                    "Applications readied to forward to client - initiated by " + emp.EmployeeName, msg, 
                    (int)EnumMessageType.CVForwardingToDocControllerToFwdCVToClient,3);
               
               return email;
          }

        private ICollection<EmailMessage> ComposeHTMLForwards(OrderItemsAndAgentsToFwdDto itemsAndAgents,  EmployeeDto senderObj, int loggedInUserId)
        {
            
            var catTable = ComposeCategoryTableForDLFwd(itemsAndAgents.Items);
            
            var msgs = new List<EmailMessage>();
            foreach(var agent in itemsAndAgents.Agents) {
                var hdr = DateTime.Now + "<br><br>" + agent.Title + " " + agent.CustomerOfficialName;
                if(!string.IsNullOrEmpty(agent.OfficialDesignation)) hdr +=", " + agent.OfficialDesignation;
                hdr +="<br>" + agent.CustomerName + "<br>" + agent.CustomerCity;
                hdr += "email: " + agent.OfficialEmailId + "<br><br>";
                hdr += "Sub.: Manpower requirement for " + agent.CustomerCity;
                hdr += "<br><br>We have following manpower requirement.  If you have interested candidates, please refer them to us<br><br>";
                hdr += catTable + "<br><br>Regards<br><br>" + senderObj.EmployeeName + "<br>" + senderObj.Position;

                var message = new EmailMessage("DLFwdToAgent", loggedInUserId,
                        agent.OfficialId, agent.OfficialEmailId,
                        senderObj.OfficialEmailAddress, agent.CustomerName,
                        agent.OfficialEmailId, "","", "Requirement", hdr, 
                        (int)EnumMessageType.DLForwardToAgents,0 );
                msgs.Add(message);
            }

            return msgs;
        }

        private ICollection<SMSMessage> ComposeSMSForwards(OrderItemsAndAgentsToFwdDto itemsAndAgents, EmployeeDto senderObj, int loggedInUserId)
        {
            var catTable = ComposeCategoryTableForDLFwdBySMS(itemsAndAgents.Items);

            var msgs = new List<SMSMessage>();
            foreach(var agent in itemsAndAgents.Agents) {
                var hdr = agent.Title + " " + agent.CustomerOfficialName + _smsNewLine;
                hdr += _smsNewLine + "We hv flg manpower requirement.  If u hv interested candidates, pl refer them to us" + _smsNewLine + _smsNewLine;
                hdr += catTable + "Rgds" + _smsNewLine + senderObj.EmployeeName + _smsNewLine + senderObj.Position;

                var message = new SMSMessage(loggedInUserId, agent.Phoneno, DateTime.Now, hdr);
                _unitOfWork.Repository<SMSMessage>().Add(message);
                msgs.Add(message);
            }
            
            return null;
        }

		private string ComposeCategoryTableForEmail(ICollection<OrderItemBriefDto> orderItems)
		{
			int srno = 0;
			string TableBody = "<Table><TH>Sr No</TH><TH width='75'>Cat Ref</TH><TH width='300'>Category Name</TH><TH width='40'>Quantity</TH><TH width='350'>Job Description</TH><TH width='350'>Remuneration</TH><TH width='150'>Remarks</TH>";
			string jd = "";
			string remun = "";
			foreach (var item in orderItems)
			{
				if (item.JobDescription != null)
				{
					if (!string.IsNullOrEmpty(item.JobDescription.JobDescInBrief)) jd = item.JobDescription.JobDescInBrief;
					if (item.JobDescription.MaxAge > 0) jd += "Max Age:" + item.JobDescription.MaxAge + " yrs";
					if (item.JobDescription.ExpDesiredMin > 0) jd += "Exp: " + item.JobDescription.ExpDesiredMin;
					if (item.JobDescription.ExpDesiredMax > 0) jd += " - " + item.JobDescription.ExpDesiredMax;
					if (item.JobDescription.ExpDesiredMin > 0 || item.JobDescription.ExpDesiredMax > 0) jd += " yrs.";
					if (!string.IsNullOrEmpty(item.JobDescription.QualificationDesired)) jd += " Qualification: " + item.JobDescription.QualificationDesired;
				}

				if (item.Remuneration != null)
				{
					remun = "Salary: ";
					remun += string.IsNullOrEmpty(item.Remuneration.SalaryCurrency) ? "" : item.Remuneration.SalaryCurrency;
					remun += item.Remuneration.SalaryMin > 0 ? item.Remuneration.SalaryMin : "";
					remun += item.Remuneration.SalaryMax > 0 ? "-" + item.Remuneration.SalaryMax : "";
					remun += "; Housing: ";
					remun += item.Remuneration.HousingProvidedFree ? "Free" :
					    item.Remuneration.HousingAllowance > 0 ? item.Remuneration.HousingAllowance : "Not provided";
					remun += "; Food: ";
					remun += item.Remuneration.FoodAllowance > 0 ? item.Remuneration :
					    item.Remuneration.FoodProvidedFree ? "Provided Free" : "Not Provided";
					remun += "; Transport: ";
					remun += item.Remuneration.TransportProvidedFree ? "Provided Free" :
					    item.Remuneration.TransportAllowance > 0 ? item.Remuneration.TransportAllowance : "Not Provided";
					remun += "; Medical Facilities: As per labour laws;";
					if (item.Remuneration.OtherAllowance > 0) remun += "; Other Allowance: " + item.Remuneration.OtherAllowance;

				}
				TableBody += "<TR><TD>" + ++srno + "</TD><TD>" + item.CategoryRef + "</TD><TD>" + item.CategoryName + "</TD><TD>" + item.Quantity + "</TD>" +
				    "<TD></TD><TD></TD></TR>";
			}

			TableBody += "</TABLE><BR>";

			return TableBody;
		}

		private ICollection<SMSMessage> ComposeWhatsAppForwards(OrderItemsAndAgentsToFwdDto itemsAndAgents, EmployeeDto senderObj, int loggedInUserId)
        {
            var catTable = ComposeCategoryTableForDLFwdByWhatsApp(itemsAndAgents.Items);
            var msgs = new List<SMSMessage>();
            foreach(var agent in itemsAndAgents.Agents) {
                var hdr = agent.Title + " " + agent.CustomerOfficialName + _smsNewLine;
                hdr += _smsNewLine + "We hv flg manpower requirement.  If u hv interested candidates, pl refer them to us" + _smsNewLine + _smsNewLine;
                hdr += catTable + "Rgds" + _smsNewLine + senderObj.EmployeeName + _smsNewLine + senderObj.Position;

                var message = new SMSMessage(loggedInUserId, agent.Phoneno, DateTime.Now, hdr);
                _unitOfWork.Repository<SMSMessage>().Add(message);
                msgs.Add(message);
            }
            return null;
        }

        private async Task<ICollection<EmailMessage>> ComposeEmailMsgForDLForwards(ICollection<OrderItemBriefDto> OrderItems, 
            string subject, ICollection<DLForwardCategoryOfficial> fwdDates, List<int> OfficialIds, EmployeeDto senderObj, int LoggedInEmployeeId, 
            string LoggedInEmpName, string LoggedInEmpEmail)
        {
            
            var TableBody = ComposeCategoryTableForEmail(OrderItems);

            var msgs = new List<EmailMessage>();
            string IntroductoryBody = "We have following requirements.  If you have friends interested in the opportunity, please ask them to submit their profiles " + 
                "to us along with copies of certificates and testimonials. <br><br><b>Country of requirement</b>:";
            
            fwdDates.OrderBy(x => x.CustomerOfficialId).Distinct();

            string RAName = _confg.GetSection("RAName").Value;
            foreach(var fwd in fwdDates)
            {
                var off = await _customerOfficialServices.GetCustomerOfficialDetail(fwd.CustomerOfficialId);
                if(off == null) continue;
                
                var msgBody = DateTime.Now + "<br><br>" + off.OfficialName + ", " + off.CustomerName + "<br>" + off.City + "<br>eMail: " + off.OfficialEmailId +
                    "<br>Mobile:" + off.MobileNo + "<br><br>Dear Sir: <br><br>" + IntroductoryBody + off.Country ?? "" + "<br>City of employment: " + off.City +
                    "<br>Employer Known As: " + off.CompanyKnownAs + "<br>About Employer: " + off.AboutCompany ?? "undefined" + "<br><br>Requirement details<br>:";

                msgBody +=  TableBody +  "<br><br>Regards<br><br>" + senderObj.KnownAs + "/" + senderObj.Position + 
                    "<br>Phone:" + senderObj.OfficialPhoneNo ?? "undefined";
                msgBody += "<br><b>" + RAName + "</b>";
                
                msgs.Add(new EmailMessage("forwardToAssociate", LoggedInEmployeeId, fwd.CustomerOfficialId, 
                    LoggedInEmpEmail, LoggedInEmpName, off.OfficialName, off.OfficialEmailId, CCEmail, BCCEmail, subject, msgBody, 
                    (int)EnumMessageType.DLForwardToAgents,0));
            }
            
            return msgs;
        }

        public EmailMessage ComposeEmailMsgForDLForwardToHRHead(ICollection<OrderItemBriefDto> OrderItems, 
             EmployeeDto senderObj, EmployeeDto recipient)
        {
            var dto = OrderItems.Select(x => new {x.CustomerName, x.AboutEmployer, x.OrderNo, x.OrderDate}).FirstOrDefault();

            string AboutEmployer = dto.AboutEmployer;
            string CustomerName = dto.CustomerName;

            string Subject = "Order No.:" + dto.OrderNo + " dated " + dto.OrderDate + " from " + CustomerName + " availabe for your work";

            var TableBody = ComposeCategoryTableForEmail(OrderItems);
            
            string IntroductoryBody = "Following Requirement is tasked to you. <br><br><b>Country of requirement</b>:";
            
            string RAName = _confg.GetSection("RAName").Value;

            var msgBody = DateTime.Now + "<br><br>" + recipient.EmployeeName + ", " + recipient.Position + "<br>eMail: " + 
                recipient.OfficialEmailAddress + "<br>Mobile:" + recipient.OfficialMobileNo + "<br><br>Dear Sir: <br><br>" +
                "<br>Employer Known As: " + CustomerName + "<br>About Employer: " + AboutEmployer + 
                "<br><br>" + IntroductoryBody + "<br><br>Requirement details<br>:";

            msgBody +=  TableBody +  "<br><br>Regards<br><br>" + senderObj.KnownAs + "/" + senderObj.Position + 
                "<br>Phone:" + senderObj.OfficialPhoneNo ?? "undefined";
            msgBody += "<br><b>" + RAName + "</b>";
            
            var msg =new EmailMessage("AdviseToHRDeptHead", senderObj.EmployeeId, recipient.EmployeeId, 
                recipient.OfficialEmailAddress, senderObj.EmployeeName, recipient.EmployeeName, 
                recipient.OfficialEmailAddress, CCEmail, BCCEmail, Subject, msgBody, 
                (int)EnumMessageType.DLForwardToAgents, 0);
            
            return msg;
        }

        public async Task<EmailSMSWhatsAppCollectionDto> ComposeMsgsToForwardOrdersToAgents(DLForwardToAgent dlforward, 
            int LoggedInEmpId, string LoggedInEmpKNownas, string LoggedInEmpEmail)
        {
            
            var fwdDates = new List<DLForwardCategoryOfficial>();
            
            var OrderItemIds = new List<int>();

            var senderObj = await _empService.GetEmployeeBriefAsyncFromEmployeeId(dlforward.ProjectManagerId);
            if(senderObj==null) return null;

            var msgs = new List<EmailMessage>();

            foreach(var item in dlforward.DlForwardCategories)
            {
                OrderItemIds.Add(item.OrderItemId);
                foreach(var dt in item.DlForwardCategoryOfficials)
                {
                    fwdDates.Add(dt);
                }
            }

            var qry = _context.OrderItems.Include(x => x.JobDescription).Include(x => x.Remuneration).Where(x => OrderItemIds.Contains(x.Id)).Distinct().OrderBy(x => x.SrNo).AsQueryable();
            var ItemBriefDtos = await qry.ProjectTo<OrderItemBriefDto>(_mapper.ConfigurationProvider).ToListAsync();
            var OfficialIdsForEmails = fwdDates.Where(x => !string.IsNullOrEmpty(x.EmailIdForwardedTo)).Select(x => x.CustomerOfficialId).ToList();
            var OfficialIdsForSMS  = fwdDates.Where(x => !string.IsNullOrEmpty(x.PhoneNoForwardedTo) ).Select(x => x.CustomerOfficialId).ToList();
            var OfficialIdsForWhatsApp = fwdDates.Where(x => !string.IsNullOrEmpty(x.WhatsAppNoForwardedTo)).Select(x => x.CustomerOfficialId).ToList();
            
            if(OfficialIdsForEmails !=null || OfficialIdsForEmails.Count > 0) {
                string subject = "Requirements under Order No.: " + dlforward.OrderNo + " dated " + dlforward.OrderDate.Date + dlforward.CustomerCity;
                msgs = (List<EmailMessage>)await ComposeEmailMsgForDLForwards(ItemBriefDtos, subject, fwdDates, OfficialIdsForEmails, senderObj, LoggedInEmpId, LoggedInEmpKNownas, LoggedInEmpEmail);
            }
            
            var dto = new EmailSMSWhatsAppCollectionDto();
            dto.EmailMessages=msgs;

            return dto;

            /*
            var orderitemsandagentstofwd = new OrderItemsAndAgentsToFwdDto();
            var agentsByEmail = itemsAndagents.Agents.Where(x => x.Checked==true).ToList();
            if (agentsByEmail != null && agentsByEmail.Count > 0) {
                orderitemsandagentstofwd.Agents = agentsByEmail;
                orderitemsandagentstofwd.Items = itemsAndagents.Items;
                emailmsgs  = (List<EmailMessage>)ComposeHTMLForwards(orderitemsandagentstofwd, senderObj, LoggedInUserId);
                if (emailmsgs !=null && emailmsgs.Count() > 0) returnDto.EmailMessages = emailmsgs;
            }
            
            var agentsBySMS = itemsAndagents.Agents.Where(x => x.CheckedPhone==true).ToList();
            if (agentsBySMS != null && agentsBySMS.Count > 0) {
                orderitemsandagentstofwd = new OrderItemsAndAgentsToFwdDto();
                orderitemsandagentstofwd.Agents = agentsBySMS;
                orderitemsandagentstofwd.Items = itemsAndagents.Items;
                smsmsgs  = (List<SMSMessage>)ComposeSMSForwards(orderitemsandagentstofwd, senderObj, LoggedInUserId);
                if (smsmsgs != null && smsmsgs.Count() > 0) returnDto.SMSMessages = smsmsgs;
            }


            return dto;
            */
        }

        public async Task<ICollection<SMSMessage>> ForwardEnquiryToAgentsBySMS(OrderItemsAndAgentsToFwdDto itemsAndagents, int LoggedInUserId)
        {
            var senderObj = await _empService.GetEmployeeBriefAsyncFromEmployeeId(itemsAndagents.ProjectManagerId);
            if (senderObj == null) return null;

            var catTable = ComposeCategoryTableForDLFwdBySMS(itemsAndagents.Items);
            var msgs = new List<SMSMessage>();
            foreach(var agent in itemsAndagents.Agents) {
                var hdr = agent.Title + " " + agent.CustomerOfficialName + _smsNewLine;
                hdr += _smsNewLine + "We hv flg manpower requirement.  If u hv interested candidates, pl refer them to us" + _smsNewLine + _smsNewLine;
                hdr += catTable + "Rgds" + _smsNewLine + senderObj.EmployeeName + _smsNewLine + senderObj.Position;

                var message = new SMSMessage(LoggedInUserId, agent.Phoneno, DateTime.Now, hdr);
                _unitOfWork.Repository<SMSMessage>().Add(message);
                msgs.Add(message);
            }
            
            if (await _unitOfWork.Complete() > 0) return (ICollection<SMSMessage>)msgs;
            return null;
        }

        private string ComposeCategoryTableForDLFwd(ICollection<OrderItemToFwdDto> orderitems) {
               
               var para = "<Table border='1'><tr><th>Order Ref</th><th>Customer</th>" +
                    "<th>Category</th><th>Quantity</th><th>Basic Salary</th>" +
                    "<th>Job Description</th>" +
                    "<th>Remuneration and Facilities</th></tr>";
               
               foreach(var item in orderitems ) {
                    para += "<tr><td>" + item.CategoryRef + "</td><td>" + 
                         item.CategoryName + "</td><td>" + item.Quantity + "</td><td>" 
                         /*+
                         item.SalaryCurrency + " " + 
                         item.BasicSalary  + "<td><td>" +
                         item.JobDescriptionURL + "</td><td>" + 
                         item.RemunerationURL + "</td></tr>";
                         */
                         ;
               }
               para += "</table>";

               return para;
          }

        private string ComposeCategoryTableForDLFwdBySMS(ICollection<OrderItemToFwdDto> orderitems) {
               
            int srno=0;              
            string para="";
            foreach(var item in orderitems ) {
                para += ++srno + ". Ref:" + item.CategoryRef + item.CategoryName + _smsNewLine +
                        "Customer: " + " to resolve " + _smsNewLine +
                        "Qnty: " + item.Quantity + _smsNewLine +
                        "Salary: " ;
                        /*+ item.SalaryCurrency + " " + item.BasicSalary  + _smsNewLine +
                        "Job Description: " + item.JobDescriptionURL + _smsNewLine + 
                        "Remuneration: " + item.RemunerationURL  + _smsNewLine;
                        */
            }

            return para;
        }

        private string ComposeCategoryTableForDLFwdByWhatsApp(ICollection<OrderItemToFwdDto> orderitems) {
               
            int srno=0;              
            string para="";
            foreach(var item in orderitems ) {
                para += ++srno + ". Ref:" + item.CategoryRef + item.CategoryName + _WAPNewLine +
                        "Customer: " + " to resolve "   //item.CustomerName + _WAPNewLine 
                        +
                        "Qnty: " + item.Quantity + _WAPNewLine ;
                        /*+
                        "Salary: " + item.SalaryCurrency + " " + item.BasicSalary  + _WAPNewLine +
                        "Job Description: " + item.JobDescriptionURL + _WAPNewLine + 
                        "Remuneration: " + item.RemunerationURL  + _WAPNewLine;
                        */
            }

            return para;
        }

    }
    
}