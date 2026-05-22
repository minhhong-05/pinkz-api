using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace ShopManagement.Models
{
    public class User
    {
        public int UserID { get; set; } // [cite: 126]
        public string Username { get; set; } // [cite: 127]
        public string Email { get; set; } // [cite: 128]
        public string Password { get; set; } // [cite: 129]
        public string Role { get; set; } // Admin / Customer [cite: 130]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // [cite: 131]
        public bool Status { get; set; } // [cite: 132]
    }
}
