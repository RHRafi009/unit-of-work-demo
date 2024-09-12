namespace UnitOfWorkDemo.Dtos
{
    public class BlogCommentCreateDto
    {
        public int BlogId { get; set; }
        public int CommentedById { get; set; }
        public string Comment { get; set; }
    }
}
