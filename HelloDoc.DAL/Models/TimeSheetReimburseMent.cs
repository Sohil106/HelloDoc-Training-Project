using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.DAL.Models;

[Table("TimeSheetReimburseMent")]
public partial class TimeSheetReimburseMent
{
    [Key]
    public int ReimbursementId { get; set; }

    public int TimePeriodId { get; set; }

    public DateOnly? TimeSheetDate { get; set; }

    [StringLength(50)]
    public string? Item { get; set; }

    [Precision(4, 2)]
    public decimal? Amount { get; set; }

    [StringLength(200)]
    public string? Bill { get; set; }

    public bool? IsDeleted { get; set; }

    [ForeignKey("TimePeriodId")]
    [InverseProperty("TimeSheetReimburseMents")]
    public virtual PhysicianBiWeeklyTimePeriod TimePeriod { get; set; } = null!;
}
