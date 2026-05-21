using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeacherPortfolio.API.Models;

[Table("academic_performances")]
public class AcademicPerformance
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int TeacherId { get; set; }

    [Column("academicyearid")]
    public int AcademicYearId { get; set; }

    [Column("discipline")]
    [StringLength(200)]
    public string Discipline { get; set; } = string.Empty;

    [Column("groupname")]
    [StringLength(50)]
    public string GroupName { get; set; } = string.Empty;

    [Column("quality_percent")]
    public decimal QualityPercent { get; set; }

    [Column("success_percent")]
    public decimal SuccessPercent { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [ForeignKey("TeacherId")]
    public virtual Teacher Teacher { get; set; } = null!;

    [ForeignKey("AcademicYearId")]
    public virtual Academicyear AcademicYear { get; set; } = null!;
}