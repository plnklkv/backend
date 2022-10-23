using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class UserDTO
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [MinLength(6)]
        [Required]
        public string Password { get; set; }
    }
}
