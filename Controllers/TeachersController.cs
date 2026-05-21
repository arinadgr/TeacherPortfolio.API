using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherPortfolio.API.Models;

namespace TeacherPortfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly AppDbContext _context;

    public TeachersController(AppDbContext context)
    {
        _context = context;
    }

    // Метод для получения текущего пользователя
    private User? GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return null;

        var userId = int.Parse(userIdClaim.Value);
        return _context.Users.FirstOrDefault(u => u.Id == userId);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var user = GetCurrentUser();
        if (user == null)
            return Unauthorized("Пользователь не найден");

        return Ok(new { user.Email, user.Role });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teachers = await _context.Teachers.ToListAsync();
        return Ok(teachers);
    }
}