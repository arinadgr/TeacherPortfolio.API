using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("departmentactivitytypes")]
public partial class Departmentactivitytype
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(300)]
    public string Name { get; set; } = null!;

    [InverseProperty("Activitytype")]
    public virtual ICollection<Departmentactivity> Departmentactivities { get; set; } = new List<Departmentactivity>();
}
