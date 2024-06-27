namespace UnitOfWorkDemo.Dtos
{
    public class BlogCommentResponseDto
    {
        public int CommentId { get; set; }
        public int ParentBlogId { get; set; }
        public string CommentedByName { get; set; }
        public string CommentedByEmail { get; set; }
        public DateTimeOffset CommentedOn { get; set; }
        public string CommentContent { get; set; }
    }
}
