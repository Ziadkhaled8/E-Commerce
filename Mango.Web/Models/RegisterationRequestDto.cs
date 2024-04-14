namespace Mango.Mango.Web.Models
{
    public class RegisterationRequesteDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? roleName { get; set; }
    }
}
