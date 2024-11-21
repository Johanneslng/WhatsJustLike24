using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Identity;

namespace WhatsJustLike24.Server.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        public AuthController(
            SignInManager<AppUser> signInManager, 
            UserManager<AppUser> userManager,
            IConfiguration configuration
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
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

        [HttpPost, AllowAnonymous]
        [Route("auth/signin")]
        public async Task<IActionResult> Signin([FromBody] UserLoginDTO userLoginDTO)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDTO.Email);
            if(user != null && await _userManager.CheckPasswordAsync(user, userLoginDTO.Password))
            {
                var signInKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _configuration["JWT:SigningKey"]!));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID", user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(
                        signInKey,
                        SecurityAlgorithms.HmacSha256Signature
                        )
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new
                {
                    token = token,
                    expiration = tokenDescriptor.Expires
                });
            }
            else
                return BadRequest(new { message = "Username or password is incorrect."});
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

        [HttpGet("UserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            string userId = User.Claims.First(x => x.Type == "UserID").Value;

            var userDetails = await _userManager.FindByIdAsync(userId);

            if (userDetails == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            return Ok(new
            {
                Email = userDetails.Email,
                FullName = userDetails.FullName
            });
        }

        [HttpGet("CheckAuth")]
        public Boolean CheckAuth()
        {
            return true;
        }
    }
}
