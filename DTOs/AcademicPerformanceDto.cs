namespace TeacherPortfolio.API.DTOs;

public class AcademicPerformanceDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public string Discipline { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public decimal QualityPercent { get; set; }
    public decimal SuccessPercent { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAcademicPerformanceRequest
{
    public int AcademicYearId { get; set; }
    public string Discipline { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public decimal QualityPercent { get; set; }
    public decimal SuccessPercent { get; set; }
}

public class UpdateAcademicPerformanceRequest
{
    public int AcademicYearId { get; set; }
    public string Discipline { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public decimal QualityPercent { get; set; }
    public decimal SuccessPercent { get; set; }
}