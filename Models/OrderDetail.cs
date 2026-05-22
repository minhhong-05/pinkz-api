namespace ShopManagement.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; } // [cite: 165]
        public int OrderID { get; set; } // [cite: 166]
        public int ProductID { get; set; } // [cite: 167]
        public int Quantity { get; set; } // [cite: 168]
        public decimal Price { get; set; } // Giá lúc mua [cite: 169]
        public string Size { get; set; } // [cite: 170]
    }
}
