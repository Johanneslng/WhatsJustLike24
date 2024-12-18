using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("BookIsLike")]
    public class BookIsLike
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("BookA")]
        public int BookIdA { get; set; }

        [Required]
        [ForeignKey("BookB")]
        public int BookIdB { get; set; }

        // Navigation properties
        public virtual Book BookA { get; set; }
        public virtual Book BookB { get; set; }
        public virtual ICollection<BookIsLikeDetails> BookIsLikeDetails { get; set; }
    }
}
