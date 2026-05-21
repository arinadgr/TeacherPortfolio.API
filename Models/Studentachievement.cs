using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("studentachievements")]
public partial class Studentachievement
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("academicyearid")]
    public int Academicyearid { get; set; }

    [Column("eventdate")]
    public DateOnly Eventdate { get; set; }

    [Column("eventname")]
    [StringLength(500)]
    public string Eventname { get; set; } = null!;

    [Column("eventorganizer")]
    [StringLength(300)]
    public string? Eventorganizer { get; set; }

    [Column("directionid")]
    public int? Directionid { get; set; }

    [Column("levelid")]
    public int Levelid { get; set; }

    [Column("studentname")]
    [StringLength(200)]
    public string Studentname { get; set; } = null!;

    [Column("groupname")]
    [StringLength(50)]
    public string? Groupname { get; set; }

    [Column("resultid")]
    public int? Resultid { get; set; }

    [Column("resultdescription")]
    [StringLength(300)]
    public string? Resultdescription { get; set; }

    [Column("orderdetails")]
    public string? Orderdetails { get; set; }

    [Column("documentlink")]
    [StringLength(500)]
    public string? Documentlink { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Academicyearid")]
    [InverseProperty("Studentachievements")]
    public virtual Academicyear Academicyear { get; set; } = null!;

    [ForeignKey("Directionid")]
    [InverseProperty("Studentachievements")]
    public virtual Eventdirection? Direction { get; set; }

    [ForeignKey("Levelid")]
    [InverseProperty("Studentachievements")]
    public virtual Eventlevel Level { get; set; } = null!;

    [ForeignKey("Resultid")]
    [InverseProperty("Studentachievements")]
    public virtual Participationresult? Result { get; set; }

    [ForeignKey("Teacherid")]
    [InverseProperty("Studentachievements")]
    public virtual Teacher Teacher { get; set; } = null!;
}
