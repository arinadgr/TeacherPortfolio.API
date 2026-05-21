using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeacherPortfolio.API.DTOs;
using TeacherPortfolio.API.Models;
using TeacherPortfolio.API.Services;

namespace TeacherPortfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentAchievementsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentAchievementsController(AppDbContext context)
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

    // Получить полное имя преподавателя
    private string GetFullName(Teacher teacher)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(teacher.Lastname)) parts.Add(teacher.Lastname);
        if (!string.IsNullOrEmpty(teacher.Firstname)) parts.Add(teacher.Firstname);
        if (!string.IsNullOrEmpty(teacher.Middlename)) parts.Add(teacher.Middlename);
        return parts.Count > 0 ? string.Join(" ", parts) : "Преподаватель";
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var user = GetCurrentUser();
        if (user == null)
            return Unauthorized("Пользователь не найден");

        var teacher = await _context.Teachers
            .Include(t => t.Qualificationcategory)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Userid == user.Id);

        if (teacher == null)
            return NotFound("Профиль преподавателя не найден. Обратитесь к администратору.");

        var achievements = await _context.Studentachievements
            .Include(s => s.Academicyear)
            .Include(s => s.Direction)
            .Include(s => s.Level)
            .Include(s => s.Result)
            .Where(s => s.Teacherid == teacher.Id)
            .OrderByDescending(s => s.Createdat)
            .ToListAsync();

        var response = new TeacherProfileResponse
        {
            Id = teacher.Id,
            FullName = GetFullName(teacher),
            Position = teacher.Position ?? "Не указана",
            Workplace = teacher.Workplace ?? "Не указано",
            QualificationCategory = teacher.Qualificationcategory?.Name,
            Email = user.Email,
            Achievements = achievements.Select(a => new StudentAchievementResponse
            {
                Id = a.Id,
                StudentName = a.Studentname,
                AchievementType = a.Eventname,
                Level = a.Level?.Name ?? "Не указан",
                AcademicYear = a.Academicyear?.Name ?? "Не указан",
                Result = a.Result?.Name,
                EventDate = a.Eventdate,
                EventOrganizer = a.Eventorganizer,
                GroupName = a.Groupname,
                ResultDescription = a.Resultdescription,
                CreatedAt = a.Createdat
            }).ToList()
        };

        return Ok(response);
    }

    [HttpGet("my-achievements")]
    public async Task<IActionResult> GetMyAchievements()
    {
        var user = GetCurrentUser();
        if (user == null)
            return Unauthorized();

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Userid == user.Id);

        if (teacher == null)
            return NotFound("Профиль преподавателя не найден");

        var achievements = await _context.Studentachievements
            .Include(s => s.Academicyear)
            .Include(s => s.Direction)
            .Include(s => s.Level)
            .Include(s => s.Result)
            .Where(s => s.Teacherid == teacher.Id)
            .OrderByDescending(s => s.Createdat)
            .Select(s => new StudentAchievementResponse
            {
                Id = s.Id,
                StudentName = s.Studentname,
                AchievementType = s.Eventname,
                Level = s.Level != null ? s.Level.Name : "Не указан",
                AcademicYear = s.Academicyear != null ? s.Academicyear.Name : "Не указан",
                Result = s.Result != null ? s.Result.Name : null,
                EventDate = s.Eventdate,
                EventOrganizer = s.Eventorganizer,
                GroupName = s.Groupname,
                ResultDescription = s.Resultdescription,
                CreatedAt = s.Createdat
            })
            .ToListAsync();

        return Ok(achievements);
    }

    [HttpPost("achievements")]
    public async Task<IActionResult> CreateAchievement(CreateStudentAchievementRequest request)
    {
        var user = GetCurrentUser();
        if (user == null)
            return Unauthorized();

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Userid == user.Id);

        if (teacher == null)
            return NotFound("Профиль преподавателя не найден");

        var achievement = new Studentachievement
        {
            Teacherid = teacher.Id,
            Academicyearid = request.AcademicyearId,
            Eventdate = request.EventDate,
            Eventname = request.AchievementType,
            Eventorganizer = request.EventOrganizer,
            Directionid = request.DirectionId,
            Levelid = request.LevelId,
            Studentname = request.StudentName,
            Groupname = request.GroupName,
            Resultid = request.ResultId,
            Resultdescription = request.ResultDescription,
            Orderdetails = request.OrderDetails,
            Documentlink = request.DocumentLink,
            Createdat = DateTime.Now
        };

        _context.Studentachievements.Add(achievement);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Достижение успешно добавлено", id = achievement.Id });
    }

    [HttpDelete("achievements/{id}")]
    public async Task<IActionResult> DeleteAchievement(int id)
    {
        var user = GetCurrentUser();
        if (user == null)
            return Unauthorized();

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Userid == user.Id);

        if (teacher == null)
            return NotFound("Профиль преподавателя не найден");

        var achievement = await _context.Studentachievements
            .FirstOrDefaultAsync(a => a.Id == id && a.Teacherid == teacher.Id);

        if (achievement == null)
            return NotFound("Достижение не найдено или у вас нет прав на его удаление");

        _context.Studentachievements.Remove(achievement);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Достижение успешно удалено" });
    }
    [HttpPut("achievements/{id}")]
    public async Task<IActionResult> UpdateAchievement(int id, UpdateStudentAchievementRequest request)
    {
        var user = GetCurrentUser();
        if (user == null)
            return Unauthorized();

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Userid == user.Id);

        if (teacher == null)
            return NotFound("Профиль преподавателя не найден");

        var achievement = await _context.Studentachievements
            .FirstOrDefaultAsync(a => a.Id == id && a.Teacherid == teacher.Id);

        if (achievement == null)
            return NotFound("Достижение не найдено или у вас нет прав на его редактирование");

        // Обновляем поля
        achievement.Studentname = request.StudentName;
        achievement.Eventname = request.AchievementType;
        achievement.Eventdate = request.EventDate;
        achievement.Eventorganizer = request.EventOrganizer;
        achievement.Groupname = request.GroupName;
        achievement.Resultdescription = request.ResultDescription;
        achievement.Orderdetails = request.OrderDetails;
        achievement.Documentlink = request.DocumentLink;
        achievement.Academicyearid = request.AcademicyearId;
        achievement.Directionid = request.DirectionId;
        achievement.Levelid = request.LevelId;
        achievement.Resultid = request.ResultId;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Достижение успешно обновлено", id = achievement.Id });
    }
    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var user = GetCurrentUser();
        if (user == null)
            return Unauthorized();

        var teacher = await _context.Teachers
            .Include(t => t.Qualificationcategory)
            .FirstOrDefaultAsync(t => t.Userid == user.Id);

        if (teacher == null)
            return NotFound("Профиль преподавателя не найден");

        var achievements = await _context.Studentachievements
            .Include(s => s.Academicyear)
            .Include(s => s.Level)
            .Include(s => s.Result)
            .Where(s => s.Teacherid == teacher.Id)
            .OrderByDescending(s => s.Createdat)
            .Select(s => new StudentAchievementResponse
            {
                Id = s.Id,
                StudentName = s.Studentname,
                AchievementType = s.Eventname,
                Level = s.Level != null ? s.Level.Name : "Не указан",
                AcademicYear = s.Academicyear != null ? s.Academicyear.Name : "Не указан",
                Result = s.Result != null ? s.Result.Name : null,
                EventDate = s.Eventdate,
                EventOrganizer = s.Eventorganizer,
                GroupName = s.Groupname,
                ResultDescription = s.Resultdescription,
                CreatedAt = s.Createdat
            })
            .ToListAsync();

        var profile = new TeacherProfileResponse
        {
            Id = teacher.Id,
            FullName = GetFullName(teacher),
            Position = teacher.Position ?? "Не указана",
            Workplace = teacher.Workplace ?? "Не указано",
            Email = user.Email,
            Achievements = achievements
        };

        var pdfService = HttpContext.RequestServices.GetRequiredService<PdfService>();
        var pdfBytes = pdfService.GenerateAchievementsReport(profile);

        return File(pdfBytes, "application/pdf", $"Достижения_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.pdf");
    }
    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var user = GetCurrentUser();
        if (user == null)
            return Unauthorized();

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Userid == user.Id);

        if (teacher == null)
            return NotFound("Профиль преподавателя не найден");

        var achievements = await _context.Studentachievements
            .Include(s => s.Academicyear)
            .Include(s => s.Level)
            .Include(s => s.Result)
            .Where(s => s.Teacherid == teacher.Id)
            .OrderByDescending(s => s.Createdat)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Достижения");

        // Заголовки
        worksheet.Cell(1, 1).Value = "Студент";
        worksheet.Cell(1, 2).Value = "Мероприятие";
        worksheet.Cell(1, 3).Value = "Уровень";
        worksheet.Cell(1, 4).Value = "Дата";
        worksheet.Cell(1, 5).Value = "Организатор";
        worksheet.Cell(1, 6).Value = "Группа";
        worksheet.Cell(1, 7).Value = "Результат";
        worksheet.Cell(1, 8).Value = "Учебный год";

        // Стиль заголовков
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Данные
        int row = 2;
        foreach (var a in achievements)
        {
            worksheet.Cell(row, 1).Value = a.Studentname;
            worksheet.Cell(row, 2).Value = a.Eventname;
            worksheet.Cell(row, 3).Value = a.Level?.Name ?? "";
            worksheet.Cell(row, 4).Value = a.Eventdate.ToString("dd.MM.yyyy");
            worksheet.Cell(row, 5).Value = a.Eventorganizer ?? "";
            worksheet.Cell(row, 6).Value = a.Groupname ?? "";
            worksheet.Cell(row, 7).Value = a.Result?.Name ?? "";
            worksheet.Cell(row, 8).Value = a.Academicyear?.Name ?? "";
            row++;
        }

        // Автоматическая ширина колонок
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Достижения_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}