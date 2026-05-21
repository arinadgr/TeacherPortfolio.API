using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("teachers")]
[Index("Userid", Name = "teachers_userid_key", IsUnique = true)]
public partial class Teacher
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userid")]
    public int Userid { get; set; }

    [Column("lastname")]
    [StringLength(100)]
    public string Lastname { get; set; } = null!;

    [Column("firstname")]
    [StringLength(100)]
    public string Firstname { get; set; } = null!;

    [Column("middlename")]
    [StringLength(100)]
    public string? Middlename { get; set; }

    [Column("position")]
    [StringLength(200)]
    public string? Position { get; set; }

    [Column("workplace")]
    [StringLength(300)]
    public string? Workplace { get; set; }

    [Column("qualificationcategoryid")]
    public int? Qualificationcategoryid { get; set; }

    [Column("ordernumber")]
    [StringLength(100)]
    public string? Ordernumber { get; set; }

    [Column("orderdate")]
    public DateOnly? Orderdate { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [InverseProperty("Teacher")]
    public virtual ICollection<Academicresult> Academicresults { get; set; } = new List<Academicresult>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Contest> Contests { get; set; } = new List<Contest>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Departmentactivity> Departmentactivities { get; set; } = new List<Departmentactivity>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Digitalresource> Digitalresources { get; set; } = new List<Digitalresource>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Educationaltechnology> Educationaltechnologies { get; set; } = new List<Educationaltechnology>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Experiencesharing> Experiencesharings { get; set; } = new List<Experiencesharing>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Expertactivity> Expertactivities { get; set; } = new List<Expertactivity>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Graduatework> Graduateworks { get; set; } = new List<Graduatework>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Methodicalmaterial> Methodicalmaterials { get; set; } = new List<Methodicalmaterial>();

    [ForeignKey("Qualificationcategoryid")]
    [InverseProperty("Teachers")]
    public virtual Qualificationcategory? Qualificationcategory { get; set; }

    [InverseProperty("Teacher")]
    public virtual ICollection<Studentachievement> Studentachievements { get; set; } = new List<Studentachievement>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Studentgroup> Studentgroups { get; set; } = new List<Studentgroup>();

    [InverseProperty("Teacher")]
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();

    [ForeignKey("Userid")]
    [InverseProperty("Teacher")]
    public virtual User User { get; set; } = null!;
}
