using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public CouponController(AppDbContext db,IMapper mapper )
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> obj = _db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess= false;
                _response.Messege= ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(c => c.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Messege = ex.Message;
            }
            return _response;
        }
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon obj = _db.Coupons.First(c => c.CouponCode.ToLower() == code.ToLower());
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Messege = ex.Message;
            }
            return _response;
        }
        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto Create([FromBody]CouponDto coupon)
        {
            try
            {
                Coupon model=_mapper.Map<Coupon>(coupon);
                _db.Coupons.Add(model);
                _db.SaveChanges();

                _response.Result= _mapper.Map<CouponDto>(model);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Messege = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Update([FromBody]CouponDto coupon)
        {
            try
            {
                    Coupon model = _mapper.Map<Coupon>(coupon);
                    _db.Coupons.Update(model);
                    _db.SaveChanges();
                    _response.Result = _mapper.Map<CouponDto>(model);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Messege = ex.Message;
            }
            return _response;
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(c => c.CouponId == id);
                _db.Coupons.Remove(obj);
                    _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Messege = ex.Message;
            }
            return _response;
        }
    }
}
