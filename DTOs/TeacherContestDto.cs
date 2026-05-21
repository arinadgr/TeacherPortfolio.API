namespace TeacherPortfolio.API.DTOs;

public class TeacherContestDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public string ContestName { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string? OrderDetails { get; set; }
    public string? Link { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTeacherContestRequest
{
    public int AcademicYearId { get; set; }
    public string ContestName { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string? OrderDetails { get; set; }
    public string? Link { get; set; }
}

public class UpdateTeacherContestRequest
{
    public int AcademicYearId { get; set; }
    public string ContestName { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string? OrderDetails { get; set; }
    public string? Link { get; set; }
}