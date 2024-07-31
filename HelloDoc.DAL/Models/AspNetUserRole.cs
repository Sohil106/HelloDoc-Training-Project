using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.DAL.Models;

public partial class AspNetUserRole
{
    [Key]
    [StringLength(128)]
    public string UserId { get; set; } = null!;

    [StringLength(128)]
    public string? RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("AspNetUserRoles")]
    public virtual AspNetRole? Role { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AspNetUserRole")]
    public virtual AspNetUser User { get; set; } = null!;
}
