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
public class PassportController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ScoreCalculatorService _scoreCalculator;

    public PassportController(AppDbContext context, ScoreCalculatorService scoreCalculator)
    {
        _context = context;
        _scoreCalculator = scoreCalculator;
    }

    private User? GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return null;
        var userId = int.Parse(userIdClaim.Value);
        return _context.Users.FirstOrDefault(u => u.Id == userId);
    }

    private string GetFullName(Teacher teacher)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(teacher.Lastname)) parts.Add(teacher.Lastname);
        if (!string.IsNullOrEmpty(teacher.Firstname)) parts.Add(teacher.Firstname);
        if (!string.IsNullOrEmpty(teacher.Middlename)) parts.Add(teacher.Middlename);
        return parts.Count > 0 ? string.Join(" ", parts) : "Преподаватель";
    }

    // ========== ОСНОВНОЙ МЕТОД ПОЛУЧЕНИЯ ПАСПОРТА ==========
    [HttpGet]
    public async Task<IActionResult> GetPassport()
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var teacher = await _context.Teachers
            .Include(t => t.Qualificationcategory)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Userid == user.Id);

        if (teacher == null)
            return NotFound("Профиль преподавателя не найден");

        var passport = await BuildPassport(teacher, user);
        return Ok(passport);
    }

    // ========== ЭКСПОРТ В PDF ==========
    [HttpGet("export-pdf")]
    public async Task<IActionResult> ExportToPdf()
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var teacher = await _context.Teachers
            .Include(t => t.Qualificationcategory)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Userid == user.Id);

        if (teacher == null)
            return NotFound("Профиль преподавателя не найден");

        var passport = await BuildPassport(teacher, user);

        var pdfService = HttpContext.RequestServices.GetRequiredService<PdfService>();
        var pdfBytes = pdfService.GeneratePassportPdf(passport);

        return File(pdfBytes, "application/pdf", $"Модельный_паспорт_{GetFullName(teacher)}_{DateTime.Now:yyyyMMdd}.pdf");
    }

    // ========== ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ ФОРМИРОВАНИЯ ПАСПОРТА ==========
    private async Task<ModelPassportDto> BuildPassport(Teacher teacher, User user)
    {
        var passport = new ModelPassportDto();

        // Информация о преподавателе
        passport.TeacherInfo = new TeacherInfoDto
        {
            Id = teacher.Id,
            FullName = GetFullName(teacher),
            Position = teacher.Position ?? "Не указана",
            Workplace = teacher.Workplace ?? "Не указано",
            QualificationCategory = teacher.Qualificationcategory?.Name,
            OrderNumber = teacher.Ordernumber,
            OrderDate = teacher.Orderdate,
            Email = user.Email
        };

        // ========== ПАРАМЕТР I ==========

        // 1.1 Успеваемость
        var academicPerformances = await _context.AcademicPerformances
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .ToListAsync();

        passport.Parameter1.AcademicPerformances = academicPerformances.Select(x => new AcademicPerformanceTableDto
        {
            AcademicYear = x.AcademicYear.Name,
            Discipline = x.Discipline,
            GroupName = x.GroupName,
            QualityPercent = x.QualityPercent,
            SuccessPercent = x.SuccessPercent
        }).ToList();

        var summary = academicPerformances
            .GroupBy(x => x.Discipline)
            .Select(g => new AcademicPerformanceSummaryDto
            {
                Discipline = g.Key,
                AverageQuality = g.Average(x => x.QualityPercent),
                AverageSuccess = g.Average(x => x.SuccessPercent),
                Score = _scoreCalculator.CalculateAcademicPerformanceScore(g.Average(x => x.QualityPercent))
            }).FirstOrDefault();

        passport.Parameter1.AcademicPerformanceSummary = summary;

        // 1.2 ГИА
        var graduationResults = await _context.GraduationResults
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .ToListAsync();

        passport.Parameter1.GraduationResults = graduationResults.Select(x => new GraduationResultTableDto
        {
            AcademicYear = x.AcademicYear.Name,
            StudentName = x.StudentName,
            GroupName = x.GroupName,
            ThesisTopic = x.ThesisTopic,
            Grade = x.Grade
        }).ToList();

        var graduationSummary = graduationResults
            .GroupBy(x => x.AcademicYear.Name)
            .Select(g => new GraduationResultSummaryDto
            {
                AcademicYear = g.Key,
                TotalCount = g.Count(),
                QualityPercent = g.Count() > 0 ? (decimal)g.Count(x => x.Grade == "отлично" || x.Grade == "хорошо") / g.Count() * 100 : 0,
                SuccessPercent = g.Any(x => x.Grade == "неудовлетворительно") ? 0 : 100,
                Score = _scoreCalculator.CalculateGraduationResultScore(
                    g.Count() > 0 ? (decimal)g.Count(x => x.Grade == "отлично" || x.Grade == "хорошо") / g.Count() * 100 : 0,
                    g.Any(x => x.Grade == "неудовлетворительно") ? 0 : 100)
            }).FirstOrDefault();

        passport.Parameter1.GraduationResultSummary = graduationSummary;

        // 1.3 Достижения студентов
        var studentAchievements = await _context.Studentachievements
            .Include(x => x.Academicyear)
            .Include(x => x.Level)
            .Include(x => x.Result)
            .Include(x => x.Direction)
            .Where(x => x.Teacherid == teacher.Id)
            .ToListAsync();

        passport.Parameter1.StudentAchievements = studentAchievements.Select(x => new StudentAchievementTableDto
        {
            AcademicYear = x.Academicyear?.Name ?? "",
            EventDate = x.Eventdate,
            EventName = x.Eventname,
            Organizer = x.Eventorganizer ?? "",
            Direction = x.Direction?.Name ?? "",
            Level = x.Level?.Name ?? "",
            StudentName = x.Studentname,
            GroupName = x.Groupname ?? "",
            Result = x.Result?.Name ?? "",
            DocumentDetails = x.Orderdetails,
            Link = x.Documentlink
        }).ToList();

        var scores = _scoreCalculator.CalculateStudentAchievementScores(studentAchievements);
        passport.Parameter1.StudentAchievementScores = new StudentAchievementScoresDto
        {
            WinnersScore = scores.winnersScore,
            ParticipantsScore = scores.participantsScore,
            TotalScore = scores.totalScore
        };

        // ========== ПАРАМЕТР II ==========

        // 2.1.1 Методические материалы
        var methodicalMaterials = await _context.Methodicalmaterials
            .Include(x => x.Academicyear)
            .Include(x => x.Materialtype)
            .Where(x => x.Teacherid == teacher.Id)
            .ToListAsync();

        passport.Parameter2.MethodicalMaterials = methodicalMaterials.Select(x => new MethodicalMaterialTableDto
        {
            AcademicYear = x.Academicyear.Name,
            MaterialType = x.Materialtype.Name,
            Topic = x.Topic,
            Link = x.Internetlink,
            ApprovalDetails = x.Approvaldetails,
            ReviewOrganization = x.Reviewingorganization
        }).ToList();

        passport.Parameter2.MethodicalMaterialScore = methodicalMaterials.Any()
            ? _scoreCalculator.CalculateMethodicalMaterialScore(methodicalMaterials.Any(m => !string.IsNullOrEmpty(m.Reviewingorganization)))
            : 0;

        // 2.1.3 Электронные ресурсы
        var electronicResources = await _context.ElectronicResources
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .ToListAsync();

        passport.Parameter2.ElectronicResources = electronicResources.Select(x => new ElectronicResourceTableDto
        {
            AcademicYear = x.AcademicYear.Name,
            Name = x.Name,
            Topic = x.Topic,
            InteractionForm = x.InteractionForm,
            Link = x.Link
        }).ToList();

        passport.Parameter2.ElectronicResourceScore = electronicResources.Any() ? 15 : 5;

        // 2.3.1 Трансляция опыта
        var experienceSharings = await _context.Experiencesharings
            .Include(x => x.Level)
            .Include(x => x.Format)
            .Include(x => x.Sharingform)
            .Where(x => x.Teacherid == teacher.Id)
            .ToListAsync();

        passport.Parameter2.ExperienceSharings = experienceSharings.Select(x => new ExperienceSharingTableDto
        {
            EventDate = x.Eventdate,
            EventName = x.Eventname,
            Level = x.Level?.Name ?? "",
            Format = x.Format?.Name ?? "",
            SharingForm = x.Sharingform?.Name ?? "",
            Topic = x.Topic,
            Organizer = x.Organizer
        }).ToList();

        passport.Parameter2.ExperienceSharingScore = experienceSharings
            .Select(x => _scoreCalculator.CalculateExperienceSharingScore(x.Level?.Name ?? ""))
            .DefaultIfEmpty(0)
            .Max();

        // 2.4 Конкурсы профмастерства
        var teacherContests = await _context.TeacherContests
            .Include(x => x.AcademicYear)
            .Where(x => x.TeacherId == teacher.Id)
            .ToListAsync();

        passport.Parameter2.TeacherContests = teacherContests.Select(x => new TeacherContestTableDto
        {
            AcademicYear = x.AcademicYear.Name,
            ContestName = x.ContestName,
            Organizer = x.Organizer,
            Level = x.Level,
            Result = x.Result,
            OrderDetails = x.OrderDetails,
            Link = x.Link
        }).ToList();

        passport.Parameter2.TeacherContestScore = teacherContests
            .Select(x => _scoreCalculator.CalculateTeacherContestScore(x.Level, x.Result, true))
            .DefaultIfEmpty(0)
            .Max();

        // 2.5 Экспертная деятельность
        var expertActivities = await _context.Expertactivities
            .Include(x => x.Academicyear)
            .Include(x => x.Level)
            .Where(x => x.Teacherid == teacher.Id)
            .ToListAsync();

        passport.Parameter2.ExpertActivities = expertActivities.Select(x => new ExpertActivityTableDto
        {
            EventDate = x.Eventdate,
            AcademicYear = x.Academicyear?.Name ?? "",
            EventName = x.Eventname,
            Level = x.Level?.Name ?? "",
            ActivityType = x.Activitytype ?? "",
            DocumentDetails = x.Documentdetails
        }).ToList();

        passport.Parameter2.ExpertActivityScore = expertActivities
            .Select(x => _scoreCalculator.CalculateExpertActivityScore(x.Level?.Name ?? "", x.Activitytype ?? ""))
            .DefaultIfEmpty(0)
            .Max();

        // 2.6 Образовательные технологии
        var educationalTechnologies = await _context.Educationaltechnologies
            .Where(x => x.Teacherid == teacher.Id)
            .ToListAsync();

        passport.Parameter2.EducationalTechnologies = educationalTechnologies.Select(x => new EducationalTechnologyTableDto
        {
            TechnologyName = x.Technologyname,
            Purpose = x.Purpose,
            Result = x.Result,
            ResourceLink = x.Resourcelink
        }).ToList();

        passport.Parameter2.EducationalTechnologyScore = educationalTechnologies.Any() ? 20 : 0;

        // ========== ИТОГОВЫЕ БАЛЛЫ ==========

        int param1Total = (summary?.Score ?? 0) + (graduationSummary?.Score ?? 0) + (scores.totalScore);
        int param2Total = passport.Parameter2.MethodicalMaterialScore +
                          passport.Parameter2.ElectronicResourceScore +
                          passport.Parameter2.ExperienceSharingScore +
                          passport.Parameter2.TeacherContestScore +
                          passport.Parameter2.ExpertActivityScore +
                          passport.Parameter2.EducationalTechnologyScore;

        passport.TotalScores = new TotalScoresDto
        {
            Parameter1Total = param1Total,
            Parameter2Total = param2Total,
            GrandTotal = param1Total + param2Total,
            RequiredForFirst = 115,
            RequiredForHigher = 230,
            RecommendedCategory = (param1Total + param2Total) >= 230 ? "Высшая квалификационная категория" :
                                  (param1Total + param2Total) >= 115 ? "Первая квалификационная категория" :
                                  "Категория не рекомендуется"
        };

        return passport;
    }
}