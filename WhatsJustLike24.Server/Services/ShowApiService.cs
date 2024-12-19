using RestSharp;
using System.Text.Json;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Models;

namespace WhatsJustLike24.Server.Services
{
    public class ShowApiService
    {
        private readonly RestClient _client;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ImageBlobService _imageBlobService;
        public ShowApiService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ImageBlobService imageBlobService
            )
        {
            _context = context;
            _configuration = configuration;
            _imageBlobService = imageBlobService;

            var options = new RestClientOptions("https://api.themoviedb.org/3/search/tv");
            {
                // Additional options if needed
            };
            _client = new RestClient(options);
        }

        public async Task<MovieDBShowDTO> GetShowAsync(string query)
        {
            try
            {
                var request = new RestRequest("");
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", _configuration["APIKeys:MovieDB"]);
                request.AddQueryParameter("query", query);
                request.AddQueryParameter("include_adult", "false");
                request.AddQueryParameter("language", "en-US");
                request.AddQueryParameter("page", "1");

                // Send the request
                var response = await _client.GetAsync(request);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception("Failed to retrieve show data from the API.");
                }

                var jsonDocument = JsonDocument.Parse(response.Content);

                // Check if results array exists and is not empty
                var results = jsonDocument.RootElement.GetProperty("results");
                if (results.GetArrayLength() == 0)
                {
                    throw new Exception("No shows found for the given query.");
                }

                var firstResult = results[0];

                var imagePath = "https://image.tmdb.org/t/p/w500" + firstResult.GetProperty("poster_path").GetString();

                // Upload image to Blob Storage
                var blobName = await _imageBlobService.UploadImageFromUrlAsync(imagePath, "shows");

                return new MovieDBShowDTO
                {
                    OriginalLanguage = firstResult.GetProperty("original_language").GetString(),
                    OriginalTitle = firstResult.GetProperty("original_name").GetString(),
                    Summary = firstResult.GetProperty("overview").GetString(),
                    PosterPath = blobName,
                    FirstAirDate = DateTime.Parse(firstResult.GetProperty("first_air_date").GetString())
                };
            }
            catch (Exception ex)
            {
                // Log the error (replace with your logging mechanism)
                Console.WriteLine($"Error in GetShowAsync: {ex.Message}");

                // Optionally rethrow the exception or return a default/fallback value
                throw new Exception("An error occurred while fetching the Shows details.", ex);

            }
        }

        public async Task<Show> CreateShowAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("Show query cannot be empty.");
            }

            var data = await GetShowAsync(query);

            if (data == null)
            {
                throw new InvalidOperationException("Show not found in external API.");
            }

            if (string.IsNullOrEmpty(data.OriginalTitle))
            {
                throw new InvalidOperationException("Original title not found in the movie data.");
            }

            var show = new Show
            {
                Title = data.OriginalTitle,
                ShowDetails = new ShowDetails
                {
                    Description = data.Summary,
                    PosterPath = data.PosterPath,
                    FirstAirDate = data.FirstAirDate,
                    Director = "Placeholder",
                    Genre = "Placeholder"
                }
            };

            _context.Shows.Add(show);
            await _context.SaveChangesAsync();

            return show;
        }
    }
}
