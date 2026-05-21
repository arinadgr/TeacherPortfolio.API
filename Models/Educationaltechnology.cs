using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("educationaltechnologies")]
public partial class Educationaltechnology
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("technologyname")]
    [StringLength(300)]
    public string Technologyname { get; set; } = null!;

    [Column("purpose")]
    public string? Purpose { get; set; }

    [Column("result")]
    public string? Result { get; set; }

    [Column("resourcelink")]
    [StringLength(500)]
    public string? Resourcelink { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Teacherid")]
    [InverseProperty("Educationaltechnologies")]
    public virtual Teacher Teacher { get; set; } = null!;
}
