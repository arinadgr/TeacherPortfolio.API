namespace TeacherPortfolio.API.DTOs;

public class UpdateProfileRequest
{
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? Position { get; set; }
    public string? Workplace { get; set; }
}