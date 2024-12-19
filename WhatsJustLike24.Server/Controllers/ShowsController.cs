using Microsoft.AspNetCore.Mvc;
using WhatsJustLike24.Server.Data.Mappers;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WhatsJustLike24.Server.Data.Models;
using WhatsJustLike24.Server.Data.DTOs;
using Microsoft.Data.SqlClient;
using WhatsJustLike24.Server.Data.Requests;

namespace WhatsJustLike24.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ShowApiService _showApiService;
        private readonly ApplicationDbContext _context;
        private readonly ShowDTOMapper _showDTOMapper;
        public ShowsController(ApplicationDbContext context, ShowApiService showApiService, ShowDTOMapper showDTOMapper)
        {
            _context = context;
            _showApiService = showApiService;
            _showDTOMapper = showDTOMapper;
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] string query)
        {
            try
            {
                var show = await _showApiService.CreateShowAsync(query);

                var showDTO = _showDTOMapper.MapToDTO(show);
                return CreatedAtAction(nameof(GetById), new { id = show.Id }, showDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shows = await _context.Shows.ToListAsync();
            return Ok(shows);
        }

        // READ: GET /Shows/5
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var show = await _context.Shows.FindAsync(id);

            if (show == null)
            {
                return NotFound();
            }

            return Ok(show);
        }

        // READ: GET /Shows/title?name=Inception
        [HttpGet("title"), AllowAnonymous]
        public async Task<IActionResult> GetByTitle([FromQuery] string name)
        {
            var show = await _context.Shows
                .FirstOrDefaultAsync(s => s.Title.ToLower() == name.ToLower());

            if (show == null)
            {
                return NotFound();
            }

            return Ok(show);
        }

        // UPDATE: PUT /Shows/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Show updatedShow)
        {
            if (id != updatedShow.Id)
            {
                return BadRequest();
            }

            _context.Entry(updatedShow).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: DELETE /Shows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var show = await _context.Shows.FindAsync(id);

            if (show == null)
            {
                return NotFound();
            }

            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("DetailsByTitle"), AllowAnonymous]
        public async Task<ActionResult<MovieDetailsAndTitleDTO>> GetShowDetailsByTitle(string title)
        {
            var result = from show in _context.Shows
                         join detail in _context.ShowDetails
                         on show.Id equals detail.ShowId
                         where show.Title == title
                         select new
                         {
                             show.Title,
                             detail.Genre,
                             detail.Director,
                             detail.Description,
                             detail.PosterPath,
                             detail.FirstAirDate
                         };

            var showData = result.FirstOrDefault();

            if (showData == null)
            {
                return NotFound();
            }

            return Ok(showData);
        }


        //[Authorize]
        [HttpGet("SimilarByTitle"), AllowAnonymous]
        public async Task<ActionResult<List<SimilarityByTitleDTO>>> GetSimilarShowsByTitle(string title)
        {
            var similarShows = await _context.GetShowSimilarityDetails(title).ToListAsync();

            return similarShows.Select(m => new SimilarityByTitleDTO
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
                        FROM Shows AS A
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

        // POST: /Shows/AddSimilarity
        //[Authorize]
        [HttpPost("AddSimilarity"), AllowAnonymous]
        public async Task<IActionResult> AddShowSimilarity([FromBody] SimilarityRequest request)
        {
            Show showA = await _context.Shows
                .FirstOrDefaultAsync(m => m.Title.ToLower() == request.TitleA.ToLower())
                ?? await _showApiService.CreateShowAsync(request.TitleA);
            Show showB = await _context.Shows
                .FirstOrDefaultAsync(m => m.Title.ToLower() == request.TitleB.ToLower())
                ?? await _showApiService.CreateShowAsync(request.TitleB);

            bool isNewShowA = showA.Id == 0;
            bool isNewShowB = showB.Id == 0;

            if (isNewShowA)
            {
                _context.Shows.Add(showA);
            }
            if (isNewShowB)
            {
                _context.Shows.Add(showB);
            }

            // Save the movies if they are new
            if (isNewShowA || isNewShowB)
            {
                await _context.SaveChangesAsync();
            }

            ShowIsLike existingSimilarity = await _context.ShowIsLike
                .FirstOrDefaultAsync(il => (il.ShowIdA == showA.Id && il.ShowIdB == showB.Id) ||
                (il.ShowIdA == showB.Id && il.ShowIdB == showA.Id));

            if (existingSimilarity == null)
            {
                existingSimilarity = new ShowIsLike
                {
                    ShowIdA = showA.Id,
                    ShowIdB = showB.Id
                };

                _context.ShowIsLike.Add(existingSimilarity);
                await _context.SaveChangesAsync();
            }

            // Map the MovieIsLike entity to MovieIsLikeDto
            var showIsLikeDto = new ShowIsLikeDTO
            {
                Id = existingSimilarity.Id,
                ShowIdA = existingSimilarity.ShowIdA,
                ShowIdB = existingSimilarity.ShowIdB,
            };

            // Add entry to MovieIsLikeDetails
            var similarityDetail = new ShowIsLikeDetails
            {
                ShowIsLikeId = existingSimilarity.Id,
                SimilarityScore = request.SimilarityScore,
                Description = request.Description
            };

            _context.ShowIsLikeDetails.Add(similarityDetail);
            await _context.SaveChangesAsync();

            // Map the MovieIsLikeDetails entity to MovieIsLikeDetailDto
            var showIsLikeDetailDto = new ShowIsLikeDetailDTO
            {
                Id = similarityDetail.Id,
                ShowIsLikeId = similarityDetail.ShowIsLikeId,
                SimilarityScore = similarityDetail.SimilarityScore,
                Description = similarityDetail.Description
            };

            return Ok(new { Similarity = showIsLikeDto, Details = showIsLikeDetailDto });
        }

        private bool ShowExists(int id)
        {
            return _context.Shows.Any(e => e.Id == id);
        }
    }
}
