using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("eventlevels")]
public partial class Eventlevel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Level")]
    public virtual ICollection<Contest> Contests { get; set; } = new List<Contest>();

    [InverseProperty("Level")]
    public virtual ICollection<Experiencesharing> Experiencesharings { get; set; } = new List<Experiencesharing>();

    [InverseProperty("Level")]
    public virtual ICollection<Expertactivity> Expertactivities { get; set; } = new List<Expertactivity>();

    [InverseProperty("Level")]
    public virtual ICollection<Studentachievement> Studentachievements { get; set; } = new List<Studentachievement>();
}
