using System.Text.Json.Serialization;

namespace WhatsJustLike24.Server.Data.DTOs
{
    public class GameApiResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("cover")]
        public int Cover { get; set; }

        [JsonPropertyName("first_release_date")]
        public long FirstReleaseDate { get; set; }

        [JsonPropertyName("genres")]
        public List<int> Genres { get; set; }

        [JsonPropertyName("involved_companies")]
        public List<int> InvolvedCompanies { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("platforms")]
        public List<int> Platforms { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }
    }
}
