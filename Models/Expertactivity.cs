using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("expertactivities")]
public partial class Expertactivity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("eventdate")]
    public DateOnly? Eventdate { get; set; }

    [Column("academicyearid")]
    public int? Academicyearid { get; set; }

    [Column("eventname")]
    [StringLength(500)]
    public string Eventname { get; set; } = null!;

    [Column("levelid")]
    public int Levelid { get; set; }

    [Column("activitytype")]
    [StringLength(300)]
    public string? Activitytype { get; set; }

    [Column("documentdetails")]
    public string? Documentdetails { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Academicyearid")]
    [InverseProperty("Expertactivities")]
    public virtual Academicyear? Academicyear { get; set; }

    [ForeignKey("Levelid")]
    [InverseProperty("Expertactivities")]
    public virtual Eventlevel Level { get; set; } = null!;

    [ForeignKey("Teacherid")]
    [InverseProperty("Expertactivities")]
    public virtual Teacher Teacher { get; set; } = null!;
}
