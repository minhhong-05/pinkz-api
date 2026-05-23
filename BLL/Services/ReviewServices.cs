using Npgsql;
using ShopManagement.DTOs;
using ShopManagement.BLL.Interfaces;
namespace ShopManagement.BLL.Services
{
    public class ReviewServices: IReviewServices
    {
        private readonly IConfiguration _configuration;
        public ReviewServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // thêm đánh giá
        public async Task<string> Create(int userId, ReviewDTO dto)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            // insert review
            string query = @"
                INSERT INTO Reviews (UserID, ProductID, Rating, Comment)
                VALUES (@UserID, @ProductID, @Rating, @Comment)";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@ProductID", dto.ProductID);
            cmd.Parameters.AddWithValue("@Rating", dto.Rating);
            cmd.Parameters.AddWithValue("@Comment", dto.Comment ?? "");

            await cmd.ExecuteNonQueryAsync();

            return "Đánh giá thành công";
        }
        // lấy đánh giá theo sản phẩm
        public async Task<List<object>> GetByProduct(int productId)
        {
            var list = new List<object>();

            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
                SELECT r.ReviewID, r.Rating, r.Comment, r.CreatedAt, u.Username
                FROM Reviews r
                JOIN Users u ON r.UserID = u.UserID
                WHERE r.ProductID = @ProductID
                ORDER BY r.CreatedAt DESC";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ProductID", productId);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new
                {
                    reviewId = reader["ReviewID"],
                    username = reader["Username"],
                    rating = reader["Rating"],
                    comment = reader["Comment"],
                    createdAt = reader["CreatedAt"]
                });
            }

            return list;
        }
        //xoa đánh giá
        public async Task<string> Delete(int reviewId)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = "DELETE FROM Reviews WHERE ReviewID = @ID";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ID", reviewId);

            await cmd.ExecuteNonQueryAsync();

            return "Xóa review thành công";
        }
    }
}
