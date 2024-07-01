namespace UnitOfWorkDemo.Dtos
{
    public class CreateUserDto
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public DateTimeOffset DOB { get; set; }
        public string? ContactNumber { get; set; }
    }
}
