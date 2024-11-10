using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("GameDetails")]
    public class GameDetails
    {
        [Key]
        public int Id { get; set; }

        public string Genre { get; set; }

        public DateTime FirstRelease { get; set; }

        public string Franchise { get; set; }

        public string Developer { get; set; }

        public string Platforms { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Cover { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }

        // Navigation property
        public virtual Game Game { get; set; }
    }
}
