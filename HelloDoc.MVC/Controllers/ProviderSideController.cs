using HelloDoc.BAL.Interface;
using HelloDoc.BAL.Repo;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.ViewModels;
using HelloDoc.MVC.Auth;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;


namespace HelloDoc.MVC.Controllers
{
    [CustomAuthorize("Physician","Admin")]
    public class ProviderSideController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProviderDashRepo _iProviderdashRepo;
        private readonly IEncounterRepo _iEncounterRepo;
        private readonly IAdminDashRepo _iAdminDashRepo;
        public ProviderSideController(ApplicationDbContext context, IProviderDashRepo iProviderdashRepo, IEncounterRepo iEncounterRepo, IAdminDashRepo iAdminDashRepo)
        {
            _context = context;
            _iProviderdashRepo = iProviderdashRepo;
            _iEncounterRepo = iEncounterRepo;
            _iAdminDashRepo = iAdminDashRepo;
        }

        // ******************************************************* Dashboard ***********************************************************

        [CustomAuthorize("Physician")]
        public IActionResult ProviderDashboard()
        {
            int[] arr = { 1 };

            string aspId = HttpContext.Session.GetString("AspnetUserid");
            //int physicianId = user.UserId;
            var physicianId = _context.Physicians.FirstOrDefault(i=>i.AspNetUserId == aspId).PhysicianId;

            AdminDashvm adminDashvm = new AdminDashvm()
            {
                StatusForName = 1,
                RequestListAdminDash = _iProviderdashRepo.getRequestData(arr, null, physicianId),
                countRequest = _iProviderdashRepo.GetCountRequest(physicianId),
                sessionId = physicianId,
                AspId = aspId,

            };

            return View(adminDashvm);
        }

        public IActionResult GetProviderDashboard(int physicianId)
        {
            byte[] byteArray = HttpContext.Session.Get("providerDashStatus");

            if (byteArray == null)
            {
                int[] arr = { 1 };

                AdminDashvm adminDashvm = new AdminDashvm()
                {
                    StatusForName = 1,
                    RequestListAdminDash = _iProviderdashRepo.getRequestData(arr, null, physicianId),
                    countRequest = _iProviderdashRepo.GetCountRequest(physicianId),
                    sessionId = physicianId,
                    AspId = _context.Physicians.FirstOrDefault(i=>i.PhysicianId == physicianId).AspNetUserId,
                };

                return PartialView("Provider/Dashboard/_Provider_Dashboard", adminDashvm);
            }
            else
            {
                int[] statusArray = new int[byteArray.Length / sizeof(int)];
                Buffer.BlockCopy(byteArray, 0, statusArray, 0, byteArray.Length);

                AdminDashvm adminDashvm = new AdminDashvm()
                {
                    StatusForName = statusArray[0],
                    RequestListAdminDash = _iProviderdashRepo.getRequestData(statusArray, null, physicianId),
                    countRequest = _iProviderdashRepo.GetCountRequest(physicianId),
                };

                return PartialView("Provider/Dashboard/_Provider_Dashboard", adminDashvm);
            }
        }

        // ************************** Tables **************************
        [HttpPost]
        public IActionResult ProviderTableRecords(int[] status, string requesttypeid, int physicianId)
        {
            byte[] byteArray = new byte[status.Length * sizeof(int)];
            Buffer.BlockCopy(status, 0, byteArray, 0, byteArray.Length);

            HttpContext.Session.Set("providerDashStatus", byteArray);

            AdminDashvm adminDashvm = new AdminDashvm()
            {
                StatusForName = status[0],
                countRequest = _iProviderdashRepo.GetCountRequest(physicianId),
                RequestListAdminDash = _iProviderdashRepo.getRequestData(status, requesttypeid, physicianId),
            };
            return PartialView("Provider/Dashboard/_Provider_DashboardContent", adminDashvm);
        }

