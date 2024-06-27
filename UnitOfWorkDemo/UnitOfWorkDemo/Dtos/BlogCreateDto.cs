namespace UnitOfWorkDemo.Dtos
{
    public class BlogCreateDto
    {
        public int CreatedById { get; set; }
        public bool IsPublished { get; set; }
        public string Content { get; set; }
    }
}
