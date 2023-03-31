using core.Entities.MasterEntities;
using core.Params;

namespace core.Specifications
{
     public class CategoryForCountSpecs: BaseSpecification<Category>
    {
        public CategoryForCountSpecs(CategorySpecParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search.ToLower())) &&
                (string.IsNullOrEmpty(specParams.Name) || x.Name.ToLower() == specParams.Search.ToLower()) &&
                (!specParams.Id.HasValue || x.Id == specParams.Id )                
            )
        {
        }

        public CategoryForCountSpecs(int id) 
            : base(x => x.Id == id)
        {
        }
  
    }
}