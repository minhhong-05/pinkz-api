using Npgsql;
using ShopManagement.DTOs;
using ShopManagement.BLL.Interfaces;
namespace ShopManagement.BLL.Services
{
    public class WishlistServices: IWishlistServices
    {
        private readonly IConfiguration _configuration;

        public WishlistServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ================= ADD =================
        public async Task<string> Add(int userId, WishlistDTO dto)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            // check tồn tại
            string check = @"
                SELECT COUNT(*) 
                FROM Wishlist 
                WHERE UserID = @UserID AND ProductID = @ProductID";

            using (NpgsqlCommand cmdCheck = new NpgsqlCommand(check, conn))
            {
                cmdCheck.Parameters.AddWithValue("@UserID", userId);
                cmdCheck.Parameters.AddWithValue("@ProductID", dto.ProductID);

                int count = (int)await cmdCheck.ExecuteScalarAsync();

                if (count > 0)
                    return "Đã có trong danh sách yêu thích";
            }

            string insert = @"
                INSERT INTO Wishlist (UserID, ProductID)
                VALUES (@UserID, @ProductID)";

            using NpgsqlCommand cmd = new NpgsqlCommand(insert, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@ProductID", dto.ProductID);

            await cmd.ExecuteNonQueryAsync();

            return "Thêm wishlist thành công";
        }

        // ================= REMOVE =================
        public async Task<string> Remove(int userId, int productId)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
                DELETE FROM Wishlist 
                WHERE UserID = @UserID AND ProductID = @ProductID";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@ProductID", productId);

            await cmd.ExecuteNonQueryAsync();

            return "Xóa khỏi wishlist thành công";
        }

        // ================= GET BY USER =================
        public async Task<List<object>> GetByUser(int userId)
        {
            var list = new List<object>();

            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
                SELECT w.ProductID, p.ProductName, p.Price, p.ImageURL
                FROM Wishlist w
                JOIN Products p ON w.ProductID = p.ProductID
                WHERE w.UserID = @UserID";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new
                {
                    productId = reader["ProductID"],
                    productName = reader["ProductName"],
                    price = reader["Price"],
                    image = reader["ImageURL"]
                });
            }

            return list;
        }
    }
}

