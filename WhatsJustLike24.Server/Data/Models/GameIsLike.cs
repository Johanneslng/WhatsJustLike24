using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("GameIsLike")]
    public class GameIsLike
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("GameA")]
        public int GameIdA { get; set; }

        [Required]
        [ForeignKey("GameB")]
        public int GameIdB { get; set; }

        // Navigation properties
        public virtual Game GameA { get; set; }
        public virtual Game GameB { get; set; }
        public virtual ICollection<GameIsLikeDetails> GameIsLikeDetails { get; set; }
    }
}
