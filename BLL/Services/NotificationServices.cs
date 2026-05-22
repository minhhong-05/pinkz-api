using Microsoft.Data.SqlClient;
using ShopManagement.BLL.Interfaces;

namespace ShopManagement.Services
{
    public class NotificationServices : INotificationServices
    {
        private readonly IConfiguration _configuration;
        public NotificationServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task Create(int userId, string title, string content)
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
        INSERT INTO Notifications (UserID, Title, Content, IsRead)
        VALUES (@UserID, @Title, @Content, 0)";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Content", content);

            await cmd.ExecuteNonQueryAsync();
        }

        // 📜 lấy thông báo
        public async Task<List<object>> GetByUser(int userId)
        {
            var list = new List<object>();

            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
        SELECT NotificationID, Title, Content, IsRead, CreatedAt
        FROM Notifications
        WHERE UserID = @UserID
        ORDER BY CreatedAt DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new
                {
                    NotificationID = reader["NotificationID"],
                    Title = reader["Title"],
                    Content = reader["Content"],
                    IsRead = reader["IsRead"],
                    CreatedAt = reader["CreatedAt"]
                });
            }

            return list;
        }

        // ✔ đánh dấu đã đọc
        public async Task MarkAsRead(int id)
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = "UPDATE Notifications SET IsRead = 1 WHERE NotificationID = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
