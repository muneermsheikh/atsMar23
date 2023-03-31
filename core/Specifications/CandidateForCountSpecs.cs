using core.Entities.Users;
using core.Params;

namespace core.Specifications
{
     public class CandidateForCountSpecs: BaseSpecification<Candidate>
    {
        public CandidateForCountSpecs(CandidateSpecParams candParams)
            : base(x => 
                (!candParams.CandidateId.HasValue || x.Id == candParams.CandidateId)
                && (string.IsNullOrEmpty(candParams.Search) || 
                  x.FirstName.ToLower().Contains(candParams.Search.ToLower()) 
                  || x.SecondName.ToLower().Contains(candParams.Search.ToLower())
                  || x.FamilyName.ToLower().Contains(candParams.Search.ToLower()))
               /* && ((!candParams.ApplicationNoFrom.HasValue && !candParams.ApplicationNoUpto.HasValue)||
                    x.ApplicationNo >= candParams.ApplicationNoFrom &&
                    x.ApplicationNo <= candParams.ApplicationNoUpto) 
                */
                && (string.IsNullOrEmpty(candParams.City) || x.City.ToLower() == candParams.City.ToLower())
                && (!candParams.AgentId.HasValue || x.CompanyId == candParams.AgentId)
            )
        {
        }

        public CandidateForCountSpecs(int id, string dummy ) 
            : base(x => x.Id == id)
        {
        }

  /*
        public CandidateForCountSpecs(int appUserId)
        : base(x => x.AppUserId == appUserId)
        {
        }
  */
    }
}