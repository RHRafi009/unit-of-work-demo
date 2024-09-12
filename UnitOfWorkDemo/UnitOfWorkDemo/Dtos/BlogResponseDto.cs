namespace UnitOfWorkDemo.Dtos
{
    public class BlogResponseDto
    {
        public int BlogId { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedByEmail { get; set; }
        public DateTimeOffset PublishedOn { get; set; }
        public string BlogContent { get; set; }
        public IEnumerable<BlogCommentResponseDto>? BlogComments { get; set; }

    }
}
