using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.ViewModels;
using HelloDoc.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using System.Collections;


namespace HelloDoc.BAL.Repo
{
    public class AdminDashRepo : IAdminDashRepo
    {
        private readonly ApplicationDbContext _db;
        public readonly IHostingEnvironment _environment;
        public readonly ISecurityRepo _ISecurityRepo;

        public AdminDashRepo(ApplicationDbContext db, IHostingEnvironment environment, ISecurityRepo iSecurityRepo)
        {
            _db = db;
            _environment = environment;
            _ISecurityRepo = iSecurityRepo;
        }

        /// <summary>
        /// AdminDashboard Table Fetch Records
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="requesttypeid"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public List<RequestListAdminDash> getRequestData(int[] Status, string requesttypeid, int regionid)
        {
            var requestList = _db.Requests.Where(i => Status.Contains(i.Status));

            if (requesttypeid != null)
            {
                requestList = requestList.Where((i => i.RequestTypeId.ToString() == requesttypeid));
            }

            if (regionid != 0)
            {
                requestList = requestList.Where(i => i.RequestClients.Select(rc => rc.RegionId.ToString()).Contains(regionid.ToString()));
            }
            var GetRequestData = requestList.Select(r => new RequestListAdminDash()
            {
                Name = r.RequestClients.Select(x => x.FirstName).First() + " " + r.RequestClients.Select(x => x.LastName).First(),
                Requestor = r.FirstName,
                Phone = r.RequestClients.Select(x => x.PhoneNumber).First(),
                Address = r.RequestClients.Select(x => x.Street).First() + " " + r.RequestClients.Select(x => x.City).First() + " " + r.RequestClients.Select(x => x.State).First(),
                Notes = r.RequestStatusLogs.Where(x => x.TransToPhysicianId != null).First() == null ? "-" : "Admin transferred to" + r.RequestStatusLogs.Where(x => x.TransToPhysicianId != null).First().TransToPhysician.FirstName + " " + r.RequestStatusLogs.Where(x => x.TransToPhysicianId != null).First().TransToPhysician.LastName + " on " + r.RequestStatusLogs.Where(x => x.TransToPhysicianId != null).First().CreatedDate.ToShortDateString() + " at " + r.RequestStatusLogs.Where(x => x.TransToPhysicianId != null).First().CreatedDate.ToShortTimeString() + " : " + r.RequestStatusLogs.Where(x => x.TransToPhysicianId != null).First().Notes,
                Status = r.Status,
                ChatWith = r.PhysicianId.ToString(),
                Physician = r.Physician == null ? "-" : r.Physician.FirstName ,
                DateOfBirth = new DateTime(r.RequestClients.Select(x => Convert.ToInt32(x.IntYear)).First(), r.RequestClients.Select(x => Convert.ToInt32(x.StrMonth)).First(), r.RequestClients.Select(x => Convert.ToInt32(x.IntDate)).First()),
                RequestTypeId = r.RequestTypeId,
                Email = r.RequestClients.Select(x => x.Email).First(),
                RequestId = r.RequestId,
                RequestDate = r.CreatedDate,
                totalHours = (int)(DateTime.Now - r.CreatedDate).TotalMinutes / 60,
                totalMinutes = (int)(DateTime.Now - r.CreatedDate).TotalMinutes % 60,
                totalSeconds = (int)(DateTime.Now - r.CreatedDate).TotalSeconds % 60,
                callType = r.CallType == null ? 0 : (int)r.CallType,
                isFinalized = r.EncounterForms.Select(x => x.IsFinalize).First() == true,
                RegionName = r.RequestClients.Select(i => i.Region).FirstOrDefault(x => x.RegionId == r.RequestClients.Select(i => i.RegionId).First()) == null ? "-" :  r.RequestClients.Select(i => i.Region).FirstOrDefault(x => x.RegionId == r.RequestClients.Select(i => i.RegionId).First()).Name
            })
            .OrderByDescending(r => r.RequestDate)
            .ToList();
            return GetRequestData;
        }

        /// <summary>
        /// ViewCase Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public ViewCaseModel getViewCaseData(int requestid)
        {
            var casedata = _db.RequestClients.Include(x => x.Request).FirstOrDefault(i => i.RequestId == requestid);
            var requestList = _db.Requests.Where(i => i.RequestId == requestid);
            var BirthYear = Convert.ToInt32(casedata.IntYear);
            var BirthMonth = Convert.ToInt32(casedata.StrMonth);
            var BirthDate = Convert.ToInt32(casedata.IntDate);

            ViewCaseModel viewCaseModel = new ViewCaseModel
            {
                ConformNum = requestList.Select(x => x.ConfirmationNumber).First() == null ? null : requestList.Select(x => x.ConfirmationNumber).First(),
                PatientNotes = casedata.Notes,
                FirstName = casedata.FirstName,
                LastName = casedata.LastName,
                BirthDate = new DateTime(BirthYear, BirthMonth, BirthDate),
                Email = casedata.Email,
                PhoneNumber = casedata.PhoneNumber,
                Region = casedata.State,
                BusinessAddress = casedata.Street + ", " + casedata.City + ", " + casedata.State,
                Room = casedata.Location,
                RequestTypeId = requestList.Select(x => x.RequestTypeId).First(),
                RequestId = requestid,
                UserId = casedata.Request.UserId,
            };
            return viewCaseModel;
        }

