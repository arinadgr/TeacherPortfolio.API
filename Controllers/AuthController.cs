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
        var exists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (exists)
            return BadRequest("Пользователь с таким email уже существует.");

        var user = new User
        {
            Email = request.Email,
            Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Teacher" : request.Role,
            Createdat = DateTime.Now 
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

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
}