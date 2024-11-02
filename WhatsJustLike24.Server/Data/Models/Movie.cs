using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("Movies")]
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } // Changed Title to Name for clarity

        // Navigation properties
        public virtual MovieDetails MovieDetails { get; set; }
        public virtual ICollection<MovieIsLike> IsLikeConnections { get; set; }
    }
}
