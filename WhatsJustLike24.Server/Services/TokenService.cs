using RestSharp;
using System.Text.Json;

namespace WhatsJustLike24.Server.Services
{
    public class TokenService
    {

        private readonly RestClient _twitchClient;
        private readonly IConfiguration _configuration;
        private readonly string _clientId;
        private string _cachedToken;
        private DateTime _tokenExpiration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _clientId = _configuration["APIKeys:GameDB:Client-ID"];

            var optionsTwitch = new RestClientOptions("https://id.twitch.tv/oauth2/token");
            _twitchClient = new RestClient(optionsTwitch);
        }
        public async Task<string> GetTwitchTokenAsync()
        {
            if (_cachedToken != null && DateTime.UtcNow < _tokenExpiration)
            {
                return _cachedToken;
            }

            var newToken = await FetchTokenFromTwitchAsync();
            return newToken;
        }

        private async Task<string> FetchTokenFromTwitchAsync()
        {
            string clientSecret = _configuration["APIKeys:GameDB:Client-Secret"];
            string url = "https://id.twitch.tv/oauth2/token";

            var request = new RestRequest(url, Method.Post);
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", clientSecret);
            request.AddParameter("grant_type", "client_credentials");

            var response = await _twitchClient.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                // Parse the JSON response to retrieve the access_token
                var jsonResponse = JsonDocument.Parse(response.Content);
                _cachedToken = jsonResponse.RootElement.GetProperty("access_token").GetString();
                _tokenExpiration = DateTime.UtcNow.AddSeconds(jsonResponse.RootElement.GetProperty("expires_in").GetInt64()); ;

                return _cachedToken;
            }
            else
            {
                throw new Exception($"Failed to get token from Twitch: {response.ErrorMessage}");
            }
        }
    }
}
