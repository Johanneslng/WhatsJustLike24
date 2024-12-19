namespace WhatsJustLike24.Server.Data.DTOs
{
    public class ShowDetailsDTO
    {
        public int Id { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        public string PosterPath { get; set; }
        public DateTime FirstAirDate { get; set; }
    }
}
