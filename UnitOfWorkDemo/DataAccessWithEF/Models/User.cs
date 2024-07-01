using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessWithEF.Models
{
    [Table("Users")]
    public class User : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public DateTimeOffset DOB { get; set; }

        [MaxLength(11)]
        public string? ContactNumber { get; set; }
        
    }
}
