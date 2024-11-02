using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Mappers;
using WhatsJustLike24.Server.Data.Models;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Services;

namespace WhatsJustLike24.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly MovieApiService _movieApiService;
        private readonly ApplicationDbContext _context;
        private readonly MovieDTOMapper _movieDTOMapper;

        public MoviesController(ApplicationDbContext context, MovieApiService movieApiService, MovieDTOMapper movieDTOMapper)
        {
            _context = context;
            _movieApiService = movieApiService;
            _movieDTOMapper = movieDTOMapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string movieQuery)
        {
            try
            {
                var movie = await _movieApiService.CreateMovieAsync(movieQuery);

                var movieDTO = _movieDTOMapper.MapToDTO(movie);
                return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movieDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // READ: GET /Movies
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _context.Movies.ToListAsync();
            return Ok(movies);
        }

        // READ: GET /Movies/5
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        // READ: GET /Movies/title?name=Inception
        [HttpGet("title"), AllowAnonymous]
        public async Task<IActionResult> GetByTitle([FromQuery] string name)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Title.ToLower() == name.ToLower());

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }



        // UPDATE: PUT /Movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Movie updatedMovie)
        {
            if (id != updatedMovie.Id)
            {
                return BadRequest();
            }

            _context.Entry(updatedMovie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
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

        // DELETE: DELETE /Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("DetailsByTitle"), AllowAnonymous]
        public async Task<ActionResult<MovieDetailsAndTitleDTO>> GetMovieDetailsByTitle(string title)
        {
            var result = from movie in _context.Movies
                         join detail in _context.MovieDetails
                         on movie.Id equals detail.MovieId
                         where movie.Title == title
                         select new
                         {
                             movie.Title,
                             detail.Genre,
                             detail.Director,
                             detail.Description,
                             detail.PosterPath
                         };

            var movieData = result.FirstOrDefault();

            if (movieData == null)
            {
                return NotFound();
            }

            return Ok(movieData);
        }


        //[Authorize]
        [HttpGet("SimilarByTitle"), AllowAnonymous]
        public async Task<ActionResult<List<SimilarityByTitleDTO>>> GetSimilarMoviesByTitle(string title)
        {
            var similarMovies = await _context.Set<SimilarityByTitleDTO>()
                .FromSqlInterpolated($@"
                     SELECT 
	                    m2.Id
	                    , m2.Title
	                    , md2.PosterPath
	                    , AVG(il.SimilarityScore) AS AverageSimilarityScore
	                    , COUNT(il.SimilarityScore) AS SimilarityScoreCount
	                    , STRING_AGG(il.description, '; ') WITHIN GROUP (ORDER BY il.description) AS DescriptionList
	                    , STRING_AGG(il.SimilarityScore, '; ') WITHIN GROUP (ORDER BY il.description) AS SimilarityScoreList
                     FROM Movies m
                     JOIN MovieIsLike mil ON (m.Id = mil.MovieIdA OR m.Id = mil.MovieIdB) AND m.Title = {title}
                     JOIN Movies m2 ON (m2.Id = mil.MovieIdA OR m2.Id = mil.MovieIdB) AND m2.Id != m.Id
                     LEFT JOIN MovieDetails md2 ON m2.Id = md2.MovieId
                     LEFT JOIN IsLikeDetails il ON mil.Id = il.MovieIsLikeId
                     GROUP BY m2.Id, m2.Title, md2.PosterPath
                    ")
                .ToListAsync();

            return similarMovies.Select(m => new SimilarityByTitleDTO
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
                FROM Movies AS A
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



        // POST: /Movies/AddSimilarity
        [Authorize]
        [HttpPost("AddSimilarity")]
        public async Task<IActionResult> AddMovieSimilarity([FromBody] MovieSimilarityRequest request)
        {
            Movie movieA = await _context.Movies
                .FirstOrDefaultAsync(m => m.Title.ToLower() == request.TitleMovieA.ToLower())
                ?? await _movieApiService.CreateMovieAsync(request.TitleMovieA);
            Movie movieB = await _context.Movies
                .FirstOrDefaultAsync(m => m.Title.ToLower() == request.TitleMovieB.ToLower())
                ?? await _movieApiService.CreateMovieAsync(request.TitleMovieB);

            bool isNewMovieA = movieA.Id == 0;
            bool isNewMovieB = movieB.Id == 0;

            if (isNewMovieA)
            {
                _context.Movies.Add(movieA);
            }
            if (isNewMovieB)
            {
                _context.Movies.Add(movieB);
            }

            // Save the movies if they are new
            if (isNewMovieA || isNewMovieB)
            {
                await _context.SaveChangesAsync();
            }

            MovieIsLike existingSimilarity = await _context.MovieIsLike
                .FirstOrDefaultAsync(il => (il.MovieIdA == movieA.Id && il.MovieIdB == movieB.Id) ||
                (il.MovieIdA == movieB.Id && il.MovieIdB == movieA.Id));

            if (existingSimilarity == null)
            {
                existingSimilarity = new MovieIsLike
                {
                    MovieIdA = movieA.Id,
                    MovieIdB = movieB.Id
                };

                _context.MovieIsLike.Add(existingSimilarity);
                await _context.SaveChangesAsync();
            }

            // Map the MovieIsLike entity to MovieIsLikeDto
            var movieIsLikeDto = new MovieIsLikeDTO
            {
                Id = existingSimilarity.Id,
                MovieIdA = existingSimilarity.MovieIdA,
                MovieIdB = existingSimilarity.MovieIdB,
            };

            // Add entry to MovieIsLikeDetails
            var similarityDetail = new IsLikeDetails
            {
                MovieIsLikeId = existingSimilarity.Id,
                SimilarityScore = request.SimilarityScore,
                Description = request.Description
            };

            _context.IsLikeDetails.Add(similarityDetail);
            await _context.SaveChangesAsync();

            // Map the MovieIsLikeDetails entity to MovieIsLikeDetailDto
            var movieIsLikeDetailDto = new IsLikeDetailDTO
            {
                Id = similarityDetail.Id,
                MovieIsLikeId = similarityDetail.MovieIsLikeId,
                SimilarityScore = similarityDetail.SimilarityScore,
                Description = similarityDetail.Description
            };

            return Ok(new { Similarity = movieIsLikeDto, Details = movieIsLikeDetailDto });
        }




        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        public class MovieSimilarityRequest
        {
            public string TitleMovieA { get; set; }
            public string TitleMovieB { get; set; }
            public int SimilarityScore { get; set; }
            public string Description { get; set; }  // Optional, based on whether you'd like a description.
        }
    }
}
