namespace WhatsJustLike24.Server.Data.DTOs
{
    public class GameDetailsAndTitleDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Developer { get; set; }
        public string Description { get; set; }
        public string Cover { get; set; }
        public DateTime FirstRelease { get; set; }
        public string Platforms { get; set; }
    }
}
