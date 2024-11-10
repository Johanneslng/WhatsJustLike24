using Azure;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Collections.Concurrent;
using System.Text.Json;
using WhatsJustLike24.Server.Services;

public class GameLookupService
{
    private readonly RestClient _client;
    private readonly TokenService _tokenService;
    private readonly string _clientId;
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<int, Task<string>> _coverCache = new();
    private readonly ConcurrentDictionary<int, Task<string>> _genreCache = new();
    private readonly ConcurrentDictionary<int, Task<string>> _platformCache = new();
    private readonly ConcurrentDictionary<int, Task<string>> _companyCache = new();

    public GameLookupService(
        RestClient client,
        IConfiguration configuration,
        TokenService tokenService
        )
    {
        _client = client;
        _configuration = configuration;
        _clientId = _configuration["APIKeys:GameDB:Client-ID"];
        _tokenService = tokenService;
    }

    private string baseUrl = "https://api.igdb.com/v4/";

    public Task<string> GetCoverUrlAsync(int coverId) => GetOrAddAsync(_coverCache, coverId, async () => await RetrieveCoverAsync(coverId, await _tokenService.GetTwitchTokenAsync(), _clientId));

    public Task<string> GetGenreNameAsync(int genreId) => GetOrAddAsync(_genreCache, genreId, async () => await RetrieveGenreAsync(genreId, await _tokenService.GetTwitchTokenAsync(), _clientId));

    //public Task<string> GetPlatformNameAsync(int platformId) => GetOrAddAsync(_platformCache, platformId, async () => await RetrievePlatformAsync(platformId));

    //public Task<string> GetCompanyNameAsync(int companyId) => GetOrAddAsync(_companyCache, companyId, async () => await RetrieveCompanyAsync(companyId));

    private async Task<string> RetrieveCoverAsync(int coverId, string token, string clientId)
    {
        string coverUrl = "";
        var request = new RestRequest($"{baseUrl}covers", Method.Post)
            .AddHeader("Client-ID", clientId)
            .AddHeader("Authorization", "Bearer " + token)
            .AddHeader("Accept", "application/json")
            .AddStringBody($"fields url; where id = {coverId};", ContentType.Plain);

        var response = await _client.PostAsync(request);
        if(response.IsSuccessful)
        {
            var jsonResponse = JsonDocument.Parse(response.Content);
            if (jsonResponse.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement element in jsonResponse.RootElement.EnumerateArray())
                {
                    // Access "url" property within each object
                    if (element.TryGetProperty("url", out JsonElement urlElement))
                    {
                        coverUrl = urlElement.GetString();
                        Console.WriteLine($"URL: {coverUrl}");
                    }
                }
            }
            return coverUrl;
        }
        else
        {
            throw new Exception($"Failed to get Cover from API: {response.ErrorMessage}");
        }
    }

    private async Task<string> RetrieveGenreAsync(int coverId, string token, string clientId)
    {
        var request = new RestRequest($"{baseUrl}/covers", Method.Post)
            .AddHeader("Client-ID", clientId)
            .AddHeader("Authorization", "Bearer " + token)
            .AddHeader("Accept", "application/json")
            .AddStringBody($"fields url; where id = {coverId}", ContentType.Plain);

        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            var jsonResponse = JsonDocument.Parse(response.Content);
            var coverUrl = jsonResponse.RootElement.GetProperty("url").GetString();

            return coverUrl;
        }
        else
        {
            throw new Exception($"Failed to get Cover from API: {response.ErrorMessage}");
        }
    }

    private Task<string> GetOrAddAsync(ConcurrentDictionary<int, Task<string>> cache, int key, Func<Task<string>> valueFactory)
    {
        return cache.GetOrAdd(key, _ => valueFactory());
    }
}
