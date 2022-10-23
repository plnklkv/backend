using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int Number { get; set; }
    }
}
