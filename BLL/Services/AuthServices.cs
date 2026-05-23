using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.IdentityModel.Tokens;
using ShopManagement.BLL.Interfaces;
using ShopManagement.DTOs;
using ShopManagement.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace ShopManagement.Services
{
    public class AuthServices: IAuthServices
    {
        private readonly IConfiguration _configuration;
        public AuthServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Tạo token JWT
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)

            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //ddawng ký
        public async Task<string> Register(RegisterDTO request)
        {

            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();

                // Kiểm tra nếu username đã tồn tại
                string checkUser = "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Email =@Email";
                using (NpgsqlCommand cmdCheck = new NpgsqlCommand(checkUser, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@Username", request.Username);
                    cmdCheck.Parameters.AddWithValue("@Email", request.Email);
                    int Count = (int)await cmdCheck.ExecuteScalarAsync();
                    if (Count > 0)
                       return "Username hoặc Email đã tồn tại";
                }
                string insert = "INSERT INTO Users (Username, Email, Password, Role, Status) VALUES (@Username, @Email, @Password, 'Customer', @Status)";
                using (NpgsqlCommand cmd = new NpgsqlCommand(insert, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", request.Username);
                    cmd.Parameters.AddWithValue("@Email", request.Email);
                    cmd.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(request.Password));
                    cmd.Parameters.AddWithValue("@Status", true);
                    await cmd.ExecuteNonQueryAsync();
                    
                }
                    return "Đăng ký thành công";
            }
        }

        //đăng nhập

        public async Task<object> Login(LoginDto request)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM Users WHERE Username = @Input OR Email = @Input";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Input", request.UsernameOrEmail);
                    using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            return "Tài khoản không tồn tại";

                        string passwordHash = reader["Password"].ToString();
                        bool status = (bool)reader["Status"];
                        string role = reader["Role"].ToString();
                        string username = reader["Username"].ToString();
                        int userId = (int)reader["UserId"];
                        if (!BCrypt.Net.BCrypt.Verify(request.Password, passwordHash))
                            return "Sai mật khẩu";
                        if (!status)
                            return "Tài khoản bị khóa";
                        var user = new User
                        {
                            UserID = userId,
                            Username = username,
                            Role = role
                        };
                        return new
                        {
                            token = CreateToken(user),
                            username,
                            role,
                        };
                    }
                }
            }
        }

    }
}
