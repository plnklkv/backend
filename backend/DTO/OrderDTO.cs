namespace backend.Models
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public int[] Products { get; set; }

        public int Price { get; set; }
    }
}
