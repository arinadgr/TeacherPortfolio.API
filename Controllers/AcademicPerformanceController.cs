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
public class AcademicPerformanceController : ControllerBase
{
    private readonly AppDbContext _context;

    public AcademicPerformanceController(AppDbContext context)
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

    // ========== ЭКСПОРТ (конкретные маршруты) ==========

    

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var summary = await _context.AcademicPerformances
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .GroupBy(x => new { x.Discipline, x.AcademicYear.Name })
            .Select(g => new
            {
                Discipline = g.Key.Discipline,
                AcademicYear = g.Key.Name,
                AverageQuality = g.Average(x => x.QualityPercent),
                AverageSuccess = g.Average(x => x.SuccessPercent)
            })
            .OrderBy(x => x.Discipline)
            .ToListAsync();

        return Ok(summary);
    }

    // ========== CRUD МАРШРУТЫ ==========

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var items = await _context.AcademicPerformances
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AcademicPerformanceDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                Discipline = x.Discipline,
                GroupName = x.GroupName,
                QualityPercent = x.QualityPercent,
                SuccessPercent = x.SuccessPercent,
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

        var item = await _context.AcademicPerformances
            .Include(x => x.AcademicYear)
            .FirstOrDefaultAsync(x => x.Id == id && x.TeacherId == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        return Ok(new AcademicPerformanceDto
        {
            Id = item.Id,
            AcademicYearId = item.AcademicYearId,
            AcademicYearName = item.AcademicYear.Name,
            Discipline = item.Discipline,
            GroupName = item.GroupName,
            QualityPercent = item.QualityPercent,
            SuccessPercent = item.SuccessPercent,
            CreatedAt = item.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAcademicPerformanceRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        if (request.SuccessPercent != 100)
            return BadRequest("Успеваемость должна быть 100%");

        var item = new AcademicPerformance
        {
            TeacherId = teacher.Id,
            AcademicYearId = request.AcademicYearId,
            Discipline = request.Discipline,
            GroupName = request.GroupName,
            QualityPercent = request.QualityPercent,
            SuccessPercent = request.SuccessPercent,
            CreatedAt = DateTime.Now
        };

        _context.AcademicPerformances.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Данные успеваемости добавлены", id = item.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateAcademicPerformanceRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.AcademicPerformances
            .FirstOrDefaultAsync(x => x.Id == id && x.TeacherId == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        if (request.SuccessPercent != 100)
            return BadRequest("Успеваемость должна быть 100%");

        item.AcademicYearId = request.AcademicYearId;
        item.Discipline = request.Discipline;
        item.GroupName = request.GroupName;
        item.QualityPercent = request.QualityPercent;
        item.SuccessPercent = request.SuccessPercent;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Данные обновлены" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.AcademicPerformances
            .FirstOrDefaultAsync(x => x.Id == id && x.TeacherId == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        _context.AcademicPerformances.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись удалена" });
    }
    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.AcademicPerformances
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AcademicPerformanceDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                Discipline = x.Discipline,
                GroupName = x.GroupName,
                QualityPercent = x.QualityPercent,
                SuccessPercent = x.SuccessPercent,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        var pdfService = HttpContext.RequestServices.GetRequiredService<PdfService>();
        var pdfBytes = pdfService.GenerateAcademicPerformancePdf(data, GetFullName(teacher), teacher.Position ?? "Преподаватель");

        return File(pdfBytes, "application/pdf", $"Успеваемость_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.AcademicPerformances
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AcademicPerformanceDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                Discipline = x.Discipline,
                GroupName = x.GroupName,
                QualityPercent = x.QualityPercent,
                SuccessPercent = x.SuccessPercent,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Успеваемость");

        worksheet.Cell(1, 1).Value = "Учебный год";
        worksheet.Cell(1, 2).Value = "Дисциплина";
        worksheet.Cell(1, 3).Value = "Группа";
        worksheet.Cell(1, 4).Value = "Качество, %";
        worksheet.Cell(1, 5).Value = "Успеваемость, %";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cell(row, 1).Value = item.AcademicYearName;
            worksheet.Cell(row, 2).Value = item.Discipline;
            worksheet.Cell(row, 3).Value = item.GroupName;
            worksheet.Cell(row, 4).Value = item.QualityPercent;
            worksheet.Cell(row, 5).Value = item.SuccessPercent;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Успеваемость_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}