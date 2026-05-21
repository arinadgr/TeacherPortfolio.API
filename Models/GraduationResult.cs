using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeacherPortfolio.API.Models;

[Table("graduation_results")]
public class GraduationResult
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int TeacherId { get; set; }

    [Column("academicyearid")]
    public int AcademicYearId { get; set; }

    [Column("studentname")]
    [StringLength(200)]
    public string StudentName { get; set; } = string.Empty;

    [Column("groupname")]
    [StringLength(50)]
    public string GroupName { get; set; } = string.Empty;

    [Column("specialty")]
    [StringLength(200)]
    public string Specialty { get; set; } = string.Empty;

    [Column("thesis_topic")]
    [StringLength(500)]
    public string ThesisTopic { get; set; } = string.Empty;

    [Column("grade")]
    [StringLength(20)]
    public string Grade { get; set; } = string.Empty;

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey("TeacherId")]
    public virtual Teacher Teacher { get; set; } = null!;

    [ForeignKey("AcademicYearId")]
    public virtual Academicyear AcademicYear { get; set; } = null!;
}