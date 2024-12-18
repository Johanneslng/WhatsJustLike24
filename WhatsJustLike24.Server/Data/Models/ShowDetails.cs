using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WhatsJustLike24.Server.Data.Models
{
    [Table("ShowDetails")]
    public class ShowDetails
    {
        [Key]
        public int Id { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public DateTime? FirstAirDate { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string PosterPath { get; set; }
        [ForeignKey("Show")]
        public int ShowId { get; set; }
        // Navigation property
        public virtual Show Show { get; set; }
    }
}
