using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessWithEF.Models
{
    [Table("Blogs")]
    public class Blog : BaseModel
    {
        public int Id { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        public int? ReviewedBy { get; set; }
        [MaxLength(10000)]
        public string? Content { get; set; }
        public bool IsPublished { get; set; }
        public DateTimeOffset? PublishedDate { get; set; }

        public User CreatedByUser { get; set; }
        public User? ReviewedByUser { get; set; }
        public ICollection<BlogComment>? Comments { get; set; }
    }
}
