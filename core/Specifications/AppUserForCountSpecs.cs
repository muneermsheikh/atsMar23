using core.Entities.Identity;
using core.Params;

namespace core.Specifications
{
     public class AppUserForCountSpecs : BaseSpecification<AppUser>
     {
          public AppUserForCountSpecs(AppUserSpecParams specParams)
            : base(x => 
          /*
                (!string.IsNullOrEmpty(specParams.City) ||
                  x.Address.City.ToLower().Contains(specParams.City.ToLower())) &&
                (!string.IsNullOrEmpty(specParams.District) ||
                  x.Address.District.ToLower().Contains(specParams.District.ToLower())) &&
          */
                (!string.IsNullOrEmpty(specParams.Email) ||
                  x.Email.ToLower() == specParams.Email.ToLower()) &&
                (!string.IsNullOrEmpty(specParams.Mobile) || x.PhoneNumber == specParams.Mobile)
              )
          {
          }
          
          public AppUserForCountSpecs(string appUserId) 
            : base(x => x.Id == appUserId)
          {
          }
          
     }
}