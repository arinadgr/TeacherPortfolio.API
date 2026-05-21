namespace TeacherPortfolio.API.DTOs;

public class CreateStudentAchievementRequest
{
    public string StudentName { get; set; } = string.Empty;
    public string AchievementType { get; set; } = string.Empty;
    public DateOnly EventDate { get; set; }
    public string? EventOrganizer { get; set; }
    public string? GroupName { get; set; }
    public string? ResultDescription { get; set; }
    public string? OrderDetails { get; set; }
    public string? DocumentLink { get; set; }

    // Внешние ключи (ID из справочников)
    public int AcademicyearId { get; set; }
    public int? DirectionId { get; set; }
    public int LevelId { get; set; }
    public int? ResultId { get; set; }
}