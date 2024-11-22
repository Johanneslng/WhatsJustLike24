namespace WhatsJustLike24.Server.Data.DTOs
{
    public class GameIsLikeDetailDTO
    {
        public int Id { get; set; }
        public int GameIsLikeId { get; set; }
        public int? SimilarityScore { get; set; }
        public string Description { get; set; }
    }
}
