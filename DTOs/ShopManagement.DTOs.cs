namespace ShopManagement.DTOs
{
    public class RegisterDTO
    {
        public string Username { get; set; } // [cite: 127]
        public string Email { get; set; } // [cite: 128]
        public string Password { get; set; } // [cite: 129]
       
    }
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; } = "";
        public string Password { get; set; } = "";
    }
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
    }
    public class UpdateCartDto
    {
        public int Quantity { get; set; }
    }
    public class ProductDTO
    {
        public int ProductID { get; set; } // [cite: 138]
        public string ProductName { get; set; } // [cite: 139]
        public int CategoryID { get; set; } // [cite: 140]
        public string Material { get; set; } // Vàng, Bạc, Kim cương [cite: 141]
        public decimal Price { get; set; } // Phải > 0 [cite: 142, 191]
        public string? Description { get; set; } // [cite: 143]
        public string? ImageURL { get; set; } // [cite: 144]
        public int Stock { get; set; } // [cite: 145]
        public IFormFile? ImageFile { get; set; }
    }
    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }
    public class ReviewDTO
    {
        public int ProductID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
    public class WishlistDTO
    {
        public int ProductID { get; set; }
    }
    public class OrderItemResponse
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
    }
    public class OrderResponse
    {
        public int OrderID { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<OrderItemResponse> Items { get; set; }
    }
    public class DashboardDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }

        public List<RevenueByDayDTO> RevenueByDays { get; set; }
    }

    public class RevenueByDayDTO
    {
        public string Date { get; set; }
        public decimal Revenue { get; set; }
    }

}
