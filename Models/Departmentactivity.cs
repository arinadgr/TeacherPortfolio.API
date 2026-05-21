using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("departmentactivities")]
public partial class Departmentactivity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("academicyearid")]
    public int Academicyearid { get; set; }

    [Column("activitytypeid")]
    public int Activitytypeid { get; set; }

    [Column("orderdetails")]
    [StringLength(500)]
    public string? Orderdetails { get; set; }

    [Column("documentlink")]
    [StringLength(500)]
    public string? Documentlink { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Academicyearid")]
    [InverseProperty("Departmentactivities")]
    public virtual Academicyear Academicyear { get; set; } = null!;

    [ForeignKey("Activitytypeid")]
    [InverseProperty("Departmentactivities")]
    public virtual Departmentactivitytype Activitytype { get; set; } = null!;

    [ForeignKey("Teacherid")]
    [InverseProperty("Departmentactivities")]
    public virtual Teacher Teacher { get; set; } = null!;
}
