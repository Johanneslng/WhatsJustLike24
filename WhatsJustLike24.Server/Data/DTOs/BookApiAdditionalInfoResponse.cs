using System.Text.Json.Serialization;

namespace WhatsJustLike24.Server.Data.DTOs
{
    public class BookApiAdditionalInfoResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("publishers")]
        public List<string>? Publishers { get; set; }

        [JsonPropertyName("isbn_10")]
        public List<string>? Isbn { get; set; }

        [JsonPropertyName("isbn_13")]
        public List<string>? Isbn13 { get; set; }

        [JsonPropertyName("series")]
        public List<string>? Series { get; set; }
    }
}
