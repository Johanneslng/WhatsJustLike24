namespace WhatsJustLike24.Server.Data.DTOs
{
    public class ShowIsLikeDetailDTO
    {
        public int Id { get; set; }
        public int ShowIsLikeId { get; set; }
        public int? SimilarityScore { get; set; }
        public string Description { get; set; }
    }
}
