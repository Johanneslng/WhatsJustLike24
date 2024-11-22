using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    public class GameIsLikeDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SimilarityScore { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("GameIsLike")]
        public int GameIsLikeId { get; set; }

        // Navigation property
        public virtual GameIsLike GameIsLike { get; set; }
    }
}
