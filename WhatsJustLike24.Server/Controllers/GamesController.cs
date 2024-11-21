using Microsoft.AspNetCore.Mvc;
using WhatsJustLike24.Server.Data.Mappers;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Services;
using Microsoft.AspNetCore.Authorization;
using WhatsJustLike24.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using static WhatsJustLike24.Server.Controllers.MoviesController;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Requests;
using static Azure.Core.HttpHeader;
using Microsoft.Data.SqlClient;
namespace WhatsJustLike24.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameApiService _gameApiService;
        private readonly TokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly GameDTOMapper _gameDTOMapper;

        public GamesController(ApplicationDbContext context, GameApiService gameApiService, GameDTOMapper gameDTOMapper, TokenService tokenService)
        {
            _context = context;
            _gameApiService = gameApiService;
            _gameDTOMapper = gameDTOMapper;
            _tokenService = tokenService;
        }

        [HttpPost("token"), AllowAnonymous]
        public async Task<IActionResult> RetrieveTwitchToken()
        {
            var token = await _tokenService.GetTwitchTokenAsync();
            return Ok(token);
        }

        [HttpGet("title"), AllowAnonymous]
        public async Task<IActionResult> GetGame([FromQuery] string name)
        {
            var Game = await _gameApiService.GetGameAsync(name);
            return Ok(Game);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] string gameQuery)
        {
            try
            {
                var game = await _gameApiService.CreateGameAsync(gameQuery);

                var gameDTO = _gameDTOMapper.MapToDTO(game);
                return CreatedAtAction(nameof(GetById), new { id = game.Id }, gameDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // READ: GET /Games/5
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        [HttpGet("DetailsByTitle"), AllowAnonymous]
        public async Task<ActionResult<GameDetailsAndTitleDTO>> GetGameDetailsByTitle(string title)
        {
            var result = from game in _context.Games
                         join detail in _context.GameDetails
                         on game.Id equals detail.GameId
                         where game.Title == title
                         select new
                         {
                             game.Title,
                             detail.Genre,
                             detail.Developer,
                             detail.FirstRelease,
                             detail.Cover,
                             detail.Platforms,
                             detail.Description
                         };

            var gameData = result.FirstOrDefault();

            if (gameData == null)
            {
                return NotFound();
            }

            return Ok(gameData);
        }

        //[Authorize]
        [HttpGet("SimilarByTitle"), AllowAnonymous]
        public async Task<ActionResult<List<SimilarityByTitleDTO>>> GetSimilarGamesByTitle(string title)
        {
            var similarGames = await _context.GetGameSimilarityDetails(title).ToListAsync();

            return similarGames.Select(m => new SimilarityByTitleDTO
            {
                Title = m.Title,
                PosterPath = m.PosterPath ?? "1E5baAaEse26fej7uHcjOgEE2t2.jpg",
                AverageSimilarityScore = m.AverageSimilarityScore ?? 100,
                SimilarityScoreCount = m.SimilarityScoreCount,
                DescriptionList = m.DescriptionList ?? "",
                SimilarityScoreList = m.SimilarityScoreList ?? "0"
            }).ToList();
        }

        [HttpGet("SimilarTitles"), AllowAnonymous]
        public async Task<ActionResult<string>> GetTheMostSimilarTitlesByDistance(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title is required.");
            }

            try
            {
                string connectionString = _context.Database.GetConnectionString(); // Or get it from IConfiguration
                string similarTitle = null;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                        SELECT TOP 1 A.Title
                        FROM Games AS A
                        ORDER BY DIFFERENCE(@Title, A.Title) DESC, dbo.LEVENSHTEIN(@Title, A.Title) ASC", connection);
                    command.Parameters.AddWithValue("@Title", title);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            similarTitle = reader.GetString(0);
                        }
                    }
                }

                if (similarTitle == null)
                {
                    return NotFound("No similar titles found.");
                }

                return Ok(similarTitle);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: /Games/AddSimilarity
        [HttpPost("AddSimilarity"), AllowAnonymous]
        public async Task<IActionResult> AddGameSimilarity([FromBody] SimilarityRequest request)
        {
            Game gameA = await _context.Games
                .FirstOrDefaultAsync(m => m.Title.ToLower() == request.TitleA.ToLower())
                ?? await _gameApiService.CreateGameAsync(request.TitleA);
            Game gameB = await _context.Games
                .FirstOrDefaultAsync(m => m.Title.ToLower() == request.TitleB.ToLower())
                ?? await _gameApiService.CreateGameAsync(request.TitleB);

            bool isNewGameA = gameA.Id == 0;
            bool isNewGameB = gameB.Id == 0;

            if (isNewGameA)
            {
                _context.Games.Add(gameA);
            }
            if (isNewGameB)
            {
                _context.Games.Add(gameB);
            }

            // Save the games if they are new
            if (isNewGameA || isNewGameB)
            {
                await _context.SaveChangesAsync();
            }

            GameIsLike existingSimilarity = await _context.GameIsLike
                .FirstOrDefaultAsync(il => (il.GameIdA == gameA.Id && il.GameIdB == gameB.Id) ||
                (il.GameIdA == gameB.Id && il.GameIdB == gameA.Id));

            if (existingSimilarity == null)
            {
                existingSimilarity = new GameIsLike
                {
                    GameIdA = gameA.Id,
                    GameIdB = gameB.Id
                };

                _context.GameIsLike.Add(existingSimilarity);
                await _context.SaveChangesAsync();
            }

            var gameIsLikeDto = new GameIsLikeDTO
            {
                Id = existingSimilarity.Id,
                GameIdA = existingSimilarity.GameIdA,
                GameIdB = existingSimilarity.GameIdB,
            };

            var similarityDetail = new GameIsLikeDetails
            {
                GameIsLikeId = existingSimilarity.Id,
                SimilarityScore = request.SimilarityScore,
                Description = request.Description
            };

            _context.GameIsLikeDetails.Add(similarityDetail);
            await _context.SaveChangesAsync();

            var gameIsLikeDetailDto = new GameIsLikeDetailDTO
            {
                Id = similarityDetail.Id,
                GameIsLikeId = similarityDetail.GameIsLikeId,
                SimilarityScore = similarityDetail.SimilarityScore,
                Description = similarityDetail.Description
            };

            return Ok(new { Similarity = gameIsLikeDto, Details = gameIsLikeDetailDto });
        }
        
    }
}