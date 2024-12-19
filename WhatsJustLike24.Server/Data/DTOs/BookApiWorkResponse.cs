using System.Text.Json.Serialization;

namespace WhatsJustLike24.Server.Data.DTOs
{
    public class BookApiWorkResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("value")]
        public string? Description { get; set; }

    }
}
