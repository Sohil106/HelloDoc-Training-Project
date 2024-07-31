using Azure.Core;
using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using HelloDoc.DAL.ViewModels.Records;
using HelloDoc.MVC.Auth;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Rotativa.AspNetCore;


namespace HelloDoc.MVC.Controllers
{
    [CustomAuthorize("Admin", "Physician")]
    public class AdminSideController : Controller
    {

        //private readonly ILogger _logger;
        private readonly ApplicationDbContext _db;
        private readonly IAdminDashRepo _iAdminDashRepo;
        private readonly IEncounterRepo _iEncounterRepo;
        private readonly IAdminProfileRepo _iAdminProfileRepo;
        private readonly IProviderDetailsRepo _iProviderDetailsRepo;
        private readonly ISchedulingRepo _iSchedulingRepo;
        private readonly IPartnersRepo _iPartnersRepo;
        private readonly IRecordsRepo _iRecordsRepo;
        private readonly IProviderDashRepo _iProviderDashRepo;

        public AdminSideController(ApplicationDbContext db, IAdminDashRepo iAdminDashRepo, IEncounterRepo iEncounterRepo, IAdminProfileRepo iAdminProfileRepo, IProviderDetailsRepo iProviderDetailsRepo, ISchedulingRepo iSchedulingRepo, IPartnersRepo iPartnersRepo, IRecordsRepo iRecordsRepo, IProviderDashRepo iProviderDashRepo)
        {
            _db = db;
            _iAdminDashRepo = iAdminDashRepo;
            _iEncounterRepo = iEncounterRepo;
            _iAdminProfileRepo = iAdminProfileRepo;
            _iProviderDetailsRepo = iProviderDetailsRepo;
            _iSchedulingRepo = iSchedulingRepo;
            _iPartnersRepo = iPartnersRepo;
            _iRecordsRepo = iRecordsRepo;
            _iProviderDashRepo = iProviderDashRepo;
        }


        /// <summary>
        /// after login redirct to Admin Dashboard with New Status Tab
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize("Admin")]
        public IActionResult AdminDashboard()
        {
            int[] arr = { 1 };
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                StatusForName = 1,
                RequestListAdminDash = _iAdminDashRepo.getRequestData(arr, null, 0),
                regions = _iAdminDashRepo.getRegions(),


            };
            _iAdminDashRepo.countRequest(adminDashvm);

