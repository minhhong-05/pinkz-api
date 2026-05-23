using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.Extensions.Configuration;
using ShopManagement.BLL.Interfaces;
using ShopManagement.DTOs;
using System.Runtime.CompilerServices;
namespace ShopManagement.Services
{
    public class CartServices : ICartServices
    {
        private readonly IConfiguration _configuration;
        public CartServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // xem giỏ hàng
        public async Task<object> GetCart(int userId)
        {
            var items = new List<object>();
            decimal total = 0;

            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
                SELECT 
                    ci.CartItemID,
                    ci.ProductID,
                    p.ProductName,
                    p.Price,
                    ci.Quantity,
                    ci.Size
                FROM Carts c
                JOIN CartItems ci ON c.CartID = ci.CartID
                JOIN Products p ON ci.ProductID = p.ProductID
                WHERE c.UserID = @UserID AND p.Status = 1";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                int qty = (int)reader["Quantity"];
                decimal price = (decimal)reader["Price"];
                decimal itemTotal = qty * price;

                total += itemTotal;

                items.Add(new
                {
                    cartItemId = reader["CartItemID"],
                    productId = reader["ProductID"],
                    productName = reader["ProductName"],
                    price = price,
                    quantity = qty,
                    size = reader["Size"],
                    itemTotal = itemTotal
                });
            }

            return new
            {
                userId,
                items,
                total
            };
        }
        //them vào giỏ hàng
        public async Task<string> AddToCart(AddToCartDto request, int userId)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            int cartId;

            // 1. Lấy hoặc tạo Cart
            string getCart = "SELECT CartID FROM Carts WHERE UserID = @UserID";
            using (NpgsqlCommand cmd = new NpgsqlCommand(getCart, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                var result = await cmd.ExecuteScalarAsync();

                if (result == null)
                {
                    string create = @"
                        INSERT INTO Carts (UserID)
                        OUTPUT INSERTED.CartID
                        VALUES (@UserID)";

                    using NpgsqlCommand cmdCreate = new NpgsqlCommand(create, conn);
                    cmdCreate.Parameters.AddWithValue("@UserID", userId);
                    cartId = (int)await cmdCreate.ExecuteScalarAsync();
                }
                else
                {
                    cartId = (int)result;
                }
            }

            // 2. Lấy thông tin sản phẩm (check tồn tại + giá + stock)
            string getProduct = "SELECT Price, Stock, Status FROM Products WHERE ProductID = @ProductID";

            decimal price;
            int stock;
            bool status;

            using (NpgsqlCommand cmd = new NpgsqlCommand(getProduct, conn))
            {
                cmd.Parameters.AddWithValue("@ProductID", request.ProductId);

                using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                    return "Sản phẩm không tồn tại";

                price = (decimal)reader["Price"];
                stock = (int)reader["Stock"];
                status = (bool)reader["Status"];
            }

            if (!status)
                return "Sản phẩm đã ngừng bán";

            if (request.Quantity > stock)
                return "Số lượng vượt quá tồn kho";

            // 3. Kiểm tra item đã có chưa
            string check = @"
                SELECT CartItemID, Quantity 
                FROM CartItems 
                WHERE CartID = @CartID AND ProductID = @ProductID AND Size = @Size";

            using (NpgsqlCommand cmdCheck = new NpgsqlCommand(check, conn))
            {
                cmdCheck.Parameters.AddWithValue("@CartID", cartId);
                cmdCheck.Parameters.AddWithValue("@ProductID", request.ProductId);
                cmdCheck.Parameters.AddWithValue("@Size", request.Size);

                using NpgsqlDataReader reader = await cmdCheck.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    int id = (int)reader["CartItemID"];
                    int oldQty = (int)reader["Quantity"];
                    reader.Close();

                    if (oldQty + request.Quantity > stock)
                        return "Vượt quá tồn kho";

                    string update = "UPDATE CartItems SET Quantity = @Qty WHERE CartItemID = @Id";

                    using NpgsqlCommand cmdUpdate = new NpgsqlCommand(update, conn);
                    cmdUpdate.Parameters.AddWithValue("@Qty", oldQty + request.Quantity);
                    cmdUpdate.Parameters.AddWithValue("@Id", id);

                    await cmdUpdate.ExecuteNonQueryAsync();

                    return "Cập nhật giỏ hàng thành công";
                }
            }

            // 4. Thêm mới
            string insert = @"
                INSERT INTO CartItems (CartID, ProductID, Quantity, Size)
                VALUES (@CartID, @ProductID, @Quantity, @Size)";

            using (NpgsqlCommand cmdInsert = new NpgsqlCommand(insert, conn))
            {
                cmdInsert.Parameters.AddWithValue("@CartID", cartId);
                cmdInsert.Parameters.AddWithValue("@ProductID", request.ProductId);
                cmdInsert.Parameters.AddWithValue("@Quantity", request.Quantity);
                cmdInsert.Parameters.AddWithValue("@Size", request.Size);

                await cmdInsert.ExecuteNonQueryAsync();
            }

            return "Thêm vào giỏ hàng thành công";
        }
        // cập nhật số lượng
        public async Task<string> Update(int cartItemId, int quantity)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            string query = @"
                UPDATE CartItems 
                SET Quantity = @Quantity 
                WHERE CartItemID = @ID";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Quantity", quantity);
            cmd.Parameters.AddWithValue("@ID", cartItemId);

            await cmd.ExecuteNonQueryAsync();

            return "Cập nhật thành công";
        }
        // xóa sản phẩm khỏi giỏ hàng
        public async Task<string> Delete(int cartItemId)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();
            string query = "DELETE FROM CartItems WHERE CartItemID = @ID";
            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ID", cartItemId);

            await cmd.ExecuteNonQueryAsync();

            return "Xóa thành công";
        }
    }
}
