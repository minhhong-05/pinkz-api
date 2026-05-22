using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopManagement.BLL.Interfaces;
using ShopManagement.BLL.Services;
using ShopManagement.Services;

using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseWebRoot("wwwroot");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddScoped<ICartServices, CartServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IOrderServices, OrderServices>();
builder.Services.AddScoped<INotificationServices, NotificationServices>();
builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<IReviewServices, ReviewServices>();
builder.Services.AddScoped<IWishlistServices, WishlistServices>();
builder.Services.AddScoped<IDashboardServices, DashboardServices>();
// Cấu hình JWT Authentication (Yêu cầu FR-02)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(opt => opt.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
var app = builder.Build();

//tạo admin khi khởi động ứng dụng lần đầu (Yêu cầu FR-01)
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    using (SqlConnection conn = new SqlConnection(config.GetConnectionString("DefaultConnection")))
    {
        conn.Open();

        string check = "SELECT COUNT(*) FROM Users WHERE Role = 'Admin'";
        using (SqlCommand cmd = new SqlCommand(check, conn))
        {
            int count = (int)cmd.ExecuteScalar();
            if (count == 0)
            {
                string insert = @"INSERT INTO Users (Username, Email, Password, Role, Status)
                                  VALUES (@Username, @Email, @Password, 'Admin', 1)";

                using (SqlCommand insertCmd = new SqlCommand(insert, conn))
                {
                    insertCmd.Parameters.AddWithValue("@Username", "admin");
                    insertCmd.Parameters.AddWithValue("@Email", "admin@gmail.com");
                    insertCmd.Parameters.AddWithValue("@Password",
                        BCrypt.Net.BCrypt.HashPassword("123456"));

                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();