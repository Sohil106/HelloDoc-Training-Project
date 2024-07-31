using HelloDoc.DAL.Models;


namespace HelloDoc.DAL.ViewModels
{
    public class BiWeeklyTimesheet
    {
        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public List<PhysicianTimeSheet> Timesheet { get; set; }

        public List<TimeSheetReimburseMent> Reimbursement { get; set; }

        public int Timeperiodid { get; set; }

        public int PhysicianId { get; set; }

        public int ShiftPayrate { get; set; }

        public int WeekendNightShiftPayret { get; set; }

        public int HousecallPayrate { get; set; }

        public int PhoneconsultsPayrate { get; set; }
    }
}
