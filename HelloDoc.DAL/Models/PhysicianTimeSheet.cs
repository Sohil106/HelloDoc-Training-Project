using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.DAL.Models;

[Table("PhysicianTimeSheet")]
public partial class PhysicianTimeSheet
{
    [Key]
    public int TimeSheetDateId { get; set; }

    public int PhysicianId { get; set; }

    public int TimePeriodId { get; set; }

    public DateOnly? TimeSheetDate { get; set; }

    public int? Shift { get; set; }

    public int? HouseCalls { get; set; }

    public int? HouseCallNightsWeekend { get; set; }

    public int? PhoneConsults { get; set; }

    public int? PhoneConsultsNightsWeekend { get; set; }

    public int? BatchTesting { get; set; }

    public bool? NightShiftWeekend { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianTimeSheets")]
    public virtual Physician Physician { get; set; } = null!;

    [ForeignKey("TimePeriodId")]
    [InverseProperty("PhysicianTimeSheets")]
    public virtual PhysicianBiWeeklyTimePeriod TimePeriod { get; set; } = null!;
}
