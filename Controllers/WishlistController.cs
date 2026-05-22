using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.BLL.Interfaces;
using ShopManagement.DTOs;

namespace ShopManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistServices _service;

        public WishlistController(IWishlistServices service)
        {
            _service = service;
        }

        // ADD
        [HttpPost]
        public async Task<IActionResult> Add(int userId, WishlistDTO dto)
        {
            return Ok(await _service.Add(userId, dto));
        }

        // GET
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            return Ok(await _service.GetByUser(userId));
        }

        // DELETE
        [HttpDelete]
        public async Task<IActionResult> Remove(int userId, int productId)
        {
            return Ok(await _service.Remove(userId, productId));
        }
    }
}
