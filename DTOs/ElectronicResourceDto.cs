namespace TeacherPortfolio.API.DTOs;

public class ElectronicResourceDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string InteractionForm { get; set; } = string.Empty;
    public string? Link { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateElectronicResourceRequest
{
    public int AcademicYearId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string InteractionForm { get; set; } = string.Empty;
    public string? Link { get; set; }
}

public class UpdateElectronicResourceRequest
{
    public int AcademicYearId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string InteractionForm { get; set; } = string.Empty;
    public string? Link { get; set; }
}