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
public class GraduationResultController : ControllerBase
{
    private readonly AppDbContext _context;

    public GraduationResultController(AppDbContext context)
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

        var items = await _context.GraduationResults
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GraduationResultDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                StudentName = x.StudentName,
                GroupName = x.GroupName,
                Specialty = x.Specialty,
                ThesisTopic = x.ThesisTopic,
                Grade = x.Grade,
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

        var item = await _context.GraduationResults
            .Include(x => x.AcademicYear)
            .FirstOrDefaultAsync(x => x.Id == id && x.TeacherId == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        return Ok(new GraduationResultDto
        {
            Id = item.Id,
            AcademicYearId = item.AcademicYearId,
            AcademicYearName = item.AcademicYear.Name,
            StudentName = item.StudentName,
            GroupName = item.GroupName,
            Specialty = item.Specialty,
            ThesisTopic = item.ThesisTopic,
            Grade = item.Grade,
            CreatedAt = item.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateGraduationResultRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = new GraduationResult
        {
            TeacherId = teacher.Id,
            AcademicYearId = request.AcademicYearId,
            StudentName = request.StudentName,
            GroupName = request.GroupName,
            Specialty = request.Specialty,
            ThesisTopic = request.ThesisTopic,
            Grade = request.Grade,
            CreatedAt = DateTime.Now
        };

        _context.GraduationResults.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Результат ГИА добавлен", id = item.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateGraduationResultRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.GraduationResults
            .FirstOrDefaultAsync(x => x.Id == id && x.TeacherId == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        item.AcademicYearId = request.AcademicYearId;
        item.StudentName = request.StudentName;
        item.GroupName = request.GroupName;
        item.Specialty = request.Specialty;
        item.ThesisTopic = request.ThesisTopic;
        item.Grade = request.Grade;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Результат ГИА обновлён" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.GraduationResults
            .FirstOrDefaultAsync(x => x.Id == id && x.TeacherId == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        _context.GraduationResults.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись удалена" });
    }

    // ========== ЭКСПОРТ ==========

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var results = await _context.GraduationResults
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .ToListAsync();

        var summary = results
            .GroupBy(x => x.AcademicYear.Name)
            .Select(g => new
            {
                AcademicYear = g.Key,
                TotalCount = g.Count(),
                ExcellentCount = g.Count(x => x.Grade == "отлично"),
                GoodCount = g.Count(x => x.Grade == "хорошо"),
                QualityPercent = g.Count() > 0 ? (decimal)(g.Count(x => x.Grade == "отлично" || x.Grade == "хорошо")) / g.Count() * 100 : 0,
                SuccessPercent = g.Any(x => x.Grade == "неудовлетворительно") ? 0 : 100
            })
            .OrderBy(x => x.AcademicYear)
            .ToList();

        return Ok(summary);
    }

    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.GraduationResults
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GraduationResultDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                StudentName = x.StudentName,
                GroupName = x.GroupName,
                Specialty = x.Specialty,
                ThesisTopic = x.ThesisTopic,
                Grade = x.Grade,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        var pdfService = HttpContext.RequestServices.GetRequiredService<PdfService>();
        var pdfBytes = pdfService.GenerateGraduationResultPdf(data, GetFullName(teacher), teacher.Position ?? "Преподаватель");

        return File(pdfBytes, "application/pdf", $"ГИА_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.GraduationResults
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GraduationResultDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear.Name,
                StudentName = x.StudentName,
                GroupName = x.GroupName,
                Specialty = x.Specialty,
                ThesisTopic = x.ThesisTopic,
                Grade = x.Grade,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("ГИА");

        worksheet.Cell(1, 1).Value = "Учебный год";
        worksheet.Cell(1, 2).Value = "Студент";
        worksheet.Cell(1, 3).Value = "Группа";
        worksheet.Cell(1, 4).Value = "Специальность";
        worksheet.Cell(1, 5).Value = "Тема ВКР";
        worksheet.Cell(1, 6).Value = "Оценка";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cell(row, 1).Value = item.AcademicYearName;
            worksheet.Cell(row, 2).Value = item.StudentName;
            worksheet.Cell(row, 3).Value = item.GroupName;
            worksheet.Cell(row, 4).Value = item.Specialty;
            worksheet.Cell(row, 5).Value = item.ThesisTopic;
            worksheet.Cell(row, 6).Value = item.Grade;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"ГИА_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}