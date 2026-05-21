using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("contests")]
public partial class Contest
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("academicyearid")]
    public int Academicyearid { get; set; }

    [Column("contestname")]
    [StringLength(500)]
    public string Contestname { get; set; } = null!;

    [Column("organizer")]
    [StringLength(300)]
    public string? Organizer { get; set; }

    [Column("levelid")]
    public int Levelid { get; set; }

    [Column("resultid")]
    public int? Resultid { get; set; }

    [Column("resultdescription")]
    [StringLength(300)]
    public string? Resultdescription { get; set; }

    [Column("orderdetails")]
    public string? Orderdetails { get; set; }

    [Column("link")]
    [StringLength(500)]
    public string? Link { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Academicyearid")]
    [InverseProperty("Contests")]
    public virtual Academicyear Academicyear { get; set; } = null!;

    [ForeignKey("Levelid")]
    [InverseProperty("Contests")]
    public virtual Eventlevel Level { get; set; } = null!;

    [ForeignKey("Resultid")]
    [InverseProperty("Contests")]
    public virtual Participationresult? Result { get; set; }

    [ForeignKey("Teacherid")]
    [InverseProperty("Contests")]
    public virtual Teacher Teacher { get; set; } = null!;
}
