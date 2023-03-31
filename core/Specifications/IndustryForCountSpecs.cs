using core.Entities;
using core.Params;

namespace core.Specifications
{
     public class IndustryForCountSpecs: BaseSpecification<Industry>
    {
        public IndustryForCountSpecs(IndustrySpecParams specParams)
            : base(x => 
                (string.IsNullOrEmpty(specParams.IndustryNameLike) || 
                  x.Name.ToLower().Contains(specParams.IndustryNameLike.ToLower())) &&
                (!specParams.IndustryId.HasValue || x.Id == specParams.IndustryId)
            )
        {
        }

        public IndustryForCountSpecs(int id) 
            : base(x => x.Id == id)
        {
        }
  
    }
}