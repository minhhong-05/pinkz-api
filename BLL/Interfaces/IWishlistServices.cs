using ShopManagement.DTOs;

namespace ShopManagement.BLL.Interfaces
{
    public interface IWishlistServices
    {
        Task<string> Add(int userId, WishlistDTO dto);
        Task<string> Remove(int userId, int productId);
        Task<List<object>> GetByUser(int userId);
    }
}