        public IActionResult ProviderFilterTable(int[] status, string requesttypeid, int physicianId)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                StatusForName = status[0],
                RequestListAdminDash = _iProviderdashRepo.getRequestData(status, requesttypeid, physicianId),
                reqTypeId = requesttypeid,
            };

            return PartialView("Provider/Dashboard/_Provider_DashboardTable", adminDashvm);
        }

        // ************************** Accept Case **************************
        public IActionResult AcceptCaseModal(int requestid)
        {
            AdminDashvm AdminDashvm = new AdminDashvm();
            AdminDashvm.AcceptRequestId = requestid;

            return PartialView("Provider/Dashboard/_Provider_AcceptCase", AdminDashvm);
        }

        [HttpPost]
        public IActionResult AcceptCase(int requestId, int physicianId)
        {
            _iProviderdashRepo.SetAcceptCaseData(requestId, physicianId);

            return Ok();
        }

        public IActionResult Transfer(int requestid, int physicianId)
        {
            TransferCaseModal transferCaseModal = new TransferCaseModal()
            {
                RequestId = requestid,
                PhysicianId = physicianId
            };
            return PartialView("Provider/Dashboard/_Provider_TransferCase", transferCaseModal);
        }

        [HttpPost]
        public IActionResult TransferCase(TransferCaseModal transferCaseModal)
        {
            if (transferCaseModal.RequestId != 0)
            {
                _iProviderdashRepo.TransferCaseData(transferCaseModal);

                return Ok();
            }
            return Json(new { Error = "Returned in else" });
        }

        // ************************** Encounter Case **************************
        public IActionResult EncounterModal(int requestid, int physicianId)
        {
            Encountervm encountervm = new Encountervm();
            encountervm.RequestId = requestid;
            encountervm.PhysicianId = physicianId;

            return PartialView("Provider/Dashboard/_Provider_EncounterModal", encountervm);
        }

        [HttpPost]
        public IActionResult PostEncounterCare(Encountervm encountervm)
        {
            _iProviderdashRepo.SetEncounterCareType(encountervm);

            if (encountervm.Option == 1)
            {
                return Ok(true);
            }
            return Ok(false);
        }

        public IActionResult finalizeEncounterModal(int requestId)
        {
            Encountervm encountervm = new Encountervm();
            encountervm.RequestId = requestId;

            return PartialView("Provider/Dashboard/_Provider_EncounterModal", encountervm);
        }

        [HttpPost]
        public IActionResult finalizeEncounter(int requestId)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            _iProviderdashRepo.FinalizeEncounterCase(requestId,aspId);

            return Ok();
        }

        public IActionResult DownloadEncounter(int requestId)
        {
            Encountervm encountervm = new Encountervm();
            encountervm.RequestId = requestId;

            return PartialView("Provider/Dashboard/_Provider_EncounterModal", encountervm);
        }

        public IActionResult GeneratePDF([FromQuery] int requestid)
        {
            var encounterFormView = _iEncounterRepo.getEncounterFormData(requestid, 0);
            if (encounterFormView == null)
            {
                return NotFound();
            }
            return new ViewAsPdf("FinalizeForm", encounterFormView)
            {
                FileName = "EncounterForm.pdf"
            };
        }

        public IActionResult HouseCall(int requestId)
        {
            Encountervm encountervm = new Encountervm();
            encountervm.RequestId = requestId;

            return PartialView("Provider/Dashboard/_Provider_EncounterModal", encountervm);
        }

        [HttpPost]
        public IActionResult HouseCallPost(int requestId)
        {
            _iProviderdashRepo.HouseCallConclude(requestId);

            return Ok();
        }

        // ************************** Conclude Case **************************
        public IActionResult GetConcludeCare(int requestid)
        {
            AdminViewUploadsvm adminViewUploadsvm = new AdminViewUploadsvm();
            adminViewUploadsvm = _iProviderdashRepo.GetViewDocumentsData(requestid);
            adminViewUploadsvm.viewUploadsList = _iProviderdashRepo.GetViewDocumentsList(requestid);

            return PartialView("Provider/Dashboard/_Provider_ConcludeCare", adminViewUploadsvm);
        }

        [HttpPost]
        public IActionResult UploadConcludeDocument(AdminViewUploadsvm adminViewUploadsvm)
        {
            _iProviderdashRepo.SetViewDocumentData(adminViewUploadsvm);

            return Ok(adminViewUploadsvm.RequestId);
        }

        public IActionResult DeleteConcludeFile(int requestwisefileid)
        {
            _iProviderdashRepo.DeleteFileData(requestwisefileid);

            return Ok();
        }

        [HttpPost]
        public IActionResult ConcludeCare(AdminViewUploadsvm adminViewUploadsvm)
        {
            _iProviderdashRepo.ConfirmConcludeCare(adminViewUploadsvm);

            return Ok();
        }

        // ************************** Request To Admin **************************

        [HttpPost]
        public IActionResult SendRequestToAdmin(ProviderProfilevm providerProfilevm)
        {
            _iProviderdashRepo.RequestForEdit(providerProfilevm);

            return Ok();
        }

        #region Invoicing

        // ************************** Invoicing**************************
        public IActionResult GetProviderInvoicing(string dateSelected)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");
            var phyid = _context.Physicians.FirstOrDefault(x=>x.AspNetUserId == aspId).PhysicianId;

            ProviderInvoicingvm providerInvoicingvm = new ProviderInvoicingvm()
            {
                Timesheetdetails = _iProviderdashRepo.GetTimeSheetDetails(phyid, dateSelected),
                Timesheetdetailreimbursements = _iProviderdashRepo.GetTimeSheetDetailsReimbursements(phyid, dateSelected),

            };
            providerInvoicingvm.TimesheetsFinalize = providerInvoicingvm.Timesheetdetails.Any(x => x.Timesheet.IsFinalize == true);

            return PartialView("Provider/Invoicing/_Provider_Invoicing", providerInvoicingvm);
        }

        public IActionResult OpenFinalizeTimeSheet(string dateSelected)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");
            var phyid = _context.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId).PhysicianId;

            ProviderInvoicingvm providerInvoicingvm = new ProviderInvoicingvm()
            {
                ProviderTimesheetDetails = _iProviderdashRepo.GetFinalizeTimeSheetDetails(phyid, dateSelected),
                CallId = 2,
            };

            return PartialView("Provider/Invoicing/_Provider_FinalizeTimeSheet", providerInvoicingvm);
        }

        [HttpPost]
        public IActionResult PostFinalizeTimesheet(List<ProviderTimesheetDetails> providerTimesheetDetails)
        {
            if (_iProviderdashRepo.PostFinalizeTimesheet(providerTimesheetDetails))
            {
                return Ok(true);
            };
            return Ok(false);
        }

        public IActionResult GetAddReceipts(int[] timeSheetDetailId,int callId)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            ProviderInvoicingvm providerInvoicingvm = new ProviderInvoicingvm()
            {
                AddReceiptsDetails = _iProviderdashRepo.GetAddReceiptsDetails(timeSheetDetailId, aspId),
                CallId = callId,
            };

            return PartialView("Provider/Invoicing/_Provider_AddReceipts", providerInvoicingvm);
        }

        [HttpPost]
        public IActionResult PostAddReceipt(int timeSheetDetailId, string item, int amount, IFormFile file)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");


            if (_iProviderdashRepo.EditReceipt(aspId, timeSheetDetailId, item, amount, file))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost]
        public IActionResult DeleteReceipt(int timeSheetDetailId)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            if (_iProviderdashRepo.DeleteReceipt(aspId, timeSheetDetailId))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost]
        public IActionResult ConfirmFinalizeTimeSheet(int timeSheetId)
        {
            if (_iProviderdashRepo.FinalizeTimeSheet(timeSheetId))
            {
                return Ok(true);
            }
            return Ok(false);
        }
        #endregion
    }
}
