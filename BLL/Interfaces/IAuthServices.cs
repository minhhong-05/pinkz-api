using ShopManagement.DTOs;
namespace ShopManagement.BLL.Interfaces
{
    public interface IAuthServices
    {
        Task<string>Register(RegisterDTO request);
        Task<object>Login(LoginDto request);
    }
}
