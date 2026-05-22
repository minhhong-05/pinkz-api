using Microsoft.Data.SqlClient;
using ShopManagement.BLL.Interfaces;
using ShopManagement.DTOs;
namespace ShopManagement.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IConfiguration _configuration;
        public OrderServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // tạo đơn hàng từ giỏ hàng
        public async Task<string> CreateOrder(int userId)
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            using SqlTransaction tran = conn.BeginTransaction();

            try
            {
                // 1. Cart
                string getCart = "SELECT CartID FROM Carts WHERE UserID = @UserID";
                int cartId;

                using (SqlCommand cmd = new SqlCommand(getCart, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    var result = await cmd.ExecuteScalarAsync();

                    if (result == null)
                        return "Chưa có giỏ hàng";

                    cartId = (int)result;
                }

                // 2. Items
                string getItems = @"
        SELECT ci.ProductID, ci.Quantity, p.Price, ci.Size
        FROM CartItems ci
        JOIN Products p ON ci.ProductID = p.ProductID
        WHERE ci.CartID = @CartID";

                var items = new List<(int productId, int qty, decimal price, string size)>();

                using (SqlCommand cmd = new SqlCommand(getItems, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@CartID", cartId);

                    using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        items.Add((
                            (int)reader["ProductID"],
                            (int)reader["Quantity"],
                            (decimal)reader["Price"],
                            reader["Size"].ToString()
                        ));
                    }
                }

                if (items.Count == 0)
                    return "Giỏ hàng trống";

                // 3. total
                decimal total = items.Sum(x => x.qty * x.price);

                // 4. order
                string insertOrder = @"
        INSERT INTO Orders (UserID, TotalAmount, Status)
        OUTPUT INSERTED.OrderID
        VALUES (@UserID, @Total, 'Pending')";

                int orderId;

                using (SqlCommand cmd = new SqlCommand(insertOrder, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Total", total);

                    orderId = (int)await cmd.ExecuteScalarAsync();
                }

                // 5. order detail
                foreach (var item in items)
                {
                    string insertDetail = @"
            INSERT INTO OrderDetails (OrderID, ProductID, Quantity, Price, Size)
            VALUES (@OrderID, @ProductID, @Quantity, @Price, @Size)";

                    using SqlCommand cmd = new SqlCommand(insertDetail, conn, tran);

                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@ProductID", item.productId);
                    cmd.Parameters.AddWithValue("@Quantity", item.qty);
                    cmd.Parameters.AddWithValue("@Price", item.price);
                    cmd.Parameters.AddWithValue("@Size", item.size);

                    await cmd.ExecuteNonQueryAsync();
                }

                // 6. clear cart
                string clear = "DELETE FROM CartItems WHERE CartID = @CartID";

                using (SqlCommand cmd = new SqlCommand(clear, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@CartID", cartId);
                    await cmd.ExecuteNonQueryAsync();
                }

                tran.Commit();
                return "Đặt hàng thành công";
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
        //lấy đơn hàng của user
        public async Task<List<OrderResponse>> GetMyOrders(int userId)
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            var orders = new List<OrderResponse>();

            // 1. orders
            string queryOrder = "SELECT * FROM Orders WHERE UserID = @UserID ORDER BY CreatedAt DESC";

            using (SqlCommand cmd = new SqlCommand(queryOrder, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    orders.Add(new OrderResponse
                    {
                        OrderID = (int)reader["OrderID"],
                        TotalAmount = (decimal)reader["TotalAmount"],
                        Status = reader["Status"].ToString(),
                        CreatedAt = (DateTime)reader["CreatedAt"],
                        Items = new List<OrderItemResponse>()
                    });
                }
            }

            // 2. details
            foreach (var order in orders)
            {
                string queryDetail = @"
        SELECT ProductID, Quantity, Price, Size
        FROM OrderDetails
        WHERE OrderID = @OrderID";

                using SqlCommand cmd = new SqlCommand(queryDetail, conn);
                cmd.Parameters.AddWithValue("@OrderID", order.OrderID);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    order.Items.Add(new OrderItemResponse
                    {
                        ProductID = (int)reader["ProductID"],
                        Quantity = (int)reader["Quantity"],
                        Price = (decimal)reader["Price"],
                        Size = reader["Size"].ToString()
                    });
                }
            }

            return orders;
        }
    }
}
