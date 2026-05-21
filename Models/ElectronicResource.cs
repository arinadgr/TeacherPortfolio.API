using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeacherPortfolio.API.Models;

[Table("electronic_resources")]
public class ElectronicResource
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int TeacherId { get; set; }

    [Column("academicyearid")]
    public int AcademicYearId { get; set; }

    [Column("name")]
    [StringLength(300)]
    public string Name { get; set; } = string.Empty;

    [Column("topic")]
    [StringLength(300)]
    public string Topic { get; set; } = string.Empty;

    [Column("interaction_form")]
    [StringLength(100)]
    public string InteractionForm { get; set; } = string.Empty;

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