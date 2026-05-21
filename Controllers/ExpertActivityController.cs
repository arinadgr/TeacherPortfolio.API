using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeacherPortfolio.API.DTOs;
using TeacherPortfolio.API.Models;
using ClosedXML.Excel;
using TeacherPortfolio.API.Services;

namespace TeacherPortfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpertActivityController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExpertActivityController(AppDbContext context)
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

    private string GetFullName(Teacher teacher)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(teacher.Lastname)) parts.Add(teacher.Lastname);
        if (!string.IsNullOrEmpty(teacher.Firstname)) parts.Add(teacher.Firstname);
        if (!string.IsNullOrEmpty(teacher.Middlename)) parts.Add(teacher.Middlename);
        return parts.Count > 0 ? string.Join(" ", parts) : "Преподаватель";
    }

    // ========== CRUD МАРШРУТЫ ==========

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var items = await _context.Expertactivities
            .Include(x => x.Academicyear)
            .Include(x => x.Level)
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new ExpertActivityDto
            {
                Id = x.Id,
                EventDate = x.Eventdate,
                AcademicYearId = x.Academicyearid,
                AcademicYearName = x.Academicyear != null ? x.Academicyear.Name : "",
                EventName = x.Eventname,
                LevelId = x.Levelid,
                LevelName = x.Level != null ? x.Level.Name : "",
                ActivityType = x.Activitytype,
                DocumentDetails = x.Documentdetails,
                CreatedAt = x.Createdat
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.Expertactivities
            .Include(x => x.Academicyear)
            .Include(x => x.Level)
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        return Ok(new ExpertActivityDto
        {
            Id = item.Id,
            EventDate = item.Eventdate,
            AcademicYearId = item.Academicyearid,
            AcademicYearName = item.Academicyear != null ? item.Academicyear.Name : "",
            EventName = item.Eventname,
            LevelId = item.Levelid,
            LevelName = item.Level != null ? item.Level.Name : "",
            ActivityType = item.Activitytype,
            DocumentDetails = item.Documentdetails,
            CreatedAt = item.Createdat
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExpertActivityRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = new Expertactivity
        {
            Teacherid = teacher.Id,
            Eventdate = request.EventDate,
            Academicyearid = request.AcademicYearId,
            Eventname = request.EventName,
            Levelid = request.LevelId,
            Activitytype = request.ActivityType,
            Documentdetails = request.DocumentDetails,
            Createdat = DateTime.Now
        };

        _context.Expertactivities.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Экспертная деятельность добавлена", id = item.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateExpertActivityRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.Expertactivities
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        item.Eventdate = request.EventDate;
        item.Academicyearid = request.AcademicYearId;
        item.Eventname = request.EventName;
        item.Levelid = request.LevelId;
        item.Activitytype = request.ActivityType;
        item.Documentdetails = request.DocumentDetails;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись обновлена" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.Expertactivities
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        _context.Expertactivities.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись удалена" });
    }

    // ========== ЭКСПОРТ ==========

    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.Expertactivities
            .Include(x => x.Academicyear)
            .Include(x => x.Level)
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new ExpertActivityDto
            {
                Id = x.Id,
                EventDate = x.Eventdate,
                AcademicYearId = x.Academicyearid,
                AcademicYearName = x.Academicyear != null ? x.Academicyear.Name : "",
                EventName = x.Eventname,
                LevelId = x.Levelid,
                LevelName = x.Level != null ? x.Level.Name : "",
                ActivityType = x.Activitytype,
                DocumentDetails = x.Documentdetails,
                CreatedAt = x.Createdat
            })
            .ToListAsync();

        var pdfService = HttpContext.RequestServices.GetRequiredService<PdfService>();
        var pdfBytes = pdfService.GenerateExpertActivityPdf(data, GetFullName(teacher), teacher.Position ?? "Преподаватель");

        return File(pdfBytes, "application/pdf", $"Экспертная_деятельность_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.Expertactivities
            .Include(x => x.Academicyear)
            .Include(x => x.Level)
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new ExpertActivityDto
            {
                Id = x.Id,
                EventDate = x.Eventdate,
                AcademicYearId = x.Academicyearid,
                AcademicYearName = x.Academicyear != null ? x.Academicyear.Name : "",
                EventName = x.Eventname,
                LevelId = x.Levelid,
                LevelName = x.Level != null ? x.Level.Name : "",
                ActivityType = x.Activitytype,
                DocumentDetails = x.Documentdetails,
                CreatedAt = x.Createdat
            })
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Экспертная деятельность");

        worksheet.Cell(1, 1).Value = "Дата";
        worksheet.Cell(1, 2).Value = "Учебный год";
        worksheet.Cell(1, 3).Value = "Мероприятие";
        worksheet.Cell(1, 4).Value = "Уровень";
        worksheet.Cell(1, 5).Value = "Тип деятельности";
        worksheet.Cell(1, 6).Value = "Реквизиты документа";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cell(row, 1).Value = item.EventDate?.ToString("dd.MM.yyyy") ?? "";
            worksheet.Cell(row, 2).Value = item.AcademicYearName;
            worksheet.Cell(row, 3).Value = item.EventName;
            worksheet.Cell(row, 4).Value = item.LevelName;
            worksheet.Cell(row, 5).Value = item.ActivityType;
            worksheet.Cell(row, 6).Value = item.DocumentDetails;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Экспертная_деятельность_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}