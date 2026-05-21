using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("experiencesharingforms")]
public partial class Experiencesharingform
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [InverseProperty("Sharingform")]
    public virtual ICollection<Experiencesharing> Experiencesharings { get; set; } = new List<Experiencesharing>();
}
