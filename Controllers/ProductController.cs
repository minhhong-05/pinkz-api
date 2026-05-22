using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopManagement.DTOs;
using ShopManagement.Services;
using ShopManagement.BLL.Interfaces;
namespace ShopManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _service;

        public ProductController(IProductServices service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);
            if (result == null)
                return NotFound("Không tìm thấy sản phẩm");

            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword = "")
        {
            var results = await _service.Search(keyword ?? "");
            return Ok(results);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] ProductDTO dto)
        {
            var result = await _service.Create(dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("upadte/{Id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductDTO dto)
        {
            return Ok(await _service.Update(id, dto));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
            => Ok(await _service.Delete(id));

     
    }
}
