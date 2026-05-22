using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.BLL.Interfaces;
using ShopManagement.Services;
namespace ShopManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationServices _services;
        public NotificationController(INotificationServices services)
        {
            _services = services;
        }
        /// 📜 tạo thông báo
        [HttpPost("create")]
        public async Task<IActionResult> Create(int userId, string title, string content)
        {
            await _services.Create(userId, title, content);
            return Ok("Tạo thông báo thành công");
        }
        /// 📜 lấy thông báo theo user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var result = await _services.GetByUser(userId);
            return Ok(result);
        }
        [HttpPut("read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _services.MarkAsRead(id);
            return Ok("Đã đánh dấu là đã đọc");
        }

    }
}
