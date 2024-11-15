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

    public Task<string> GetCoverUrlAsync(int coverId) => GetOrAddAsync(_coverCache, coverId, async () => await RetrieveListValuesAsync(coverId, "url", "covers"));

    public Task<string> GetGenreNameAsync(int genreId) => GetOrAddAsync(_genreCache, genreId, async () => await RetrieveListValuesAsync(genreId, "name", "genres"));

    public Task<string> GetPlatformNameAsync(int platformId) => GetOrAddAsync(_platformCache, platformId, async () => await RetrieveListValuesAsync(platformId, "name", "platforms"));

    public Task<string> GetCompanyNameAsync(int companyId) => GetOrAddAsync(_companyCache, companyId, async () => 
        await RetrieveListValuesAsync(
            Convert.ToInt32(await RetrieveListValuesAsync(companyId, "company", "involved_companies")),
            "name",
            "companies"
        )
    );

    private async Task<string> RetrieveListValuesAsync(int listValueId, string requestField, string urlEnding)
    {
        string value = "";
        var request = new RestRequest($"{baseUrl}{urlEnding}", Method.Post)
            .AddHeader("Client-ID", _clientId)
            .AddHeader("Authorization", "Bearer " + await _tokenService.GetTwitchTokenAsync())
            .AddHeader("Accept", "application/json")
            .AddStringBody($"fields {requestField}; where id = {listValueId};", ContentType.Plain);

        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            var jsonResponse = JsonDocument.Parse(response.Content);
            if (jsonResponse.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement element in jsonResponse.RootElement.EnumerateArray())
                {
                    if (element.TryGetProperty(requestField, out JsonElement valueElement))
                    {

                        if (valueElement.ValueKind == JsonValueKind.String)
                        {
                            value = valueElement.GetString();
                        }
                        else if (valueElement.ValueKind == JsonValueKind.Number)
                        {
                            value = valueElement.GetInt32().ToString();
                        }
                    }
                }
            }
            return value;
        }
        else
        {
            throw new Exception($"Failed to get Value from API: {response.ErrorMessage}");
        }
    }
    private Task<string> GetOrAddAsync(ConcurrentDictionary<int, Task<string>> cache, int key, Func<Task<string>> valueFactory)
    {
        return cache.GetOrAdd(key, _ => valueFactory());
    }
}
