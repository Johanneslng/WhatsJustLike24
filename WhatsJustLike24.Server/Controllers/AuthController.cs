using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Identity;

namespace WhatsJustLike24.Server.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AuthController(
            SignInManager<AppUser> signInManager, 
            UserManager<AppUser> userManager
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpPost]
        [Route("auth/signup")]
        public async Task<IActionResult> Signup([FromBody] UserRegistrationDTO userRegistrationDTO)
        {

            var test = userRegistrationDTO;
            var existingUser = await _userManager.FindByEmailAsync(userRegistrationDTO.Email);
            
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email is already in use." });
            }

            AppUser user = new AppUser()
            {
                Email = userRegistrationDTO.Email,
                FullName = userRegistrationDTO.FullName,
                UserName = userRegistrationDTO.Email
            };
            var result = await _userManager.CreateAsync(user, userRegistrationDTO.Password);

            if (result.Succeeded)
            {
                return Ok(new { succeeded = true, message = "User registered successfully", result });
            }

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { succeeded = false, message = "User registration failed", errors });
        }


        [HttpPost]
        [Route("auth/logout")]
        public async Task<IActionResult> Logout([FromBody] object empty)
        {
            if (empty != null)
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }

            return Unauthorized();
        }
    }
}
