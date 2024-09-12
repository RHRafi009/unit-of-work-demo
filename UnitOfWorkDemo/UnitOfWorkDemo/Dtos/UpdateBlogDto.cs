namespace UnitOfWorkDemo.Dtos
{
    public class UpdateBlogDto
    {
        public int BlogId { get; set; }
        public bool IsPublished { get; set; }
        public string Content { get; set; }
    }
}
