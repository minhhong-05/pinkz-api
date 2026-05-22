namespace ShopManagement.Models
{
    public class Cart
    {
        public int CartID { get; set; } // [cite: 149]
        public int UserID { get; set; } // [cite: 150]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // [cite: 151]
    }
}
