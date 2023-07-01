using System.Runtime.Serialization;

namespace core.Entities.HR
{
    //tabe: SelectionStatuses
    public enum EnumCVRefStatus
    {
        /* [EnumMember(Value="Referred")] Referred=11,
        [EnumMember(Value="Selected")] Selected=10,
        [EnumMember(Value="Rejected - Not Suitable")] RejectedNotSuitable=300,
        [EnumMember(Value="Rejected - Medically Unfit")] RejectedMedicallyUnfit=400,
        [EnumMember(Value="Rejected - Salary Expectation High")] RejectedHighSalaryExpectation=3,
        [EnumMember(Value="Rejected - No relevant exp")] RejectedNoRelevantExp=6,
        [EnumMember(Value="Rejected - Not qualified")] RejectedNotQualified=12,
        [EnumMember(Value="Rejected - Overage")] RejectedOverAge=2,
        [EnumMember(Value="Rejected - Not Available")] NotAvailable=900,
        [EnumMember(Value="Not Interested")] NotInterested=13
        */
        [EnumMember(Value="Referred")] Referred=11,
        [EnumMember(Value="Selected")] Selected=1,
        [EnumMember(Value="Withdrawn By Client")] WithdrawnByClient=2,
        [EnumMember(Value="Rejeced - Not Qualified")] RejectedNotQualified=3,
        [EnumMember(Value="Rejected - profile does not match")] RejectedProfileDoesNotMatch=4,
        [EnumMember(Value="Rejected - Exp not relevat")] RejectedExpNotRelevant=5,
        [EnumMember(Value="Rejected - No Insufficient exp")] RejectedExpNotSufficient=6,
        [EnumMember(Value="Rejected - Documents Suspect")] RejectedDocumentsSuspect=7,
        [EnumMember(Value="Rejected - High Salary Expctation")] RejectedHighSalaryExpectation=8,
        [EnumMember(Value="Rejected - over Age")] RejectedOverAge=9,
        [EnumMember(Value="Rejected - Reasons Not Known")] RejectedReasonNotknown=10,
        [EnumMember(Value="Candidate Not Interested")] CandidateNotInterested=12,
        [EnumMember(Value="Candidate Not Reachable")] CandidateNotReachable=13,
        [EnumMember(Value="Rejected - Medically Unfit")] RejectedMedicallyUnfit=14
    }
}