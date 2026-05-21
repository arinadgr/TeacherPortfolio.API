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
public class ExperienceSharingController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExperienceSharingController(AppDbContext context)
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

        var items = await _context.Experiencesharings
            .Include(x => x.Level)
            .Include(x => x.Format)
            .Include(x => x.Sharingform)
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new ExperienceSharingDto
            {
                Id = x.Id,
                LevelId = x.Levelid,
                LevelName = x.Level != null ? x.Level.Name : "",
                FormatId = x.Formatid,
                FormatName = x.Format != null ? x.Format.Name : "",
                SharingFormId = x.Sharingformid,
                SharingFormName = x.Sharingform != null ? x.Sharingform.Name : "",
                EventName = x.Eventname,
                Topic = x.Topic,
                EventDate = x.Eventdate,
                Organizer = x.Organizer,
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

        var item = await _context.Experiencesharings
            .Include(x => x.Level)
            .Include(x => x.Format)
            .Include(x => x.Sharingform)
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        return Ok(new ExperienceSharingDto
        {
            Id = item.Id,
            LevelId = item.Levelid,
            LevelName = item.Level != null ? item.Level.Name : "",
            FormatId = item.Formatid,
            FormatName = item.Format != null ? item.Format.Name : "",
            SharingFormId = item.Sharingformid,
            SharingFormName = item.Sharingform != null ? item.Sharingform.Name : "",
            EventName = item.Eventname,
            Topic = item.Topic,
            EventDate = item.Eventdate,
            Organizer = item.Organizer,
            CreatedAt = item.Createdat
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExperienceSharingRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = new Experiencesharing
        {
            Teacherid = teacher.Id,
            Levelid = request.LevelId,
            Formatid = request.FormatId,
            Sharingformid = request.SharingFormId,
            Eventname = request.EventName,
            Topic = request.Topic,
            Eventdate = request.EventDate,
            Organizer = request.Organizer,
            Createdat = DateTime.Now
        };

        _context.Experiencesharings.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись о трансляции опыта добавлена", id = item.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateExperienceSharingRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.Experiencesharings
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        item.Levelid = request.LevelId;
        item.Formatid = request.FormatId;
        item.Sharingformid = request.SharingFormId;
        item.Eventname = request.EventName;
        item.Topic = request.Topic;
        item.Eventdate = request.EventDate;
        item.Organizer = request.Organizer;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись обновлена" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.Experiencesharings
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        _context.Experiencesharings.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись удалена" });
    }

    // ========== ЭКСПОРТ ==========

    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.Experiencesharings
            .Include(x => x.Level)
            .Include(x => x.Format)
            .Include(x => x.Sharingform)
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new ExperienceSharingDto
            {
                Id = x.Id,
                LevelId = x.Levelid,
                LevelName = x.Level != null ? x.Level.Name : "",
                FormatId = x.Formatid,
                FormatName = x.Format != null ? x.Format.Name : "",
                SharingFormId = x.Sharingformid,
                SharingFormName = x.Sharingform != null ? x.Sharingform.Name : "",
                EventName = x.Eventname,
                Topic = x.Topic,
                EventDate = x.Eventdate,
                Organizer = x.Organizer,
                CreatedAt = x.Createdat
            })
            .ToListAsync();

        var pdfService = HttpContext.RequestServices.GetRequiredService<PdfService>();
        var pdfBytes = pdfService.GenerateExperienceSharingPdf(data, GetFullName(teacher), teacher.Position ?? "Преподаватель");

        return File(pdfBytes, "application/pdf", $"Трансляция_опыта_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.Experiencesharings
            .Include(x => x.Level)
            .Include(x => x.Format)
            .Include(x => x.Sharingform)
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new ExperienceSharingDto
            {
                Id = x.Id,
                LevelId = x.Levelid,
                LevelName = x.Level != null ? x.Level.Name : "",
                FormatId = x.Formatid,
                FormatName = x.Format != null ? x.Format.Name : "",
                SharingFormId = x.Sharingformid,
                SharingFormName = x.Sharingform != null ? x.Sharingform.Name : "",
                EventName = x.Eventname,
                Topic = x.Topic,
                EventDate = x.Eventdate,
                Organizer = x.Organizer,
                CreatedAt = x.Createdat
            })
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Трансляция опыта");

        worksheet.Cell(1, 1).Value = "Дата";
        worksheet.Cell(1, 2).Value = "Мероприятие";
        worksheet.Cell(1, 3).Value = "Уровень";
        worksheet.Cell(1, 4).Value = "Формат";
        worksheet.Cell(1, 5).Value = "Форма трансляции";
        worksheet.Cell(1, 6).Value = "Тема";
        worksheet.Cell(1, 7).Value = "Организатор";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cell(row, 1).Value = item.EventDate.ToString("dd.MM.yyyy");
            worksheet.Cell(row, 2).Value = item.EventName;
            worksheet.Cell(row, 3).Value = item.LevelName;
            worksheet.Cell(row, 4).Value = item.FormatName;
            worksheet.Cell(row, 5).Value = item.SharingFormName;
            worksheet.Cell(row, 6).Value = item.Topic;
            worksheet.Cell(row, 7).Value = item.Organizer;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Трансляция_опыта_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}