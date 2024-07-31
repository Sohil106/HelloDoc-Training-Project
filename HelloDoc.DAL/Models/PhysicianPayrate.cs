using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.DAL.Models;

[Table("PhysicianPayrate")]
public partial class PhysicianPayrate
{
    [Key]
    public int PayrateId { get; set; }

    public int PhysicianId { get; set; }

    public int? NightShiftWeekend { get; set; }

    public int? Shift { get; set; }

    public int? HouseCallNightsWeekend { get; set; }

    public int? PhoneConsults { get; set; }

    public int? PhoneConsultsNightsWeekend { get; set; }

    public int? BatchTesting { get; set; }

    public int? HouseCalls { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianPayrates")]
    public virtual Physician Physician { get; set; } = null!;
}
