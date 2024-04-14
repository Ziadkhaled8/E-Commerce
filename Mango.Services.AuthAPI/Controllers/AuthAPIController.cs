using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequesteDto model)
        {
            var errorMessage= await _authService.Register(model);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Messege=errorMessage;
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Messege = "Username Or Password is incorrect";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }
        
        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterationRequesteDto model)
        {
            var assginRoleSuccess = await _authService.AssignRole(model.Email,model.roleName.ToUpper());
            if (!assginRoleSuccess)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Messege = "Error Encountered";
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }
    }
}
