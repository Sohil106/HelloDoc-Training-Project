using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Practice.DAL.Models;

[Table("course")]
public partial class Course
{
    [Key]
    [Column("courseid")]
    public int Courseid { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("CourseNavigation")]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
