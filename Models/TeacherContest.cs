using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeacherPortfolio.API.Models;

[Table("teacher_contests")]
public class TeacherContest
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int TeacherId { get; set; }

    [Column("academicyearid")]
    public int AcademicYearId { get; set; }

    [Column("contest_name")]
    [StringLength(500)]
    public string ContestName { get; set; } = string.Empty;

    [Column("organizer")]
    [StringLength(300)]
    public string Organizer { get; set; } = string.Empty;

    [Column("level")]
    [StringLength(100)]
    public string Level { get; set; } = string.Empty;

    [Column("result")]
    [StringLength(200)]
    public string Result { get; set; } = string.Empty;

    [Column("order_details")]
    [StringLength(300)]
    public string? OrderDetails { get; set; }

    [Column("link")]
    [StringLength(500)]
    public string? Link { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey("TeacherId")]
    public virtual Teacher Teacher { get; set; } = null!;

    [ForeignKey("AcademicYearId")]
    public virtual Academicyear AcademicYear { get; set; } = null!;
}