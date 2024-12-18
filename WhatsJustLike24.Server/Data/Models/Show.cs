using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("Shows")]
    public class Show
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        // Navigation properties
        public virtual ShowDetails ShowDetails { get; set; }
        public virtual ICollection<ShowIsLike> IsLikeConnections { get; set; }
    }
}
