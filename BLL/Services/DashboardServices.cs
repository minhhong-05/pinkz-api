using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ShopManagement.DTOs;
using ShopManagement.BLL.Interfaces;
namespace ShopManagement.BLL.Services
{
    public class DashboardServices: IDashboardServices
    {
        private readonly IConfiguration _configuration;
        public DashboardServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<DashboardDTO> GetDashboard()
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            var dashboard = new DashboardDTO();

            // 1. Tổng doanh thu
            string revenueQuery = "SELECT ISNULL(SUM(TotalAmount),0) FROM Orders WHERE Status = 'Done'";
            using (SqlCommand cmd = new SqlCommand(revenueQuery, conn))
            {
                dashboard.TotalRevenue = (decimal)await cmd.ExecuteScalarAsync();
            }

            // 2. Tổng đơn
            string orderQuery = "SELECT COUNT(*) FROM Orders";
            using (SqlCommand cmd = new SqlCommand(orderQuery, conn))
            {
                dashboard.TotalOrders = (int)await cmd.ExecuteScalarAsync();
            }

            // 3. Tổng user
            string userQuery = "SELECT COUNT(*) FROM Users";
            using (SqlCommand cmd = new SqlCommand(userQuery, conn))
            {
                dashboard.TotalUsers = (int)await cmd.ExecuteScalarAsync();
            }

            // 4. Tổng product
            string productQuery = "SELECT COUNT(*) FROM Products";
            using (SqlCommand cmd = new SqlCommand(productQuery, conn))
            {
                dashboard.TotalProducts = (int)await cmd.ExecuteScalarAsync();
            }

            // 5. Doanh thu 7 ngày gần nhất
            string chartQuery = @"
    SELECT 
        CAST(CreatedAt AS DATE) as Date,
        SUM(TotalAmount) as Revenue
    FROM Orders
    WHERE Status = 'Done'
    AND CreatedAt >= DATEADD(DAY, -7, GETDATE())
    GROUP BY CAST(CreatedAt AS DATE)
    ORDER BY Date";

            dashboard.RevenueByDays = new List<RevenueByDayDTO>();

            using (SqlCommand cmd = new SqlCommand(chartQuery, conn))
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    dashboard.RevenueByDays.Add(new RevenueByDayDTO
                    {
                        Date = ((DateTime)reader["Date"]).ToString("yyyy-MM-dd"),
                        Revenue = (decimal)reader["Revenue"]
                    });
                }
            }

            return dashboard;
        }
    }
}
