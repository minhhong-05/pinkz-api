using Npgsql;
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
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
        INSERT INTO Notifications (UserID, Title, Content, IsRead)
        VALUES (@UserID, @Title, @Content, 0)";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Content", content);

            await cmd.ExecuteNonQueryAsync();
        }

        // 📜 lấy thông báo
        public async Task<List<object>> GetByUser(int userId)
        {
            var list = new List<object>();

            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
        SELECT NotificationID, Title, Content, IsRead, CreatedAt
        FROM Notifications
        WHERE UserID = @UserID
        ORDER BY CreatedAt DESC";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

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
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = "UPDATE Notifications SET IsRead = 1 WHERE NotificationID = @Id";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
