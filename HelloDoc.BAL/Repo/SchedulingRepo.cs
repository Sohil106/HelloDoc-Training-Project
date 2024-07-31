using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace HelloDoc.BAL.Repo
{
    public class SchedulingRepo : ISchedulingRepo
    {
        private readonly ApplicationDbContext _db;

        public SchedulingRepo(ApplicationDbContext db)
        {
            _db = db;
        }
        // ******************************************************* Scheduling  ************************************************************
        /// <summary>
        /// Fetch Records From Region Table
        /// </summary>
        /// <returns></returns>
        public List<Region> GetRegions()
        {
            var regions = _db.Regions.ToList();
            return regions;
        }

        /// <summary>
        /// Fetch Records On Scheduling Page
        /// </summary>
        /// <param name="date"></param>
        /// <param name="sunday"></param>
        /// <param name="saturday"></param>
        /// <param name="type"></param>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday, string type,string aspId)
        {
            var physician = _db.Physicians.FirstOrDefault(i => i.AspNetUserId == aspId);

            var shiftdetails = _db.ShiftDetails.Where(u => u.ShiftDate.Month == date.Month && u.
            ShiftDate.Year == date.Year);

            BitArray deletedBit = new BitArray(new[] { true });

            switch (type)
            {
                case "month":
                    shiftdetails = _db.ShiftDetails.Where(u => u.ShiftDate.Month == date.Month && u.ShiftDate.Year == date.Year && !u.IsDeleted.Equals(deletedBit));
                    break;

                case "week":
                    shiftdetails = _db.ShiftDetails.Where(u => (u.ShiftDate >= sunday || u.ShiftDate <= saturday) && !u.IsDeleted.Equals(deletedBit));
                    break;

                case "day":
                    shiftdetails = _db.ShiftDetails.Where(u => u.ShiftDate.Month == date.Month && u.ShiftDate.Year == date.Year && u.ShiftDate.Day == date.Day && !u.IsDeleted.Equals(deletedBit));
                    break;
            }


            var list = shiftdetails.Select(s => new ShiftDetailsmodal
            {
                Shiftid = s.ShiftId,
                Shiftdetailid = s.ShiftDetailId,
                Shiftdate = s.ShiftDate,
                Startdate = s.Shift.StartDate,
                Starttime = s.StartTime,
                Endtime = s.EndTime,
                Physicianid = s.Shift.PhysicianId,
                PhysicianName = s.Shift.Physician.FirstName,
                Status = s.Status,
                regionname = _db.Regions.FirstOrDefault(i => i.RegionId == s.RegionId).Name,
                Abberaviation = _db.Regions.FirstOrDefault(i => i.RegionId == s.RegionId).Abbreviation,
                Regionid = s.RegionId,
            });

            if (physician != null)
            {
                list = list.Where(i => i.Physicianid == physician.PhysicianId);
            }

            return list.ToList();

            
        }

        /// <summary>
        /// Add Records to Shift and ShiftDetails Table Via Create Shift Page
        /// </summary>
        /// <param name="scheduleModel"></param>
        /// <param name="Aspid"></param>
        /// <returns></returns>
        public bool createShift(ScheduleModel scheduleModel, string Aspid)
        {
            if (_db.Shifts.Where(x => x.PhysicianId == scheduleModel.Physicianid).Count() >= 1)
            {
                var shiftData = _db.Shifts.Where(i => i.PhysicianId == scheduleModel.Physicianid).ToList();
                var shiftDetailData = new List<ShiftDetail>();

                foreach (var obj in shiftData)
                {
                    var details = _db.ShiftDetails.Where(x => x.ShiftId == obj.ShiftId).ToList();
                    shiftDetailData.AddRange(details);
                }


                foreach (var obj in shiftDetailData)
                {
                    var shiftDate = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);

                    if (obj.ShiftDate.Date == shiftDate.Date)
                    {
                        if ((obj.StartTime <= scheduleModel.Starttime && obj.EndTime >= scheduleModel.Starttime) || (obj.StartTime <= scheduleModel.Endtime && obj.EndTime >= scheduleModel.Endtime) || (obj.StartTime >= scheduleModel.Starttime && obj.EndTime <= scheduleModel.Endtime))
                        {
                            return false;
                        }
                    }
                }
            }

            Shift shift = new Shift();
            shift.PhysicianId = scheduleModel.Physicianid;
            shift.RepeatUpto = scheduleModel.Repeatupto;
            shift.StartDate = scheduleModel.Startdate;
            shift.CreatedBy = Aspid;
            shift.CreatedDate = DateTime.Now;
            shift.IsRepeat = scheduleModel.Isrepeat == false ? new BitArray(1, false) : new BitArray(1, true);
            shift.RepeatUpto = scheduleModel.Repeatupto;
            _db.Shifts.Add(shift);
            _db.SaveChanges();

            ShiftDetail sd = new ShiftDetail();
            sd.ShiftId = shift.ShiftId;
            sd.ShiftDate = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);
            sd.StartTime = scheduleModel.Starttime;
            sd.EndTime = scheduleModel.Endtime;
            sd.RegionId = scheduleModel.Regionid;
            sd.Status = 1;
            sd.IsDeleted = new BitArray(1, false);
            _db.ShiftDetails.Add(sd);
            _db.SaveChanges();

            ShiftDetailRegion sr = new ShiftDetailRegion();
            sr.ShiftDetailId = sd.ShiftDetailId;
            sr.RegionId = scheduleModel.Regionid;
            sr.IsDeleted = new BitArray(1, false);
            _db.ShiftDetailRegions.Add(sr);
            _db.SaveChanges();


            if (scheduleModel.Isrepeat != false)
            {

                List<int> day = scheduleModel.checkWeekday.Split(',').Select(int.Parse).ToList();

                foreach (int d in day)
                {
                    DayOfWeek desiredDayOfWeek = (DayOfWeek)d;
                    DateTime today = DateTime.Today;
                    DateTime nextOccurrence = new DateTime(scheduleModel.Startdate.Year, scheduleModel.Startdate.Month, scheduleModel.Startdate.Day);
                    int occurrencesFound = 0;
                    while (occurrencesFound < scheduleModel.Repeatupto)
                    {
                        if (nextOccurrence.DayOfWeek == desiredDayOfWeek)
                        {

                            ShiftDetail sdd = new ShiftDetail();
                            sdd.ShiftId = shift.ShiftId;
                            sdd.ShiftDate = nextOccurrence;
                            sdd.StartTime = scheduleModel.Starttime;
                            sdd.EndTime = scheduleModel.Endtime;
                            sdd.RegionId = scheduleModel.Regionid;
                            sdd.Status = 1;
                            sdd.IsDeleted = new BitArray(1, false);
                            _db.ShiftDetails.Add(sdd);
                            _db.SaveChanges();

                            ShiftDetailRegion srr = new ShiftDetailRegion();
                            srr.ShiftDetailId = sdd.ShiftDetailId;
                            srr.RegionId = scheduleModel.Regionid;
                            srr.IsDeleted = new BitArray(1, false);
                            _db.ShiftDetailRegions.Add(srr);
                            _db.SaveChanges();
                            occurrencesFound++;
                        }
                        nextOccurrence = nextOccurrence.AddDays(1);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Get Edit Shit Page With ShiftDetails Table Records
        /// </summary>
        /// <param name="shiftdetailsid"></param>
        /// <returns></returns>
        public ShiftDetailsmodal GetShift(int shiftdetailsid)
        {
            var shiftdetails = _db.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == shiftdetailsid);
            var physicianlist = _db.PhysicianRegions.Where(p => p.RegionId == shiftdetails.RegionId).Select(s => s.PhysicianId).ToList();
            var shiftid = shiftdetails.ShiftId;
            var ShiftPhysicainId = _db.Shifts.FirstOrDefault(x => x.ShiftId == shiftid).PhysicianId ;

            ShiftDetailsmodal shift = new ShiftDetailsmodal
            {
                Shiftdetailid = shiftdetailsid,
                Shiftdate = shiftdetails.ShiftDate,
                Shiftid = shiftdetails.ShiftId,
                Starttime = shiftdetails.StartTime,
                Endtime = shiftdetails.EndTime,
                Regionid = shiftdetails.RegionId,
                Abberaviation = _db.Regions.FirstOrDefault(i => i.RegionId == shiftdetails.RegionId).Abbreviation,
                Status = shiftdetails.Status,
                regions = _db.Regions.ToList(),
                //Physicians = _db.Physicians.Where(p => physicianlist.Contains(p.PhysicianId)).ToList(),
                Physicians = _db.Physicians.Where(x => x.PhysicianId == ShiftPhysicainId).ToList(),
            };
            return shift;
        }

        /// <summary>
        /// (Update)Approve or UnApprove Shift
        /// </summary>
        /// <param name="status"></param>
        /// <param name="shiftdetailid"></param>
        /// <param name="Aspid"></param>
        public void SetReturnShift(int status, int shiftdetailid, string Aspid)
        {
            var shiftdetails = _db.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == shiftdetailid);
            if (status == 1)
            {
                shiftdetails.Status = 2;
                shiftdetails.ModifiedDate = DateTime.Now;
                shiftdetails.ModifiedBy = Aspid;
            }
            else
            {
                shiftdetails.Status = 1;
                shiftdetails.ModifiedDate = DateTime.Now;
                shiftdetails.ModifiedBy = Aspid;
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete Shift
        /// </summary>
        /// <param name="shiftdetailid"></param>
        /// <param name="Aspid"></param>
        public void SetDeleteShift(int shiftdetailid, string Aspid)
        {
            var shiftdetails = _db.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == shiftdetailid);

            shiftdetails.IsDeleted = new BitArray(1, true);
            shiftdetails.ModifiedDate = DateTime.Now;
            shiftdetails.ModifiedBy = Aspid;

            _db.SaveChanges();
        }

        /// <summary>
        /// Edit Shift
        /// </summary>
        /// <param name="shiftDetailsmodal"></param>
        /// <param name="Aspid"></param>
        public void SetEditShift(ShiftDetailsmodal shiftDetailsmodal, string Aspid)
        {
            var shiftdetails = _db.ShiftDetails.FirstOrDefault(s => s.ShiftDetailId == shiftDetailsmodal.Shiftdetailid);

            if (shiftdetails != null)
            {
                shiftdetails.ShiftDate = shiftDetailsmodal.Shiftdate;
                shiftdetails.StartTime = shiftDetailsmodal.Starttime;
                shiftdetails.EndTime = shiftDetailsmodal.Endtime;
                shiftdetails.ModifiedDate = DateTime.Now;
                shiftdetails.ModifiedBy = Aspid;
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Get Shifts for Review Page
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public List<ShiftReview> GetShiftReview(int regionId, int callId)
        {
            BitArray deletedBit = new BitArray(new[] { false });

            var shiftDetail = _db.ShiftDetails.Where(i => i.IsDeleted.Equals(deletedBit) && i.Status != 2).ToList();

            DateTime currentDate = DateTime.Now;

            if (regionId != 0 || callId == 1)
            {
                shiftDetail = _db.ShiftDetails.Where(i => i.IsDeleted.Equals(deletedBit) && i.RegionId == regionId && i.Status != 2).ToList();

                //if (callId == 1)
                //{
                //    shiftDetail = _context.Shiftdetails.Where(i => i.Isdeleted.Equals(deletedBit) && i.Status != 2 && i.Shiftdate.Month == currentDate.Month && i.Regionid == regionId).ToList();
                //}
            }



            var reviewList = shiftDetail.Select(x => new ShiftReview
            {
                shiftDetailId = x.ShiftDetailId,
                PhysicianName = _db.Physicians.FirstOrDefault(y => y.PhysicianId == _db.Shifts.FirstOrDefault(z => z.ShiftId == x.ShiftId).PhysicianId).FirstName + ", " + _db.Physicians.FirstOrDefault(y => y.PhysicianId == _db.Shifts.FirstOrDefault(z => z.ShiftId == x.ShiftId).PhysicianId).LastName,
                ShiftDate = x.ShiftDate.ToString("MMM dd, yyyy"),
                ShiftTime = x.StartTime.ToString("hh:mm tt") + " - " + x.EndTime.ToString("hh:mm tt"),
                ShiftRegion = _db.Regions.FirstOrDefault(y => y.RegionId == x.RegionId).Name,

            }).ToList();

            return reviewList;
        }

        /// <summary>
        /// Approve all Selected Shifts In Checkbox
        /// </summary>
        /// <param name="shiftDetailsId"></param>
        /// <param name="Aspid"></param>
        public void ApproveSelectedShift(int[] shiftDetailsId, string Aspid)
        {
            foreach (var shiftId in shiftDetailsId)
            {
                var shift = _db.ShiftDetails.FirstOrDefault(i => i.ShiftDetailId == shiftId);
                
                shift.Status = 2;
                shift.ModifiedDate = DateTime.Now;
                shift.ModifiedBy = Aspid;

            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete All Selectes Shifts
        /// </summary>
        /// <param name="shiftDetailsId"></param>
        /// <param name="Aspid"></param>
        public void DeleteShiftReview(int[] shiftDetailsId, string Aspid)
        {
            foreach (var shiftId in shiftDetailsId)
            {
                var shift = _db.ShiftDetails.FirstOrDefault(i => i.ShiftDetailId == shiftId);

                shift.IsDeleted = new BitArray(1, true);
                shift.ModifiedDate = DateTime.Now;
                shift.ModifiedBy = Aspid;

            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Get Records of Oncall Physcians And Off Duty Physicians On Providers On Call Page
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public OnCallModal GetOnCallDetails(int regionId)
        {
            var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
            BitArray deletedBit = new BitArray(new[] { false });

            var onDutyQuery = _db.ShiftDetails
                .Include(sd => sd.Shift.Physician)
                .Where(sd => (regionId == 0 || sd.Shift.Physician.PhysicianRegions.Any(pr => pr.RegionId == regionId)) &&
                             sd.ShiftDate.Date == DateTime.Today &&
                             currentTime >= sd.StartTime &&
                             currentTime <= sd.EndTime &&
                             sd.IsDeleted.Equals(deletedBit))
                .Select(sd => sd.Shift.Physician)
                .Distinct()
                .ToList();


            var offDutyQuery = _db.Physicians
                .Include(p => p.PhysicianRegions)
                .Where(p => (regionId == 0 || p.PhysicianRegions.Any(pr => pr.RegionId == regionId)) &&
                            !_db.ShiftDetails.Any(sd => sd.Shift.PhysicianId == p.PhysicianId &&
                                                               sd.ShiftDate.Date == DateTime.Today &&
                                                               currentTime >= sd.StartTime &&
                                                               currentTime <= sd.EndTime &&
                                                               sd.IsDeleted.Equals(deletedBit)) && p.IsDeleted == null)
                .ToList();

            var onCallModal = new OnCallModal
            {
                OnCall = onDutyQuery,
                OffDuty = offDutyQuery,
                regions = GetRegions(),
                regionId =regionId,
            };
            return onCallModal;
        }

        /// <summary>
        /// Get Records Of PhysiciasLocations Table
        /// </summary>
        /// <returns></returns>
        public List<PhysicianLocation> GetPhysicianlocations()
        {
            var phyLocation = _db.PhysicianLocations.ToList();
            return phyLocation;
        }

    }
}
