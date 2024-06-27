using System.ComponentModel.DataAnnotations;

namespace DataAccessWithEF.Models
{
    public class BaseModel
    {
        [Required]
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastEditedTime { get; set; }
    }
}
