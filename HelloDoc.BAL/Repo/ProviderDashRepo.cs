using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.BAL.Repo
{
    public class ProviderDashRepo : IProviderDashRepo
    {
        private readonly ApplicationDbContext _context;

        public ProviderDashRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  CountRequest As Per Status Fetch Records
        /// </summary>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public CountRequest GetCountRequest(int physicianId)
        {
            var request = _context.Requests;

            var countData = new CountRequest()
            {
                NewRequest = request.Where(i => i.Status == 1 && i.PhysicianId == physicianId).Count(),
                PendingRequest = request.Where(i => i.Status == 2 && i.PhysicianId == physicianId).Count(),
                ActiveRequest = request.Where(i => (i.Status == 4 || i.Status == 5) && i.PhysicianId == physicianId).Count(),
                ConcludeRequest = request.Where(i => i.Status == 6 && i.PhysicianId == physicianId).Count()
            };
            return countData;
        }

        /// <summary>
        /// Fetch Table Records on ProviderDashboard Page
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="physicinId"></param>
        /// <returns></returns>
        // ***************************** Table data *****************************
        public List<RequestListAdminDash> getRequestData(int[] Status, string requesttypeid, int physicinId)
        {
            var requestList = _context.Requests.Where(i => Status.Contains(i.Status) && i.PhysicianId == physicinId);

            if (requesttypeid != null)
            {
                requestList = requestList.Where((i => i.RequestTypeId.ToString() == requesttypeid));
            }

            var tableData = requestList.Select(r => new RequestListAdminDash()
            {
                Name = r.RequestClients.Select(x => x.FirstName).First() + " " + r.RequestClients.Select(x => x.LastName).First(),
                Email = r.RequestClients.Select(x => x.Email).First(),
                Mobile = r.PhoneNumber == null ? "-" : r.PhoneNumber,
                Phone = r.RequestClients.Select(x => x.PhoneNumber).First(),
                Address = r.RequestClients.Select(x => x.Street).First() + ", " + r.RequestClients.Select(x => x.City).First() + ", " + r.RequestClients.Select(x => x.State).First(),
                DateOfBirth = new DateTime(r.RequestClients.Select(x => Convert.ToInt32(x.IntYear)).First(), r.RequestClients.Select(x => Convert.ToInt32(x.StrMonth)).First(), r.RequestClients.Select(x => Convert.ToInt32(x.IntDate)).First()),
                RequestTypeId = r.RequestTypeId,
                RequestId = r.RequestId,

                RequestDate = r.CreatedDate,
                totalHours = (int)(DateTime.Now - r.CreatedDate).TotalMinutes / 60,
                totalMinutes = (int)(DateTime.Now - r.CreatedDate).TotalMinutes % 60,
                totalSeconds = (int)(DateTime.Now - r.CreatedDate).TotalSeconds % 60,

                callType = r.CallType == null ? 0 : (int)r.CallType,
                isFinalized = r.EncounterForms.Select(x => x.IsFinalize).First() == true,

            })
            .OrderByDescending(r => r.RequestDate)
            .ToList();
            return tableData;
        }

        /// <summary>
        /// Update Records on StatusLog Table  After Accept Case
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="physicianId"></param>
        public void SetAcceptCaseData(int requestId, int physicianId)
        {
            var request = _context.Requests.FirstOrDefault(i => i.RequestId == requestId);

            var physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianId);

            string acceptNote = "Case is Accepted By " + physician.FirstName + " " + physician.LastName + " on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

            var acceptCase = new RequestStatusLog()
            {
                RequestId = requestId,
                Status = 2,
                PhysicianId = physicianId,
                CreatedDate = DateTime.Now,
                Notes = acceptNote,
            };
            _context.RequestStatusLogs.Add(acceptCase);

            request.Status = 2;
            request.ModifiedDate = DateTime.Now;

            _context.SaveChanges();
        }

        /// <summary>
        /// Update Records on StatusLog Table  After Transfer Case
        /// </summary>
        /// <param name="transferCaseModal"></param>
        public void TransferCaseData(TransferCaseModal transferCaseModal)
        {
            var request = _context.Requests.FirstOrDefault(i => i.RequestId == transferCaseModal.RequestId);

            var physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == transferCaseModal.PhysicianId);

            string transferNote = physician.FirstName + " Transferred to Admin on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt") + " : " + transferCaseModal.TransferDescription;
            var transferData = new RequestStatusLog()
            {
                RequestId = transferCaseModal.RequestId,
                Status = 1,
                CreatedDate = DateTime.Now,
                TransToAdmin = new BitArray(1, true),
                Notes = transferNote,
            };

            request.Status = 1;
            request.PhysicianId = null;

            _context.RequestStatusLogs.Add(transferData);
            _context.SaveChanges();

        }

        /// <summary>
        /// Update Records on StatusLog Table  After Select HouseCall Or Consuly On Encounter Model
        /// </summary>
        /// <param name="encountervm"></param>
        public void SetEncounterCareType(Encountervm encountervm)
        {
            var request = _context.Requests.FirstOrDefault(i => i.RequestId == encountervm.RequestId);

            if (request != null)
            {
                var physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == encountervm.PhysicianId);

                var care = "";

                if (encountervm.Option == 1)
                {
                    care = "HOUSE CALL";
                    request.CallType = 1;
                    request.Status = 5;
                }
                else if (encountervm.Option == 2)
                {
                    care = "CONSULT";
                    request.CallType = 2;
                    request.Status = 6;
                }

                request.ModifiedDate = DateTime.Now;
                _context.SaveChanges();

                string encounterNote = physician.FirstName + " Selected " + care + " on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

                var transferData = new RequestStatusLog()
                {
                    RequestId = encountervm.RequestId,
                    Status = request.Status,
                    CreatedDate = DateTime.Now,
                    Notes = encounterNote,
                    PhysicianId = encountervm.PhysicianId,
                };
                _context.RequestStatusLogs.Add(transferData);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Finalize Encounter From , After Finalize Can't Update it
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="aspId"></param>
        public void FinalizeEncounterCase(int requestId, string aspId)
        {
            var physicianId = _context.Physicians.FirstOrDefault(i=>i.AspNetUserId == aspId).PhysicianId;
            var encounter = _context.EncounterForms.FirstOrDefault(i => i.RequestId == requestId);

            if (encounter != null)
            {
                encounter.IsFinalize = true;
                _context.SaveChanges();
            }
            else
            {
                EncounterForm newencounerData = new EncounterForm()
                {
                    IsFinalize = true,
                    PhysicianId = physicianId,
                    RequestId = requestId,
                };
                _context.EncounterForms.Add(newencounerData);
                _context.SaveChanges();

            }
        }

        /// <summary>
        /// Update Records on StatusLog Table  After Select HouseCall Button
        /// </summary>
        /// <param name="requestId"></param>
        public void HouseCallConclude(int requestId)
        {
            var requestData = _context.Requests.FirstOrDefault(i => i.RequestId == requestId);

            if (requestData != null)
            {
                requestData.Status = 6;
                requestData.ModifiedDate = DateTime.Now;
                _context.SaveChanges();

                var physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == requestData.PhysicianId);

                string houseCallNote = physician.FirstName + " House Called care on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

                var statusData = new RequestStatusLog()
                {
                    RequestId = requestData.RequestId,
                    Status = 6,
                    PhysicianId = requestData.PhysicianId,
                    CreatedDate = DateTime.Now,
                    Notes = houseCallNote,
                };

                _context.RequestStatusLogs.Add(statusData);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Fetch Records On ConculdeCare Page
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public AdminViewUploadsvm GetViewDocumentsData(int requestid)
        {
            var patient = _context.RequestClients.FirstOrDefault(i => i.RequestId == requestid);

            string confirmation = _context.Requests.FirstOrDefault(i => i.RequestId == requestid).ConfirmationNumber;
            var userId = _context.Requests.FirstOrDefault(i => i.RequestId == requestid).UserId;

            var documentData = new AdminViewUploadsvm()
            {
                UserId = userId == null ? 0 : (int)userId,
                RequestId = requestid,
                Name = patient.FirstName + " " + patient.LastName,
                ConformationNumber = confirmation,
            };

            return documentData;

        }

        /// <summary>
        /// Fetch List Of RequestWiseFile Table Records On ConcludeCare Page
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public List<ViewUploadsModel> GetViewDocumentsList(int requestid)

        {
            var document = _context.RequestWiseFiles.Where(i => i.RequestId == requestid && i.IsDeleted != new BitArray(1, true));

            var viewDocuments = document.Select(r => new ViewUploadsModel()
            {
                requestWiseFileId = r.RequestWiseFileId,
                RequestId = requestid,
                DocumentName = r.FileName,
                UploadDate = r.CreatedDate,

            }).ToList();
            return viewDocuments;
        }

        /// <summary>
        /// Update Records On Conclude Care Page
        /// </summary>
        /// <param name="adminViewUploadsvm"></param>
        public void SetViewDocumentData(AdminViewUploadsvm adminViewUploadsvm)
        {

            IFormFile File1 = adminViewUploadsvm.Document;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", File1.FileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                File1.CopyTo(fileStream);
            }

            var fileName = adminViewUploadsvm.Document?.FileName;

            var fileData = new RequestWiseFile()
            {
                RequestId = adminViewUploadsvm.RequestId,
                FileName = fileName,
                CreatedDate = DateTime.Now,
            };

            _context.Add(fileData);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete File On Conclude care Page
        /// </summary>
        /// <param name="requestwisefileid"></param>
        public void DeleteFileData(int requestwisefileid)
        {
            var file = _context.RequestWiseFiles.FirstOrDefault(i => i.RequestWiseFileId == requestwisefileid);
            if(file != null)
            {
                file.IsDeleted = new BitArray(1,true);
            }
            _context.RequestWiseFiles.Remove(file);
            _context.SaveChanges();
        }

        /// <summary>
        /// Update Records on StatusLog Table  After Click On ConfirmConcludeCare Button
        /// </summary>
        /// <param name="adminViewUploadsvm"></param>
        public void ConfirmConcludeCare(AdminViewUploadsvm adminViewUploadsvm)
        {
            var request = _context.Requests.FirstOrDefault(i => i.RequestId == adminViewUploadsvm.RequestId);

            if (request != null)
            {
                request.Status = 8;
                request.ModifiedDate = DateTime.Now;
                request.CompletedByPhysician = new BitArray(1, true);

                _context.SaveChanges();

                var physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == request.PhysicianId);

                var noteData = new RequestNote()
                {
                    RequestId = request.RequestId,
                    PhysicianNotes = adminViewUploadsvm.ProviderNote,
                    CreatedBy = physician.AspNetUserId,
                    CreatedDate = DateTime.Now,
                };

                _context.RequestNotes.Add(noteData);
                _context.SaveChanges();

                string concludeNote = physician.FirstName + " Concluded care on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt") + adminViewUploadsvm.ProviderNote;

                var statusData = new RequestStatusLog()
                {
                    RequestId = request.RequestId,
                    Status = 8,
                    PhysicianId = request.PhysicianId,
                    CreatedDate = DateTime.Now,
                    Notes = concludeNote,
                };

                _context.RequestStatusLogs.Add(statusData);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Provider Sending Mail Who have Created his Account To Edit his profile Details 
        /// </summary>
        /// <param name="providerProfilevm"></param>
        public void RequestForEdit(ProviderProfilevm providerProfilevm)
        {
            var physician = _context.Physicians.FirstOrDefault(i => i.PhysicianId == providerProfilevm.PhysicianId);

            if (physician != null)
            {
                var aspData = _context.AspNetUsers.FirstOrDefault(i => i.Id == physician.CreatedBy);

                if (aspData != null)
                {
                    var mail = "tatva.dotnet.sohilvekariya123@outlook.com";
                    var password = "Devjibapa@2023";
                    var email = aspData.Email;
                    var subject = "Request For Edit";
                    var message = $"Hey <b>{aspData?.UserName}</b>, <br><br> Message : " + providerProfilevm.RequestMessage + "<br> Request by : " + physician.FirstName;

                    SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587)
                    {
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(mail, password)
                    };

                    MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
                    mailMessage.IsBodyHtml = true;

                    smtpClient.SendMailAsync(mailMessage);

                    var emailLog = new EmailLog()
                    {
                        SubjectName = subject,
                        EmailTemplate = "outlook",
                        EmailId = email,
                        RoleId = 1,
                        CreateDate = DateTime.Now,
                        SentDate = DateTime.Now,
                        IsEmailSent = new BitArray(1, true),
                        SentTries = 1,
                        PhysicianId = physician.PhysicianId
                    };

                    _context.EmailLogs.Add(emailLog);
                    _context.SaveChanges();
                }

            }
        }

        ///
        public List<TimesheetDetail> GetTimeSheetDetails(int phyid, string dateSelected)
        {
            
            var data = _context.TimesheetDetails.Include(i => i.Timesheet).Where(i => i.Timesheet.PhysicianId == phyid).OrderBy(i => i.TimesheetDetailId).ToList();          

            if(dateSelected == null)
            {
                var currentMonth = DateTime.Now.Month.ToString();
                var currentYear = DateTime.Now.Year;
                if (currentMonth.Length == 1)
                {
                    currentMonth = "0" + currentMonth;
                }

                var startDate = "01" + "-" + currentMonth + "-" + currentYear.ToString();
                var endDate = 15 + "-" + currentMonth + "-" + currentYear.ToString();

                data = data.Where(i => i.Timesheet.StartDate.ToString() == startDate && i.Timesheet.EndDate.ToString() == endDate).ToList();
            }
            if (data.Count > 0 && dateSelected != null)
            {
                var splitedDate = dateSelected.Split('-');
                var currentYear = DateTime.Now.Year;
                var daysInMonth = DateTime.DaysInMonth(currentYear, Convert.ToInt32(splitedDate[0]));
                if (splitedDate[0].Length == 1)
                {
                    splitedDate[0] = "0" + splitedDate[0];
                }

                if (splitedDate[1] == "1")
                {
                    var startDate = "01" + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = 15 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    data = data.Where(i => i.Timesheet.StartDate.ToString() == startDate && i.Timesheet.EndDate.ToString() == endDate).ToList();
                }
                else if (splitedDate[1] == "2")
                {
                    var startDate = 16 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = daysInMonth.ToString() + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    data = data.Where(i => i.Timesheet.StartDate.ToString() == startDate && i.Timesheet.EndDate.ToString() == endDate).ToList();
                }
            }

            return data;
        }

        public List<TimesheetDetailReimbursement> GetTimeSheetDetailsReimbursements(int phyid, string dateSelected)
        {
            var data = _context.TimesheetDetailReimbursements.Include(i => i.TimesheetDetail.Timesheet).Where(i => i.TimesheetDetail.Timesheet.PhysicianId == phyid && i.IsDeleted != true).OrderBy(i => i.TimesheetDetailId).ToList();

            if (dateSelected == null)
            {
                var currentMonth = DateTime.Now.Month.ToString();
                var currentYear = DateTime.Now.Year;
                if (currentMonth.Length == 1)
                {
                    currentMonth = "0" + currentMonth;
                }

                var startDate = "01" + "-" + currentMonth + "-" + currentYear.ToString();
                var endDate = 15 + "-" + currentMonth + "-" + currentYear.ToString();

                data = data.Where(i => i.TimesheetDetail.Timesheet.StartDate.ToString() == startDate && i.TimesheetDetail.Timesheet.EndDate.ToString() == endDate).ToList();
            }

            if (data.Count > 0 && dateSelected != null)
            {
                var splitedDate = dateSelected.Split('-');
                var currentYear = DateTime.Now.Year;
                var daysInMonth = DateTime.DaysInMonth(currentYear, Convert.ToInt32(splitedDate[0]));
                if (splitedDate[0].Length == 1)
                {
                    splitedDate[0] = "0" + splitedDate[0];
                }

                if (splitedDate[1] == "1")
                {
                    var startDate = "01" + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = 15 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    data = data.Where(i => i.TimesheetDetail.Timesheet.StartDate.ToString() == startDate && i.TimesheetDetail.Timesheet.EndDate.ToString() == endDate).ToList();
                }
                else if (splitedDate[1] == "2")
                {
                    var startDate = 16 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = daysInMonth.ToString() + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    data = data.Where(i => i.TimesheetDetail.Timesheet.StartDate.ToString() == startDate && i.TimesheetDetail.Timesheet.EndDate.ToString() == endDate).ToList();
                }
            }

            return data;
        }

        public List<ProviderTimesheetDetails> GetFinalizeTimeSheetDetails(int phyid, string dateSelected)
        {
            var currentYear = DateTime.Now.Year;
            var data = _context.TimesheetDetails
                                .Include(i => i.Timesheet)
                                .Where(i => i.Timesheet.PhysicianId == phyid)
                                .OrderBy(i => i.TimesheetDetailId)
                                .ToList();

            if (!string.IsNullOrEmpty(dateSelected))
            {
                var splitedDate = dateSelected.Split('-');
                var month = Convert.ToInt32(splitedDate[0]);
                var daysInMonth = DateTime.DaysInMonth(currentYear, month);
                var startDate = splitedDate[1] == "1" ? new DateOnly(currentYear, month, 1) : new DateOnly(currentYear, month, 16);
                var endDate = splitedDate[1] == "1" ? new DateOnly(currentYear, month, 15) : new DateOnly(currentYear, month, daysInMonth);

                data = data.Where(i => i.Timesheet.StartDate == startDate && i.Timesheet.EndDate == endDate).ToList();

                if (data.Count == 0)
                {
                    var newShift = new Timesheet
                    {
                        PhysicianId = phyid,
                        StartDate = startDate,
                        EndDate = endDate,
                        IsFinalize = false,
                        IsApproved = false,
                        CreatedBy = _context.Physicians.FirstOrDefault(i => i.PhysicianId == phyid).AspNetUserId,
                    };

                    _context.Timesheets.Add(newShift);

                    _context.SaveChanges();

                    for (int i = splitedDate[1] == "1" ? 1 : 16; i <= (splitedDate[1] == "1" ? 15 : daysInMonth); i++)
                    {
                        var newShiftData = new TimesheetDetail
                        {
                            Timesheet = newShift,
                            TimesheetDate = new DateOnly(currentYear, month, i),
                        };

                        _context.TimesheetDetails.Add(newShiftData);
                    }

                    _context.SaveChanges();

                    data = _context.TimesheetDetails
                                   .Include(i => i.Timesheet)
                                   .Where(i => i.Timesheet.PhysicianId == phyid &&
                                               i.Timesheet.StartDate == startDate &&
                                               i.Timesheet.EndDate == endDate)
                                   .OrderBy(i => i.TimesheetDetailId)
                                   .ToList();
                }
            }

            var timeSheetData = data.Select(i => new ProviderTimesheetDetails
            {
                TimeSheetId = i.TimesheetId,
                TimeSheetDetailId = i.TimesheetDetailId,
                ShiftDetailDate = i.TimesheetDate,
                Hours = (int?)i.TotalHours,
                NoOfConsults = i.NumberOfPhoneCall,
                NoOfHouseCalls = i.NumberOfHouseCall,
                IsWeekend = i.IsWeekend,
            }).ToList();

            return timeSheetData;
        }

        public bool PostFinalizeTimesheet(List<ProviderTimesheetDetails> providerTimesheetDetails)
        {
            try
            {
                if (providerTimesheetDetails != null && providerTimesheetDetails.Count > 0)
                {
                    foreach (var detail in providerTimesheetDetails)
                    {
                        var data = _context.TimesheetDetails.FirstOrDefault(x => x.TimesheetDetailId == detail.TimeSheetDetailId);

                        if (data != null)
                        {
                            data.TotalHours = detail.Hours;
                            data.NumberOfHouseCall = detail.NoOfHouseCalls;
                            data.NumberOfPhoneCall = detail.NoOfConsults;
                            data.IsWeekend = detail.IsWeekend;
                        }
                    }
                    _context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }


        public List<AddReceiptsDetails> GetAddReceiptsDetails(int[] timeSheetDetailId, string AspId)
        {
            var data = _context.TimesheetDetailReimbursements
                                .Where(x => timeSheetDetailId.Contains(x.TimesheetDetailId) && x.IsDeleted != true)
                                .OrderBy(x => x.TimesheetDetailReimbursementId)
                                .ToList();


            if (data.Count == 0)
            {
                for (int i = 0; i < timeSheetDetailId.Length; i++)
                {
                    var newReimbursementsData = new TimesheetDetailReimbursement
                    {
                        TimesheetDetailId = timeSheetDetailId[i],
                        TimeSheetDate = _context.TimesheetDetails.FirstOrDefault(x => x.TimesheetDetailId == timeSheetDetailId[i])?.TimesheetDate,
                        CreatedDate = DateTime.Now,
                        CreatedBy = AspId,
                    };

                    _context.TimesheetDetailReimbursements.Add(newReimbursementsData);
                }

                _context.SaveChanges();

                data = _context.TimesheetDetailReimbursements
                                .Where(x => timeSheetDetailId.Contains(x.TimesheetDetailId))
                                .ToList();
            }


            var reimbursementsData = data.Select(i => new AddReceiptsDetails
            {
                TimeSheetDetailId = i.TimesheetDetailId,
                ShiftDetailDate = i.TimeSheetDate,
                Item = i.ItemName,
                Amount = i.Amount,
                BillValue = i.Bill,
            }).ToList();

            return reimbursementsData;
        }

        public bool EditReceipt(string aspId, int timeSheetDetailId, string item, int amount, IFormFile file)
        {
            try
            {
                var reimbursementsData = _context.TimesheetDetailReimbursements.FirstOrDefault(x => x.TimesheetDetailId == timeSheetDetailId);
                var physician = _context.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId);

                reimbursementsData.ItemName = item;
                reimbursementsData.Amount = amount;
                reimbursementsData.ModifiedBy = aspId;
                reimbursementsData.ModifiedDate = DateTime.Now;

                string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content");

                if (file != null)
                {
                    //var fileName = "Receipt_" + reimbursementsData.Timesheetdate;

                    string path = Path.Combine(directory, file.FileName);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    reimbursementsData.Bill = file.FileName;
                }

                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteReceipt(string aspId, int timeSheetDetailId)
        {
            try
            {
                var reimbursementsData = _context.TimesheetDetailReimbursements.FirstOrDefault(x => x.TimesheetDetailId == timeSheetDetailId);

                reimbursementsData.IsDeleted = true;
                reimbursementsData.ModifiedBy = aspId;
                reimbursementsData.ModifiedDate = DateTime.Now;

                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool FinalizeTimeSheet(int timeSheetId)
        {
            try
            {
                var timeSheetData = _context.Timesheets.FirstOrDefault(x => x.TimesheetId == timeSheetId);

                timeSheetData.IsFinalize = true;
                timeSheetData.ModifiedDate = DateTime.Now;

                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
