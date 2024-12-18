using Microsoft.AspNetCore.Mvc;
using WhatsJustLike24.Server.Data.Mappers;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WhatsJustLike24.Server.Data.Models;
using WhatsJustLike24.Server.Data.DTOs;

namespace WhatsJustLike24.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ShowApiService _showApiService;
        private readonly ApplicationDbContext _context;
        //private readonly ShowDTOMapper _showDTOMapper;
        public ShowsController(ApplicationDbContext context, ShowApiService showApiService)//, ShowDTOMapper showDTOMapper)
        {
            _context = context;
            _showApiService = showApiService;
            //_showDTOMapper = showDTOMapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string query)
        {
            try
            {
                var show = await _showApiService.CreateShowAsync(query);

                var showDTO = show;//_showDTOMapper.MapToDTO(movie);
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

        private bool ShowExists(int id)
        {
            return _context.Shows.Any(e => e.Id == id);
        }
    }
}
