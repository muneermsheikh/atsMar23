using System.ComponentModel.DataAnnotations;

namespace core.Dtos
{
    public class InterviewFollowupItemDto
    {
        [Required]
        public int InterviewItemCandidateId {get; set;}
        [MinLength(10), MaxLength(10), Required]
        public string MobileNoCalled { get; set; }
    }
}