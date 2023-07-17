using core.Dtos;
using core.Entities.EmailandSMS;
using core.Interfaces;
using infra.Data;

namespace infra.Services
{
     public class ComposeMessageForProspectiveCandidateService : IComposeMessageForCandidates
     {
          private readonly ATSContext _context;
          private readonly IUnitOfWork _unitOfWork;
          string smsBr = "<br>";
          string stringResponseNo = "3948593422";

          public ComposeMessageForProspectiveCandidateService(ATSContext context, IUnitOfWork unitOfWork)
          {
               _unitOfWork = unitOfWork;
               _context = context;
          }

        
          public EmailMessage ComposeMessagesForConsentOfInterest(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {
               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " + dto.City + ", " + dto.Country;
               var content = DateTime.Now.Date + "<br><br>" + dto.CandidateName + "<br><br>" +
                    "<u><b>" + sSubject + "</u></b><br><br>" +
                    "Thank you very much for confirming your interest in the above opening.  As discussed, please send your updated Profile by return " +
                    "to forward same to the client.<br><br>Regards<br><br>";
               
               var emailMsg = new EmailMessage("HR", userDto.LoggedInEmployeeId, dto.Id, userDto.LoggedInAppUserEmail,
                    userDto.LoggedIAppUsername, dto.CandidateName, dto.Email, "", "",
                    sSubject, content, (int)EnumMessageType.CVAcknowledgementByEMail, 3, 
                    null, userDto.LoggedInAppUserId, "candidate");
          
               return emailMsg;

          }

          public EmailMessage ComposeMessagesForFailureToReach(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {

               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " +  dto.City + ", " + dto.Country;
               var content = DateTime.Now.Date + "<br><br>" + dto.CandidateName + "<br><br>" +
                    "<u><b>" + sSubject + "</u></b><br><br>" +
                    "A customer of ours based in " + dto.City + ", " + dto.Country + " is looking for a " + dto.CategoryName + 
                    "<br><br>We have your details from " + dto.Source + ", and we think it meets with the client requirements.  " +
                    "To discuss the openign with you, we tried many a times to reach you on your numebr " + dto.PhoneNo + 
                    " but failed to reach you.<br><br>If you are interested in the opening, please share with us your correct contact details along with your updated profile " +
                    "so as to submit the same to our Client for their review.<br><br>If you are not interested, kindly respond with a <i>Not Interested</i>" +
                    " message, so as not to bother you again.<br><br>Regards<br><br>";
               
               var emailMsg = new EmailMessage("HR", userDto.LoggedInEmployeeId, dto.Id, userDto.LoggedInAppUserEmail,
                    userDto.LoggedIAppUsername, dto.CandidateName, dto.Email, "", "",
                    sSubject, content, (int)EnumMessageType.CandidateFollowup, 3,  
                    null, userDto.LoggedInAppUserId, "candidate");

               return emailMsg;
          }

          public SMSMessage ComposeSMSForConsentOfInterest(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {
               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " + dto.City + ", " + dto.Country;
               var content = dto.CandidateName + smsBr +
                    "<u><b>" + sSubject + "</u></b><br><br>" +
                    "Thank you very much for confirming your interest in the opening of " + dto.CategoryName + " for our client in " + 
                         dto.City + ", " + dto.Country + ". As discussed, pl send yr updated Profile to: hr@afreenintl.in " +
                    smsBr + smsBr + "Rgds/" + userDto.LoggedIAppUsername;
               var sms = new SMSMessage(userDto.LoggedInEmployeeId, dto.PhoneNo, DateTime.Now, content);
               return sms;
          }

          public SMSMessage ComposeSMSForFailureToReach(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {

               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " +  dto.City + ", " + dto.Country;
               var content =  dto.CandidateName + smsBr +
                    "We hv yr details from " + dto.Source + smsBr +
                    "A customer of ours based in " + dto.City + ", " + dto.Country + " is looking for a " + dto.CategoryName + 
                    ". We think it meets with client requirements.  " +
                    "To discuss the opening with you, we tried many a times but failed to reach you on your numebr " + dto.PhoneNo +  smsBr + smsBr +
                    "If you are interested in the opening, pl share yr correct contact details to disc further. " + smsBr + smsBr +
                    "If you are not interested, pl respond with NO to " + stringResponseNo + smsBr + smsBr + "Rgds/" + userDto.LoggedIAppUsername;
               
               var sms = new  SMSMessage(userDto.LoggedInEmployeeId, dto.PhoneNo, DateTime.Now, content);
               
               return sms;
          }

          public EmailMessage ComposeMessageForNoInterest(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {

               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " +  dto.City + ", " + dto.Country;
               var content = DateTime.Now.Date + "<br><br>" + dto.CandidateName + "<br><br>" +
                    "<u><b>" + sSubject + "</u></b><br><br>" +
                    "Thank you for the courtesy extended when we reached you on phone to know your interest for the above opening.  You declined." +
                    "<br><br>Should you change your mind, please do reach us.<br><br>Regards<br><br>";
               
               var emailMsg = new EmailMessage("HR", userDto.LoggedInEmployeeId, dto.Id, userDto.LoggedInAppUserEmail,
                    userDto.LoggedIAppUsername, dto.CandidateName, dto.Email, "", "", sSubject, content, 
                    (int)EnumMessageType.CandidateFollowup, 3, null, userDto.LoggedInAppUserId, "candidate");

               return emailMsg;
          }

          public EmailMessage ComposeMessageForSCNotAcceptable(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {
               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " +  dto.City + ", " + dto.Country;
               var content = DateTime.Now.Date + "<br><br>" + dto.CandidateName + "<br><br>" +
                    "<u><b>" + sSubject + "</u></b><br><br>" +
                    "Thank you for the courtesy extended when we reached you on phone to know your interest for the above opening.  You declined." +
                    "<br><br>We will revert to you Should there be other openings that suit your requirement.<br><br>Regards<br><br>";
               
               var emailMsg = new EmailMessage("HR", userDto.LoggedInEmployeeId, dto.Id, userDto.LoggedInAppUserEmail,
                    userDto.LoggedIAppUsername, dto.CandidateName, dto.Email, "", "", sSubject, content, 
                    (int)EnumMessageType.CandidateFollowup, 3, null, userDto.LoggedInAppUserId, "candidate");

               return emailMsg;
          }

          public EmailMessage ComposeMessageForDeclinedDueToowSalary(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {
               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " +  dto.City + ", " + dto.Country;
               var content = DateTime.Now.Date + "<br><br>" + dto.CandidateName + "<br><br>" +
                    "<u><b>" + sSubject + "</u></b><br><br>" +
                    "Thank you for the courtesy extended when we reached you on phone to know your interest for the above opening.  You declined as you were not satified with the remuenration offered." +
                    "<br><br>Should there be openings that suit your needs, we will revert to you.<br><br>Regards<br><br>";
               
               var emailMsg = new EmailMessage("HR", userDto.LoggedInEmployeeId, dto.Id, userDto.LoggedInAppUserEmail,
                    userDto.LoggedIAppUsername, dto.CandidateName, dto.Email, "", "", sSubject, content, (int)EnumMessageType.CandidateFollowup, 
                    3, null, userDto.LoggedInAppUserId, "candidate");

               return emailMsg;
          }

          public EmailMessage ComposeMessageForNotInterestedForOverseasJob(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {
               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " +  dto.City + ", " + dto.Country;
               var content = DateTime.Now.Date + "<br><br>" + dto.CandidateName + "<br><br>" +
                    "<u><b>" + sSubject + "</u></b><br><br>We have your details from " + dto.Source +"." +
                    "Thank you for the courtesy extended when we reached you on phone to know your interest for the above opening.  You declined as you stated you are not interested for overeas opportunities." +
                    "<br><br>We have noted your interested accordingly in our database.  Should you ever change your mind, please do let us know.<br><br>Regards<br><br>";
               
               var emailMsg = new EmailMessage("HR", userDto.LoggedInEmployeeId, dto.Id, userDto.LoggedInAppUserEmail,
                    userDto.LoggedIAppUsername, dto.CandidateName, dto.Email, "", "", sSubject, content, (int)EnumMessageType.CandidateFollowup, 
                    3, null, userDto.LoggedInAppUserId, "candidate");

               return emailMsg;

          }

          public EmailMessage ComposeMessageForAskToReachLater(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {
               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " +  dto.City + ", " + dto.Country;
               var content = DateTime.Now.Date + "<br><br>" + dto.CandidateName + "<br><br>" +
                    "<u><b>" + sSubject + "</u></b><br><br>We have your details from " + dto.Source +"." +
                    "Thank you for the courtesy extended when we reached you on phone to know your interest for the above opening.  You were too busy to engage with us, and asked us to reach you later." +
                    "<br><br>Please reach us when you are free to discuss the above opening. If we fail to hear from you in next couple of days, we will assume you are not interested. <br><br>Regards<br><br>";
               
               var emailMsg = new EmailMessage("HR", userDto.LoggedInEmployeeId, dto.Id, userDto.LoggedInAppUserEmail,
                    userDto.LoggedIAppUsername, dto.CandidateName, dto.Email, "", "", sSubject, content, (int)EnumMessageType.CandidateFollowup, 
                    3, null, userDto.LoggedInAppUserId, "candidate");

               return emailMsg;

          }


          public SMSMessage ComposeSMSForAskToReachLater(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto)
          {
               string sSubject = "Requirement of " + dto.CategoryName + " for our client in " +  dto.City + ", " + dto.Country;
               var content =  dto.CandidateName + smsBr +
                    "We hv yr details from " + dto.Source + smsBr +
                    "A customer of ours based in " + dto.City + ", " + dto.Country + " is looking for a " + dto.CategoryName + 
                    ". To discuss the opening with you, we tried to speak to you, but you were busy and asked us to contact you later. " + smsBr + smsBr +
                    "If you are interested in the opening, pl revert next couple of days, else we will assume you are not interested. " + smsBr + smsBr +
                     "Rgds/" + userDto.LoggedIAppUsername;
               
               var sms = new  SMSMessage(userDto.LoggedInEmployeeId, dto.PhoneNo, DateTime.Now, content);
               
               return sms;
          }
          

     }
}