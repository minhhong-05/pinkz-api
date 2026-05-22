using ShopManagement.DTOs;

namespace ShopManagement.BLL.Interfaces
{
    public interface IProductServices
    {
        Task<List<object>> GetAll();
        Task<object?> GetById(int id);
        Task<string> Create(ProductDTO dto);
        Task<string> Update(int id, ProductDTO dto);
        Task<string> Delete(int id);
        Task<List<object>> Search(string keyword);
    }
}
