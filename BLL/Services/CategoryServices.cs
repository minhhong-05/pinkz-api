using Npgsql;
using ShopManagement.DTOs;
using ShopManagement.BLL.Interfaces;
namespace ShopManagement.BLL.Services
{
    public class CategoryServices: ICategoryServices
    {
        private readonly IConfiguration _configuration;

        public CategoryServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ================= GET ALL =================
        public async Task<List<object>> GetAll()
        {
            var list = new List<object>();

            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = "SELECT * FROM Categories";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new
                {
                    CategoryID = reader["CategoryID"],
                    CategoryName = reader["CategoryName"],
                    Description = reader["Description"]
                });
            }

            return list;
        }

        // ================= GET BY ID =================
        public async Task<object?> GetById(int id)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = "SELECT * FROM Categories WHERE CategoryID = @ID";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ID", id);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new
                {
                    CategoryID = reader["CategoryID"],
                    CategoryName = reader["CategoryName"],
                    Description = reader["Description"]
                };
            }

            return null;
        }

        // ================= CREATE =================
        public async Task<string> Create(CategoryDTO dto)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
                INSERT INTO Categories (CategoryName, Description)
                VALUES (@Name, @Desc)";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", dto.CategoryName);
            cmd.Parameters.AddWithValue("@Desc", dto.Description ?? "");

            await cmd.ExecuteNonQueryAsync();

            return "Tạo category thành công";
        }

        // ================= UPDATE =================
        public async Task<string> Update(int id, CategoryDTO dto)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
                UPDATE Categories 
                SET CategoryName = @Name,
                    Description = @Desc
                WHERE CategoryID = @ID";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", dto.CategoryName);
            cmd.Parameters.AddWithValue("@Desc", dto.Description ?? "");
            cmd.Parameters.AddWithValue("@ID", id);

            await cmd.ExecuteNonQueryAsync();

            return "Cập nhật thành công";
        }

        // ================= DELETE =================
        public async Task<string> Delete(int id)
        {
            using NpgsqlConnection conn = new   NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = "DELETE FROM Categories WHERE CategoryID = @ID";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ID", id);

            await cmd.ExecuteNonQueryAsync();

            return "Xóa thành công";
        }
    }
}

