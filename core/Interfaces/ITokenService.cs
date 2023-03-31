using core.Entities.Identity;

namespace core.Interfaces
{
     public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}