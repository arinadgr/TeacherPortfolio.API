using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("subjects")]
public partial class Subject
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(500)]
    public string Name { get; set; } = null!;

    [Column("shortname")]
    [StringLength(200)]
    public string? Shortname { get; set; }

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [InverseProperty("Subject")]
    public virtual ICollection<Academicresult> Academicresults { get; set; } = new List<Academicresult>();

    [ForeignKey("Teacherid")]
    [InverseProperty("Subjects")]
    public virtual Teacher Teacher { get; set; } = null!;
}
