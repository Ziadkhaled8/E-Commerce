using Mango.Mango.Web.Models;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            List<CouponDto> list=new List<CouponDto>();
            ResponseDto? response=await _couponService.GetALlCouponsAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Messege;
            }
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponDto couponDto)
        {
            if (ModelState.IsValid)
            {
                CouponDto model= new CouponDto();
                ResponseDto? response = await _couponService.CreateCouponAsync(couponDto);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Messege;
                }
            }
            return View(couponDto);  
        }
        public async Task<IActionResult> Delete(int couponId)
        {
            ResponseDto? result=await _couponService.DeleteCouponAsync(couponId);
            if(result != null && result.IsSuccess)
            {
                TempData["success"] = "Coupon Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            { 
                TempData["error"] = result?.Messege;
            }
            return RedirectToAction(nameof(Index));

        }
    }
}
