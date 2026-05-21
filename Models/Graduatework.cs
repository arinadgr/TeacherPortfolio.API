using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("graduateworks")]
public partial class Graduatework
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("academicyearid")]
    public int Academicyearid { get; set; }

    [Column("studentname")]
    [StringLength(200)]
    public string Studentname { get; set; } = null!;

    [Column("groupname")]
    [StringLength(50)]
    public string Groupname { get; set; } = null!;

    [Column("specialty")]
    [StringLength(200)]
    public string? Specialty { get; set; }

    [Column("topic")]
    public string Topic { get; set; } = null!;

    [Column("gradeid")]
    public int Gradeid { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Academicyearid")]
    [InverseProperty("Graduateworks")]
    public virtual Academicyear Academicyear { get; set; } = null!;

    [ForeignKey("Gradeid")]
    [InverseProperty("Graduateworks")]
    public virtual Gradetype Grade { get; set; } = null!;

    [ForeignKey("Teacherid")]
    [InverseProperty("Graduateworks")]
    public virtual Teacher Teacher { get; set; } = null!;
}
