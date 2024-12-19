using System.Text.Json.Serialization;

namespace WhatsJustLike24.Server.Data.DTOs
{
    public class BookApiResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("author_name")]
        public List<string>? Author { get; set; }

        [JsonPropertyName("first_publish_year")]
        public int? FirstPublished { get; set; }

        [JsonPropertyName("subject")]
        public List<string>? Genres { get; set; }

        [JsonPropertyName("publisher")]
        public List<string>? Publishers { get; set; }

        [JsonPropertyName("isbn")]
        public List<string>? Isbn { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("cover_edition_key")]
        public string? Cover { get; set; }

        [JsonPropertyName("key")]
        public string? WorkLink { get; set; }

        [JsonPropertyName("language")]
        public List<string>? Langugages { get; set; }

        [JsonPropertyName("number_of_pages_median")]
        public int? Pages { get; set; }
    }
}
