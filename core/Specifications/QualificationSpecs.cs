using core.Entities.MasterEntities;
using core.Params;

namespace core.Specifications
{
     public class QualificationSpecs: BaseSpecification<Qualification>
    {
        public QualificationSpecs(QualificationSpecParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search.ToLower())) &&
                (string.IsNullOrEmpty(specParams.Name) || x.Name.ToLower() == specParams.Name.ToLower()) &&
                (!specParams.Id.HasValue || x.Id == specParams.Id)
            )
        {
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
            AddOrderBy(x => x.Name);
        }
  
    }
}