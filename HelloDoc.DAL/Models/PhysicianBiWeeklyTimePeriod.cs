using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.DAL.Models;

[Table("PhysicianBiWeeklyTimePeriod")]
public partial class PhysicianBiWeeklyTimePeriod
{
    [Key]
    public int TimePeriodId { get; set; }

    public int PhysicianId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public short? Status { get; set; }

    public bool? IsFinalize { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianBiWeeklyTimePeriods")]
    public virtual Physician Physician { get; set; } = null!;

    [InverseProperty("TimePeriod")]
    public virtual ICollection<PhysicianTimeSheet> PhysicianTimeSheets { get; set; } = new List<PhysicianTimeSheet>();

    [InverseProperty("TimePeriod")]
    public virtual ICollection<TimeSheetReimburseMent> TimeSheetReimburseMents { get; set; } = new List<TimeSheetReimburseMent>();
}
