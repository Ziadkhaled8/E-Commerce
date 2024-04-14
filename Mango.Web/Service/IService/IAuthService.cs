using Mango.Mango.Web.Models;
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegisterationRequesteDto registerRequestDto);
        Task<ResponseDto?> AssignRole(RegisterationRequesteDto registerRequestDto);
    }
}
