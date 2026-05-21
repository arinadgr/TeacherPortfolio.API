namespace TeacherPortfolio.API.DTOs;

public class EducationalTechnologyDto
{
    public int Id { get; set; }
    public string TechnologyName { get; set; } = string.Empty;
    public string? Purpose { get; set; }
    public string? Result { get; set; }
    public string? ResourceLink { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEducationalTechnologyRequest
{
    public string TechnologyName { get; set; } = string.Empty;
    public string? Purpose { get; set; }
    public string? Result { get; set; }
    public string? ResourceLink { get; set; }
}

public class UpdateEducationalTechnologyRequest
{
    public string TechnologyName { get; set; } = string.Empty;
    public string? Purpose { get; set; }
    public string? Result { get; set; }
    public string? ResourceLink { get; set; }
}