        /// <summary>
        /// ViewCase Update Records
        /// </summary>
        /// <param name="updatedViewCaseData"></param>
        /// <param name="requestid"></param>
        public void setViewCaseData(ViewCaseModel updatedViewCaseData, int requestid)
        {
            var casedata = _db.RequestClients.FirstOrDefault(x => x.RequestId == requestid);
            if (casedata != null)
            {
                casedata.PhoneNumber = updatedViewCaseData.PhoneNumber;
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// RegionTable Fetch Records
        /// </summary>
        /// <returns></returns>
        public List<Region> getRegions()
        {
            var regions = _db.Regions.ToList();
            return regions;
        }
        
        /// <summary>
        /// PhysicianTable Fetch Records
        /// </summary>
        /// <param name="physicianId"></param>
        /// <returns></returns>
        public List<Region> GetPhysicianRegions(int physicianId)
        {
            var regions = _db.Regions.Include(p => p.PhysicianRegions).Where(p => p.PhysicianRegions.Any(x => x.PhysicianId == physicianId)).ToList();
            return regions;
        }

        /// <summary>
        /// CountRequest As Per Status Fetch Records
        /// </summary>
        /// <param name="adminDashvm"></param>
        public void countRequest(AdminDashvm adminDashvm)
        {
            adminDashvm.NewRequest = _db.Requests.Where(x => x.Status == 1).Count();
            adminDashvm.PendingRequest = _db.Requests.Where(x => x.Status == 2).Count();
            adminDashvm.ActiveRequest = _db.Requests.Where(x => x.Status == 4 || x.Status == 5).Count();
            adminDashvm.ConcludeRequest = _db.Requests.Where(x => x.Status == 6).Count();
            adminDashvm.ToCloseRequest = _db.Requests.Where(x => x.Status == 3 || x.Status == 7 || x.Status == 8).Count();
            adminDashvm.UnpaidRequest = _db.Requests.Where(x => x.Status == 9).Count();
        }
       
        /// <summary>
        /// ViewNotes Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public ViewNotesModel getViewNotesData(int requestid)
        {
            var noteData = _db.RequestNotes.FirstOrDefault(i => i.RequestId == requestid);
            var requestlogData = _db.RequestStatusLogs.Include(r => r.TransToPhysician).Where(i => i.RequestId == requestid).ToList();

            ViewNotesModel viewNotesModel = new ViewNotesModel()
            {
                RequestId = requestid,
                AdminNotes = noteData != null ? noteData.AdminNotes : null,
                PhysicianNotes = noteData != null ? noteData.PhysicianNotes : null,
                TransferNotes = requestlogData != null ? requestlogData.Where(i => i.TransToPhysicianId.HasValue).Select(i => "Admin transferred to " + i.TransToPhysician.FirstName + " "  + i.TransToPhysician.LastName  + " on " + i.CreatedDate.ToShortDateString() + " at " + i.CreatedDate.ToShortTimeString() + " " + i.Notes).ToList() : null,
            };
            return viewNotesModel;
        }

        /// <summary>
        /// ViewNotes Update Records
        /// </summary>
        /// <param name="updateViewNotesData"></param>
        /// <param name="requestid"></param>
        /// <param name="aspId"></param>
        /// <param name="callId"></param>
        public void setViewNotesData(ViewNotesModel updateViewNotesData, int requestid, string aspId,int callId)
        {
            var noteData = _db.RequestNotes.FirstOrDefault(i => i.RequestId == requestid);

            if (noteData != null)
            {
                noteData.RequestId = requestid;
                if(callId == 3)
                {
                    noteData.PhysicianNotes = updateViewNotesData.PhysicianNotes;
                }
                else
                {
                    noteData.AdminNotes = updateViewNotesData.AdminNotes;
                }          
                noteData.ModifiedDate = DateTime.Now;
                noteData.ModifiedBy = aspId;
                _db.SaveChanges();
            }
            else
            {
                var newnoteData = new RequestNote()
                {
                    RequestId = requestid,
                    CreatedBy = aspId,
                    CreatedDate = DateTime.Now,
                };
                if(callId == 3)
                {
                    newnoteData.PhysicianNotes = updateViewNotesData.PhysicianNotes;
                }
                else
                {
                    newnoteData.AdminNotes = updateViewNotesData.AdminNotes;
                }
                _db.RequestNotes.Add(newnoteData);
                _db.SaveChanges();
            }
        }
        
        /// <summary>
        /// CasTagTable Fetch Records
        /// </summary>
        /// <returns></returns>
        public List<CaseTag> getCaseTags()
        {
            var reasons = _db.CaseTags.ToList();
            return reasons;
        }
       
        /// <summary>
        /// CancelCase Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public CancelCaseModal getCancelCaseData(int requestid)
        {
            var cancelCase = _db.RequestClients.FirstOrDefault(i => i.RequestId == requestid);
            var status = _db.Requests.FirstOrDefault(i => i.RequestId == requestid);

            var cancelData = new CancelCaseModal()
            {
                RequestId = requestid,
                Name = cancelCase.FirstName + " " + cancelCase.LastName,
            };

            return cancelData;
        }

        /// <summary>
        /// CancelCase Update Records
        /// </summary>
        /// <param name="updatedCancelCaseData"></param>
        public void setCancelCaseData(CancelCaseModal updatedCancelCaseData)
        {
            var requestdata = _db.Requests.FirstOrDefault(i => i.RequestId == updatedCancelCaseData.RequestId);

            var addCancelData = new RequestStatusLog()
            {
                RequestId = requestdata.RequestId,
                Notes = updatedCancelCaseData.AditionalNotes,
                Status = 3,
                CreatedDate = DateTime.Now,
            };

            requestdata.Status = 3;
            requestdata.CaseTag = updatedCancelCaseData.CasetagId.ToString();

            _db.RequestStatusLogs.Add(addCancelData);
            _db.SaveChanges();
        }
      
        /// <summary>
        /// AssignCase Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public AssignCaseModel getAssignCaseData(int requestid)
        {
            var assigndata = new AssignCaseModel()
            {
                RequestId = requestid,
            };
            return assigndata;
        }

        /// <summary>
        /// AssignCase Update Records
        /// </summary>
        /// <param name="updatedAssignCaseData"></param>
        public void setAssignCaseData(AssignCaseModel updatedAssignCaseData)
        {
            var request = _db.Requests.FirstOrDefault(i => i.RequestId == updatedAssignCaseData.RequestId);

            var addAssignData = new RequestStatusLog()
            {
                RequestId = request.RequestId,
                Status = 2,
                Notes = updatedAssignCaseData.AssignNotes,
                CreatedDate = DateTime.Now,
                PhysicianId = updatedAssignCaseData.PhysicianId,
            };
            request.PhysicianId = updatedAssignCaseData.PhysicianId;
            request.Status = 1;
            _db.RequestStatusLogs.Add(addAssignData);
            _db.SaveChanges();
        }

        /// <summary>
        /// PhysicianTable Fetch Records 
        /// </summary>
        /// <returns></returns>
        public List<Physician> GetPhysicians()
        {
            var physicians = _db.Physicians.Where(i => i.IsDeleted == null).ToList();
            return physicians;
        }

        /// <summary>
        /// PhysicianTable Fetch Records Base On Reasons 
        /// </summary>
        /// <param name="regionid"></param>
        /// <returns></returns>
        public List<Physician> getPhysicians(int regionid)
        {
            var physicians = _db.Physicians.Where(i => i.RegionId == regionid && i.IsDeleted == null).ToList();
            return physicians;
        }

        /// <summary>
        /// BlockCase Fetch Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public BlockCaseModel getBlockCaseData(int requestid)
        {
            var blockCase = _db.RequestClients.FirstOrDefault(i => i.RequestId == requestid);

            var blockCaseData = new BlockCaseModel()
            {
                RequestId = requestid,
                Name = blockCase.FirstName + " " + blockCase.LastName,
            };
            return blockCaseData;
        }

        /// <summary>
        /// BlockCase Update Records
        /// </summary>
        /// <param name="updatedBlockCaseData"></param>
        public void setBlockCaseData(BlockCaseModel updatedBlockCaseData)
        {
            var request = _db.Requests.FirstOrDefault(i => i.RequestId == updatedBlockCaseData.RequestId);
            var requestClient = _db.RequestClients.FirstOrDefault(i => i.RequestId == updatedBlockCaseData.RequestId);

            var blockCaseData = new BlockRequest()
            {

                PhoneNumber = requestClient.PhoneNumber,
                Email = requestClient.Email,
                RequestId = request.RequestId.ToString(),
                Reason = updatedBlockCaseData.Reason,
                IsActive = new BitArray(1, true),
                CreatedDate = DateTime.Now,

            };
            request.Status = 11;
            _db.BlockRequests.Add(blockCaseData);
            _db.SaveChanges();
        }
        
        /// <summary>
        /// Fetch ViewUplod Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public AdminViewUploadsvm getViewUploadsData(int requestid)
        {
            var viewupload = _db.RequestClients.FirstOrDefault(i => i.RequestId == requestid);
            var requestdata = _db.Requests.FirstOrDefault(i => i.RequestId == requestid);

            var viewuploadData = new AdminViewUploadsvm()
            {
                ConformationNumber = requestdata.ConfirmationNumber == null ? null : requestdata.ConfirmationNumber,
                RequestId = requestid,
                Name = viewupload.FirstName + " " + viewupload.LastName,
                UserId = requestdata != null && requestdata.UserId != null ? (int)requestdata.UserId : null ,

            };
            return viewuploadData;
        }

        /// <summary>
        /// Fetch ViewUploadTable List
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public List<ViewUploadsModel> getViewUploadsList(int requestid)
        {
            var document = _db.RequestWiseFiles.Where(i => i.RequestId == requestid);

            var viewUploadDocs = document.Select(r => new ViewUploadsModel()
            {
                requestWiseFileId = r.RequestWiseFileId,
                RequestId = requestid,
                DocumentName = r.FileName,
                UploadDate = r.CreatedDate
            }).ToList();
            return viewUploadDocs;
        }

        /// <summary>
        /// Update ViewUpload Records
        /// </summary>
        /// <param name="adminViewUploadsvm"></param>
        public void setViewUploadsData(AdminViewUploadsvm adminViewUploadsvm)
        {
            string path = _environment.WebRootPath;
            string filepath = "content/" + adminViewUploadsvm.Document.FileName;
            string fullpath = Path.Combine(path, filepath);

            IFormFile file1 = adminViewUploadsvm.Document;

            using (FileStream stream = new FileStream(fullpath, FileMode.Create))
            {
                file1.CopyTo(stream);
            }

            var filename = adminViewUploadsvm.Document?.FileName;
            var doctType = adminViewUploadsvm.Document?.ContentType;

            var filedata = new RequestWiseFile()
            {
                RequestId = adminViewUploadsvm.RequestId,
                FileName = filename,
                Ip = doctType,
                CreatedDate = DateTime.Now,

            };
            _db.Add(filedata);
            _db.SaveChanges();
        }

        /// <summary>
        /// Delete ViewUpload FileData 
        /// </summary>
        /// <param name="requestwisefileid"></param>
        public void DeleteFileData(int requestwisefileid)
        {
            var file = _db.RequestWiseFiles.FirstOrDefault(i => i.RequestWiseFileId == requestwisefileid);

            _db.RequestWiseFiles.Remove(file);
            _db.SaveChanges();
        }
       
        /// <summary>
        /// Send Mail Method Of ViewUpload File
        /// </summary>
        /// <param name="requestwisefileid"></param>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public async Task sendEmailWithFile(int[] requestwisefileid, int requestid)
        {
            var mail = "tatva.dotnet.sohilvekariya123@outlook.com";
            var password = "Devjibapa@2023";
            var email = _db.RequestClients.FirstOrDefault(i => i.RequestId == requestid).Email;
            var subject = "View Uploads";
            var message = "Check the attechment";

            SmtpClient smtpclient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);

            foreach (var obj in requestwisefileid)
            {
                var file = _db.RequestWiseFiles.FirstOrDefault(i => i.RequestWiseFileId == obj);              
                string fullpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content",file.FileName);

                mailMessage.Attachments.Add(new Attachment(fullpath));
            }
            smtpclient.SendMailAsync(mailMessage);
        }
        
        /// <summary>
        /// Fetch Order Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public SendOrderModel getOrderData(int requestid)
        {
            var sendorderdata = new SendOrderModel()
            {
                RequestId = requestid,
            };
            return sendorderdata;
        }

        /// <summary>
        /// Fetch HealthProfessionalTypes TableRecords
        /// </summary>
        /// <returns></returns>
        public List<HealthProfessionalType> getHealthProfessionalTypes()
        {
            var health_professional_types = _db.HealthProfessionalTypes.ToList();
            return health_professional_types;
        }

        /// <summary>
        /// Fetch HealthProfessionals TableRecords based on HealthProfessionalsType Table Id
        /// </summary>
        /// <param name="health_professional_id"></param>
        /// <returns></returns>
        public List<HealthProfessional> getHealthProfessionals(int health_professional_id)
        {
            var heathProfessionals = _db.HealthProfessionals.Where(i => i.Profession == health_professional_id).ToList();
            return heathProfessionals;
        }

        /// <summary>
        /// Fetch HealthProfessionals TabelRecords based On Their Id 
        /// </summary>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        public SendOrderModel GetVendordata(int vendorid)
        {
            var vendor = _db.HealthProfessionals.FirstOrDefault(i => i.VendorId == vendorid);

            var vendordata = new SendOrderModel()
            {
                VendorId = vendorid,
                BusinessContact = vendor.BusinessContact,
                Email = vendor.Email,
                FaxNum = vendor.FaxNumber,
            };
            return vendordata;
        }

        /// <summary>
        /// Update HealthProfessionals TableRecords and OrderDetails TableRecord
        /// </summary>
        /// <param name="sendOrderModel"></param>
        /// <param name="aspId"></param>
        public void setOrderData(SendOrderModel sendOrderModel,string aspId)
        {
            var hpUpdateData = _db.HealthProfessionals.FirstOrDefault(i => i.VendorId == sendOrderModel.VendorId);

            if (hpUpdateData != null)
            {
                hpUpdateData.BusinessContact = sendOrderModel.BusinessContact;
                hpUpdateData.Email = sendOrderModel.Email;
                hpUpdateData.FaxNumber = sendOrderModel.FaxNum;
                hpUpdateData.ModifiedDate = DateTime.Now;
                _db.SaveChanges();
                _db.Update(hpUpdateData);
            }

            var request = _db.Requests.FirstOrDefault(i => i.RequestId == sendOrderModel.RequestId);
            var healthdata = _db.HealthProfessionals.FirstOrDefault(i => i.VendorId == sendOrderModel.VendorId);

            var addorderdata = new OrderDetail()
            {
                RequestId = request.RequestId,
                VendorId = healthdata.VendorId,
                FaxNumber = healthdata.FaxNumber,
                Email = healthdata.Email,
                BusinessContact = healthdata.BusinessContact,
                Prescription = sendOrderModel.Prescription,
                NoOfRefill = sendOrderModel.Refil,
                CreatedDate = DateTime.Now,
                CreatedBy = aspId,
            };
            _db.OrderDetails.Add(addorderdata);
            _db.SaveChanges();
        }
        
        /// <summary>
        /// Fetch TransferCase Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public TransferCaseModel getTransferCaseData(int requestid)
        {
            var transferdata = new TransferCaseModel()
            {
                RequestId = requestid,
            };
            return transferdata;
        }

        /// <summary>
        /// Update TransferCase Records
        /// </summary>
        /// <param name="updateTransferCaseData"></param>
        public void setTransferCaseData(TransferCaseModel updateTransferCaseData)
        {
            var request = _db.Requests.FirstOrDefault(i => i.RequestId == updateTransferCaseData.RequestId);

            var addTransferData = new RequestStatusLog()
            {
                RequestId = request.RequestId,
                Status = 1,
                PhysicianId = request.PhysicianId,
                Notes = updateTransferCaseData.TransferNotes,
                CreatedDate = DateTime.Now,
                TransToPhysicianId = updateTransferCaseData.PhysicianId,
            };
            request.Status = 1;
            request.PhysicianId = addTransferData.TransToPhysicianId;

            _db.RequestStatusLogs.Add(addTransferData);
            _db.SaveChanges();
        }
        
        /// <summary>
        /// Fetch ClearCase Reccords
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public ClearCaseModel getClearCaseData(int requestid)
        {
            var clearCaseData = new ClearCaseModel()
            {
                RequestId = requestid,
            };
            return clearCaseData;
        }

        /// <summary>
        /// Update ClearCase Records
        /// </summary>
        /// <param name="updatedClearCaseData"></param>
        public void setClearCaseData(ClearCaseModel updatedClearCaseData)
        {

            var request = _db.Requests.FirstOrDefault(i => i.RequestId == updatedClearCaseData.RequestId);


            var addclearcaseData = new RequestStatusLog()
            {
                RequestId = request.RequestId,
                Status = 10,
                Notes = "case cleared",
                CreatedDate = DateTime.Now,
                PhysicianId = request.PhysicianId
            };

            request.Status = 10;
            _db.RequestStatusLogs.Add(addclearcaseData);
            _db.SaveChanges();
        }
        
        /// <summary>
        /// Fetch SendAgreement Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public SendAgreementModel getSendAgreementData(int requestid)
        {
            var patientData = _db.RequestClients.FirstOrDefault(i => i.RequestId == requestid);
            var requestorData = _db.Requests.FirstOrDefault(i => i.RequestId == requestid);

            var sendAgreementData = new SendAgreementModel()
            {
                RequestId = requestid,
                PhoneNumber = patientData.PhoneNumber,
                Email = patientData.Email,
                RequestTypeId = requestorData.RequestTypeId,

            };
            return sendAgreementData;
        }

        /// <summary>
        /// Send link of ReviewAgreement Page to Patient via mail
        /// </summary>
        /// <param name="sendAgreementModel"></param>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public async Task sendAgreementMail(SendAgreementModel sendAgreementModel,string aspId)
        {
            var mail = "tatva.dotnet.sohilvekariya123@outlook.com";
            var password = "Devjibapa@2023";
            var email = sendAgreementModel.Email;
            var subject = "Review Agreement";
            var here = "https://localhost:44363/PatientSide/ReviewAgreement?pid=" + _ISecurityRepo.Encrypt(sendAgreementModel.RequestId.ToString());
            var message = $"First you have to login to your account and then click <a href=\"{here}\">here</a> to review your agreement";

            SmtpClient smtpclient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            smtpclient.SendMailAsync(mailMessage);

            var data = (from anur in _db.AspNetUserRoles
                           join anu in _db.AspNetUsers
                           on anur.UserId equals anu.Id
                           select anur).ToList();
            var  aspdata = data.FirstOrDefault(x => x.UserId == aspId).RoleId;

            var emailLog = new EmailLog()
            {
                SubjectName = subject,
                EmailTemplate = "outlook",
                EmailId = email,
                RoleId = 3,   
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = new BitArray(1, true),
                SentTries = 1,
            };

            if (aspdata.Equals("2"))
            {
                var physician = _db.Physicians.FirstOrDefault(i => i.AspNetUserId == aspId);
                emailLog.PhysicianId = physician.PhysicianId;
            }

            if (aspdata.Equals("1"))
            {
                var admin = _db.Admins.FirstOrDefault(i => i.AspNetUserId == aspId);

                emailLog.AdminId = admin.AdminId;
            }
            _db.EmailLogs.Add(emailLog);
            _db.SaveChanges();
        }

        /// <summary>
        /// Fetch CloseCase Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public CloseCaseModel getCloseCaseData(int requestid)
        {
            var patientData = _db.RequestClients.FirstOrDefault(i => i.RequestId == requestid);
            var requestdata = _db.Requests.FirstOrDefault(i => i.RequestId == requestid);
            var BirthYear = Convert.ToInt32(patientData.IntYear);
            var BirthMonth = Convert.ToInt32(patientData.StrMonth);
            var BirthDate = Convert.ToInt32(patientData.IntDate);

            var closeCaseData = new CloseCaseModel()
            {
                ConfirmationNumber = requestdata.ConfirmationNumber == null ? null : requestdata.ConfirmationNumber,
                RequestId = requestid,
                PatientName = patientData.FirstName + " " + patientData.LastName,
                FirstName = patientData.FirstName,
                LastName = patientData.LastName,
                BirthDate = new DateTime(BirthYear, BirthMonth, BirthDate),
                PhoneNumber = patientData.PhoneNumber,
                Email = patientData.Email,
            };
            return closeCaseData;

        }

        /// <summary>
        /// Fetch RequestWiseFile Recordlists In CloseCase Page
        /// </summary>
        /// <param name="requestid"></param>
        /// <returns></returns>
        public List<CloseCaseList> getCloseCaseList(int requestid)
        {
            var document = _db.RequestWiseFiles.Where(i => i.RequestId == requestid).ToList();

            var closeCaseDocs = document.Select(r => new CloseCaseList()
            {
                requestWiseFileId = r.RequestWiseFileId,
                RequestId = requestid,
                DocumentName = r.FileName,
                UploadDate = r.CreatedDate
            }).ToList();

            return closeCaseDocs;
        }

        /// <summary>
        /// Update RequestClients Table Data
        /// </summary>
        /// <param name="closeCaseModel"></param>
        public void updateCloseCaseData(CloseCaseModel closeCaseModel)
        {
            var requesrclient = _db.RequestClients.FirstOrDefault(i => i.RequestId == closeCaseModel.RequestId);

            requesrclient.PhoneNumber = closeCaseModel.PhoneNumber;
            //requesrclient.Email = closeCaseModel.Email;

            _db.SaveChanges();
        }

        /// <summary>
        /// Update CloseCase Records
        /// </summary>
        /// <param name="requestid"></param>
        /// <param name="aspId"></param>
        public void setCloseCaseData(int requestid,string aspId)
        {

            var request = _db.Requests.FirstOrDefault(i => i.RequestId == requestid);
            var admin = _db.Admins.FirstOrDefault(i=>i.AspNetUserId == aspId);

            string closedNote = admin.FirstName + " Closed case on " + DateTime.Now.ToString("MMM dd, yyyy") + " at " + DateTime.Now.ToString("hh:mm tt");

            var addclosecaseData = new RequestStatusLog()
            {
                RequestId = request.RequestId,
                Status = 9,
                Notes = closedNote,
                CreatedDate= DateTime.Now,
                PhysicianId = request.PhysicianId,
                AdminId = admin.AdminId,

            };
            request.Status = 9;
            _db.RequestStatusLogs.Add(addclosecaseData);        
            _db.SaveChanges();
        }

        /// <summary>
        /// Send link of SubmitRequest Page to Patient via mail
        /// </summary>
        /// <param name="sendLinkModel"></param>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public async Task SubmitRequestMail(SendLinkModel sendLinkModel,string aspId)
        {
            var admin = _db.Admins.FirstOrDefault(x => x.AspNetUserId == aspId);
            var physician = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId);

            var mail = "tatva.dotnet.sohilvekariya123@outlook.com";
            var password = "Devjibapa@2023";
            var email = sendLinkModel.Email;
            var subject = "Submit Request";
            var link = "https://localhost:44363/PatientSide/SubmitRequest";
            var message = $"Hey <b>{sendLinkModel.FirstName}</b>, <br>" +
                $"Check below your details : <br>" +
                $"Firstname : {sendLinkModel.FirstName} <br>" +
                $"Lastname : {sendLinkModel.LastName} <br>" +
                $"Phonenumber : {sendLinkModel.PhoneNumber} <br><br><br>" +
                $"Click <a href=\"{link}\">here</a> to submit request";

            SmtpClient smtpclient = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            smtpclient.SendMailAsync(mailMessage);

            var emailLog = new EmailLog()
            {
                SubjectName = subject,
                EmailTemplate = "outlook",
                EmailId = email,
                RoleId = 3,
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = new BitArray(1, true),
                SentTries = 1,
            };
            if (admin == null)
            {
                emailLog.PhysicianId = physician.PhysicianId;
            }
            else
            {
                emailLog.AdminId = admin.AdminId;
            }
            _db.EmailLogs.Add(emailLog);
            _db.SaveChanges();
        }

