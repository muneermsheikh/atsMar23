using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace core.Entities.Admin
{
    public enum EnumContactResult
    {
        //100 to 1000 - connectivity problems
        [Display(Name = "Phone Not Reachable."), EnumMember(Value="Phone Not Reachable")] PhoneNotReachable = 1,
        [Display(Name = "Positive - in Progress."), EnumMember(Value="Positive - In Progress")] PositiveInProgress = 2,
        [Display(Name = "Asked to call later."), EnumMember(Value="Asked to call later")] AskedToCallLater = 3,
        [Display(Name = "Not Interested In the Project."), EnumMember(Value="Not Interested In the Project")] NotInterestedInProject = 4,
        [Display(Name = "Phone Unanswered."), EnumMember(Value="Phone Unanswered")] PhoneUnanswered = 5,
        [Display(Name = "Phone out of network."), EnumMember(Value="Phone out of network")] PhoneOutOfNetwork = 6,
        [Display(Name = "Phone Not reachable."), EnumMember(Value="Phone Not Reachable")] NewValue=7,
        [Display(Name = "Offer Accepted."), EnumMember(Value="Offer accepted")] OfferAccepted = 11,
        [Display(Name = "Interested in the Offer - consents to forward his profile."), EnumMember(Value="Interested and consents to forward his CV")] InterstedOkToSubmithisCV = 12,
        [Display(Name = "Medically Unfit."), EnumMember(Value="Medically Unfit")] DoesNotHavePP = 13,
        [Display(Name = "Not Interested, reasons not known."), EnumMember(Value="Not Interested - Reasons not known")] NotInterestedReasonsNotKnown = 14,
        [Display(Name = "Service Charges Not Acceptable."), EnumMember(Value="Service Charges not acceptable")] SCNotAcceptable = 15,
        [Display(Name = "PP Not In Possession."), EnumMember(Value="Does not possess Passport")] NoPassport = 16,
        [Display(Name = "Wants increased salary."), EnumMember(Value="Wants increased salary")] WantsIncreasedSalary = 17,
        [Display(Name = "Does not sound interested."), EnumMember(Value="Does not sound interested")] DoesNotSoundInterested = 18,
        [Display(Name = "Rude behavior."), EnumMember(Value="Rude behavior")] RudeBehavior = 19,
        [Display(Name = "Not Interested for overseas job."), EnumMember(Value="Not Interested for Overseas job")] NotInterestedForOverseasJob = 20,
        [Display(Name = "Wrong Phone No."), EnumMember(Value="Wrong Phone No")] PhoneNoWrong = 21,
        [Display(Name = "Phone answered by others."), EnumMember(Value="Phone answered by others")] PhoneAnsweredByOthers = 22,
        [Display(Name = ""), EnumMember(Value="Wrong Phone No")] NewNo = 24,
        [Display(Name = ""), EnumMember(Value="Wrong Phone No")] NewNoa = 25,
        [Display(Name = ""), EnumMember(Value="Wrong Phone No")] NewNob = 26
    }       
}

