namespace WhatsJustLike24.Server.Data.DTOs
{
    public class SimilarityByTitleDTO
    {
        public string Title { get; set; }
        public int? AverageSimilarityScore { get; set; }
        public int? SimilarityScoreCount { get; set; }
        public string? PosterPath { get; set; }
        public string? DescriptionList { get; set; }
        public string? SimilarityScoreList { get; set; }
    }
}
