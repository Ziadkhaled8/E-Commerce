using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public CartAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb=await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u=>u.UserId==cartDto.CartHeader.UserId);
                if (cartHeaderFromDb==null)
                {
                    //create header and details
                    CartHeader cartHeader=_mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderID = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //check if the details has the same product
                    var cartDetailsFromDb=await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u=>u.ProductId==cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderID== cartHeaderFromDb.CartHeaderId);
                    if(cartDetailsFromDb==null)
                    {
                        //create cartDetails
                        cartDto.CartDetails.First().CartHeaderID = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderID = cartDetailsFromDb.CartHeaderID;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;

            }
            catch (Exception ex)
            {
                _response.Messege = ex.Message.ToString();
                _response.IsSuccess= false;
            }
            return _response;
        } [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cartdetailsId)
        {
            try
            {
                CartDetails cartDetails=_db.CartDetails.First(u=>u.CartDetailsId==cartdetailsId);
                int totalCountItems = _db.CartDetails.Where(u => u.CartHeaderID == cartDetails.CartHeaderID).Count();
                _db.CartDetails.Remove(cartDetails);
                if(totalCountItems==1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderID);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                _response.Result = true;
                
            }
            catch (Exception ex)
            {
                _response.Messege = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
