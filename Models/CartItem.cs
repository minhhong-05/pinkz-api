namespace ShopManagement.Models
{
    public class CartItem
    {
        public int CartItemID { get; set; } // [cite: 153]
        public int CartID { get; set; } // [cite: 154]
        public int ProductID { get; set; } // [cite: 155]
        public int Quantity { get; set; } // Tối thiểu là 1 [cite: 156, 192]
        public string Size { get; set; } // [cite: 157]
    }
}
