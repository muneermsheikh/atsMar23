using System.Runtime.Serialization;

namespace core.Entities.MasterEntities
{
    public class ReviewStatus: BaseEntity
    {
       public string Status { get; set; }
    }

    public enum EnumReviewStatus {
        [EnumMember(Value="Not Reviewed")]
        NotReviewed=4,
        [EnumMember(Value="Regretted")]
        Regretted=3,
        [EnumMember(Value="Accepted with some regrets")]
        AcceptedWithSomeRegrets=2,
        [EnumMember(Value="Accepted")]
        Accepted=1
    }
}