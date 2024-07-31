using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Practice.DAL.Models;

[Table("student")]
public partial class Student
{
    [Key]
    [Column("studentid")]
    public int Studentid { get; set; }

    [Column("firstname")]
    [StringLength(100)]
    public string Firstname { get; set; } = null!;

    [Column("lastname")]
    [StringLength(100)]
    public string? Lastname { get; set; }

    [Column("courseid")]
    public int? Courseid { get; set; }

    [Column("age")]
    public short? Age { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string Email { get; set; } = null!;

    [Column("gender")]
    [StringLength(20)]
    public string? Gender { get; set; }

    [Column("course")]
    [StringLength(100)]
    public string? Course { get; set; }

    [Column("grade")]
    [StringLength(50)]
    public string? Grade { get; set; }

    [ForeignKey("Courseid")]
    [InverseProperty("Students")]
    public virtual Course? CourseNavigation { get; set; }
}
