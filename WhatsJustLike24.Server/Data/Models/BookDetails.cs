using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("BookDetails")]
    public class BookDetails
    {
        [Key]
        public int Id { get; set; }
        public string? Genre { get; set; }
        public DateTime? FirstRelease { get; set; }
        public string? Author { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Cover { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }

        // Navigation property
        public virtual Book Book { get; set; }
    }
}
