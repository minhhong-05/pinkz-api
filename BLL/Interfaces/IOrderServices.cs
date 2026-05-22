using ShopManagement.DTOs;

namespace ShopManagement.BLL.Interfaces
{
    public interface IOrderServices
    {
        Task<string> CreateOrder(int userId);
        Task<List<OrderResponse>> GetMyOrders(int userId);
    }
}
