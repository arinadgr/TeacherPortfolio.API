namespace TeacherPortfolio.API.DTOs;

public class ExpertActivityDto
{
    public int Id { get; set; }
    public DateOnly? EventDate { get; set; }
    public int? AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int LevelId { get; set; }
    public string? LevelName { get; set; }
    public string? ActivityType { get; set; }
    public string? DocumentDetails { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExpertActivityRequest
{
    public DateOnly? EventDate { get; set; }
    public int? AcademicYearId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int LevelId { get; set; }
    public string? ActivityType { get; set; }
    public string? DocumentDetails { get; set; }
}

public class UpdateExpertActivityRequest
{
    public DateOnly? EventDate { get; set; }
    public int? AcademicYearId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int LevelId { get; set; }
    public string? ActivityType { get; set; }
    public string? DocumentDetails { get; set; }
}