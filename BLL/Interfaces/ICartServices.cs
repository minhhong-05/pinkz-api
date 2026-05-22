using ShopManagement.DTOs;
namespace ShopManagement.BLL.Interfaces
{
    public interface ICartServices
    {
        Task<object>GetCart(int userId);
        Task<string> AddToCart(AddToCartDto request, int userId);
        Task<string> Update(int cartItemId, int quantity);
        Task<string> Delete(int cartItemId);
    }
}
