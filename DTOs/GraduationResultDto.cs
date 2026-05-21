namespace TeacherPortfolio.API.DTOs;

public class GraduationResultDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string ThesisTopic { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateGraduationResultRequest
{
    public int AcademicYearId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string ThesisTopic { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
}

public class UpdateGraduationResultRequest
{
    public int AcademicYearId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string ThesisTopic { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
}