using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core.Dtos
{
    public class ProspectiveCandidateEditDto
    {
        public int Id {get; set;}
        public bool Checked {get; set;}
        [Required, MaxLength(1)]
        public string Gender { get; set; }
        [Required, MaxLength(9)]
        public string CategoryRef {get; set;}
        public int? OrderItemId {get; set;}
        [Required, MaxLength(12)]
        public string Source { get; set; }
        [Required, MaxLength(15)]
        public string ResumeId { get; set; }
        public string Natioality {get; set;}="Indian";
        [MaxLength(50)]
        public string ResumeTitle {get; set;}
        [Required]
        public DateTime? Date { get; set; }
        [MaxLength(50)]
        [Required]
        public string CandidateName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(10)]
        public string Age { get; set; }
        [MaxLength(15)]
        [Required]
        public string PhoneNo { get; set; }
        [MaxLength(15)]
        public string AlternatePhoneNo { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [EmailAddress]
        public string AlternateEmail { get; set; }
        public string CurrentLocation { get; set; }
        public string Address {get; set;}
        [Required]
        public string City {get; set;}
        public string Education {get; set;}
        public string Ctc {get; set;}
        public string WorkExperience { get; set; }
        public string Status { get; set; }
        public string NewStatus {get; set;}
        public DateTime? StatusDate { get; set; }
        public int? StatusByUserId {get; set;}
        public string UserName {get; set;}
        public string Remarks {get; set;}
    }
}