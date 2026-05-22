using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.BLL.Interfaces;
using ShopManagement.DTOs;

namespace ShopManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewServices _service;

        public ReviewController(IReviewServices service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] int userId, ReviewDTO dto)
        {
            return Ok(await _service.Create(userId, dto));
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            return Ok(await _service.GetByProduct(productId));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _service.Delete(id));
        }
    }
}
