using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeacherPortfolio.API.DTOs;
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

    private User? GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return null;
        var userId = int.Parse(userIdClaim.Value);
        return _context.Users.FirstOrDefault(u => u.Id == userId);
    }

    private async Task<Teacher?> GetCurrentTeacher()
    {
        var user = GetCurrentUser();
        if (user == null) return null;
        return await _context.Teachers.FirstOrDefaultAsync(t => t.Userid == user.Id);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null)
            return Unauthorized(new { message = "Профиль преподавателя не найден" });

        var user = GetCurrentUser();

        return Ok(new
        {
            id = teacher.Id,
            fullName = $"{teacher.Lastname} {teacher.Firstname} {teacher.Middlename}".Trim(),
            position = teacher.Position,
            workplace = teacher.Workplace,
            email = user?.Email
        });
    }
}