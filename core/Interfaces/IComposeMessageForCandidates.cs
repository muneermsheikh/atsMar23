using core.Dtos;
using core.Entities.EmailandSMS;

namespace core.Interfaces
{
     public interface IComposeMessageForCandidates
    {
        EmailMessage ComposeMessagesForConsentOfInterest(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        EmailMessage ComposeMessagesForFailureToReach(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        SMSMessage ComposeSMSForConsentOfInterest(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        SMSMessage ComposeSMSForFailureToReach(ComposeMessageDtoForProspects dto, LoggedInUserDto userDt);
        EmailMessage ComposeMessageForNoInterest(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        EmailMessage ComposeMessageForSCNotAcceptable(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        EmailMessage ComposeMessageForDeclinedDueToowSalary(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        EmailMessage ComposeMessageForNotInterestedForOverseasJob(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        EmailMessage ComposeMessageForAskToReachLater(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        SMSMessage ComposeSMSForAskToReachLater(ComposeMessageDtoForProspects dto, LoggedInUserDto userDto);
        
    }
}