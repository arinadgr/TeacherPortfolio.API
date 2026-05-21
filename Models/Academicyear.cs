using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("academicyears")]
[Index("Name", Name = "academicyears_name_key", IsUnique = true)]
public partial class Academicyear
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(20)]
    public string Name { get; set; } = null!;

    [InverseProperty("Academicyear")]
    public virtual ICollection<Academicresult> Academicresults { get; set; } = new List<Academicresult>();

    [InverseProperty("Academicyear")]
    public virtual ICollection<Contest> Contests { get; set; } = new List<Contest>();

    [InverseProperty("Academicyear")]
    public virtual ICollection<Departmentactivity> Departmentactivities { get; set; } = new List<Departmentactivity>();

    [InverseProperty("Academicyear")]
    public virtual ICollection<Digitalresource> Digitalresources { get; set; } = new List<Digitalresource>();

    [InverseProperty("Academicyear")]
    public virtual ICollection<Expertactivity> Expertactivities { get; set; } = new List<Expertactivity>();

    [InverseProperty("Academicyear")]
    public virtual ICollection<Graduatework> Graduateworks { get; set; } = new List<Graduatework>();

    [InverseProperty("Academicyear")]
    public virtual ICollection<Methodicalmaterial> Methodicalmaterials { get; set; } = new List<Methodicalmaterial>();

    [InverseProperty("Academicyear")]
    public virtual ICollection<Studentachievement> Studentachievements { get; set; } = new List<Studentachievement>();
}
