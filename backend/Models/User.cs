using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SurnName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string MiddleName { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [MinLength(6)]
        [Required]
        public string Password { get; set; }

        public string Role { get; set; }
    }
}
