using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatsJustLike24.Server.Data.Identity
{
    public class AppUser:IdentityUser
    {
        [PersonalData]
        [Column(TypeName="nvarchar(150)")]
        public string FullName { get; set; }
    }
}
