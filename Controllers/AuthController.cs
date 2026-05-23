using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopManagement.BLL.Interfaces;
using ShopManagement.DTOs;
using ShopManagement.Models;
using ShopManagement.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
namespace ShopManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _services;
        public AuthController(IAuthServices services)
        {
            _services = services;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO request)
        {
            var result = await _services.Register(request);

            if (result.Contains("tồn tại"))
                return BadRequest(result);

            return Ok(result);

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var result = await _services.Login(request);
            return Ok(result);

        }
    }
}
