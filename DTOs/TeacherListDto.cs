namespace TeacherPortfolio.API.DTOs;

public class TeacherListDto
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";

    public string Position { get; set; } = "";
}