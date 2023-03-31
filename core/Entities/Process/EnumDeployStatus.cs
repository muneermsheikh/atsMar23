    using System.Runtime.Serialization;

//table: DeployStages
namespace core.Entities.Process
{
    public enum EnumDeployStatus
    {
        [EnumMember(Value="None")] None = 0,
        [EnumMember(Value="Selected")] Selected = 1,                        //200,
        [EnumMember(Value="Offer Letter Accepted")] OfferLetterAccepted = 300,
        [EnumMember(Value="Referred for Medical Test")] ReferredForMedical=400,
        [EnumMember(Value="Medically Declared Fit")] MedicallyFit=13,               //500,
        [EnumMember(Value="Medically Unfit")] MedicallyUnfit=12,                    //600,
        [EnumMember(Value="Visa Documents Prepared")] VisaDocsPrepared=18,          //1000,
        [EnumMember(Value ="Visa Documents Submitted")] VisaDocsSubmitted=11,       //1100,
        [EnumMember(Value="Visa Denied")] VisaDenied=16,                            //1250,
        [EnumMember(Value="Visa Received")] VisaReceived=10,                        //1300,
        [EnumMember(Value="Emigration Docs Lodged Online")] EmigDocsLodgedOnLine=9,     //2000,
        [EnumMember(Value="Emigration Denied")] EmigrationDenied=5,                 //2250,
        [EnumMember(Value="Emigration Granted")] EmigrationGranted=6,               //2300,
        [EnumMember(Value="Travel Ticket booked")]TravelTicketBooked=4,                     //3000,
        [EnumMember(Value="Traveled")]Traveled=2,                               //3200,
        [EnumMember(Value ="Documents couriered to candidate")] DocumentsCourieredToCandidate=3
        , [EnumMember(Value ="Documents couriered to candidate")] Concluded=17
    }
}
