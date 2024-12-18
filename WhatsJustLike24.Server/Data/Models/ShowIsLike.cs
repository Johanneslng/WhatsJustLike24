using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("ShowIsLike")]
    public class ShowIsLike
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ShowA")]
        public int ShowIdA { get; set; }

        [Required]
        [ForeignKey("ShowB")]
        public int ShowIdB { get; set; }

        // Navigation properties
        public virtual Show ShowA { get; set; }
        public virtual Show ShowB { get; set; }
        public virtual ICollection<ShowIsLikeDetails> ShowIsLikeDetails { get; set; }
    }
}
