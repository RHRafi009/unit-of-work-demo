using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessWithEF.Models
{
    [Table("BlogComments")]
    public class BlogComment : BaseModel
    {
        public int Id { get; set; }
        public int ParentBlogId { get; set; }
        public int CommentedBy { get; set; }
        public DateTimeOffset CommentedOn { get; set; }
        [MaxLength(1000)]
        public string? CommentContent { get; set; }

        public User CommentedByUser { get; set; }
        public Blog ParentBlog { get; set; }
    }
}
