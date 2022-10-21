using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public User FKUser { get; set; }

        public Product FKProduct { get; set; }

        public int Quantity { get; set; }
    }
}
