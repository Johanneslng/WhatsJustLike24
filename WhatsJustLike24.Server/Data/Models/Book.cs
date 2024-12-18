using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        // Navigation properties
        public virtual BookDetails BookDetails { get; set; }
        public virtual ICollection<BookIsLike> IsLikeConnections { get; set; }
    }
}
