using ShopManagement.DTOs;

namespace ShopManagement.BLL.Interfaces
{
    public interface IDashboardServices
    {
        Task<DashboardDTO> GetDashboard();
    }
}
