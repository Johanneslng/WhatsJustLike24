using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    public class BookIsLikeDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SimilarityScore { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("BookIsLike")]
        public int BookIsLikeId { get; set; }

        // Navigation property
        public virtual BookIsLike BookIsLike { get; set; }
    }
}
