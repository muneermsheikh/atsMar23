using core.Entities.MasterEntities;
using core.Params;

namespace core.Specifications
{
     public class CategorySpecs: BaseSpecification<Category>
    {
        public CategorySpecs(CategorySpecParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search.ToLower())) &&
                (string.IsNullOrEmpty(specParams.Name) || x.Name.ToLower() == specParams.Search.ToLower()) &&
                (!specParams.Id.HasValue || x.Id == specParams.Id )
                )
        {
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
            AddOrderBy(x => x.Name);
        }

        public CategorySpecs(int id) 
            : base(x => x.Id == id)
        {
        }
  
    }
}