using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Security.Claims;
using ShopManagement.DTOs;
using ShopManagement.BLL.Interfaces;
namespace ShopManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartServices _services;
        public CartController(ICartServices services)
        {
            _services = services;
        }
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {

            var result = await _services.GetCart(GetUserId());
            return Ok(result);

        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartDto request)
        {
            var result = await _services.AddToCart(request, GetUserId());
            return Ok(result);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id, UpdateCartDto request)
        {
            var result = await _services.Update(Id, request.Quantity);
            return Ok(result);
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var result = await _services.Delete(Id);
            return Ok(result);
        }
    }
}
