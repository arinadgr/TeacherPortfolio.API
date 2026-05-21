using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("methodicalmaterials")]
public partial class Methodicalmaterial
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("academicyearid")]
    public int Academicyearid { get; set; }

    [Column("materialtypeid")]
    public int Materialtypeid { get; set; }

    [Column("specialty")]
    [StringLength(200)]
    public string? Specialty { get; set; }

    [Column("topic")]
    [StringLength(500)]
    public string Topic { get; set; } = null!;

    [Column("internetlink")]
    [StringLength(500)]
    public string? Internetlink { get; set; }

    [Column("approvaldetails")]
    [StringLength(500)]
    public string? Approvaldetails { get; set; }

    [Column("reviewingorganization")]
    [StringLength(300)]
    public string? Reviewingorganization { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Academicyearid")]
    [InverseProperty("Methodicalmaterials")]
    public virtual Academicyear Academicyear { get; set; } = null!;

    [ForeignKey("Materialtypeid")]
    [InverseProperty("Methodicalmaterials")]
    public virtual Methodicalmaterialtype Materialtype { get; set; } = null!;

    [ForeignKey("Teacherid")]
    [InverseProperty("Methodicalmaterials")]
    public virtual Teacher Teacher { get; set; } = null!;
}
