using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using Azure;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _responseDto;
        private IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        private readonly IProductService _productService;

        public OrderAPIController(IMapper mapper, AppDbContext appDbContext, IProductService productService)
        {
            _responseDto = new ResponseDto();
            _mapper = mapper;
            _appDbContext = appDbContext;
            _productService = productService;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ResponseDto>? Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orders;
                if (User.IsInRole(SD.RoleAdmin))
                {
                     orders = await _appDbContext.OrderHeaders.Include(o => o.OrderDetails).OrderByDescending(o => o.OrderHeaderId).ToListAsync();
                }
                else
                {
                     orders=await _appDbContext.OrderHeaders.Include(o=>o.OrderDetails).Where(o=>o.UserId== userId).OrderByDescending(o=>o.OrderHeaderId).ToListAsync();
                }
                _responseDto.Result= _mapper.Map<IEnumerable<OrderHeaderDto>>(orders); ;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Messege = ex.Message;
            }
            return _responseDto;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _appDbContext.OrderHeaders.Include(u => u.OrderDetails).First(u => u.OrderHeaderId == id);
                _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Messege = ex.Message;
            }
            return _responseDto;
        }


        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody]CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto=_mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated= _appDbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _appDbContext.SaveChangesAsync();
                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _responseDto.Result= orderHeaderDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Messege=ex.Message;
            }
            return _responseDto;
        }
        
        
        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",

                };

                /*var DiscountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon=stripeRequestDto.OrderHeader.CouponCode
                    }
                };*/

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.99 -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                /*if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountsObj;
                }*/
                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _appDbContext.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _appDbContext.SaveChanges();
                _responseDto.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Messege = ex.Message;
            }
            return _responseDto;
        }
        
        
        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _appDbContext.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);
                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);
                var PaymentIntentService=new PaymentIntentService();
                PaymentIntent paymentIntent = PaymentIntentService.Get(session.PaymentIntentId);
                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntenId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    _appDbContext.SaveChanges();
                    _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
                
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Messege = ex.Message;
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public ResponseDto UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _appDbContext.OrderHeaders.First(o => o.OrderHeaderId == orderId);
                if(orderHeader != null)
                {
                    if(newStatus==SD.Status_Cancelled)
                    {
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntenId
                        };
                        var service= new RefundService();
                        Refund refund = service.Create(options);
                    }
                    orderHeader.Status = newStatus;
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Messege = ex.Message;
            }
            return _responseDto;
        }

    }
}