        /// <summary>
        /// post RequestSupport message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public bool RequestSupportViaEmail(string message, int adminId)
        {
            try
            {
                if (message != null)
                {
                    var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
                    BitArray deletedBit = new BitArray(new[] { false });

                    var offDutyQuery = _db.Physicians
                        .Where(p => !_db.ShiftDetails.Any(sd => sd.Shift.PhysicianId == p.PhysicianId &&
                                                                       sd.ShiftDate.Date == DateTime.Today &&
                                                                       currentTime >= sd.StartTime &&
                                                                       currentTime <= sd.EndTime &&
                                                                       sd.IsDeleted.Equals(deletedBit)) && p.IsDeleted == null).ToList();

                    foreach (var obj in offDutyQuery)
                    {

                        var mail = "tatva.dotnet.sohilvekariya123@outlook.com";
                        var password = "Devjibapa@2023";
                        var email = obj.Email;
                        var subject = "Need Support";

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
                            EmailTemplate = message,
                            SubjectName = subject,
                            EmailId = obj.Email,
                            RoleId = 2,
                            AdminId = adminId,
                            CreateDate = DateTime.Now,
                            SentDate = DateTime.Now,
                            IsEmailSent = new BitArray(1, true),
                            SentTries = 1,
                        };

                        _db.EmailLogs.Add(emailLog);
                        _db.SaveChanges();
                    }
                }
                return true;

            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Send link of SubmitRequest Page to Patient via mail and Insert Record In EmailLog 
        /// </summary>
        /// <param name="adminCreateRequestvm"></param>
        /// <param name="aspId"></param>
        public void SendCreateRequestData(AdminCreateRequestvm adminCreateRequestvm, string aspId)
        {
            int reqTypeId = 1;
            InsertRequestData(adminCreateRequestvm, reqTypeId, aspId);

            AspNetUser? aspnetuser = _db.AspNetUsers.FirstOrDefault(x => x.Email == adminCreateRequestvm.Email);
            var admin = _db.Admins.FirstOrDefault(x => x.AspNetUserId == aspId);
            var physician = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId);

            if (aspnetuser == null)
            {
                var mail = "tatva.dotnet.sohilvekariya123@outlook.com";
                var password = "Devjibapa@2023";
                var email = adminCreateRequestvm.Email;
                var subject = "Create Account";
                var link = "https://localhost:44363/PatientSide/Register?pid=" + _ISecurityRepo.Encrypt(adminCreateRequestvm.RequestId.ToString());
                var message = $"Hey <b>{adminCreateRequestvm.FirstName}</b>, <br>" +
                    $"Your request is created, click <a href=\"{link}\">here</a> to register and access it...";

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
                    RoleId = 3,
                    RequestId = adminCreateRequestvm.RequestId,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1, true),
                    SentTries = 1,
                };

