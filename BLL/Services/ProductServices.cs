using Microsoft.Data.SqlClient;
using ShopManagement.BLL.Interfaces;
using ShopManagement.DTOs;
namespace ShopManagement.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IConfiguration _configuration;
        public ProductServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // xem tất cả sản phẩm
        public async Task<List<object>> GetAll()
        {
            var List  = new List<object>();
            using SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await sqlConnection.OpenAsync();
            string query = "SELECT * FROM Products WHERE Status = 1";
            using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                List.Add(new
                {
                    ProductID = reader["ProductID"],
                    ProductName = reader["ProductName"],
                    Price = reader["Price"],
                    Stock = reader["Stock"],
                    Material = reader["Material"],
                    ImageURL = reader["ImageURL"]
                });
            }
            return List;
        }
        // xem chi tiết sản phẩm
        public async Task<object?> GetById(int id)
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = "SELECT * FROM Products WHERE ProductID = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new
                {
                    ProductID = reader["ProductID"],
                    ProductName = reader["ProductName"],
                    Price = reader["Price"],
                    Stock = reader["Stock"],
                    Material = reader["Material"],
                    Description = reader["Description"],
                    ImageURL = reader["ImageURL"]
                };
            }

            return null;
        }
        //thêm sản phẩm
        public async Task<string> Create(ProductDTO dto)
        {
            string imageUrl = null;

            // upload ảnh
            if (dto.ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                imageUrl = "/img/" + fileName;
            }

            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"INSERT INTO Products 
    (ProductName, CategoryID, Material, Price, Description, ImageURL, Stock, Status)
    VALUES (@Name, @Cate, @Material, @Price, @Desc, @Img, @Stock, 1)";

            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Name", dto.ProductName);
            cmd.Parameters.AddWithValue("@Cate", dto.CategoryID);
            cmd.Parameters.AddWithValue("@Material", dto.Material);
            cmd.Parameters.AddWithValue("@Price", dto.Price);
            cmd.Parameters.AddWithValue("@Desc", (object?)dto.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Img", (object?)imageUrl ?? DBNull.Value); // 🔥 FIX CHỖ NÀY
            cmd.Parameters.AddWithValue("@Stock", dto.Stock);

            await cmd.ExecuteNonQueryAsync();

            return "Thêm sản phẩm thành công";
        }
        // cập nhật sản phẩm
        public async Task<string> Update(int id, ProductDTO dto) 
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();
            string imageUrl = null;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","img");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                imageUrl = "/img/" + fileName;
            }
            string query = @"UPDATE Products SET 
            ProductName = @Name,
            CategoryID = @Cate,
            Material = @Material,
            Price = @Price,
            Description = @Desc,
            ImageURL = @Img,
            Stock = @Stock
        WHERE ProductID = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", dto.ProductName);
            cmd.Parameters.AddWithValue("@Cate", dto.CategoryID);
            cmd.Parameters.AddWithValue("@Material", dto.Material);
            cmd.Parameters.AddWithValue("@Price", dto.Price);
            cmd.Parameters.AddWithValue("@Desc", (object?)dto.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Img", (object?)dto.ImageURL ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Stock", dto.Stock);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();

            return "Cập nhật thành công";
        }
        public async Task<string> Delete(int id)
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = "UPDATE Products SET Status = 0 WHERE ProductID = @Id";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();

            return "Xóa (ẩn) sản phẩm thành công";
        }
        // tìm kiếm sản phẩm
        public async Task<List<object>> Search(string keyword)
        {
            var list = new List<object>();

            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
    SELECT * FROM Products
    WHERE Status = 1
    AND ProductName LIKE @Keyword";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new
                {
                    ProductID = reader["ProductID"],
                    ProductName = reader["ProductName"],
                    Price = reader["Price"],
                    Stock = reader["Stock"],
                    Material = reader["Material"],
                    ImageURL = reader["ImageURL"]
                });
            }

            return list;
        }
    }
}
