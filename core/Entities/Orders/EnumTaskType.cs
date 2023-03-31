using System.Runtime.Serialization;

//table: TaskTypes
namespace core.Entities.Orders
{
    public enum EnumTaskType
    {
        [EnumMember(Value="Order Edited Advise")]  OrderEditedAdvise=1,  
        [EnumMember(Value="Ticket Booking")]  TicketBooking=2,
        [EnumMember(Value="Emigration Documents Lodged Onine")]  EmigDocmtsLodged=3,
        [EnumMember(Value="Visa Documents compilation")]  VisaDocsCompilation=4,
        [EnumMember(Value="Candidate Mobilization for Medical Test")]  MedicalTestMobiization=6,
        [EnumMember(Value="Register Selection decisions")]  SelectionDecisionRegistration=7, 
        [EnumMember(Value="Selection Followup with client")] SelectionFollowupWithClient=8, 
        [EnumMember(Value="Forward CVs to client")] CVForwardToCustomers=9,
        [EnumMember(Value="Assign CV Review to HR Manager")] SubmitCVToHRMMgrForReview=10,
        [EnumMember(Value="Assign CV Review to HR Supervisor")] SubmitCVToHRSupForReview=11,
        [EnumMember(Value="Assign DL Task to HR Executive")]  AssignTaskToHRExec=12,
        [EnumMember(Value="Design Order Category Assessment Questions")]  OrderCategoryAssessmentQDesign=13,
        [EnumMember(Value="Assign DL Task to HR Dept Head")]  AssignDLToHRDeptHead=14,
        [EnumMember(Value="Order Registration ")] OrderRegistration=15,
        [EnumMember(Value="Contract Review")] ContractReview=16,
        [EnumMember(Value="Order Assignment to Project Manager")] OrderAssignmentToProjectManager=17,    //2
        [EnumMember(Value="Submit CV to Doc Controller Admin to forward CV to Clients")]  SubmitCVToDocControllerAdmin=18,  //8
        [EnumMember(Value="Travel tickets booking, by Ticketing Exec")] TravelTicketBooking=19,        //15
        [EnumMember(Value ="Offer Letter Acceptance by candidate")] OfferLetterAcceptance=20,      //18
        [EnumMember(Value = "Visa Docs submission")] VisaDocSubmission=21,
        [EnumMember(Value = "Medicaly Fit")] MedicallyFit=22,
        [EnumMember(Value = "Traveled")] Traveled = 23,
        [EnumMember(Value ="Candidate Arrival acknowledged by Client")]ArrivalAcknowledgedByClient=24,
        [EnumMember(Value = "Visa Received")] VisaReceived=25,
        [EnumMember(Value = "Prospective Candidate Followup")] ProspectiveCandidateFollowup=26,
        [EnumMember(Value ="None")] None=27

    }
}
