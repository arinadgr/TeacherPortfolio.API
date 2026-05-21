using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("studentgroups")]
public partial class Studentgroup
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("teacherid")]
    public int Teacherid { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<Academicresult> Academicresults { get; set; } = new List<Academicresult>();

    [ForeignKey("Teacherid")]
    [InverseProperty("Studentgroups")]
    public virtual Teacher Teacher { get; set; } = null!;
}
