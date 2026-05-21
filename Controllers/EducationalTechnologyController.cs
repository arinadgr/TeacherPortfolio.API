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
public class EducationalTechnologyController : ControllerBase
{
    private readonly AppDbContext _context;

    public EducationalTechnologyController(AppDbContext context)
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

        var items = await _context.Educationaltechnologies
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new EducationalTechnologyDto
            {
                Id = x.Id,
                TechnologyName = x.Technologyname,
                Purpose = x.Purpose,
                Result = x.Result,
                ResourceLink = x.Resourcelink,
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

        var item = await _context.Educationaltechnologies
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        return Ok(new EducationalTechnologyDto
        {
            Id = item.Id,
            TechnologyName = item.Technologyname,
            Purpose = item.Purpose,
            Result = item.Result,
            ResourceLink = item.Resourcelink,
            CreatedAt = item.Createdat
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEducationalTechnologyRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = new Educationaltechnology
        {
            Teacherid = teacher.Id,
            Technologyname = request.TechnologyName,
            Purpose = request.Purpose,
            Result = request.Result,
            Resourcelink = request.ResourceLink,
            Createdat = DateTime.Now
        };

        _context.Educationaltechnologies.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Образовательная технология добавлена", id = item.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateEducationalTechnologyRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.Educationaltechnologies
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        item.Technologyname = request.TechnologyName;
        item.Purpose = request.Purpose;
        item.Result = request.Result;
        item.Resourcelink = request.ResourceLink;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись обновлена" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.Educationaltechnologies
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        _context.Educationaltechnologies.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись удалена" });
    }

    // ========== ЭКСПОРТ ==========

    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.Educationaltechnologies
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new EducationalTechnologyDto
            {
                Id = x.Id,
                TechnologyName = x.Technologyname,
                Purpose = x.Purpose,
                Result = x.Result,
                ResourceLink = x.Resourcelink,
                CreatedAt = x.Createdat
            })
            .ToListAsync();

        var pdfService = HttpContext.RequestServices.GetRequiredService<PdfService>();
        var pdfBytes = pdfService.GenerateEducationalTechnologyPdf(data, GetFullName(teacher), teacher.Position ?? "Преподаватель");

        return File(pdfBytes, "application/pdf", $"Технологии_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.Educationaltechnologies
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new EducationalTechnologyDto
            {
                Id = x.Id,
                TechnologyName = x.Technologyname,
                Purpose = x.Purpose,
                Result = x.Result,
                ResourceLink = x.Resourcelink,
                CreatedAt = x.Createdat
            })
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Образовательные технологии");

        worksheet.Cell(1, 1).Value = "Технология";
        worksheet.Cell(1, 2).Value = "Цель использования";
        worksheet.Cell(1, 3).Value = "Результат";
        worksheet.Cell(1, 4).Value = "Ссылка на ресурс";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cell(row, 1).Value = item.TechnologyName;
            worksheet.Cell(row, 2).Value = item.Purpose;
            worksheet.Cell(row, 3).Value = item.Result;
            worksheet.Cell(row, 4).Value = item.ResourceLink;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Технологии_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}