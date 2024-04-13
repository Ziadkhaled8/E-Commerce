using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IjwtTokenGenetrator _jwtTokenGenetor;

        public AuthService(AppDbContext db,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, IjwtTokenGenetrator ijwtTokenGenetrator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenetor = ijwtTokenGenetrator;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user=_db.ApplicationUsers.FirstOrDefault(u=>u.UserName.ToLower()==loginRequestDto.Username.ToLower());
            bool isValid=await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if(isValid==false || user==null)
            {
                return new LoginResponseDto() { User = null,Token="" };
            }

            var token = _jwtTokenGenetor.GenerateToken(user);

            UserDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User=userDto,Token=token
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegisterationRequesteDto registerationRequesteDto)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequesteDto.Email,
                Email = registerationRequesteDto.Email,
                NormalizedEmail = registerationRequesteDto.Email.ToUpper(),
                Name = registerationRequesteDto.Name,
                PhoneNumber = registerationRequesteDto.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequesteDto.Password);
                if(result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registerationRequesteDto.Email);
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        Id = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                return result.Errors.FirstOrDefault().Description;
            }
            catch(Exception ex)
            {

            }
            return "Error Encountered";
        }
    }
}
