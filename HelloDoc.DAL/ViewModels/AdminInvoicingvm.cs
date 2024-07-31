using HelloDoc.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class AdminInvoicingvm
    {
        public List<Physician>? PhysiciansList { get; set; }

        public List<Timesheet>? TimesheetsList { get; set; }

        public List<TimesheetDetail>? Timesheetdetails { get; set; }

        public List<TimesheetDetailReimbursement>? Timesheetdetailreimbursements { get; set; }

        public int? PhysicianId { get; set; }

        public string? PhysicianName { get; set; }
    }

    public class AdminTimeSheetList
    {
        public int? TimeSheetId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Status { get; set; }
    }
}
