namespace TeacherPortfolio.API.DTOs;

public class ExperienceSharingDto
{
    public int Id { get; set; }
    public int LevelId { get; set; }
    public string? LevelName { get; set; }
    public int? FormatId { get; set; }
    public string? FormatName { get; set; }
    public int? SharingFormId { get; set; }
    public string? SharingFormName { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public DateOnly EventDate { get; set; }
    public string? Organizer { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExperienceSharingRequest
{
    public int LevelId { get; set; }
    public int? FormatId { get; set; }
    public int? SharingFormId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public DateOnly EventDate { get; set; }
    public string? Organizer { get; set; }
}

public class UpdateExperienceSharingRequest
{
    public int LevelId { get; set; }
    public int? FormatId { get; set; }
    public int? SharingFormId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public DateOnly EventDate { get; set; }
    public string? Organizer { get; set; }
}