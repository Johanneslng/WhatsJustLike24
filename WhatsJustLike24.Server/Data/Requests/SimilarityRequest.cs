namespace WhatsJustLike24.Server.Data.Requests
{
    public class SimilarityRequest
    {
        public string TitleA { get; set; }
        public string TitleB { get; set; }
        public int SimilarityScore { get; set; }
        public string Description { get; set; }  
    }
}
