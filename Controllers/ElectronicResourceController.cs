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
public class ElectronicResourceController : ControllerBase
{
    private readonly AppDbContext _context;

    public ElectronicResourceController(AppDbContext context)
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

        var items = await _context.ElectronicResources
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ElectronicResourceDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                Name = x.Name,
                Topic = x.Topic,
                InteractionForm = x.InteractionForm,
                Link = x.Link,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.ElectronicResources
            .Include(x => x.AcademicYear)
            .FirstOrDefaultAsync(x => x.Id == id && x.TeacherId == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        return Ok(new ElectronicResourceDto
        {
            Id = item.Id,
            AcademicYearId = item.AcademicYearId,
            AcademicYearName = item.AcademicYear.Name,
            Name = item.Name,
            Topic = item.Topic,
            InteractionForm = item.InteractionForm,
            Link = item.Link,
            CreatedAt = item.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateElectronicResourceRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = new ElectronicResource
        {
            TeacherId = teacher.Id,
            AcademicYearId = request.AcademicYearId,
            Name = request.Name,
            Topic = request.Topic,
            InteractionForm = request.InteractionForm,
            Link = request.Link,
            CreatedAt = DateTime.Now
        };

        _context.ElectronicResources.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Электронный ресурс добавлен", id = item.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateElectronicResourceRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.ElectronicResources
            .FirstOrDefaultAsync(x => x.Id == id && x.TeacherId == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        item.AcademicYearId = request.AcademicYearId;
        item.Name = request.Name;
        item.Topic = request.Topic;
        item.InteractionForm = request.InteractionForm;
        item.Link = request.Link;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Электронный ресурс обновлён" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.ElectronicResources
            .FirstOrDefaultAsync(x => x.Id == id && x.TeacherId == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        _context.ElectronicResources.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись удалена" });
    }

    // ========== ЭКСПОРТ ==========

    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.ElectronicResources
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ElectronicResourceDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                Name = x.Name,
                Topic = x.Topic,
                InteractionForm = x.InteractionForm,
                Link = x.Link,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        var pdfService = HttpContext.RequestServices.GetRequiredService<PdfService>();
        var pdfBytes = pdfService.GenerateElectronicResourcePdf(data, GetFullName(teacher), teacher.Position ?? "Преподаватель");

        return File(pdfBytes, "application/pdf", $"ЭОР_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.ElectronicResources
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ElectronicResourceDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                Name = x.Name,
                Topic = x.Topic,
                InteractionForm = x.InteractionForm,
                Link = x.Link,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Электронные ресурсы");

        worksheet.Cell(1, 1).Value = "Учебный год";
        worksheet.Cell(1, 2).Value = "Название";
        worksheet.Cell(1, 3).Value = "Тема";
        worksheet.Cell(1, 4).Value = "Форма взаимодействия";
        worksheet.Cell(1, 5).Value = "Ссылка";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cell(row, 1).Value = item.AcademicYearName;
            worksheet.Cell(row, 2).Value = item.Name;
            worksheet.Cell(row, 3).Value = item.Topic;
            worksheet.Cell(row, 4).Value = item.InteractionForm;
            worksheet.Cell(row, 5).Value = item.Link;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"ЭОР_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}