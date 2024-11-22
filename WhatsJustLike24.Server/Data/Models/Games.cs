using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("Games")]
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } // Changed Title to Name for clarity

        // Navigation properties
        public virtual GameDetails GameDetails { get; set; }
        public virtual ICollection<GameIsLike> IsLikeConnections { get; set; }
    }
}
