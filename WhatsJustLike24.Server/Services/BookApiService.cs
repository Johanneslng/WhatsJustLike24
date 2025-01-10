using System.Reflection.Metadata;
using System.Text.Json;
using RestSharp;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

            var options = new RestClientOptions("https://openlibrary.org");
            {
                // Additional options if needed
            };
            _client = new RestClient(options);
        }

        public async Task<BookDBDTO> GetBookAsync(string title)
        {
            try
            {
                var request = new RestRequest("/search.json", Method.Get);
                request.AddHeader("accept", "application/json");
                
                request.AddQueryParameter("title", title);

                // Send the request
                var response = await _client.GetAsync(request);

                if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
                {
                    throw new Exception("Failed to retrieve book data from the API.");
                }

                var jsonDocument = JsonDocument.Parse(response.Content);

                // Check if docs array exists and is not empty
                var docs = jsonDocument.RootElement.GetProperty("docs");
                if (docs.GetArrayLength() == 0)
                {
                    throw new Exception("No books found for the given query.");
                }
                var books = JsonSerializer.Deserialize<List<BookApiResponse>>(docs);

                int minLevenshtein = Int32.MaxValue;
                int firstPublished = 9999;
                var bookApiResponse = new BookApiResponse();
                foreach (var item in books)
                {
                    int levenshteinDistance = LevenshteinDistance.Calculate(title, item.Title);
                    if (levenshteinDistance < minLevenshtein && item.FirstPublished <= firstPublished && item.FirstPublished.HasValue)
                    {
                        bookApiResponse = item;
                        minLevenshtein = levenshteinDistance;
                        firstPublished = item.FirstPublished ?? firstPublished;
                    }
                    else if (levenshteinDistance == minLevenshtein && item.FirstPublished < firstPublished && item.FirstPublished.HasValue)
                    {
                        bookApiResponse = item;
                        firstPublished = item.FirstPublished ?? firstPublished;
                    }
                }

                //Get additional info from works and books
                BookApiWorkResponse description = new BookApiWorkResponse();
                try
                {
                    var requestWorks = new RestRequest($"{bookApiResponse.WorkLink}");
                    var worksResponse = await _client.GetAsync(requestWorks);
                    var jsonWorks = JsonDocument.Parse(worksResponse.Content);
                    var descJson = jsonWorks.RootElement.GetProperty("description");
                    description = JsonSerializer.Deserialize<BookApiWorkResponse>(descJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

                BookApiAdditionalInfoResponse booksAdditionalInfo = new BookApiAdditionalInfoResponse();
                try
                {
                    var requestBooksAdditionalInfo = new RestRequest($"books/{bookApiResponse.Cover}");
                    var booksAdditionalInfoResponse = await _client.GetAsync(requestBooksAdditionalInfo);
                    var jsonAdditionalInfo = JsonDocument.Parse(booksAdditionalInfoResponse.Content);
                    booksAdditionalInfo = JsonSerializer.Deserialize<BookApiAdditionalInfoResponse>(jsonAdditionalInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }


                var imagePath = String.Concat("https://covers.openlibrary.org/b/olid/", bookApiResponse.Cover, "-L.jpg");

                // Upload image to Blob Storage
                var blobName = await _imageBlobService.UploadImageFromUrlAsync(imagePath, "books");

                var example = new BookDBDTO
                {
                    Title = bookApiResponse.Title,
                    Description = description.Description ?? "No Description found",
                    Genre = String.Join(", ", bookApiResponse.Genres ?? new List<string>()),
                    Author = String.Join(", ", bookApiResponse.Author ?? new List<string>()),
                    FirstRelease = bookApiResponse.FirstPublished,
                    Publisher = string.IsNullOrEmpty(String.Join(", ", booksAdditionalInfo.Publishers ?? new List<string>())) ? String.Join(", ", bookApiResponse.Publishers ?? new List<string>()) : String.Join(", ", booksAdditionalInfo.Publishers ?? new List<string>()),
                    Series = String.Join(", ", booksAdditionalInfo.Series ?? new List<string>()),
                    Isbn = String.Join(", ", booksAdditionalInfo.Isbn ?? new List<string>()) ?? String.Join(", ", bookApiResponse.Isbn ?? new List<string>()),
                    //Isbn13 = String.Join(", ", booksAdditionalInfo.Isbn13) ?? "No ISBN13 found"
                    Cover = blobName,
                    Languages = String.Join(", ", bookApiResponse.Langugages ?? new List<string>()),
                    Pages = bookApiResponse.Pages ?? 0
                };

                return example;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBookAsync: {ex.Message}");

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
                    Author = bookData.Author,
                    Genre = bookData.Genre,
                    FirstRelease = new DateTime(bookData.FirstRelease?? 0000,1,1),
                    Publisher = bookData.Publisher,
                    Isbn = bookData.Isbn,
                    Languages = bookData.Languages,
                    Pages = bookData.Pages,
                    Series = bookData.Series
                }
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return book;
        }
    }
}
