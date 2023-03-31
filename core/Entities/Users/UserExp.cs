using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.Entities.Users
{
    public class UserExp: BaseEntity
    {
        public UserExp()
        {
        }

        public UserExp(int candidateId, int srNo, int positionId, string employer, string position, 
            string salaryCurrency, int monthlySalaryDrawn, DateTime workedFrom, DateTime workedUpto)
        {
            CandidateId = candidateId;
            SrNo = srNo;
            PositionId = positionId;
            Employer = employer;
            Position = position;
            SalaryCurrency = salaryCurrency;
            MonthlySalaryDrawn = monthlySalaryDrawn;
            WorkedFrom = workedFrom;
            WorkedUpto = workedUpto;
        }

        public int CandidateId { get; set; }
        public int SrNo { get; set; }
        public int? PositionId { get; set; }
        public string Employer { get; set; }
        public string Position { get; set; }
        public bool? CurrentJob {get; set;}
        public string SalaryCurrency { get; set; }
        public int? MonthlySalaryDrawn { get; set; }
        [Required]
        public DateTime WorkedFrom { get; set; }
        public DateTime? WorkedUpto {get; set;}
        //public Candidate Candidate {get; set;}
    }
}