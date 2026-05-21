using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("participationresults")]
public partial class Participationresult
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Result")]
    public virtual ICollection<Contest> Contests { get; set; } = new List<Contest>();

    [InverseProperty("Result")]
    public virtual ICollection<Studentachievement> Studentachievements { get; set; } = new List<Studentachievement>();
}
