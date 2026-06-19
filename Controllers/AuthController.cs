using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherPortfolio.API.DTOs;
using TeacherPortfolio.API.Models;
using TeacherPortfolio.API.Services;

namespace TeacherPortfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Проверка на существующего пользователя
        var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            return BadRequest("Пользователь с таким email уже существует.");

        // Создание пользователя
        var user = new User
        {
            Email = request.Email,
            Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Teacher" : request.Role,
            Createdat = DateTime.Now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // ========== АВТОМАТИЧЕСКОЕ СОЗДАНИЕ ПРОФИЛЯ ПРЕПОДАВАТЕЛЯ ==========
        // Если роль Teacher, создаём профиль в таблице teachers
        if (user.Role == "Teacher")
        {
            // Проверяем, нет ли уже профиля
            var existingTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Userid == user.Id);
            if (existingTeacher == null)
            {
                // Создаём базовый профиль преподавателя
                var teacher = new Teacher
                {
                    Userid = user.Id,
                    Lastname = "",      // Будет заполнено позже через личный кабинет
                    Firstname = "",     // Будет заполнено позже
                    Middlename = null,
                    Position = "Преподаватель",
                    Workplace = "ГБПОУ ИО ИРКПО",
                    Createdat = DateTime.Now
                };
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
            }
        }

        // Генерация токена
        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return Unauthorized("Неверный email или пароль.");

        var isValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.Passwordhash);

        if (!isValid)
            return Unauthorized("Неверный email или пароль.");

        var token = _jwtService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        });
    }
    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Userid == userId);

        if (teacher == null)
        {
            // Если профиля нет, создаём новый
            teacher = new Teacher
            {
                Userid = userId,
                Createdat = DateTime.Now
            };
            _context.Teachers.Add(teacher);
        }

        teacher.Lastname = request.LastName ?? teacher.Lastname;
        teacher.Firstname = request.FirstName ?? teacher.Firstname;
        teacher.Middlename = request.MiddleName;
        teacher.Position = request.Position ?? teacher.Position;
        teacher.Workplace = request.Workplace ?? teacher.Workplace;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Профиль обновлён" });
    }
}