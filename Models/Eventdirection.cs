using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("eventdirections")]
public partial class Eventdirection
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Direction")]
    public virtual ICollection<Studentachievement> Studentachievements { get; set; } = new List<Studentachievement>();
}
