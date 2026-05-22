namespace ShopManagement.Models
{
    public class Review
    {
        public int ReviewID { get; set; } // [cite: 172]
        public int UserID { get; set; } // [cite: 173]
        public int ProductID { get; set; } // [cite: 174]
        public int Rating { get; set; } // Từ 1 đến 5 [cite: 175, 193]
        public string Comment { get; set; } // [cite: 176]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // [cite: 177]
    }
}
