using Mango.Mango.Web.Models;
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICouponService
    {
        Task<ResponseDto> GetCouponByIdAsync(string id);
        Task<ResponseDto> GetCouponAsync(string code);
        Task<ResponseDto> GetALlCouponsAsync();
        Task<ResponseDto> CreateCouponAsync(CouponDto couponDto);
        Task<ResponseDto> UpdateCouponAsync(CouponDto couponDto);
        Task<ResponseDto> DelteCouponAsync(int id);
    }
}
