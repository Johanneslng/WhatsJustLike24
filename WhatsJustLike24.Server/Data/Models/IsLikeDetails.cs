using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    public class IsLikeDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SimilarityScore { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("MovieIsLike")]
        public int MovieIsLikeId { get; set; }

        // Navigation property
        public virtual MovieIsLike MovieIsLike { get; set; }
    }
}
