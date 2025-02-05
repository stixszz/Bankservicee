using Microsoft.AspNetCore.Mvc;
using static BankService1.FileName;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BankService1
{
    using Microsoft.EntityFrameworkCore;
    public class FileName
    {
        public class User
        {
            public long Id { get; set; }
            public required string UserName { get; set; }
            public required string HashPassword { get; set; }
        }

        public class  BankDbContext : DbContext
        {
            public DbSet<User> Users { get; set; }

            public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<User>().HasData(
                    new User { Id = 1, UserName = "admin", HashPassword = "password123" }
                );
            }
        }

        [ApiController]
        [Route("api/auth")]
        public class AuthController : ControllerBase
        {
            private readonly BankDbContext _context;

            public AuthController(BankDbContext context)
            {
                _context = context;
            }

            [HttpPost("login")]
            public IActionResult Login([FromBody] LoginRequest request)
            {
                if (request == null || string.IsNullOrEmpty(request.Password))
            {
                    return BadRequest("Пожалуйста, введите имя пользователя и пароль.");
                }

                var user = _context.Users.FirstOrDefault(u => u.UserName == request.UserName);
                if (user == null || user.HashPassword != request.Password)
                {
                    return Unauthorized("Неверные учетные данные");
                }

                return Ok("Вы залогинились");
            }
        }

        public class LoginRequest
        {
            public required string UserName { get; set; }
            public required string Password { get; set; }
        }

        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);


                builder.Services.AddDbContext<BankDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddControllers();

                var app = builder.Build();

                app.UseRouting();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
                
}
            }
        }
    }



