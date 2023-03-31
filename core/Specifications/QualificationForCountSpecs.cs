using core.Entities.MasterEntities;
using core.Params;

namespace core.Specifications
{
     public class QualificationForCountSpecs: BaseSpecification<Qualification>
    {
        public QualificationForCountSpecs(QualificationSpecParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search.ToLower())) &&
                (string.IsNullOrEmpty(specParams.Name) || x.Name.ToLower() == specParams.Name.ToLower()) &&
                (!specParams.Id.HasValue || x.Id == specParams.Id)
            )
        {
        }
  
    }
}