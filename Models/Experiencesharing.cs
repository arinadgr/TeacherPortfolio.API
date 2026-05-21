using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("experiencesharing")]
public partial class Experiencesharing
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("eventdate")]
    public DateOnly Eventdate { get; set; }

    [Column("organizer")]
    [StringLength(300)]
    public string? Organizer { get; set; }

    [Column("levelid")]
    public int Levelid { get; set; }

    [Column("formatid")]
    public int? Formatid { get; set; }

    [Column("eventname")]
    [StringLength(500)]
    public string Eventname { get; set; } = null!;

    [Column("sharingformid")]
    public int? Sharingformid { get; set; }

    [Column("topic")]
    [StringLength(500)]
    public string Topic { get; set; } = null!;

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Formatid")]
    [InverseProperty("Experiencesharings")]
    public virtual Eventformat? Format { get; set; }

    [ForeignKey("Levelid")]
    [InverseProperty("Experiencesharings")]
    public virtual Eventlevel Level { get; set; } = null!;

    [ForeignKey("Sharingformid")]
    [InverseProperty("Experiencesharings")]
    public virtual Experiencesharingform? Sharingform { get; set; }

    [ForeignKey("Teacherid")]
    [InverseProperty("Experiencesharings")]
    public virtual Teacher Teacher { get; set; } = null!;
}
