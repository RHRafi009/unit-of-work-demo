using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessWithEF.Models
{
    [Table("Blogs")]
    public class Blog : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        
        public int? ReviewedByUserId { get; set; }
        
        [MaxLength(10000)]
        public string? Content { get; set; }

        public bool IsPublished { get; set; }

        public DateTimeOffset? PublishedDate { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public User CreatedByUser { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public User? ReviewedByUser { get; set; }

        public ICollection<BlogComment>? Comments { get; set; }
    }
}
