using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.DTOs
{
    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
