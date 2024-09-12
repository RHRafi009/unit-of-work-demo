using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessWithEF.Models
{
    [Table("BlogComments")]
    public class BlogComment : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]  
        public int ParentBlogId { get; set; }

        [Required]
        public int CommentedByUserId { get; set; }

        [Required]
        public DateTimeOffset CommentedOn { get; set; }
        
        [MaxLength(1000)]
        public string? CommentContent { get; set; }

        [DeleteBehavior(DeleteBehavior.Cascade)]
        public User CommentedByUser { get; set; }

        public Blog ParentBlog { get; set; }
    }
}
