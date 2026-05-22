using ShopManagement.DTOs;
namespace ShopManagement.BLL.Interfaces
{
    public interface ICategoryServices
    {
        Task<List<object>> GetAll();
        Task<object?> GetById(int id);
        Task<string> Create(CategoryDTO dto);
        Task<string> Update(int id, CategoryDTO dto);
        Task<string> Delete(int id);
    }
}
