namespace ShopManagement.Models
{
    public class Notification
    {
        public int NotificationID { get; set; } // [cite: 183]
        public int UserID { get; set; } // [cite: 184]
        public string Title { get; set; } // [cite: 185]
        public string Content { get; set; } // [cite: 186]
        public bool IsRead { get; set; } = false; // [cite: 187]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // [cite: 188]
    }
}
