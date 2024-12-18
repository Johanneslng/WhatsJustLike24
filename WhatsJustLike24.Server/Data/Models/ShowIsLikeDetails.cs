using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    public class ShowIsLikeDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SimilarityScore { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("ShowIsLike")]
        public int ShowIsLikeId { get; set; }

        // Navigation property
        public virtual ShowIsLike ShowIsLike { get; set; }
    }
}
