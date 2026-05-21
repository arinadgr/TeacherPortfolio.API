namespace TeacherPortfolio.API.DTOs;

public class MethodicalMaterialDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public int MaterialTypeId { get; set; }
    public string MaterialTypeName { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? InternetLink { get; set; }
    public string? ApprovalDetails { get; set; }
    public string? ReviewingOrganization { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateMethodicalMaterialRequest
{
    public int AcademicYearId { get; set; }
    public int MaterialTypeId { get; set; }
    public string? Specialty { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? InternetLink { get; set; }
    public string? ApprovalDetails { get; set; }
    public string? ReviewingOrganization { get; set; }
}

public class UpdateMethodicalMaterialRequest
{
    public int AcademicYearId { get; set; }
    public int MaterialTypeId { get; set; }
    public string? Specialty { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string? InternetLink { get; set; }
    public string? ApprovalDetails { get; set; }
    public string? ReviewingOrganization { get; set; }
}