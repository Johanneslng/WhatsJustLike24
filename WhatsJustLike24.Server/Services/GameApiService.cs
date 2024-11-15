using System.Text;
using System.Text.Json;
using RestSharp;
using WhatsJustLike24.Server.Data;
using WhatsJustLike24.Server.Data.DTOs;
using WhatsJustLike24.Server.Data.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WhatsJustLike24.Server.Services
{
    public class GameApiService
    {
        private readonly RestClient _gameClient;
        private readonly RestClient _twitchClient;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ImageBlobService _imageBlobService;
        private readonly TokenService _tokenService;
        private readonly GameLookupService _gameLookupService;
        private readonly string _clientId;


        public GameApiService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ImageBlobService imageBlobService,
            TokenService tokenService,
            GameLookupService gameLookUpService,
            RestClient restClient
            )
        {
            _context = context;
            _configuration = configuration;
            _imageBlobService = imageBlobService;

            _clientId = _configuration["APIKeys:GameDB:Client-ID"];

            _gameClient = restClient;
            _tokenService = tokenService;
            _gameLookupService = gameLookUpService;
        }

        public async Task<GameDBDTO> GetGameAsync(string query)
        {
            var twitchToken =  await _tokenService.GetTwitchTokenAsync();
            var url = "https://api.igdb.com/v4/games";

            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Client-ID", _clientId);
            request.AddHeader("Authorization", "Bearer " + twitchToken);
            request.AddHeader("Accept", "application/json");
            
            string requestBody = $"search \"{query}\"; fields name, cover, first_release_date, franchise, genres, involved_companies, platforms, summary;";
            request.AddStringBody(requestBody, ContentType.Plain);

            var response = await _gameClient.PostAsync(request);
            var games = JsonSerializer.Deserialize<List<GameApiResponse>>(response.Content);
            int minLevenshtein= Int32.MaxValue;
            var gameApiResponse = new GameApiResponse();
            foreach (var item in games)
            {
                int levenshteinDistance = LevenshteinDistance.Calculate(query, item.Name);
                if(levenshteinDistance < minLevenshtein)
                {
                    gameApiResponse = item;
                    minLevenshtein = levenshteinDistance;
                }
            }

            var gameDBDTO = new GameDBDTO
            {
                Title = gameApiResponse.Name,
                Cover = await _gameLookupService.GetCoverUrlAsync(gameApiResponse.Cover),
                Description = gameApiResponse.Summary,
                Genre = await ConcatenateEndpointResultsAsync(gameApiResponse.Genres, _gameLookupService.GetGenreNameAsync),
                FirstRelease = DateTimeOffset.FromUnixTimeSeconds(gameApiResponse.FirstReleaseDate).Date,
                Platforms = await ConcatenateEndpointResultsAsync(gameApiResponse.Platforms, _gameLookupService.GetPlatformNameAsync),
                Developer = await ConcatenateEndpointResultsAsync(gameApiResponse.InvolvedCompanies, _gameLookupService.GetCompanyNameAsync)
            };
            //var imagePath = "https://image.tmdb.org/t/p/w500" + firstResult.GetProperty("poster_path").GetString();
            //Upload Image to Blob Storage
            //var blobName = await _imageBlobService.UploadImageFromUrlAsync(imagePath);

            return gameDBDTO;
        }
        public async Task<Game> CreateGameAsync(string gameQuery)
        {
            if (string.IsNullOrEmpty(gameQuery))
            {
                throw new ArgumentException("Game query cannot be empty.");
            }

            var gameData = await GetGameAsync(gameQuery);

            if (gameData == null)
            {
                throw new InvalidOperationException("Game not found in external API.");
            }

            if (string.IsNullOrEmpty(gameData.Title))
            {
                throw new InvalidOperationException("Original title not found in the Game data.");
            }

            var game = new Game
            {
                Title = gameData.Title,
                GameDetails = new GameDetails
                {
                    Description = gameData.Description,
                    Cover = gameData.Cover,
                    Developer = gameData.Developer,
                    FirstRelease = gameData.FirstRelease,
                    Platforms = gameData.Platforms,
                    Genre = gameData.Genre
                }
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return game;
        }


        //helper methods
        public async Task<string> ConcatenateEndpointResultsAsync(IEnumerable<int> ids, Func<int, Task<string>> fetchFunction)
        {
            var stringBuilder = new StringBuilder();

            foreach (var id in ids)
            {
                string result = await fetchFunction(id);

                if (!string.IsNullOrEmpty(result))
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(", ");
                    }

                    stringBuilder.Append(result);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