                if(admin == null)
                {
                    emailLog.PhysicianId = physician.PhysicianId;
                }
                else
                {
                    emailLog.AdminId = admin.AdminId;
                }
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }
            

        }

        /// <summary>
        /// Insert Records In Request Table
        /// </summary>
        /// <param name="adminCreateRequestvm"></param>
        /// <param name="reqTypeId"></param>
        /// <param name="aspId"></param>
        public void InsertRequestData(AdminCreateRequestvm adminCreateRequestvm, int reqTypeId, string aspId)
        {
            var admin = _db.Admins.FirstOrDefault(x => x.AspNetUserId == aspId);
            var physician  = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId);
            User? user = _db.Users.FirstOrDefault(i => i.Email == adminCreateRequestvm.Email);

            var requestData = new Request()
            {
                UserId = user == null ? null : user.UserId,
                RequestTypeId = reqTypeId,
                FirstName = admin == null ? physician.FirstName : admin.FirstName,
                LastName = admin == null ? physician.LastName : admin.LastName,
                Email = admin == null ? physician.Email : admin.Email,
                PhoneNumber = admin == null ? physician.Mobile : admin.Mobile,
                Status = 1,
                IsUrgentEmailSent = new BitArray(new bool[1] { true }),
                CreatedDate = DateTime.Now,
            };
            _db.Requests.Add(requestData);
            _db.SaveChanges();

            int requestId = requestData.RequestId;

            adminCreateRequestvm.RequestId = requestId;

            string aspid = admin == null ? physician.AspNetUserId : admin.AspNetUserId;

            InsertRequestClientData(adminCreateRequestvm, requestId);

            InsertNotesData(adminCreateRequestvm, requestId, aspid);

        }

        /// <summary>
        /// Insert Records In RequestClients Table
        /// </summary>
        /// <param name="adminCreateRequestvm"></param>
        /// <param name="requestId"></param>
        public void InsertRequestClientData(AdminCreateRequestvm adminCreateRequestvm, int requestId)
        {
            var clientData = new RequestClient()
            {
                RequestId = requestId,
                FirstName = adminCreateRequestvm.FirstName,
                LastName = adminCreateRequestvm.LastName,
                Email = adminCreateRequestvm.Email,
                PhoneNumber = adminCreateRequestvm.PhoneNo,
                Street = adminCreateRequestvm.Street,
                City = adminCreateRequestvm.City,
                State = _db.Regions.Where(x => x.RegionId == adminCreateRequestvm.RegionId).Select(x => x.Name).FirstOrDefault(),
                RegionId = adminCreateRequestvm.RegionId,
                ZipCode = adminCreateRequestvm.Zipcode,
                IntYear = adminCreateRequestvm.DateOfBirth?.Year,
                IntDate = adminCreateRequestvm.DateOfBirth?.Day,
                StrMonth = adminCreateRequestvm.DateOfBirth?.Month.ToString(),
            };
            _db.RequestClients.Add(clientData);
            _db.SaveChanges();

        }

        /// <summary>
        ///  Insert Records In Notes Table
        /// </summary>
        /// <param name="adminCreateRequestvm"></param>
        /// <param name="requestId"></param>
        /// <param name="aspId"></param>
        public void InsertNotesData(AdminCreateRequestvm adminCreateRequestvm, int requestId, string aspId)
        {
            var notesData = new RequestNote()
            {
                RequestId = requestId,
                AdminNotes = adminCreateRequestvm.AdminNotes,
                CreatedBy = aspId,
                CreatedDate = DateTime.Now, 
            };
            _db.RequestNotes.Add(notesData);
            _db.SaveChanges();
        }

        /// <summary>
        /// fetch records of Region Table 
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool CheckRegion(int region)
        {
            if (region == 0)
            {
                return false;
            }
            var regionName = _db.Regions.FirstOrDefault(x => x.RegionId == region).Name;
            bool isAvailable = _db.Regions.Any(x => x.Name == regionName);

            return isAvailable;
        }

       /// <summary>
       /// Fetch Records in Account Access Page
       /// </summary>
       /// <returns></returns>
        public List<AccountAccess> GetAccountAccessData()
        {
            BitArray deletedBit = new BitArray(new[] { false });
            var Roles = _db.Roles.Where(i => i.IsDeleted.Equals(deletedBit));


            var Accessdata = Roles.Select(r => new AccountAccess()
            {
                name = r.Name,
                accounttype = _db.AspNetRoles.FirstOrDefault(x => Convert.ToInt32(x.Id) == r.AccountType).Name,
                accounttypeid = r.AccountType,
                roleid = r.RoleId,
            }).ToList();

            return Accessdata;
        }

        /// <summary>
        /// Fetch AspNetRoles Table Records
        /// </summary>
        /// <returns></returns>
        public List<AspNetRole> GetAccountType()
        {
            var role = _db.AspNetRoles.ToList();
            return role;
        }

        /// <summary>
        /// Fetch Menu Table Records When on Create Access Page
        /// </summary>
        /// <param name="accounttype"></param>
        /// <returns></returns>
        public List<Menu> GetMenu(int accounttype)
        {
            if (accounttype != 0)
            {
                var menu = _db.Menus.Where(r => r.AccountType == accounttype).ToList();
                return menu;
            }
            else
            {

                var menu = _db.Menus.ToList();
                return menu;
            }
        }

        /// <summary>
        /// Fetch Menu Table Records When on Edit Access Page
        /// </summary>
        /// <param name="accounttype"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public List<AccountMenu> GetAccountMenu(int accounttype, int roleid)
        {
            var menu = _db.Menus.Where(r => r.AccountType == accounttype).ToList();
            var rolemenu = _db.RoleMenus.ToList();

            var checkedMenu = menu.Select(r1 => new AccountMenu
            {
                menuid = r1.MenuId,
                name = r1.Name,
                ExistsInTable = rolemenu.Any(r2 => r2.RoleId == roleid && r2.MenuId == r1.MenuId),

            }).ToList();
            return checkedMenu;
        }

        /// <summary>
        /// Post Records From CreateAccess Page
        /// </summary>
        /// <param name="accountAccess"></param>
        /// <param name="AccountMenu"></param>
        public void SetCreateAccessAccount(AccountAccess accountAccess, List<int> AccountMenu)
        {
            if (accountAccess != null)
            {
                var role = new Role()
                {
                   
                    Name = accountAccess.name,
                    AccountType = (short)accountAccess.accounttypeid,
                    CreatedBy = "Admin",
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(1, false),

                };
                _db.Add(role);
                _db.SaveChanges();

                if (AccountMenu != null)
                {
                    foreach (int menuid in AccountMenu)
                    {
                        _db.RoleMenus.Add(new RoleMenu
                        {
                            RoleId = role.RoleId,
                            MenuId = menuid,
                        });
                    }
                    _db.SaveChanges();

                }
            }
        }

        /// <summary>
        /// Fetch Records On EditAccess Page
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public AccountAccess GetEditAccessData(int roleid)
        {
            var role = _db.Roles.FirstOrDefault(i => i.RoleId == roleid);
            if (role != null)
            {
                var roledata = new AccountAccess()
                {
                    name = role.Name,
                    roleid = roleid,
                    accounttypeid = role.AccountType,
                };
                return roledata;
            }
            return null;
        }

        /// <summary>
        /// Update Records From EditAccess Page
        /// </summary>
        /// <param name="accountAccess"></param>
        /// <param name="AccountMenu"></param>
        public void SetEditAccessAccount(AccountAccess accountAccess, List<int> AccountMenu)
        {
            var role = _db.Roles.FirstOrDefault(x => x.RoleId == accountAccess.roleid);
            if (role != null)
            {

                role.Name = accountAccess.name;
                role.AccountType = (short)accountAccess.accounttypeid;
                role.CreatedBy = "admin";
                role.ModifiedDate = DateTime.Now;
                _db.SaveChanges();

                var rolemenu = _db.RoleMenus.Where(i => i.RoleId == accountAccess.roleid).ToList();
                if (rolemenu != null)
                {
                    _db.RoleMenus.RemoveRange(rolemenu);
                }

                if (AccountMenu != null)
                {
                    foreach (int menuid in AccountMenu)
                    {
                        _db.RoleMenus.Add(new RoleMenu
                        {
                            RoleId = role.RoleId,
                            MenuId = menuid,
                        });
                    }
                    _db.SaveChanges();

                }
            }
        }

        /// <summary>
        /// Delete Role From AccounAccess Page
        /// </summary>
        /// <param name="roleid"></param>
        public void DeleteAccountAccess(int roleid)
        {
            var role = _db.Roles.FirstOrDefault(x => x.RoleId == roleid);
            if (role != null)
            {
                role.IsDeleted = new BitArray(1, true);
                _db.SaveChanges();
            }

            var rolemenu = _db.RoleMenus.Where(i => i.RoleId == roleid).ToList();
            if (rolemenu != null)
            {
                _db.RoleMenus.RemoveRange(rolemenu);
            }
            _db.SaveChanges();
        }
        
        /// <summary>
        /// Fetch Records On UserAccess Page
        /// </summary>
        /// <param name="accountTypeId"></param>
        /// <returns></returns>
        public List<UserAccess> GetUserAccessData(int accountTypeId)
        {
            var admin = _db.Admins.Where(i => i.IsDeleted == null).ToList();
            var physician = _db.Physicians.Where(i => i.IsDeleted == null).ToList();

            var adminList = admin.Select(x => new UserAccess
            {
                AspId = x.AspNetUserId,
                AccountTypeId = 1,
                AccountType = "Admin",
                AccountHolderName = x.FirstName + " " + x.LastName,
                AccountPhone = x.Mobile,
                AccountStatus = (short)x.Status,
                AccountRequests = _db.Requests.Where(y => y.Status != 10 || y.Status != 11).Count(),
            }).ToList();

            var physicianList = physician.Select(x => new UserAccess
            {
                AspId = x.AspNetUserId,
                AccountTypeId = 2,
                AccountType = "Provider",
                AccountHolderName = x.FirstName + " " + x.LastName,
                AccountPhone = x.Mobile,
                AccountStatus = (short)x.Status,
                AccountRequests = _db.Requests.Where(y => y.PhysicianId == x.PhysicianId && (y.Status == 2 || y.Status == 4 || y.Status == 5 || y.Status == 6)).Count(),
            }).ToList();

            var finalList = adminList.Concat(physicianList).ToList();

            if (accountTypeId == 1)
            {
                return adminList;
            }
            else if (accountTypeId == 2)
            {
                return physicianList;
            }
            else if (accountTypeId == 3)
            {
                return null;
            }
            return finalList;
        }

        #region Invoicing
        public List<Timesheet> GetTimeSheetDetail(int phyid, string dateSelected)
        {
            var timesheetData = _db.Timesheets.ToList();

            if (phyid != null)
            {
                timesheetData = timesheetData.Where(i => i.PhysicianId == phyid).ToList();
            }

            if (dateSelected != null)
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
                    timesheetData = timesheetData.Where(i => i.StartDate.ToString() == startDate && i.EndDate.ToString() == endDate).ToList();
                }
                else if (splitedDate[1] == "2")
                {
                    var startDate = 16 + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    var endDate = daysInMonth.ToString() + "-" + splitedDate[0] + "-" + currentYear.ToString();
                    timesheetData = timesheetData.Where(i => i.StartDate.ToString() == startDate && i.EndDate.ToString() == endDate).ToList();
                }
            }

            return timesheetData;
        }

        public List<PhysicianPayrate> GetPayRateForProviderByPhyId(int phyid)
        {
            return _db.PhysicianPayrates.Where(i => i.PhysicianId == phyid).ToList();
        }

        public bool ApproveTimeSheet(int timeSheetId, int bonus, string notes)
        {
            try
            {
                var timeSheetData = _db.Timesheets.FirstOrDefault(x => x.TimesheetId == timeSheetId);

                timeSheetData.IsApproved = true;
                timeSheetData.ModifiedDate = DateTime.Now;

                if (bonus != null)
                {
                    timeSheetData.BonusAmount = bonus.ToString();
                }
                if (notes != null)
                {
                    timeSheetData.AdminNotes = notes;
                }

                _db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion 
    }
}
