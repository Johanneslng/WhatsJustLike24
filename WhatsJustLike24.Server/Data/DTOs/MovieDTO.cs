namespace WhatsJustLike24.Server.Data.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public MovieDetailsDTO MovieDetails { get; set; }
    }
}
