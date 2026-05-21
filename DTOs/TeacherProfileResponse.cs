namespace TeacherPortfolio.API.DTOs;

public class TeacherProfileResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Workplace { get; set; } = string.Empty;
    public string? QualificationCategory { get; set; }
    public string Email { get; set; } = string.Empty;
    public List<StudentAchievementResponse> Achievements { get; set; } = new();
}

public class StudentAchievementResponse
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AchievementType { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public string? Result { get; set; }
    public DateOnly EventDate { get; set; }
    public string? EventOrganizer { get; set; }
    public string? GroupName { get; set; }
    public string? ResultDescription { get; set; }
    public DateTime CreatedAt { get; set; }
}