using System;
using System.Runtime.Serialization;

namespace core.Entities.Orders
{
    public enum EnumReviewItemStatus
    {
        [EnumMember(Value="Not Reviewed")]
        NotReviewed = 0, 
        [EnumMember(Value="Accepted")]
        Accepted=1,
        [EnumMember(Value="Declined - Salary Not Feasible")]
        SalaryNotFeasible=2,
        [EnumMember(Value="Declined - Visa availability uncertain")]
        VisaAvailabilityUncertain=3,
        [EnumMember(Value="Negative Background report on customer")]
        NegativeBackgroundReport=4

    }
}