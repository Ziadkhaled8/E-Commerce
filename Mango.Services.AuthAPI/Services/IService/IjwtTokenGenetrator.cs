using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Services.IService
{
    public interface IjwtTokenGenetrator
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}
