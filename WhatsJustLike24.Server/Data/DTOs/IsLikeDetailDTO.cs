namespace WhatsJustLike24.Server.Data.DTOs
{
    public class IsLikeDetailDTO
    {
        public int Id { get; set; }
        public int MovieIsLikeId { get; set; }
        public int? SimilarityScore { get; set; }
        public string Description { get; set; }
    }
}
