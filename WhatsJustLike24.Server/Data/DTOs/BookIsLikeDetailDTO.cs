namespace WhatsJustLike24.Server.Data.DTOs
{
    public class BookIsLikeDetailDTO
    {
        public int Id { get; set; }
        public int BookIsLikeId { get; set; }
        public int? SimilarityScore { get; set; }
        public string Description { get; set; }
    }
}
