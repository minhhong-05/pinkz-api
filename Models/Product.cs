using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ShopManagement.Models
{
    public class Product
    {
        public int ProductID { get; set; } // [cite: 138]
        public string ProductName { get; set; } // [cite: 139]
        public int CategoryID { get; set; } // [cite: 140]
        public string Material { get; set; } // Vàng, Bạc, Kim cương [cite: 141]
        public decimal Price { get; set; } // Phải > 0 [cite: 142, 191]
        public string? Description { get; set; } // [cite: 143]
        public string? ImageURL { get; set; } // [cite: 144]
        public int Stock { get; set; } // [cite: 145]
        public bool Status { get; set; } = true; // [cite: 146]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // [cite: 147]
    }
}
