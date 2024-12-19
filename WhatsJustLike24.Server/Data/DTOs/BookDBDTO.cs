using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.DTOs
{
    public class BookDBDTO
    {
        public string Title { get; set; }
        public string? Genre { get; set; }
        public int? FirstRelease { get; set; }
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public string Description { get; set; }
        public string Cover { get; set; }
        public string? Series { get; set; }
        public string? Isbn { get; set; }
        public string? Languages { get; set; }
        public int? Pages { get; set; }
    }
}
