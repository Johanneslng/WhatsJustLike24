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
        public MovieApiService(ApplicationDbContext context)
        {
            _context = context;

            var options = new RestClientOptions("https://api.themoviedb.org/3/search/movie");
            {
                // Additional options if needed
            };
            _client = new RestClient(options);
        }
        string token = Environment.GetEnvironmentVariable("MOVIEDB_API_TOKEN");

        public async Task<MovieDBMovieDTO> GetMovieAsync(string query)
        {
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIwZWIxMDJlYmQxNDgyMWI2MWIyNmU0OWVlMjQwOWU0ZiIsInN1YiI6IjY1MmZkOGY1Y2FlZjJkMDBlMjhkYTJjYSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.4JAoRDa00krt-Pm7Wz5p4gnj7uzscJ2c_UB0Mne11oc");
            request.AddQueryParameter("query", query);
            request.AddQueryParameter("include_adult", "false");
            request.AddQueryParameter("language", "en-US");
            request.AddQueryParameter("page", "1");

            var response = await _client.GetAsync(request);
            var jsonDocument = JsonDocument.Parse(response.Content);
            var firstResult = jsonDocument.RootElement.GetProperty("results")[0];

            return new MovieDBMovieDTO
            {
                OriginalLanguage = firstResult.GetProperty("original_language").GetString(),
                OriginalTitle = firstResult.GetProperty("title").GetString(),
                Summary = firstResult.GetProperty("overview").GetString(),
                PosterPath = firstResult.GetProperty("poster_path").GetString()
            };
        }

        public async Task<Movie> CreateMovieAsync(string movieQuery)
        {
            if (string.IsNullOrEmpty(movieQuery))
            {
                throw new ArgumentException("Movie query cannot be empty.");
            }

            var movieData = await GetMovieAsync(movieQuery); // Assuming GetMovieAsync is another method in this service

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
