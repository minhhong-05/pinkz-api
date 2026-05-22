using ShopManagement.DTOs;

namespace ShopManagement.BLL.Interfaces
{
    public interface IReviewServices
    {
        Task<string> Create(int userId, ReviewDTO dto);
        Task<List<object>> GetByProduct(int productId);
        Task<string> Delete(int reviewId);
    }
}
