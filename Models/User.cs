using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TeacherPortfolio.API.Models;

[Table("users")]
[Index("Email", Name = "users_email_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("passwordhash")]
    public string Passwordhash { get; set; } = null!;

    [Column("role")]
    [StringLength(20)]
    public string Role { get; set; } = null!;

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [InverseProperty("User")]
    public virtual Teacher? Teacher { get; set; }
}
