using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("academicresults")]
public partial class Academicresult
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [Column("subjectid")]
    public int Subjectid { get; set; }

    [Column("academicyearid")]
    public int Academicyearid { get; set; }

    [Column("groupid")]
    public int Groupid { get; set; }

    [Column("qualitypercent")]
    [Precision(5, 2)]
    public decimal Qualitypercent { get; set; }

    [Column("successpercent")]
    [Precision(5, 2)]
    public decimal Successpercent { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [ForeignKey("Academicyearid")]
    [InverseProperty("Academicresults")]
    public virtual Academicyear Academicyear { get; set; } = null!;

    [ForeignKey("Groupid")]
    [InverseProperty("Academicresults")]
    public virtual Studentgroup Group { get; set; } = null!;

    [ForeignKey("Subjectid")]
    [InverseProperty("Academicresults")]
    public virtual Subject Subject { get; set; } = null!;

    [ForeignKey("Teacherid")]
    [InverseProperty("Academicresults")]
    public virtual Teacher Teacher { get; set; } = null!;
}
