using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("MovieIsLike")]
    public class MovieIsLike
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("MovieA")]
        public int MovieIdA { get; set; }

        [Required]
        [ForeignKey("MovieB")]
        public int MovieIdB { get; set; }

        // Navigation properties
        public virtual Movie MovieA { get; set; }
        public virtual Movie MovieB { get; set; }
        public virtual ICollection<IsLikeDetails> IsLikeDetails { get; set; }
    }
}
