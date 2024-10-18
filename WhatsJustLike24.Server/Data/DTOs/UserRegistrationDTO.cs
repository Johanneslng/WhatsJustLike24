using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.DTOs
{
    public class UserRegistrationDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
