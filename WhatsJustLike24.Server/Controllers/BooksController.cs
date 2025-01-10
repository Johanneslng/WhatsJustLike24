using Microsoft.AspNetCore.Mvc;
using WhatsJustLike24.Server.Data.Mappers;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Services;
using Microsoft.AspNetCore.Authorization;
using WhatsJustLike24.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Requests;
using Microsoft.Data.SqlClient;
namespace WhatsJustLike24.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookApiService _bookApiService;
        private readonly TokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly BookDTOMapper _bookDTOMapper;

        public BooksController(ApplicationDbContext context, BookApiService bookApiService, BookDTOMapper bookDTOMapper)
        {
            _context = context;
            _bookApiService = bookApiService;
            _bookDTOMapper = bookDTOMapper;
        }


        [HttpGet("title"), AllowAnonymous]
        public async Task<IActionResult> GetBook([FromQuery] string name)
        {
            var Book = await _bookApiService.GetBookAsync(name);
            return Ok(Book);
        }
        
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] string query)
        {
            try
            {
                var book = await _bookApiService.CreateBookAsync(query);

                var bookDTO = _bookDTOMapper.MapToDTO(book);
                return CreatedAtAction(nameof(GetById), new { id = book.Id }, bookDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // READ: GET /Book/5
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet("DetailsByTitle"), AllowAnonymous]
        public async Task<ActionResult<BookDetailsAndTitleDTO>> GetBookDetailsByTitle(string title)
        {
            var result = from book in _context.Books
                         join detail in _context.BookDetails
                         on book.Id equals detail.BookId
                         where book.Title == title
                         select new
                         {
                             book.Title,
                             detail.Genre,
                             detail.Author,
                             detail.FirstRelease,
                             detail.Cover,
                             detail.Publisher,
                             detail.Description,
                             detail.Isbn,
                             detail.Series,
                             detail.Pages,
                             detail.Languages
                         };

            var data = result.FirstOrDefault();

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        //[Authorize]
        [HttpGet("SimilarByTitle"), AllowAnonymous]
        public async Task<ActionResult<List<SimilarityByTitleDTO>>> GetSimilarBooksByTitle(string title)
        {
            var similarBooks = await _context.GetBookSimilarityDetails(title).ToListAsync();

            return similarBooks.Select(m => new SimilarityByTitleDTO
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
                        FROM Books AS A
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

        // POST: /Books/AddSimilarity
        [HttpPost("AddSimilarity"), AllowAnonymous]
        public async Task<IActionResult> AddBookSimilarity([FromBody] SimilarityRequest request)
        {
            Book bookA = await _context.Books
                .FirstOrDefaultAsync(m => m.Title.ToLower() == request.TitleA.ToLower())
                ?? await _bookApiService.CreateBookAsync(request.TitleA);
            Book bookB = await _context.Books
                .FirstOrDefaultAsync(m => m.Title.ToLower() == request.TitleB.ToLower())
                ?? await _bookApiService.CreateBookAsync(request.TitleB);

            bool isNewBookA = bookA.Id == 0;
            bool isNewBookB = bookB.Id == 0;

            if (isNewBookA)
            {
                _context.Books.Add(bookA);
            }
            if (isNewBookB)
            {
                _context.Books.Add(bookB);
            }

            // Save the games if they are new
            if (isNewBookA || isNewBookB)
            {
                await _context.SaveChangesAsync();
            }

            BookIsLike existingSimilarity = await _context.BookIsLike
                .FirstOrDefaultAsync(il => (il.BookIdA == bookA.Id && il.BookIdB == bookB.Id) ||
                (il.BookIdA == bookB.Id && il.BookIdB == bookA.Id));

            if (existingSimilarity == null)
            {
                existingSimilarity = new BookIsLike
                {
                    BookIdA = bookA.Id,
                    BookIdB = bookB.Id
                };

                _context.BookIsLike.Add(existingSimilarity);
                await _context.SaveChangesAsync();
            }

            var BookIsLikeDto = new BookIsLikeDTO
            {
                Id = existingSimilarity.Id,
                BookIdA = existingSimilarity.BookIdA,
                BookIdB = existingSimilarity.BookIdB,
            };

            var similarityDetail = new BookIsLikeDetails
            {
                BookIsLikeId = existingSimilarity.Id,
                SimilarityScore = request.SimilarityScore,
                Description = request.Description
            };

            _context.BookIsLikeDetails.Add(similarityDetail);
            await _context.SaveChangesAsync();

            var bookIsLikeDetailDto = new BookIsLikeDetailDTO
            {
                Id = similarityDetail.Id,
                BookIsLikeId = similarityDetail.BookIsLikeId,
                SimilarityScore = similarityDetail.SimilarityScore,
                Description = similarityDetail.Description
            };

            return Ok(new { Similarity = BookIsLikeDto, Details = bookIsLikeDetailDto });
        }
    }
}