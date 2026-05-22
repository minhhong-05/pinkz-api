namespace ShopManagement.Models
{
    public class Order
    {
        public int OrderID { get; set; } // [cite: 159]
        public int UserID { get; set; } // [cite: 160]
        public decimal TotalAmount { get; set; } // [cite: 161]
        public string Status { get; set; } // [cite: 162]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // [cite: 163]
    }
}
