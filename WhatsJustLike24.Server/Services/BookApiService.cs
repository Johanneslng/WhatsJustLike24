using System.Text.Json;
using RestSharp;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Models;

namespace WhatsJustLike24.Server.Services
{
    public class BookApiService
    {
        private readonly RestClient _client;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ImageBlobService _imageBlobService;
        public BookApiService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ImageBlobService imageBlobService
            )
        {
            _context = context;
            _configuration = configuration;
            _imageBlobService = imageBlobService;

            var options = new RestClientOptions("https://openlibrary.org/search.json");
            {
                // Additional options if needed
            };
            _client = new RestClient(options);
        }

        public async Task<BookDBDTO> GetBookAsync(string title)
        {
            try
            {
                var request = new RestRequest("");
                request.AddHeader("accept", "application/json");
                request.AddQueryParameter("title", title);

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

                var imagePath = "test"; //https://image.tmdb.org/t/p/w500" + firstResult.GetProperty("poster_path").GetString();

                // Upload image to Blob Storage
                //var blobName = await _imageBlobService.UploadImageFromUrlAsync(imagePath, "movies");

                return new BookDBDTO
                {
                    Title = "Test",
                    Description = "Description",
                    Cover = imagePath
                };
            }
            catch (Exception ex)
            {
                // Log the error (replace with your logging mechanism)
                Console.WriteLine($"Error in GetBookAsync: {ex.Message}");

                // Optionally rethrow the exception or return a default/fallback value
                throw new Exception("An error occurred while fetching the book details.", ex);
            }
        }


        public async Task<Book> CreateBookAsync(string bookQuery)
        {
            if (string.IsNullOrEmpty(bookQuery))
            {
                throw new ArgumentException("Book query cannot be empty.");
            }

            var bookData = await GetBookAsync(bookQuery);

            if (bookData == null)
            {
                throw new InvalidOperationException("Book not found in external API.");
            }

            if (string.IsNullOrEmpty(bookData.Title))
            {
                throw new InvalidOperationException("Title not found in the Book data.");
            }

            var book = new Book
            {
                Title = bookData.Title,
                BookDetails = new BookDetails
                {
                    Description = bookData.Description,
                    Cover = bookData.Cover,
                    Author = "Placeholder",
                    Genre = "Placeholder"
                }
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return book;
        }
    }
}
