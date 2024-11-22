using System.Text.Json;
using RestSharp;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Models;

namespace WhatsJustLike24.Server.Services
{
    public class MovieApiService
    {
        private readonly RestClient _client;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ImageBlobService _imageBlobService;
        public MovieApiService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ImageBlobService imageBlobService
            )
        {
            _context = context;
            _configuration = configuration;
            _imageBlobService = imageBlobService;

            var options = new RestClientOptions("https://api.themoviedb.org/3/search/movie");
            {
                // Additional options if needed
            };
            _client = new RestClient(options);
        }

        public async Task<MovieDBMovieDTO> GetMovieAsync(string query)
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
                    throw new Exception("Failed to retrieve movie data from the API.");
                }

                var jsonDocument = JsonDocument.Parse(response.Content);

                // Check if results array exists and is not empty
                var results = jsonDocument.RootElement.GetProperty("results");
                if (results.GetArrayLength() == 0)
                {
                    throw new Exception("No movies found for the given query.");
                }

                var firstResult = results[0];

                var imagePath = "https://image.tmdb.org/t/p/w500" + firstResult.GetProperty("poster_path").GetString();

                // Upload image to Blob Storage
                var blobName = await _imageBlobService.UploadImageFromUrlAsync(imagePath, "movies");

                return new MovieDBMovieDTO
                {
                    OriginalLanguage = firstResult.GetProperty("original_language").GetString(),
                    OriginalTitle = firstResult.GetProperty("title").GetString(),
                    Summary = firstResult.GetProperty("overview").GetString(),
                    PosterPath = blobName
                };
            }
            catch (Exception ex)
            {
                // Log the error (replace with your logging mechanism)
                Console.WriteLine($"Error in GetMovieAsync: {ex.Message}");

                // Optionally rethrow the exception or return a default/fallback value
                throw new Exception("An error occurred while fetching the movie details.", ex);
            }
        }


        public async Task<Movie> CreateMovieAsync(string movieQuery)
        {
            if (string.IsNullOrEmpty(movieQuery))
            {
                throw new ArgumentException("Movie query cannot be empty.");
            }

            var movieData = await GetMovieAsync(movieQuery);

            if (movieData == null)
            {
                throw new InvalidOperationException("Movie not found in external API.");
            }

            if (string.IsNullOrEmpty(movieData.OriginalTitle))
            {
                throw new InvalidOperationException("Original title not found in the movie data.");
            }

            var movie = new Movie
            {
                Title = movieData.OriginalTitle,
                MovieDetails = new MovieDetails
                {
                    Description = movieData.Summary,
                    PosterPath = movieData.PosterPath,
                    Director = "Placeholder",
                    Genre = "Placeholder"
                }
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return movie;
        }
    }
}
