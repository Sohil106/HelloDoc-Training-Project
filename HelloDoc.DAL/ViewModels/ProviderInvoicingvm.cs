using HelloDoc.DAL.Models;
using Microsoft.AspNetCore.Http;

namespace HelloDoc.DAL.ViewModels
{
    public class ProviderInvoicingvm
    {
        public bool? TimesheetsFinalize { get; set; }

        public List<TimesheetDetail>? Timesheetdetails { get; set; }

        public List<TimesheetDetailReimbursement>? Timesheetdetailreimbursements { get; set; }

        public List<ProviderTimesheetDetails>? ProviderTimesheetDetails { get; set; }

        public List<AddReceiptsDetails>? AddReceiptsDetails { get; set; }

        public List<PhysicianPayrate>? physicianPayrates { get; set; }


        //public int? callfromtabid { get; set; }

        public int? CallId { get; set; }

    }
    public class ProviderTimesheetDetails
    {
        public int? TimeSheetId { get; set; }

        public int? TimeSheetDetailId { get; set; }

        public int? Hours { get; set; }

        public bool? IsWeekend { get; set; }

        public int? NoOfHouseCalls { get; set; }

        public int? NoOfConsults { get; set; }

        public DateOnly? ShiftDetailDate { get; set; }

    }

    public class AddReceiptsDetails
    {
        public int? TimeSheetDetailId { get; set; }

        public int? Amount { get; set; }

        public string? Item { get; set; }

        public string? BillValue { get; set; }

        public IFormFile? Bill { get; set; }

        public DateOnly? ShiftDetailDate { get; set; }

    }
}
