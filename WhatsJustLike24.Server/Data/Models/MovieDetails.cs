using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("MovieDetails")]
    public class MovieDetails
    {
        [Key]
        public int Id { get; set; }

        public string Genre { get; set; }

        public string Director { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string PosterPath { get; set; }

        [ForeignKey("Movie")]
        public int MovieId { get; set; }

        // Navigation property
        public virtual Movie Movie { get; set; }
    }
}
