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
public class MethodicalMaterialController : ControllerBase
{
    private readonly AppDbContext _context;

    public MethodicalMaterialController(AppDbContext context)
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

        var items = await _context.Methodicalmaterials
            .Include(x => x.Academicyear)
            .Include(x => x.Materialtype)
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new MethodicalMaterialDto
            {
                Id = x.Id,
                AcademicYearId = x.Academicyearid,
                AcademicYearName = x.Academicyear.Name,
                MaterialTypeId = x.Materialtypeid,
                MaterialTypeName = x.Materialtype.Name,
                Specialty = x.Specialty,
                Topic = x.Topic,
                InternetLink = x.Internetlink,
                ApprovalDetails = x.Approvaldetails,
                ReviewingOrganization = x.Reviewingorganization,
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

        var item = await _context.Methodicalmaterials
            .Include(x => x.Academicyear)
            .Include(x => x.Materialtype)
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        return Ok(new MethodicalMaterialDto
        {
            Id = item.Id,
            AcademicYearId = item.Academicyearid,
            AcademicYearName = item.Academicyear.Name,
            MaterialTypeId = item.Materialtypeid,
            MaterialTypeName = item.Materialtype.Name,
            Specialty = item.Specialty,
            Topic = item.Topic,
            InternetLink = item.Internetlink,
            ApprovalDetails = item.Approvaldetails,
            ReviewingOrganization = item.Reviewingorganization,
            CreatedAt = item.Createdat
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMethodicalMaterialRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = new Methodicalmaterial
        {
            Teacherid = teacher.Id,
            Academicyearid = request.AcademicYearId,
            Materialtypeid = request.MaterialTypeId,
            Specialty = request.Specialty,
            Topic = request.Topic,
            Internetlink = request.InternetLink,
            Approvaldetails = request.ApprovalDetails,
            Reviewingorganization = request.ReviewingOrganization,
            Createdat = DateTime.Now
        };

        _context.Methodicalmaterials.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Методический материал добавлен", id = item.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateMethodicalMaterialRequest request)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.Methodicalmaterials
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        item.Academicyearid = request.AcademicYearId;
        item.Materialtypeid = request.MaterialTypeId;
        item.Specialty = request.Specialty;
        item.Topic = request.Topic;
        item.Internetlink = request.InternetLink;
        item.Approvaldetails = request.ApprovalDetails;
        item.Reviewingorganization = request.ReviewingOrganization;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Методический материал обновлён" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var item = await _context.Methodicalmaterials
            .FirstOrDefaultAsync(x => x.Id == id && x.Teacherid == teacher.Id);

        if (item == null) return NotFound("Запись не найдена");

        _context.Methodicalmaterials.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Запись удалена" });
    }

    // ========== ЭКСПОРТ ==========

    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.Methodicalmaterials
            .Include(x => x.Academicyear)
            .Include(x => x.Materialtype)
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new MethodicalMaterialDto
            {
                Id = x.Id,
                AcademicYearId = x.Academicyearid,
                AcademicYearName = x.Academicyear.Name,
                MaterialTypeId = x.Materialtypeid,
                MaterialTypeName = x.Materialtype.Name,
                Specialty = x.Specialty,
                Topic = x.Topic,
                InternetLink = x.Internetlink,
                ApprovalDetails = x.Approvaldetails,
                ReviewingOrganization = x.Reviewingorganization,
                CreatedAt = x.Createdat
            })
            .ToListAsync();

        var pdfService = HttpContext.RequestServices.GetRequiredService<PdfService>();
        var pdfBytes = pdfService.GenerateMethodicalMaterialPdf(data, GetFullName(teacher), teacher.Position ?? "Преподаватель");

        return File(pdfBytes, "application/pdf", $"Методические_материалы_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportToExcel()
    {
        var teacher = await GetCurrentTeacher();
        if (teacher == null) return NotFound("Профиль преподавателя не найден");

        var data = await _context.Methodicalmaterials
            .Include(x => x.Academicyear)
            .Include(x => x.Materialtype)
            .Where(x => x.Teacherid == teacher.Id)
            .OrderByDescending(x => x.Createdat)
            .Select(x => new MethodicalMaterialDto
            {
                Id = x.Id,
                AcademicYearId = x.Academicyearid,
                AcademicYearName = x.Academicyear.Name,
                MaterialTypeId = x.Materialtypeid,
                MaterialTypeName = x.Materialtype.Name,
                Specialty = x.Specialty,
                Topic = x.Topic,
                InternetLink = x.Internetlink,
                ApprovalDetails = x.Approvaldetails,
                ReviewingOrganization = x.Reviewingorganization,
                CreatedAt = x.Createdat
            })
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Методические материалы");

        worksheet.Cell(1, 1).Value = "Учебный год";
        worksheet.Cell(1, 2).Value = "Тип материала";
        worksheet.Cell(1, 3).Value = "Специальность";
        worksheet.Cell(1, 4).Value = "Тема";
        worksheet.Cell(1, 5).Value = "Ссылка";
        worksheet.Cell(1, 6).Value = "Реквизиты утверждения";
        worksheet.Cell(1, 7).Value = "Рецензент";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cell(row, 1).Value = item.AcademicYearName;
            worksheet.Cell(row, 2).Value = item.MaterialTypeName;
            worksheet.Cell(row, 3).Value = item.Specialty;
            worksheet.Cell(row, 4).Value = item.Topic;
            worksheet.Cell(row, 5).Value = item.InternetLink;
            worksheet.Cell(row, 6).Value = item.ApprovalDetails;
            worksheet.Cell(row, 7).Value = item.ReviewingOrganization;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Методические_материалы_{teacher.Lastname}_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}