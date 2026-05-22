namespace ShopManagement.BLL.Interfaces
{
    public interface INotificationServices
    {
        Task Create(int userId, string title, string content);
        Task<List<object>> GetByUser(int userId);
        Task MarkAsRead(int id);
    }
}
