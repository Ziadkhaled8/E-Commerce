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

        public AuthService(AppDbContext db,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> Register(RegisterationRequesteDto registerationRequesteDto)
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
                    return userDto;
                }
            }
            catch(Exception ex)
            {

            }
            return new UserDto();
        }
    }
}
