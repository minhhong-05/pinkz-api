using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.BLL.Interfaces;
using ShopManagement.Services;
using System.Security.Claims;
namespace ShopManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServices _services;
        public OrderController(IOrderServices services)
        {
            _services = services;
        }
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
        //taoj đơn hàng
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder()
        {
            int userId = GetUserId();
            return Ok(await _services.CreateOrder(userId));
        }
        // xem đơn hàng
        [HttpGet("GetOrder")]
        public async Task<IActionResult> GetMyOrders([FromQuery] int userId)
        {
            var result = await _services.GetMyOrders(userId);
            return Ok(result);

        }
    }
}
