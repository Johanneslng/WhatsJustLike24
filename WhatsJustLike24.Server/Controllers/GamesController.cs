using Microsoft.AspNetCore.Mvc;
using WhatsJustLike24.Server.Data.Mappers;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Services;
using Microsoft.AspNetCore.Authorization;
namespace WhatsJustLike24.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameApiService _gameApiService;
        private readonly TokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly MovieDTOMapper _movieDTOMapper;

        public GamesController(ApplicationDbContext context, GameApiService gameApiService, MovieDTOMapper movieDTOMapper, TokenService tokenService)
        {
            _context = context;
            _gameApiService = gameApiService;
            _movieDTOMapper = movieDTOMapper;
            _tokenService = tokenService;
        }

        [HttpPost("token"), AllowAnonymous]
        public async Task<IActionResult> RetrieveTwitchToken()
        {
            var token = await _tokenService.GetTwitchTokenAsync();
            return Ok(token);
        }

        [HttpGet("Game"), AllowAnonymous]
        public async Task<IActionResult> GetGame([FromQuery] string name)
        {
            var Game = await _gameApiService.GetGameAsync(name);
            return Ok(Game);
        }
    }
}