            return View(adminDashvm);
        }

        /// <summary>
        /// After going in somepartial view than than click on back button redirect to same tab in dashboard
        /// </summary>
        /// <returns></returns>
        public IActionResult GetDashboard()
        {
            byte[] byteArray = HttpContext.Session.Get("statusArray");

            if (byteArray == null)
            {
                int[] arr = { 1 };

                AdminDashvm adminDashvm = new AdminDashvm()
                {
                    StatusForName = 1,
                    RequestListAdminDash = _iAdminDashRepo.getRequestData(arr, null, 0),
                    regions = _iAdminDashRepo.getRegions(),

                };
                _iAdminDashRepo.countRequest(adminDashvm);
                return PartialView("Admin/Dashboard/_AdminDashboard", adminDashvm);
            }
            else
            {
                int[] statusArray = new int[byteArray.Length / sizeof(int)];
                Buffer.BlockCopy(byteArray, 0, statusArray, 0, byteArray.Length);


                AdminDashvm adminDashvm = new AdminDashvm()
                {
                    StatusForName = statusArray[0],
                    RequestListAdminDash = _iAdminDashRepo.getRequestData(statusArray, null, 0),
                    regions = _iAdminDashRepo.getRegions(),

                };
                _iAdminDashRepo.countRequest(adminDashvm);

                return PartialView("Admin/Dashboard/_AdminDashboard", adminDashvm);
            }
        }

        /// <summary>
        /// get table records based on Status (New,Pending,Active,Unpaid,Conclude,ToClose,UnPaid)
        /// </summary>
        /// <param name="status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult TableRecords(int[] status, string requesttypeid, int regionid)
        {
            byte[] byteArray = new byte[status.Length * sizeof(int)];
            Buffer.BlockCopy(status, 0, byteArray, 0, byteArray.Length);

            HttpContext.Session.Set("statusArray", byteArray);

            AdminDashvm adminDashvm = new AdminDashvm()
            {
                StatusForName = status[0],
                RequestListAdminDash = _iAdminDashRepo.getRequestData(status, requesttypeid, regionid),
                regions = _iAdminDashRepo.getRegions(),

            };
            _iAdminDashRepo.countRequest(adminDashvm);

            return PartialView("Admin/Dashboard/_AdminTable", adminDashvm);
        }

        /// <summary>
        /// Filter Table Records based on requesttypeid,regionid
        /// </summary>
        /// <param name="status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult FilterTableRecords(int[] status, string requesttypeid, int regionid)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                StatusForName = status[0],
                RequestListAdminDash = _iAdminDashRepo.getRequestData(status, requesttypeid, regionid),
                regions = _iAdminDashRepo.getRegions(),
                reqTypeId = requesttypeid,

            };
            return PartialView("Admin/Dashboard/_AdminRequestTable", adminDashvm);
        }

        /// <summary>
        /// GET SenDLink Modal PartialView On Dashboard by Clicking on Send Link btn
        /// </summary>
        /// <param name="status"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult sendLinkModal(int status, int callId)
        {
            AdminDashvm adminDashvm = new AdminDashvm();
            adminDashvm.StatusForName = status;
            adminDashvm.CallId = callId;

            return PartialView("Admin/Dashboard/_AdminSendLink", adminDashvm);
        }

        /// <summary>
        /// Send Link to Patient For submit a Request 
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendLink(AdminDashvm adminDashvm)
        {
            String aspId = HttpContext.Session.GetString("AspnetUserid");

            _iAdminDashRepo.SubmitRequestMail(adminDashvm.sendLinkModel, aspId);

            return Ok();
        }


        /// <summary>
        /// Get "Create Request by Admin/Provider" Page by clicking on Create Request Btn
        /// </summary>
        /// <param name="status"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        // ************************** Create Request by Admin/Provider **************************
        public IActionResult CreateRequestByAdmin(int status, int callId)
        {
            AdminCreateRequestvm adminCreateRequestvm = new AdminCreateRequestvm();
            adminCreateRequestvm.Regions = _iAdminDashRepo.getRegions();
            adminCreateRequestvm.StatusForName = status;
            adminCreateRequestvm.CallId = callId;

            return PartialView("Admin/Dashboard/_AdminCreateRequest", adminCreateRequestvm);
        }

        /// <summary>
        /// Created Request Post Method
        /// </summary>
        /// <param name="adminCreateRequestvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult sendAdminCreateRequest(AdminCreateRequestvm adminCreateRequestvm)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            _iAdminDashRepo.SendCreateRequestData(adminCreateRequestvm, aspId);

            return Ok();
        }

        /// <summary>
        ///Check Region is Available or not for Service
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult checkRegionAvailability(int region)
        {
            if (_iAdminDashRepo.CheckRegion(region))
            {
                return Json(true);
            }
            return Json(false);
        }

       /// <summary>
       /// Export RequestList to Excel
       /// </summary>
       /// <param name="arr"></param>
       /// <param name="requesttypeid"></param>
       /// <param name="regionid"></param>
       /// <returns></returns>
        [HttpPost]
        public IActionResult Export(int[] arr, string requesttypeid, int regionid)
        {

            var requestData = _iAdminDashRepo.getRequestData(arr, requesttypeid, regionid);

            // Set LicenseContext to suppress the LicenseException
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create a new Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                // Add a new worksheet to the Excel package
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RequestData");

                // Add headers to the worksheet
                worksheet.Cells[1, 1].Value = "Patient Name";
                worksheet.Cells[1, 2].Value = "Patient Phone";
                worksheet.Cells[1, 3].Value = "Patient Email";
                worksheet.Cells[1, 4].Value = "Patient Address";
                worksheet.Cells[1, 5].Value = "Birth Date";
                worksheet.Cells[1, 6].Value = "Request ID";
                worksheet.Cells[1, 7].Value = "Requestor";
                worksheet.Cells[1, 8].Value = "Requestor Phone";
                worksheet.Cells[1, 9].Value = "Request Type ID";
                worksheet.Cells[1, 10].Value = "Requested Date";
                worksheet.Cells[1, 11].Value = "Waiting Time";
                worksheet.Cells[1, 12].Value = "Request Status";
                worksheet.Cells[1, 13].Value = "Notes";
                worksheet.Cells[1, 14].Value = "Physician Name";

                // Populate the worksheet with table data
                for (int i = 0; i < requestData.Count; i++)
                {
                    worksheet.Cells[i + 3, 1].Value = requestData[i].Name;
                    worksheet.Cells[i + 3, 2].Value = requestData[i].Phone;
                    worksheet.Cells[i + 3, 3].Value = requestData[i].Email;
                    worksheet.Cells[i + 3, 4].Value = requestData[i].Address;
                    worksheet.Cells[i + 3, 5].Value = requestData[i].DateOfBirth;
                    worksheet.Cells[i + 3, 6].Value = requestData[i].RequestId;
                    worksheet.Cells[i + 3, 7].Value = requestData[i].Requestor;
                    worksheet.Cells[i + 3, 8].Value = requestData[i].Mobile;
                    worksheet.Cells[i + 3, 9].Value = requestData[i].RequestTypeId;
                    worksheet.Cells[i + 3, 10].Value = requestData[i].RequestDate;
                    worksheet.Cells[i + 3, 11].Value = "( " + requestData[i].totalHours + " " + requestData[i].totalMinutes + " " + requestData[i].totalSeconds + " )";
                    worksheet.Cells[i + 3, 12].Value = requestData[i].Status;
                    worksheet.Cells[i + 3, 13].Value = requestData[i].Notes;
                    worksheet.Cells[i + 3, 14].Value = requestData[i].Physician;
                }

                // Convert the Excel package to a byte array
                byte[] excelBytes = package.GetAsByteArray();

                // Return the Excel file as a download
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

       /// <summary>
       /// Get VieCase Page
       /// </summary>
       /// <param name="status"></param>
       /// <param name="requestid"></param>
       /// <param name="callId"></param>
       /// <returns></returns>
        public IActionResult ViewCaseRecords(int status, int requestid, int callId)
        {
            var viewCaseData = _iAdminDashRepo.getViewCaseData(requestid);

            AdminDashvm adminDashvm = new AdminDashvm()
            {
                StatusForName = status,
                viewCaseModel = viewCaseData,
                CallId = callId
            };
            return PartialView("Admin/Dashboard/_AdminViewCase", adminDashvm);
        }

        /// <summary>
        /// Update ViewCase Page
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateViewCaseRecords(AdminDashvm adminDashvm)
        {
            _iAdminDashRepo.setViewCaseData(adminDashvm.viewCaseModel, adminDashvm.viewCaseModel.RequestId);

            return PartialView("Admin/Dashboard/_AdminViewCase", adminDashvm);
        }

       /// <summary>
       /// Get ViewNotes Page
       /// </summary>
       /// <param name="requestid"></param>
       /// <param name="status"></param>
       /// <param name="callId"></param>
       /// <returns></returns>
        public IActionResult ViewNotes(int requestid, int status, int callId)
        {
            AdminDashvm adminDashvm = new AdminDashvm()

            {
                viewNotesModel = _iAdminDashRepo.getViewNotesData(requestid),
                StatusForName = status,
                CallId = callId

            };
            return PartialView("Admin/Dashboard/_AdminViewNotes", adminDashvm);
        }

        /// <summary>
        /// Update ViewNotes Page
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateViewNotes(AdminDashvm adminDashvm)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            _iAdminDashRepo.setViewNotesData(adminDashvm.viewNotesModel, adminDashvm.viewNotesModel.RequestId, aspId, adminDashvm.CallId);

            return Json(new { reqId = adminDashvm.viewNotesModel.RequestId, status = adminDashvm.StatusForName, callId = adminDashvm.CallId });
        }

        /// <summary>
        /// Get CancelCase Modal
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult CancelCase(int requestid, int status)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                cancelCaseModal = _iAdminDashRepo.getCancelCaseData(requestid),
                caseTags = _iAdminDashRepo.getCaseTags(),
                StatusForName = status,
            };
            return PartialView("Admin/Dashboard/_AdminCancelCase", adminDashvm);
        }
       
        /// <summary>
        /// Post CancelCase Modal
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CancelCase(AdminDashvm adminDashvm)
        {
            if (adminDashvm.cancelCaseModal.RequestId != 0)
            {
                _iAdminDashRepo.setCancelCaseData(adminDashvm.cancelCaseModal);
                return Ok();
            }
            return Json(new { Error = "error returned in else" });
        }

       /// <summary>
       /// Get Request Support Modal
       /// </summary>
       /// <returns></returns>
        public IActionResult RequestSupport()
        {
            return PartialView("Admin/Dashboard/_AdminRequestSupport");
        }

        /// <summary>
        /// Post Request Support Modal
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        public IActionResult RequestDTYPost(AdminDashvm adminDashvm)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");
            int adminId = _db.Admins.FirstOrDefault(_ => _.AspNetUserId == aspId).AdminId;

            if (_iAdminDashRepo.RequestSupportViaEmail(adminDashvm.RequestSupportMessage, adminId))
            {
                return Ok(true);
            }
            return Ok(false);
        }


        /// <summary>
        /// Get ChatWith Support Modal
        /// </summary>
        /// <returns></returns>
        public IActionResult ChatWith()
        {
            return PartialView("Admin/Dashboard/_ChatWith");
        }

        /// <summary>
        /// Get AssignCase Modal
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult AssignCase(int requestid, int status)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                assignCaseModel = _iAdminDashRepo.getAssignCaseData(requestid),
                regions = _iAdminDashRepo.getRegions(),
                physicians = _iAdminDashRepo.getPhysicians(0),
                StatusForName = status,

            };
            return PartialView("Admin/Dashboard/_AdminAssignCase", adminDashvm);
        }
        
        /// <summary>
        ///Get  FilterAssignCase Modal by regionwise Physicians
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult FilterAssignCase(int regionid)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                physicians = _iAdminDashRepo.getPhysicians(regionid)
            };
            return Json(new { success = adminDashvm.physicians });
        }

        /// <summary>
        /// Post AssignCase Modal
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AssignCase(AdminDashvm adminDashvm)
        {
            if (adminDashvm.assignCaseModel.RequestId != 0)
            {
                _iAdminDashRepo.setAssignCaseData(adminDashvm.assignCaseModel);

                return Ok();
            }
            return Json(new { Error = "error returned in else" });
        }

        /// <summary>
        /// Get BlockCase Modal
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult BlockCase(int requestid, int status)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                blockCaseModel = _iAdminDashRepo.getBlockCaseData(requestid),
                StatusForName = status,
            };
            return PartialView("Admin/Dashboard/_AdminBlockCase", adminDashvm);
        }

        /// <summary>
        /// Post BlockCase Modal
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BlockCase(AdminDashvm adminDashvm)
        {
            if (adminDashvm.blockCaseModel.RequestId != 0)
            {
                _iAdminDashRepo.setBlockCaseData(adminDashvm.blockCaseModel);

                return Ok();
            }

            return Json(new { Eroor = "error in update blockcase" });
        }

       /// <summary>
       /// Get ViewUploads Page
       /// </summary>
       /// <param name="requestid"></param>
       /// <param name="status"></param>
       /// <param name="callId"></param>
       /// <returns></returns>
        public IActionResult ViewUploads(int requestid, int status, int callId)
        {
            AdminViewUploadsvm adminViewUploadsvm = new AdminViewUploadsvm();

            adminViewUploadsvm = _iAdminDashRepo.getViewUploadsData(requestid);
            adminViewUploadsvm.viewUploadsList = _iAdminDashRepo.getViewUploadsList(requestid);
            adminViewUploadsvm.statusForName = status;
            adminViewUploadsvm.CallId = callId;
            return PartialView("Admin/Dashboard/_AdminViewUploads", adminViewUploadsvm);
        }

        /// <summary>
        /// Post ViewUploads Page
        /// </summary>
        /// <param name="adminViewUploadsvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadDocument(AdminViewUploadsvm adminViewUploadsvm)
        {
            _iAdminDashRepo.setViewUploadsData(adminViewUploadsvm);

            return Ok(adminViewUploadsvm.RequestId);
        }

        /// <summary>
        /// Delete File Action in ViewCase Page
        /// </summary>
        /// <param name="requestwisefileid"></param>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult DeleteFile(int requestwisefileid, int requestid, int status)
        {
            _iAdminDashRepo.DeleteFileData(requestwisefileid);

            return Json(new { reqId = requestid, status = status });
        }

        /// <summary>
        /// SendFile Method in ViewCase Page
        /// </summary>
        /// <param name="requestwisefileid"></param>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult SendFile(int[] requestwisefileid, int requestid, int status)
        {
            _iAdminDashRepo.sendEmailWithFile(requestwisefileid, requestid);

            return Json(new { reqId = requestid, status = status });
        }

       /// <summary>
       /// Get SendOrder Page
       /// </summary>
       /// <param name="requestid"></param>
       /// <param name="status"></param>
       /// <param name="callId"></param>
       /// <returns></returns>
        public IActionResult SendOrder(int requestid, int status, int callId)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                sendOrderModel = _iAdminDashRepo.getOrderData(requestid),
                healthProfessionalTypes = _iAdminDashRepo.getHealthProfessionalTypes(),
                healthProfessionals = _iAdminDashRepo.getHealthProfessionals(0),
                StatusForName = status,
                CallId = callId,
            };
            return PartialView("Admin/Dashboard/_SendOrder", adminDashvm);
        }

        /// <summary>
        /// Filter Data On SendOrder Page
        /// </summary>
        /// <param name="health_professional_id"></param>
        /// <returns></returns>
        public IActionResult FilterSendOrder(int health_professional_id)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                healthProfessionals = _iAdminDashRepo.getHealthProfessionals(health_professional_id)
            };
            return Json(new { success = adminDashvm.healthProfessionals });
        }

        /// <summary>
        /// Get Vendor Details On SendOrder Page Based On VenderId
        /// </summary>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        public IActionResult VendorData(int vendorid)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                sendOrderModel = _iAdminDashRepo.GetVendordata(vendorid),
            };
            return Json(new { success = adminDashvm.sendOrderModel });

        }

        /// <summary>
        /// Post SendOrder Action
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SendOrder(AdminDashvm adminDashvm)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            if (adminDashvm.sendOrderModel.RequestId != 0)
            {
                _iAdminDashRepo.setOrderData(adminDashvm.sendOrderModel, aspId);

                return Ok();
            }
            return Json(new { Error = "error returned in else" });

        }

        /// <summary>
        /// Get TransferCase Modal Action
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult TransferCase(int requestid, int status)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                transferCaseModel = _iAdminDashRepo.getTransferCaseData(requestid),
                regions = _iAdminDashRepo.getRegions(),
                physicians = _iAdminDashRepo.getPhysicians(0),
                StatusForName = status,
            };
            return PartialView("Admin/Dashboard/_AdminTransferCase", adminDashvm);
        }

        /// <summary>
        /// Get  FilterTransferCase Modal by regionwise Physicians
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public IActionResult FilterTransferCase(int regionid)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                physicians = _iAdminDashRepo.getPhysicians(regionid)
            };
            return Json(new { success = adminDashvm.physicians });
        }
        
        /// <summary>
        /// Post TransferCase Form Action
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TransferCase(AdminDashvm adminDashvm)
        {
            if (adminDashvm.transferCaseModel.RequestId != 0)
            {
                _iAdminDashRepo.setTransferCaseData(adminDashvm.transferCaseModel);

                return Ok();
            }
            return Json(new { Error = "error returned in else" });
        }

        /// <summary>
        /// Get ClearCase Modal Action
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult ClearCase(int requestid, int status)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                clearCaseModel = _iAdminDashRepo.getClearCaseData(requestid),
                StatusForName = status,
            };
            return PartialView("Admin/Dashboard/_AdminClearCase", adminDashvm);
        }

        /// <summary>
        /// Post ClearCase Form Action
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ClearCase(AdminDashvm adminDashvm)
        {
            if (adminDashvm.clearCaseModel.RequestId != 0)
            {
                _iAdminDashRepo.setClearCaseData(adminDashvm.clearCaseModel);

                return Ok();
            }
            return Json(new { Eroor = "error in update clearcase" });
        }

        /// <summary>
        /// Get SendAgreement Modal Action
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult SendAgreement(int requestid, int status, int callId)
        {
            AdminDashvm adminDashvm = new AdminDashvm()
            {
                sendAgreementModel = _iAdminDashRepo.getSendAgreementData(requestid),
                StatusForName = status,
                CallId = callId,
            };
            return PartialView("Admin/Dashboard/_AdminSendAgreement", adminDashvm);
        }

        /// <summary>
        /// Post SendAgreeMent Form Action
        /// </summary>
        /// <param name="sendAgreementModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult sendMailLink(SendAgreementModel sendAgreementModel)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            _iAdminDashRepo.sendAgreementMail(sendAgreementModel, aspId);


            return Ok();
        }

        /// <summary>
        /// Get Encounter Form Page Action
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult Encounter(int requestid, int status, int callId)
        {
            Encountervm EncounterData = new Encountervm();

            EncounterData = _iEncounterRepo.getEncounterFormData(requestid, status);
            EncounterData.CallId = callId;

            return PartialView("Admin/Dashboard/_Encounter", EncounterData);

        }

        /// <summary>
        /// Post Encounter Form Action
        /// </summary>
        /// <param name="updatedEncounterData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Encounter(Encountervm updatedEncounterData)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");


            _iEncounterRepo.setEncounterFormData(updatedEncounterData, aspId);

            return Json(new { reqId = updatedEncounterData.RequestId, status = updatedEncounterData.StatusForName, callid = updatedEncounterData.CallId });
        }

        /// <summary>
        /// Get CloseCase Page Action
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        //*******************************************************************Close case Page**********************************************************************************************************
        public IActionResult CloseCase(int requestid, int status)
        {

            AdminDashvm adminDashvm = new AdminDashvm()
            {
                closeCaseModel = _iAdminDashRepo.getCloseCaseData(requestid),
                closeCaseLists = _iAdminDashRepo.getCloseCaseList(requestid),
                StatusForName = status,
            };
            return PartialView("Admin/Dashboard/_AdminCloseCase", adminDashvm);
        }

        /// <summary>
        /// Update CloseCase Form Action
        /// </summary>
        /// <param name="adminDashvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateCloseCase(AdminDashvm adminDashvm)
        {
            _iAdminDashRepo.updateCloseCaseData(adminDashvm.closeCaseModel);

            return Json(new { reqId = adminDashvm.closeCaseModel.RequestId, status = adminDashvm.StatusForName });
        }

        /// <summary>
        /// Post CloseCase Form Action
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CloseCase(int requestid)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            _iAdminDashRepo.setCloseCaseData(requestid, aspId);
            return Ok();

        }

        /// <summary>
        /// Get AdminProfile Page Action
        /// </summary>
        /// <param name="aspId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult AdminProfile(string aspId, int callId)
        {
            string Id = HttpContext.Session.GetString("AspnetUserid");
            if (aspId == null)
            {
                aspId = Id;
            }

            AdminProfilevm adminProfilevm = _iAdminProfileRepo.getAdminProfileData(aspId);
            adminProfilevm.Roles = _iAdminProfileRepo.GetRoles();
            adminProfilevm.Regions = _iAdminDashRepo.getRegions();
            adminProfilevm.AdminRegions = _iAdminProfileRepo.GetAdminregions(aspId);
            adminProfilevm.callId = callId;

            return PartialView("Admin/Profile/_AdminProfile", adminProfilevm);
        }

        /// <summary>
        /// Update Maling Information On AdminProfile Page
        /// </summary>
        /// <param name="adminProfilevm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateMailingForm(AdminProfilevm adminProfilevm)
        {

            _iAdminProfileRepo.setAdminProfileData(adminProfilevm);
            return Ok(adminProfilevm.AspNetUserId);
        }

        /// <summary>
        /// Update Info On AdminProfile Page
        /// </summary>
        /// <param name="adminProfilevm"></param>
        /// <param name="regions"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateInfoForm(AdminProfilevm adminProfilevm, List<int> regions)
        {

            _iAdminProfileRepo.setAdminInfoData(adminProfilevm, regions);
            return Ok(adminProfilevm.AspNetUserId);
        }

        /// <summary>
        /// Update Password On AdminProfile Page
        /// </summary>
        /// <param name="adminProfilevm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdatePassword(AdminProfilevm adminProfilevm)
        {

            _iAdminProfileRepo.setPasswordData(adminProfilevm);
            return Ok(adminProfilevm.AspNetUserId);
        }

        /// <summary>
        /// Delete Admin Account
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteAdminAccount(int adminId)
        {
            _iAdminProfileRepo.RemoveAdmin(adminId);

            return Ok();
        }

       /// <summary>
       /// Provider Page
       /// </summary>
       /// <param name="regionId"></param>
       /// <returns></returns>
        public IActionResult Provider(int regionId)
        {

            ProviderDetailsvm providerDetailsvm = new ProviderDetailsvm();
            providerDetailsvm.Regions = _iProviderDetailsRepo.getRegions();
            providerDetailsvm.Providers = _iProviderDetailsRepo.GetProviders(regionId);

            return PartialView("Admin/Providers/Provider/_Provider", providerDetailsvm);

        }

        /// <summary>
        /// Stop Notification Feature By clicking on checkbox admin can't contact to provider
        /// </summary>
        /// <param name="physicianid"></param>
        /// <returns></returns>
        public IActionResult Notification(int physicianid)
        {
            if(_iProviderDetailsRepo.stopNotification(physicianid) == true)
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });

        }

       /// <summary>
       /// ContactProvider Modal On Provider Page
       /// </summary>
       /// <returns></returns>
        public IActionResult ContactProvider()
        {
            return Ok();
        }

       /// <summary>
       /// Set my to provider
       /// </summary>
       /// <param name="providerDetailsvm"></param>
       /// <returns></returns>
        public IActionResult SendEmailToProvider(ProviderDetailsvm providerDetailsvm)
        {
            String aspId = HttpContext.Session.GetString("AspnetUserid");

            _iProviderDetailsRepo.ContactProvider(providerDetailsvm, aspId);
            return Ok();
        }
        
        /// <summary>
        /// Send SMS to Provider
        /// </summary>
        /// <param name="providerDetailsvm"></param>
        /// <returns></returns>
        public IActionResult SendSMSToProvider(ProviderDetailsvm providerDetailsvm)
        {
            String aspId = HttpContext.Session.GetString("AspnetUserid");

            _iProviderDetailsRepo.ContactSMSProvider(providerDetailsvm, aspId);
            return Ok();
        }

        /// <summary>
        /// Edit Provider Page by clicking on Edit button  which is on Provider Page
        /// </summary>
        /// <param name="aspId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult GetEditProvider(string aspId, int callId)
        {
            ProviderProfilevm providerProfilevm = new ProviderProfilevm();
            providerProfilevm = _iProviderDetailsRepo.GetProviderProfileData(aspId);
            providerProfilevm.Regions = _iProviderDetailsRepo.getRegions();
            providerProfilevm.Roles = _iProviderDetailsRepo.GetRoles();
            providerProfilevm.PhysicianRegionTables = _iProviderDetailsRepo.GetPhysicianRegionTables(aspId);
            providerProfilevm.callId = callId;

            return PartialView("Admin/Providers/Provider/_ProviderEditProfile", providerProfilevm);

        }

        /// <summary>
        /// Get Payrate Page
        /// </summary>
        /// <param name="aspId"></param>
        /// <param name="PhysicianId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult Payrate(string aspId,int PhysicianId, int callId)
        {
            ProviderProfilevm providerProfilevm = new ProviderProfilevm();
            providerProfilevm.PhysicianId = PhysicianId;
            providerProfilevm.callId = callId;
            providerProfilevm.AspId = aspId;
            providerProfilevm.payrateData = _iProviderDetailsRepo.GetPayrateData(PhysicianId);

            return PartialView("Admin/Providers/Provider/_Payrate", providerProfilevm);

        }

        /// <summary>
        /// Post Pyrate PageData
        /// </summary>
        /// <param name="aspId"></param>
        /// <param name="PhysicianId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult SetPayrate(ProviderProfilevm providerProfilevm)
        {
            _iProviderDetailsRepo.SetPayrateData(providerProfilevm.payrateData);

            return Json(new { aspId = providerProfilevm.AspId, physicianId = providerProfilevm.payrateData.PhysicianId});
        }

        /// <summary>
        /// Update Provider Password Action
        /// </summary>
        /// <param name="providerProfilevm"></param>
        /// <returns></returns>
        public IActionResult PhysicianProfileResetPassword(ProviderProfilevm providerProfilevm)
        {
            if (providerProfilevm.Password != null)
            {
                _iProviderDetailsRepo.PhysicianResetPassword(providerProfilevm.Password, providerProfilevm.AspId);

                return Json(new { Success = true, aspId = providerProfilevm.AspId });
            }
            return Json(new { Success = false });
        }

        /// <summary>
        /// Update Provider Account Information Action
        /// </summary>
        /// <param name="providerProfilevm"></param>
        /// <returns></returns>
        public IActionResult PhysicianAccountEdit(ProviderProfilevm providerProfilevm)
        {
            _iProviderDetailsRepo.PhysicianAccountUpdate((short)providerProfilevm.Status, (int)providerProfilevm.RoleId, providerProfilevm.AspId, providerProfilevm.Username);

            return Ok(providerProfilevm.AspId);
        }

        /// <summary>
        /// Update Provider Administrative Info Action
        /// </summary>
        /// <param name="providerProfilevm"></param>
        /// <param name="physicianRegions"></param>
        /// <returns></returns>
        public IActionResult PhysicianAdministratorEdit(ProviderProfilevm providerProfilevm, List<int> physicianRegions)
        {
            _iProviderDetailsRepo.PhysicianAdministratorDataUpdate(providerProfilevm, physicianRegions);

            return Ok(providerProfilevm.AspId);
        }

        /// <summary>
        /// Update Provider Mailing Info Action
        /// </summary>
        /// <param name="providerProfilevm"></param>
        /// <returns></returns>
        public IActionResult PhysicianMailingEdit(ProviderProfilevm providerProfilevm)
        {
            _iProviderDetailsRepo.PhysicianMailingDataUpdate(providerProfilevm);

            return Ok(providerProfilevm.AspId);
        }

        /// <summary>
        /// Update Provider Business Info Edit
        /// </summary>
        /// <param name="providerProfilevm"></param>
        /// <returns></returns>
        public IActionResult PhysicianBusinessInfoEdit(ProviderProfilevm providerProfilevm)
        {
            _iProviderDetailsRepo.PhysicianBusinessInfoUpdate(providerProfilevm);

            return Ok(providerProfilevm.AspId);
        }

        /// <summary>
        /// Update Provider Onboarding Data
        /// </summary>
        /// <param name="providerProfilevm"></param>
        /// <returns></returns>
        public IActionResult UpdateOnBoarding(ProviderProfilevm providerProfilevm)
        {
            _iProviderDetailsRepo.EditOnBoardingData(providerProfilevm);

            return Ok(providerProfilevm.AspId);
        }

        /// <summary>
        /// Delete Provider Account
        /// </summary>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public IActionResult DeletePhysicianAccount(int physicianId)
        {
            _iProviderDetailsRepo.RemovePhysician(physicianId);

            return Ok();
        }

        /// <summary>
        /// Get Create Provider Account Page
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult CreateProviderAccount(int callId)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            ProviderProfilevm providerProfilevm = new ProviderProfilevm();
            providerProfilevm.Roles = _iProviderDetailsRepo.GetRoles();
            providerProfilevm.Regions = _iProviderDetailsRepo.getRegions();
            providerProfilevm.AspId = aspId;
            providerProfilevm.callId = callId;

            return PartialView("Admin/Providers/Provider/_CreateProviderAcc", providerProfilevm);
        }

        /// <summary>
        /// Post Creat Provider Account Form Action
        /// </summary>
        /// <param name="providerProfilevm"></param>
        /// <param name="physicianRegions"></param>
        /// <returns></returns>
        public IActionResult CreateProviderAccountPost(ProviderProfilevm providerProfilevm, List<int> physicianRegions)
        {
            if (_iProviderDetailsRepo.CreatePhysicianAccount(providerProfilevm, physicianRegions))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

       /// <summary>
       /// Get Account Access Page Action
       /// </summary>
       /// <returns></returns>
        public IActionResult GetAccountAccess()
        {
            AdminAccountAccessvm adminAccountAccessvm = new AdminAccountAccessvm();
            adminAccountAccessvm.AccountAccess = _iAdminDashRepo.GetAccountAccessData();

            return PartialView("Admin/Access/AccountAccess/_AdminAccountAccess", adminAccountAccessvm);
        }

        /// <summary>
        /// Get Create Access Page Action 
        /// </summary>
        /// <returns></returns>
        public IActionResult GetCreateAccess()
        {
            AdminAccountAccessvm adminAccountAccessvm = new AdminAccountAccessvm
            {
                Aspnetroles = _iAdminDashRepo.GetAccountType(),
                Menus = _iAdminDashRepo.GetMenu(0),
            };

            return PartialView("Admin/Access/AccountAccess/_AdminAccountAccessCreate", adminAccountAccessvm);
        }

        /// <summary>
        /// Filters RolesMenu on Create Access Page 
        /// </summary>
        /// <param name="accounttype"></param>
        /// <returns></returns>
        public IActionResult FilterRolesMenu(int accounttype)
        {
            var menu = _iAdminDashRepo.GetMenu(accounttype);

            return Json(menu);
        }

        /// <summary>
        /// Post Create Access Account Form Action
        /// </summary>
        /// <param name="adminAccountAccessvm"></param>
        /// <param name="AccountMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetCreateAccessAccount(AdminAccountAccessvm adminAccountAccessvm, List<int> AccountMenu)
        {
            _iAdminDashRepo.SetCreateAccessAccount(adminAccountAccessvm.CreateAccountAccess, AccountMenu);

            return Ok();
        }

        /// <summary>
        /// Get Edit Access Page Action 
        /// </summary>
        /// <param name="accounttypeid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public IActionResult GetEditAccess(int accounttypeid, int roleid)
        {
            var roledata = _iAdminDashRepo.GetEditAccessData(roleid);
            var Accounttype = _iAdminDashRepo.GetAccountType();
            var menu = _iAdminDashRepo.GetAccountMenu(accounttypeid, roleid);
            AdminAccountAccessvm adminAccountAccessvm = new AdminAccountAccessvm
            {
                Aspnetroles = Accounttype,
                AccountMenu = menu,
                CreateAccountAccess = roledata,
            };
            return PartialView("Admin/Access/AccountAccess/_AdminAccountAccessEdit", adminAccountAccessvm);
        }

        /// <summary>
        /// Filter RolesMenu on Edit Access Page 
        /// </summary>
        /// <param name="accounttypeid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public IActionResult FilterEditRolesMenu(int accounttypeid, int roleid)
        {
            var menu = _iAdminDashRepo.GetAccountMenu(accounttypeid, roleid);
            var htmlcontent = "";
            foreach (var obj in menu)
            {
                htmlcontent += $"<div class='form-check form-check-inline px-2 mx-3'><input class='form-check-input' {(obj.ExistsInTable ? "checked" : "")} name='AccountMenu' type='checkbox' id='{obj.menuid}' value='{obj.menuid}'/><label class='form-check-label' for='{obj.menuid}'>{obj.name}</label></div>";
            }
            return Content(htmlcontent);
        }

        /// <summary>
        /// Update Edit Access Page Data
        /// </summary>
        /// <param name="adminAccountAccessvm"></param>
        /// <param name="AccountMenu"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SetEditAccessAccount(AdminAccountAccessvm adminAccountAccessvm, List<int> AccountMenu)
        {
            _iAdminDashRepo.SetEditAccessAccount(adminAccountAccessvm.CreateAccountAccess, AccountMenu);

            return Ok();
        }

        /// <summary>
        /// Delete Account Access Action
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteAccountAccess(int roleid)
        {
            _iAdminDashRepo.DeleteAccountAccess(roleid);

            return Ok();
        }

        /// <summary>
        /// Get User Access Page
        /// </summary>
        /// <param name="accountTypeId"></param>
        /// <returns></returns>
        public IActionResult GetUserAccess(int accountTypeId)
        {
            AdminUserAccessvm adminUserAccessvm = new AdminUserAccessvm();
            adminUserAccessvm.Aspnetroles = _iAdminDashRepo.GetAccountType();
            adminUserAccessvm.UserAccesses = _iAdminDashRepo.GetUserAccessData(accountTypeId);

            return PartialView("Admin/Access/UserAccess/_AdminUserAccess", adminUserAccessvm);
        }

        /// <summary>
        /// Get Create Admin Account Page
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult GetCreateAdminAccount(int callId)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            AdminProfilevm adminProfilevm = new AdminProfilevm();
            adminProfilevm.Roles = _iAdminProfileRepo.GetRoles();
            adminProfilevm.Regions = _iAdminDashRepo.getRegions();
            adminProfilevm.AspNetUserId = aspId;
            adminProfilevm.callId = callId;

            return PartialView("Admin/Access/_AdminCreateAccount", adminProfilevm);
        }

        /// <summary>
        /// Post Data Of Create Admin Account Page
        /// </summary>
        /// <param name="adminProfilevm"></param>
        /// <param name="adminRegions"></param>
        /// <returns></returns>
        public IActionResult CreateAdminAccountPost(AdminProfilevm adminProfilevm, List<int> adminRegions)
        {
            if (_iAdminProfileRepo.CreateAdminAccount(adminProfilevm, adminRegions))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        /// <summary>
        /// Get Scheduling Page Data
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult GetScheduling(int callId)
        {
            Schedulingvm schedulingvm = new Schedulingvm()
            {
                regions = _iAdminDashRepo.getRegions(),
                callId = callId,

            };
            return PartialView("Admin/Providers/Scheduling/_AdminScheduling", schedulingvm);
        }

        /// <summary>
        /// Get AddShift Modal 
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult CreateNewShift(int callId)
        {
            Schedulingvm schedulingvm = new Schedulingvm();
            if (callId == 3)
            {
                string aspId = HttpContext.Session.GetString("AspnetUserid");
                Physician? physican = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId);
                var physicianId = physican.PhysicianId;

                schedulingvm.regions = _iAdminDashRepo.GetPhysicianRegions(physicianId);
                schedulingvm.callId = callId;
                schedulingvm.physicianId = physicianId;
            }
            else
            {
                schedulingvm.regions = _iAdminDashRepo.getRegions();
            }

            return PartialView("Admin/Providers/Scheduling/_CreateShift", schedulingvm);
        }

        /// <summary>
        /// Post Data Of AddShift Modal
        /// </summary>
        /// <param name="schedulingvm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult createShiftPost(Schedulingvm schedulingvm)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");


            if (_iSchedulingRepo.createShift(schedulingvm.ScheduleModel, aspId))
            {
                return Ok(true);
            }

            return Ok(false);
        }

        /// <summary>
        /// show shift daywise,weekwise,monthwise  based on click of day,week or month button click
        /// </summary>
        /// <param name="datestring"></param>
        /// <param name="sundaystring"></param>
        /// <param name="saturdaystring"></param>
        /// <param name="shifttype"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult loadshift(string datestring, string sundaystring, string saturdaystring, string shifttype, int regionid)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            DateTime date = DateTime.Parse(datestring);
            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);
            
            switch (shifttype)
            {
                case "month":
                    MonthShiftModal monthShift = new MonthShiftModal();

                    var totalDays = DateTime.DaysInMonth(date.Year, date.Month);
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var startDayIndex = (int)firstDayOfMonth.DayOfWeek;

                    var dayceiling = (int)Math.Ceiling((float)(totalDays + startDayIndex) / 7);

                    monthShift.daysLoop = (int)dayceiling * 7;
                    monthShift.daysInMonth = totalDays;
                    monthShift.firstDayOfMonth = firstDayOfMonth;
                    monthShift.startDayIndex = startDayIndex;
                    monthShift.Physicians = _iAdminDashRepo.getPhysicians(regionid);
                    if (regionid == 0)
                    {
                        monthShift.shiftDetailsmodals = _iSchedulingRepo.ShiftDetailsmodal(date, sunday, saturday, "month", aspId);
                    }
                    else
                    {
                        monthShift.shiftDetailsmodals = _iSchedulingRepo.ShiftDetailsmodal(date, sunday, saturday, "month", aspId).Where(i => i.Regionid == regionid).ToList();
                    }

                    return PartialView("Admin/Providers/Scheduling/_MonthWiseShift", monthShift);

                case "week":

                    WeekShiftModal weekShift = new WeekShiftModal();
                    weekShift.Physicians = regionid == 0 ? _iAdminDashRepo.GetPhysicians() : _iAdminDashRepo.getPhysicians(regionid);
                    weekShift.shiftDetailsmodals = _iSchedulingRepo.ShiftDetailsmodal(date, sunday, saturday, "week", aspId);

                    List<int> dlist = new List<int>();

                    for (var i = 0; i < 7; i++)
                    {
                        var date12 = sunday.AddDays(i);
                        dlist.Add(date12.Day);
                    }

                    weekShift.datelist = dlist.ToList();
                    weekShift.dayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

                    return PartialView("Admin/Providers/Scheduling/_WeekWiseShift", weekShift);

                case "day":

                    DayShiftModal dayShift = new DayShiftModal();
                    dayShift.Physicians = regionid == 0 ? dayShift.Physicians = _iAdminDashRepo.GetPhysicians() : dayShift.Physicians = _iAdminDashRepo.getPhysicians(regionid);
                    dayShift.shiftDetailsmodals = _iSchedulingRepo.ShiftDetailsmodal(date, sunday, saturday, "day", aspId);

                    return PartialView("Admin/Providers/Scheduling/_DayWiseShift", dayShift);

                default:
                    return PartialView();
            }

        }

        /// <summary>
        /// Get View Shift Or MoreShift Model Basd on button click on MontShift View
        /// </summary>
        /// <param name="viewShiftModal"></param>
        /// <returns></returns>
        public IActionResult OpenScheduledModalMonth(ViewShiftModal viewShiftModal)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");
            switch (viewShiftModal.actionType)
            {
                case "shiftdetails":
                    ShiftDetailsmodal shift = _iSchedulingRepo.GetShift(viewShiftModal.shiftdetailsid);
                    return PartialView("Admin/Providers/Scheduling/_ViewShift", shift);

                case "moreshifts":
                    DateTime date = DateTime.Parse(viewShiftModal.datestring);
                    ShiftDetailsmodal ScheduleModel = new ShiftDetailsmodal();
                    var list = ScheduleModel.ViewAllList = _iSchedulingRepo.ShiftDetailsmodal(date, DateTime.Now, DateTime.Now, "month", aspId).Where(i => i.Shiftdate.Day == viewShiftModal.columnDate.Day).ToList();
                    ViewBag.TotalShift = list.Count();
                    return PartialView("Admin/Providers/Scheduling/_MoreShift", ScheduleModel);


                default:

                    return PartialView();
            }
        }

        /// <summary>
        /// Get View Shift Or MoreShift Model Basd on button click on MontShift View
        /// </summary>
        /// <param name="sundaystring"></param>
        /// <param name="saturdaystring"></param>
        /// <param name="datestring"></param>
        /// <param name="shiftdate"></param>
        /// <param name="physicianid"></param>
        /// <returns></returns>
        public IActionResult OpenScheduledModalWeek(string sundaystring, string saturdaystring, string datestring, DateTime shiftdate, int physicianid)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");
            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);

            DateTime date1 = DateTime.Parse(datestring);
            ShiftDetailsmodal ScheduleModel = new ShiftDetailsmodal();
            var list = ScheduleModel.ViewAllList = _iSchedulingRepo.ShiftDetailsmodal(date1, sunday, saturday, "week", aspId).Where(i => i.Shiftdate.Day == shiftdate.Day && i.Physicianid == physicianid).ToList();
            ViewBag.TotalShift = list.Count();
            return PartialView("Admin/Providers/Scheduling/_MoreShift", ScheduleModel);
        }

        /// <summary>
        /// Approve oR DisApprove Shifts
        /// </summary>
        /// <param name="status"></param>
        /// <param name="shiftdetailid"></param>
        /// <returns></returns>
        public IActionResult ReturnShift(int status, int shiftdetailid)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");


            _iSchedulingRepo.SetReturnShift(status, shiftdetailid, aspId);
            return Ok();
        }

        /// <summary>
        /// Delete Shifts
        /// </summary>
        /// <param name="shiftdetailid"></param>
        /// <returns></returns>
        public IActionResult deleteShift(int shiftdetailid)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            _iSchedulingRepo.SetDeleteShift(shiftdetailid, aspId);
            return Ok();
        }

        /// <summary>
        /// Update ViewShift Modal 
        /// </summary>
        /// <param name="shiftDetailsmodal"></param>
        /// <returns></returns>
        public IActionResult EditShiftDetails(ShiftDetailsmodal shiftDetailsmodal)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            _iSchedulingRepo.SetEditShift(shiftDetailsmodal, aspId);
            return Ok();
        }

        /// <summary>
        /// get Shift Review Page
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        public IActionResult ShiftReview(int regionId, int callId)
        {
            Schedulingvm schedulingvm = new Schedulingvm()
            {
                regions = _iAdminDashRepo.getRegions(),
                ShiftReview = _iSchedulingRepo.GetShiftReview(regionId, callId),
                regionId = regionId,
                callId = callId,
            };

            return PartialView("Admin/Providers/Scheduling/_ShiftReview", schedulingvm);
        }
        /// <summary>
        /// Aprove Selected Shifts
        /// </summary>
        /// <param name="shiftDetailsId"></param>
        /// <returns></returns>
        public IActionResult ApproveShift(int[] shiftDetailsId)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            _iSchedulingRepo.ApproveSelectedShift(shiftDetailsId, aspId);

            return Ok();
        }

        /// <summary>
        /// Delete Selected Shifts
        /// </summary>
        /// <param name="shiftDetailsId"></param>
        /// <returns></returns>
        public IActionResult DeleteSelectedShift(int[] shiftDetailsId)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");


            _iSchedulingRepo.DeleteShiftReview(shiftDetailsId, aspId);

            return Ok();
        }

        /// <summary>
        /// Get MDOnCall Page Action 
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public IActionResult MDOnCall(int regionId)
        {

            var onCallModal = _iSchedulingRepo.GetOnCallDetails(regionId);

            return PartialView("Admin/Providers/Scheduling/_MDsOnCall", onCallModal);
        }

        /// <summary>
        /// Get Provider Location Page Action
        /// </summary>
        /// <returns></returns>
        public IActionResult ProviderLocation()
        {
            return PartialView("Admin/ProviderLocation/_ProviderLocation");
        }

        /// <summary>
        /// get provider data like longitude and latitude from provider Location Table
        /// </summary>
        /// <returns></returns>
        public IActionResult GetLocations()
        {
            List<PhysicianLocation> getLocation = _iSchedulingRepo.GetPhysicianlocations();
            return Ok(getLocation);
        }

        /// <summary>
        /// Get Partners Page
        /// </summary>
        /// <param name="professionid"></param>
        /// <returns></returns>
        public IActionResult Partners(int professionid)
        {
            Partnersvm partnersvm = new Partnersvm
            {
                Partnersdata = _iPartnersRepo.GetPartnersdata(professionid),
                Professions = _iPartnersRepo.GetProfession(),
            };
            return PartialView("Admin/Partners/_Partners", partnersvm);
        }

        /// <summary>
        /// Get AddBusiness Page
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public IActionResult AddBusiness(int vendorID)
        {
            if (vendorID == 0)
            {
                Partnersvm partnersvm = new Partnersvm
                {
                    Professions = _iPartnersRepo.GetProfession(),
                    regions = _iAdminDashRepo.getRegions(),
                    vendorID = vendorID,
                };
                return PartialView("Admin/Partners/_PartnersCreateEdit", partnersvm);
            }
            else
            {
                Partnersvm partnersvm = _iPartnersRepo.GetEditBusinessData(vendorID);
                partnersvm.Professions = _iPartnersRepo.GetProfession();
                partnersvm.regions = _iAdminDashRepo.getRegions();
                partnersvm.vendorID = vendorID;
                return PartialView("Admin/Partners/_PartnersCreateEdit", partnersvm);
            }

        }

        /// <summary>
        /// Create New Business Page
        /// </summary>
        /// <param name="partnersvm"></param>
        /// <returns></returns>
        public IActionResult CreateNewBusiness(Partnersvm partnersvm)
        {
            string aspId = HttpContext.Session.GetString("AspnetUserid");

            if (_iPartnersRepo.CreateNewBusiness(partnersvm, aspId))
            {
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        /// <summary>
        /// Update AddBusiness Page
        /// </summary>
        /// <param name="partnersvm"></param>
        /// <returns></returns>
        public IActionResult UpdateBusiness(Partnersvm partnersvm)
        {
            if (_iPartnersRepo.UpdateBusiness(partnersvm))
            {
                return Json(new { Success = true, vendorid = partnersvm.vendorID });
            }
            return Json(new { Success = false, vendorid = partnersvm.vendorID });
        }

        /// <summary>
        /// Delete Vendors
        /// </summary>
        /// <param name="VendorID"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DelPartner(int VendorID)
        {
            _iPartnersRepo.DelPartner(VendorID);
            return Ok();
        }

        /// <summary>
        /// Get Patient History Page
        /// </summary>
        /// <param name="getRecordsvm"></param>
        /// <returns></returns>
        public IActionResult GetRecordsTab(GetRecordsvm getRecordsvm)
        {
            getRecordsvm = _iRecordsRepo.GetRecordsTab(0, getRecordsvm);

            return PartialView("Admin/Records/_GetRecordsTab", getRecordsvm);
        } 

        /// <summary>
        /// Get Patient Record Explore Page
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public IActionResult GetPatientRecordExplore(int UserId)
        {
            GetRecordsvm model = new GetRecordsvm();
            model = _iRecordsRepo.GetRecordsTab(UserId, model);
            return PartialView("Admin/Records/_ExploreRecords", model);
        }

       /// <summary>
       /// Get Search Records Page
       /// </summary>
       /// <param name="searchRecordsvm"></param>
       /// <returns></returns>
        public IActionResult GetSearchRecords(SearchRecordsvm searchRecordsvm)
        {

            searchRecordsvm = _iRecordsRepo.GetSearchRecords(searchRecordsvm);
            searchRecordsvm.requestType = _iRecordsRepo.GetRequesttypes();

            return PartialView("Admin/Records/_SearchRecords", searchRecordsvm);
        }

        /// <summary>
        /// download SearchRecords Page Data on Excel
        /// </summary>
        /// <param name="searchRecordsvm"></param>
        /// <returns></returns>
        public IActionResult ExportSearchRecords(SearchRecordsvm searchRecordsvm)
        {
            var requests = _iRecordsRepo.GetSearchRecords(searchRecordsvm);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("SearchRecords");

                // Add headers
                worksheet.Cells[1, 1].Value = "Request ID";
                worksheet.Cells[1, 2].Value = "Patient Name";
                worksheet.Cells[1, 3].Value = "Requestor";
                worksheet.Cells[1, 4].Value = "Date of Service";
                worksheet.Cells[1, 5].Value = "Close Case Date";
                worksheet.Cells[1, 6].Value = "Email";
                worksheet.Cells[1, 7].Value = "Contact";
                worksheet.Cells[1, 8].Value = "Address";
                worksheet.Cells[1, 9].Value = "Zip";
                worksheet.Cells[1, 10].Value = "Status";
                worksheet.Cells[1, 11].Value = "Physician";
                worksheet.Cells[1, 12].Value = "Physician Note";
                worksheet.Cells[1, 13].Value = "Provider Note";
                worksheet.Cells[1, 14].Value = "Admin Note";
                worksheet.Cells[1, 15].Value = "Patient Note";
                worksheet.Cells[1, 16].Value = "Request Type ID";
                worksheet.Cells[1, 17].Value = "User ID";

                // Populate data
                for (int i = 0; i < requests.requestList.Count; i++)
                {
                    var rowData = requests.requestList[i];
                    worksheet.Cells[i + 2, 1].Value = rowData.requestid;
                    worksheet.Cells[i + 2, 2].Value = rowData.patientname;
                    worksheet.Cells[i + 2, 3].Value = rowData.requestor;
                    worksheet.Cells[i + 2, 4].Value = rowData.dateOfService;
                    worksheet.Cells[i + 2, 5].Value = rowData.closeCaseDate;
                    worksheet.Cells[i + 2, 6].Value = rowData.email;
                    worksheet.Cells[i + 2, 7].Value = rowData.contact;
                    worksheet.Cells[i + 2, 8].Value = rowData.address;
                    worksheet.Cells[i + 2, 9].Value = rowData.zip;
                    worksheet.Cells[i + 2, 10].Value = rowData.status;
                    worksheet.Cells[i + 2, 11].Value = rowData.physician;
                    worksheet.Cells[i + 2, 12].Value = rowData.physicianNote;
                    worksheet.Cells[i + 2, 13].Value = rowData.providerNote;
                    worksheet.Cells[i + 2, 14].Value = rowData.AdminNote;
                    worksheet.Cells[i + 2, 15].Value = rowData.pateintNote;
                    worksheet.Cells[i + 2, 16].Value = rowData.requestTypeId;
                    worksheet.Cells[i + 2, 17].Value = rowData.userid;
                }

                // Converting Excel package to a byte array
                byte[] excelBytes = excelPackage.GetAsByteArray();

                //  Excel file for download
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }

        }

        /// <summary>
        /// Delete Request 
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IActionResult deletRequest(int requestId)
        {
            _iRecordsRepo.deletRequest(requestId);
            return Ok();
        }

       /// <summary>
       /// Get EmilLog Page Or SMSLOG Page Based On tempId
       /// </summary>
       /// <param name="tempId"></param>
       /// <returns></returns>
        public IActionResult GetEmailSmsLog(int tempId)
        {
            EmailSmsLogvm model = new EmailSmsLogvm();
            model = _iRecordsRepo.GetEmailSmsLog(tempId, model);

            return PartialView("Admin/Records/_EmailSmsLog", model);
        }

        /// <summary>
        /// Filter data of EmailLog and SMSLog Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult emailSmsLogFilter(EmailSmsLogvm model)
        {
            model = _iRecordsRepo.GetEmailSmsLog((int)model.tempid, model);
            return PartialView("Admin/Records/_EmailSmsLog", model);
        }

        /// <summary>
        /// Get Block Request Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult GetBlockedRequest(BlockRequestvm model)
        {

            model = _iRecordsRepo.GetBlockedRequest(model);

            return PartialView("Admin/Records/_BlockedHistory", model);
        }

        /// <summary>
        /// Unblock Request
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IActionResult UnblockRequest(int requestId)
        {
            _iRecordsRepo.UnblockRequest(requestId);
            return Ok();
        }

        /// <summary>
        /// Generate Encounter Form Pdfs
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
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

        #region Invoicing

        public IActionResult GetAdminInvoicing()
        {
            AdminInvoicingvm adminInvoicingvm = new AdminInvoicingvm()
            {
                PhysiciansList = _iAdminDashRepo.GetPhysicians(),
            };

            return PartialView("Admin/Providers/Invoicing/_Invoicing", adminInvoicingvm);
        }

        public IActionResult GetTimeSheetDetail(int phyid, string dateSelected)
        {
            AdminInvoicingvm adminInvoicingvm = new AdminInvoicingvm
            {
                PhysiciansList = _iAdminDashRepo.GetPhysicians(),
                TimesheetsList = _iAdminDashRepo.GetTimeSheetDetail(phyid, dateSelected),
                Timesheetdetails = _iProviderDashRepo.GetTimeSheetDetails(phyid, dateSelected),
                Timesheetdetailreimbursements = _iProviderDashRepo.GetTimeSheetDetailsReimbursements(phyid, dateSelected)          
                
            };
            if(phyid != 0)
            {
                adminInvoicingvm.PhysicianName = "Dr. " + _db.Physicians.FirstOrDefault(_ => _.PhysicianId == phyid).FirstName + " " + _db.Physicians.FirstOrDefault(_ => _.PhysicianId == phyid).LastName + " has not finalized the timesheet in specified time period.";
            }
            else
            {
                adminInvoicingvm.PhysicianName = "Please Select Provider First!";
            }

            return PartialView("Admin/Providers/Invoicing/_Invoicing", adminInvoicingvm);
        }

        public IActionResult GetAdminFinalizeTimeSheet(string dateSelected, int phyid)
        {;
            ProviderInvoicingvm? providerInvoicingvm = new ProviderInvoicingvm()
            {
                ProviderTimesheetDetails = _iProviderDashRepo.GetFinalizeTimeSheetDetails(phyid, dateSelected),
                CallId = 1,
                physicianPayrates = _iAdminDashRepo.GetPayRateForProviderByPhyId(phyid),
            };


            return PartialView("Provider/Invoicing/_Provider_FinalizeTimeSheet", providerInvoicingvm);
        }

        public IActionResult ConfirmApproveTimeSheet(int timeSheetId, int bonus, string notes)
        {
            if (_iAdminDashRepo.ApproveTimeSheet(timeSheetId, bonus, notes))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        #endregion

    }
}