using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ShopManagement.BLL.Interfaces;
namespace ShopManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IDashboardServices _services;
        public AdminDashboardController(IDashboardServices services)
        {
            _services = services;
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            return Ok(await _services.GetDashboard());
        }
    }
}
