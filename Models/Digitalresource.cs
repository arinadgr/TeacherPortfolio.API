using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("digitalresources")]
public partial class Digitalresource
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("academicyearid")]
    public int? Academicyearid { get; set; }

    [Column("lessontopic")]
    [StringLength(500)]
    public string Lessontopic { get; set; } = null!;

    [Column("resourcename")]
    [StringLength(500)]
    public string Resourcename { get; set; } = null!;

    [Column("interactionform")]
    [StringLength(300)]
    public string? Interactionform { get; set; }

    [Column("resourcelink")]
    [StringLength(500)]
    public string Resourcelink { get; set; } = null!;

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Academicyearid")]
    [InverseProperty("Digitalresources")]
    public virtual Academicyear? Academicyear { get; set; }

    [ForeignKey("Teacherid")]
    [InverseProperty("Digitalresources")]
    public virtual Teacher Teacher { get; set; } = null!;
}
