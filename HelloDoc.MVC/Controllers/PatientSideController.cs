using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using HelloDoc.DAL.DataContext;
using HelloDoc.BAL.Interface;
using Business_Logic.Interface;
using Data_Access.Dash;
using Microsoft.AspNetCore.Routing.Constraints;
using HelloDoc.MVC.Auth;
using NuGet.Common;
using System.Collections;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;


namespace HelloDoc.MVC.Controllers
{
    public class PatientSideController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPatientReqRepo _ipatientreq;
        private readonly IPatientDashRepo _ipatientdash;
        private readonly IRegisterRepo _iregister;
        private readonly IJwtServiceRepo _ijwtservice;
        public readonly ISecurityRepo _ISecurityRepo;
        private readonly ILogger<PatientSideController> _logger;



        public PatientSideController(ApplicationDbContext db, IPatientReqRepo ipatientreq, IPatientDashRepo ipatientdash, IRegisterRepo iregister, IJwtServiceRepo ijwtservice, ILogger<PatientSideController> logger, ISecurityRepo iSecurityRepo)
        {
            _db = db;
            _ipatientreq = ipatientreq;
            _ipatientdash = ipatientdash;
            _iregister = iregister;
            _ijwtservice = ijwtservice;
            _logger = logger;
            _ISecurityRepo = iSecurityRepo;
        }

        /// <summary>
        /// Get Review Agreement Page
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        public IActionResult ReviewAgreement(string pid)
        {
            int requesrid = Int32.Parse(_ISecurityRepo.Decrypt(pid));
            ReviewAgreementvm reviewAgreementvm = new ReviewAgreementvm();
            reviewAgreementvm = _ipatientreq.GetReviewAgreement(requesrid);

            if (reviewAgreementvm != null)
            {
                return View(reviewAgreementvm);
            }
            return RedirectToAction("AccessDenied");
        }

        /// <summary>
        /// Post Agree Agrement 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        public IActionResult ReviewAgree(int pid)
        {
            _ipatientreq.AgreeReview(pid);
            TempData["success"] = "Agreement Agreed!!";
            return RedirectToAction("Dashboard");
        }

        /// <summary>
        /// Post Cancel Agreement
        /// </summary>
        /// <param name="reviewAgreementvm"></param>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        [HttpPost]
        public IActionResult CancelAgree(ReviewAgreementvm reviewAgreementvm)
        {
            _ipatientreq.CancelReview(reviewAgreementvm);
            TempData["success"] = "Agreement Cancelled!!";
            return RedirectToAction("Dashboard");
        }

        /// <summary>
        /// Get Landing Page
        /// </summary>
        /// <returns></returns>
        public IActionResult LandingPage()
        {

            return View();
        }

        /// <summary>
        /// Get Login Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Post Login Data
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(AspNetUser user)
        {
            var myUser = _db.AspNetUsers.Where(x => x.Email == user.Email && x.PasswordHash == user.PasswordHash).FirstOrDefault();
            var us = _db.Users.FirstOrDefault(x => x.Email.Trim() == user.Email);

            var admin = _db.Admins.FirstOrDefault(x => x.Email == user.Email);

            var physician = _db.Physicians.FirstOrDefault(x => x.Email == user.Email);

            int? conformationnumuserid = us == null ? null : us.UserId;
            var req = _db.Requests.FirstOrDefault(x => x.UserId == conformationnumuserid && x.ConfirmationNumber != null);

            if (myUser != null)
            {
                //User? users = _db.Users.FirstOrDefault(i => i.Email == user.Email);

                //HttpContext.Session.SetString("UserSession",myUser.Email);
                HttpContext.Session.SetString("AspnetUserid", myUser.Id);
                if (us != null)
                {
                    HttpContext.Session.SetInt32("Userid", us.UserId);
                    HttpContext.Session.SetString("_sessionUserName", us.FirstName + " " + us.LastName);
                }

                if (req != null)
                {
                    HttpContext.Session.SetString("_sessionConformationNumber", req.ConfirmationNumber);
                }
                if (admin != null)
                {
                    HttpContext.Session.SetString("_sessionAdminName", admin.FirstName + " " + admin.LastName);
                }
                if (physician != null)
                {
                    HttpContext.Session.SetString("_sessionPhysicianName", physician.FirstName + " " + physician.LastName);
                }

                AspNetUser aspnetuser = _iregister.GetUserRole(user.Email);
                string token = _ijwtservice.GenerateJwtToken(aspnetuser);
                HttpContext.Session.SetString("token", token);
                TempData["success"] = "Login Successfull!!";

                if (aspnetuser.AspNetUserRole.RoleId == "1")
                {
                    return RedirectToAction("AdminDashboard", "AdminSide");
                }
                else if (aspnetuser.AspNetUserRole.RoleId == "2")
                {
                    return RedirectToAction("ProviderDashboard", "ProviderSide");
                }

                else if (aspnetuser.AspNetUserRole.RoleId == "3")
                {
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                ViewBag.Message = "Login Failed..";
                TempData["error"] = "Invalid Password!!";

            }
            return View();
        }

       /// <summary>
       /// Logout Action
       /// </summary>
       /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["success"] = "Logout Successfull!!";
            return RedirectToAction("Login", "Patientside");
        }

        /// <summary>
        /// Get Forgot Password Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Forgot()
        {
            return View();
        }

        /// <summary>
        /// Post Forgot Password Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Forgot(PatientReqData model)
        {
            //var myUser = _db.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();
            var myUser = _db.AspNetUsers.Include(y => y.AspNetUserRole).FirstOrDefault(x => x.Email == model.Email);
            var request = _db.RequestClients.FirstOrDefault(x => x.Email == model.Email);

            if (myUser != null)
            {
                var reciever = model.Email;
                var subject = "Reset Password";
                var here = "https://localhost:44363/PatientSide/Reset";
                var otp = GenerateOTP(model.Email);
                var message = $"We trust this message finds you in good spirits.To Reset your Account Password click <a href=\"{here}\">here</a><p>OTP : {otp}";

                _ipatientreq.EmailSender(reciever, subject, message);

                var emailLog = new EmailLog()
                {
                    SubjectName = subject,
                    EmailTemplate = "outlook",
                    EmailId = model.Email,
                    RoleId = Convert.ToInt32(myUser.AspNetUserRole.RoleId),
                    RequestId = request == null ? null : request.RequestId,
                    //AdminId = admin.AdminId,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1, true),
                    SentTries = 1,
                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();

                TempData["success"] = "Email Sent for Register";
                return RedirectToAction("Reset");

            }
            else
            {
                TempData["error"] = "Email is not registered";
            }
            return View();
        }

        /// <summary>
        /// Generate OtP Method
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private string GenerateOTP(string email)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var otp = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

            var aspnetuser = _db.AspNetUsers.FirstOrDefault(x => x.Email == email);
            aspnetuser.Otp = otp;
            _db.SaveChanges();

            return otp;
        }

        /// <summary>
        /// OTP check Method
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otp"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult OtpCheck(string email, string otp)
        {
            AspNetUser? aspnetuser = _db.AspNetUsers.FirstOrDefault(x => x.Email == email);
            if (aspnetuser == null)
            {
                return Json(new { Success = false });
            }
            if (aspnetuser.Otp == null)
            {
                return Json(new { Success = false });
            }
            else
            {
                if (aspnetuser.Otp == otp)
                {
                    return Json(new { Success = true });
                }
                else
                {
                    return Json(new { Success = false });
                }
            }
        }

       /// <summary>
       /// Page Reset Pasword
       /// </summary>
       /// <returns></returns>
        public IActionResult Reset()
        {
            return View();
        }

        /// <summary>
        /// Post Reset Password Action
        /// </summary>
        /// <param name="registervm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Reset(Registervm registervm)
        {
            AspNetUser? aspnetuser = _db.AspNetUsers.FirstOrDefault(f => f.Email == registervm.Email);       
            if (aspnetuser == null)
            {
                TempData["error"] = "Email is not registered!";
            }
            else
            {
                if (registervm.Password == registervm.ConfirmPassword)
                {
                    _iregister.ResetPassword(registervm);
                    TempData["success"] = "Password Updated!";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

       /// <summary>
       /// Get SubmitRequest Page 
       /// </summary>
       /// <returns></returns>
        public IActionResult SubmitRequest()
        {
            return View();
        }

        /// <summary>
        /// Get Patient Request Page
        /// </summary>
        /// <returns></returns>
        public IActionResult PatientReq()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }

        /// <summary>
        /// Post Patient Request Form Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PatientReq(PatientReqData model)
        {

            model.Regions = _ipatientreq.GetRegions();
            if (_db.Admins.Any(x=> x.Email == model.Email) || _db.Physicians.Any( x=> x.Email == model.Email))
            {
                TempData["error"] = "Please Enter Valid Email!";
                return View(model);
            }
            if (!_ipatientreq.EmailCheck(model.Email))
            {     
                _ipatientreq.AddToUser(model);
            }
            else
            {
                model.UserId = _ipatientreq.GetUserId(model.Email);
            }
            model.OtherFirstName = model.FirstName;
            model.OtherLastName = model.LastName;
            model.OtherEmail = model.Email;
            model.OtherPhoneNumber = model.PhoneNumber;
            model.RequestTypeId = 1;
            _ipatientreq.AddToRequest(model);

            _ipatientreq.AddToReqClient(model);

            if (model.FileName != null)
            {
             _ipatientreq.UploadFile(model);
            }
            TempData["success"] = "Request Added";
            return RedirectToAction("LandingPage");
        }

        /// <summary>
        /// Get Family/Friend Request Page Action
        /// </summary>
        /// <returns></returns>
        public IActionResult FamilyfriendReq()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }

        /// <summary>
        /// Post Family/Friend Request Page Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult FamilyfriendReq(PatientReqData model)
        {
            var myUser = _db.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();

            model.RequestTypeId = 2;

            _ipatientreq.AddToRequest(model);
            _ipatientreq.AddToReqClient(model);

            if (model.FileName != null)
            {

                _ipatientreq.UploadFile(model);
            }
            if (myUser == null)
            {
                var reciever = model.Email;
                var subject = "create patient account";
                var message = "Tap on link for Create Account: https://localhost:44363/PatientSide/Register?pid=" + _ISecurityRepo.Encrypt(model.RequestId.ToString()) ;

                _ipatientreq.EmailSender(reciever, subject, message);

                var emailLog = new EmailLog()
                {
                    SubjectName = subject,
                    EmailTemplate = "outlook",
                    EmailId = model.Email,
                    RoleId = 3,
                    RequestId = _db.RequestClients.FirstOrDefault(x => x.Email == model.Email).RequestId,
                    //AdminId = admin.AdminId,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1, true),
                    SentTries = 1,
                    ConfirmationNumber =model.ConfirrmationNumber,
                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }
            TempData["success"] = "Request Added";
            return RedirectToAction("LandingPage");

        }

        /// <summary>
        /// Email Check On PatientReq Page
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> EmailCheck(string email)
        {
            var user = await _db.AspNetUsers.FirstOrDefaultAsync(x => x.Email == email);
            return user != null;
        }

       /// <summary>
       /// Get Business Request Page
       /// </summary>
       /// <returns></returns>
        public IActionResult BussinessReq()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }

        /// <summary>
        /// Post Business Request Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BussinessReq(PatientReqData model)
        {

            //_ipatientreq.AddToUser(model);

            var myUser = _db.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();
           
            model.RequestTypeId = 4;
            _ipatientreq.AddToRequest(model);

            _ipatientreq.AddToReqClient(model);

            _ipatientreq.AddToBusiness(model);

            _ipatientreq.AddToReqBusiness(model);

            if (myUser == null)
            {
                var reciever = model.Email;
                var subject = "create patient account";
                var message = "Tap on link for Create Account: https://localhost:44363/PatientSide/Register?pid=" + _ISecurityRepo.Encrypt(model.RequestId.ToString());

                _ipatientreq.EmailSender(reciever, subject, message);

                var emailLog = new EmailLog()
                {
                    SubjectName = subject,
                    EmailTemplate = "outlook",
                    EmailId = model.Email,
                    RoleId = 3,
                    RequestId = _db.RequestClients.FirstOrDefault(x => x.Email == model.Email).RequestId,
                    //AdminId = admin.AdminId,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1, true),
                    SentTries = 1,
                    ConfirmationNumber = model.ConfirrmationNumber,
                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();

            }
            TempData["success"] = "Request Added";
            return RedirectToAction("LandingPage");
        }

        /// <summary>
        /// Get Concierge Request Page
        /// </summary>
        /// <returns></returns>
        public IActionResult ConciergeReq()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }

        /// <summary>
        /// Post Concierge Request Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ConciergeReq(PatientReqData model)
        {
            var myUser = _db.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();

            model.RequestTypeId = 3;
            _ipatientreq.AddToRequest(model);

            _ipatientreq.AddToReqClient(model);

            _ipatientreq.AddToConcierge(model);

            _ipatientreq.AddToReqConcierge(model);


            if (myUser == null)
            {
                var reciever = model.Email;
                var subject = "create patient account";
                var message = "Tap on link for Create Account: https://localhost:44363/PatientSide/Register?pid=" + _ISecurityRepo.Encrypt(model.RequestId.ToString());

                _ipatientreq.EmailSender(reciever, subject, message);

                var emailLog = new EmailLog()
                {
                    SubjectName = subject,
                    EmailTemplate = "outlook",
                    EmailId = model.Email,
                    RoleId = 3,
                    RequestId = _db.RequestClients.FirstOrDefault(x => x.Email == model.Email).RequestId,
                    //AdminId = admin.AdminId,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1, true),
                    SentTries = 1,
                    ConfirmationNumber = model.ConfirrmationNumber,
                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();
            }

            
            TempData["success"] = "Request Added";
            return RedirectToAction("LandingPage");
        }

        /// <summary>
        /// Get Patient Dashboard Action
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        public IActionResult Dashboard()
        {
            int userId = (int)(HttpContext.Session.GetInt32("Userid"));
            var getProfileData = _ipatientdash.GetProfileData(userId);

            PatientDashData patientDashData = new PatientDashData();
            patientDashData.dashboardData = _ipatientdash.RequestList(userId);
            patientDashData.profileData = getProfileData;

            return View(patientDashData);
        }

        /// <summary>
        /// Post Dashboard Action
        /// </summary>
        /// <param name="spid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Dashboard(string spid)
        {
            return RedirectToAction(spid);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Get ViewDocuments Page
        /// </summary>
        /// <param name="rid"></param>
        /// <returns></returns>
        public IActionResult ViewDoc(int rid)
        {
            HttpContext.Session.SetInt32("_sessionReqId", rid);

            PatientDashData patientDashData = new PatientDashData();
            patientDashData.documentData = _ipatientdash.DocumentList(rid);

            return View(patientDashData);
        }

        /// <summary>
        /// Post ViewDocuments Page
        /// </summary>
        /// <param name="patientDashData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ViewDoc(PatientDashData patientDashData)
        {
            int reqId = (int)HttpContext.Session.GetInt32("_sessionReqId");

            if (_ipatientdash.DashboardUpload(patientDashData, reqId))
            {
                patientDashData.documentData = _ipatientdash.DocumentList(reqId);
                return View(patientDashData);
            }

            return View();
        }

        /// <summary>
        /// Get Submit For Me Page 
        /// </summary>
        /// <returns></returns>
        public IActionResult SubmitForMe()

        {
            int userId = (int)(HttpContext.Session.GetInt32("Userid"));
            var users = _db.Users.FirstOrDefault(i => i.UserId == userId);
            var BirthDay = Convert.ToInt32(users.IntDate);
            var BirthMonth = Convert.ToInt32(users.StrMonth);
            var BirthYear = Convert.ToInt32(users.IntYear);

            PatientReqData model = new PatientReqData();
            //User u = new User();
            model.FirstName = users.FirstName;
            model.LastName = users.LastName;
            model.Email = users.Email;
            model.PhoneNumber = users.Mobile;
            model.BirthDate = new DateTime(BirthYear, BirthMonth, BirthDay);
            model.Regions = _ipatientreq.GetRegions();

            return View(model);
        }

        /// <summary>
        /// Post Submit For Me Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SubmitForMe(PatientReqData model)
        {
            int userId = (int)(HttpContext.Session.GetInt32("Userid"));
            model.UserId = userId;

            model.OtherFirstName = model.FirstName;
            model.OtherLastName = model.LastName;
            model.OtherEmail = model.Email;
            model.OtherPhoneNumber = model.PhoneNumber;

            model.RequestTypeId = 1;
            _ipatientreq.AddToRequest(model);

            _ipatientreq.AddToReqClient(model);

            if (model.FileName != null)
            {

                _ipatientreq.UploadFile(model);
            }
            TempData["success"] = "Request Created";
            return RedirectToAction("Dashboard");
        }

        /// <summary>
        /// Get SubmitForSomeOne Page
        /// </summary>
        /// <returns></returns>
        public IActionResult SubmitForSomeOne()
        {
            PatientReqData patientReqData = new PatientReqData();
            patientReqData.Regions = _ipatientreq.GetRegions();

            return View(patientReqData);
        }

        /// <summary>
        /// Post SubmitForSomeOne Form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SubmitForSomeOne(PatientReqData model)

        {
            int userId = (int)(HttpContext.Session.GetInt32("Userid"));
            int requestCount = _db.Requests.Count(i => i.CreatedDate.Date == DateTime.Now.Date) + 1;

            model.RequestTypeId = 2;

            User? user = _db.Users.FirstOrDefault(i => i.UserId == userId);
            //model.UserId = userId;
                User? patient = _db.Users.FirstOrDefault(i => i.Email == model.Email);

                Request r = new Request();

                //r.UserId = model.UserId;
                r.UserId = null;
                r.FirstName = user.FirstName;
                r.LastName = user.LastName;
                r.Email = user.Email;
                r.PhoneNumber = user.Mobile;
                r.RelationName = model.OtherRelation;
                r.RequestTypeId = model.RequestTypeId;
                r.Status = 1;
                r.CreatedDate = DateTime.Now;
                r.IsUrgentEmailSent = new BitArray(new bool[1] { true });
                
                string confirmation = "MD" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + r.LastName.Remove(2).ToUpper() + r.FirstName.Remove(2).ToUpper() + requestCount.ToString("D4");

                r.ConfirmationNumber = confirmation;
                r.RequestId = model.RequestId;


                _db.Requests.Add(r);
                _db.SaveChanges();
                model.RequestId = r.RequestId;
                model.ConfirrmationNumber = confirmation;

            _ipatientreq.AddToReqClient(model);

            if (model.FileName != null)
            {

                _ipatientreq.UploadFile(model);
            }

            var myUser = _db.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();

            if (myUser == null)
            {
                var reciever = model.Email;
                var subject = "create patient account";
                var message = "Tap on link for Create Account: https://localhost:44363/PatientSide/Register?pid=" + _ISecurityRepo.Encrypt(model.RequestId.ToString());

                _ipatientreq.EmailSender(reciever, subject, message);

                var emailLog = new EmailLog()
                {
                    SubjectName = subject,
                    EmailTemplate = "outlook",
                    EmailId = model.Email,
                    RoleId = 3,
                    RequestId = _db.RequestClients.FirstOrDefault(x => x.Email == model.Email).RequestId,
                    //AdminId = admin.AdminId,
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    IsEmailSent = new BitArray(1, true),
                    SentTries = 1,
                    ConfirmationNumber = model.ConfirrmationNumber,
                };
                _db.EmailLogs.Add(emailLog);
                _db.SaveChanges();

            }
            TempData["success"] = "Request Created";
            return RedirectToAction("Dashboard");
        }
        
        /// <summary>
        /// Get Profile Page data
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize("Patient")]
        public IActionResult Profile()
        {
            int userId = (int)(HttpContext.Session.GetInt32("Userid"));

            var getProfileData = _ipatientdash.GetProfileData(userId);

            PatientDashData patientdashdata = new PatientDashData()
            {
                profileData = getProfileData,
            };
            return View(patientdashdata);
        }

        /// <summary>
        /// Update Profile Page Data
        /// </summary>
        /// <param name="patientDashData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Profile(PatientDashData patientDashData)

        {
            int userId = (int)(HttpContext.Session.GetInt32("Userid"));
            _ipatientdash.SetProfileData(patientDashData.profileData, userId);
            patientDashData.profileData = _ipatientdash.GetProfileData(userId);

            return View(patientDashData);
        }

        /// <summary>
        /// Get Register Page Action
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public IActionResult Register(string pid)
        {
            var User = _db.Requests.FirstOrDefault(x => x.RequestId == Int32.Parse(_ISecurityRepo.Decrypt(pid)));

            if (pid == null || User == null || User.UserId != null)
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {
                Registervm registervm = new Registervm();
                registervm.RequestId = Int32.Parse(_ISecurityRepo.Decrypt(pid));
                return View(registervm);
            }
        }

        /// <summary>
        /// Post Register Page Action
        /// </summary>
        /// <param name="registervm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Register(Registervm registervm)
        {
            if (registervm.Email != _db.RequestClients.FirstOrDefault(x => x.RequestId == registervm.RequestId).Email)
            {
                TempData["error"] = "Please Enter Valid Email";
                return View(registervm);
            }
            else
            {
                if (registervm.Password == registervm.ConfirmPassword)
                {
                    _iregister.RegisterUser(registervm);
                    TempData["success"] = "User Registered";
                    return RedirectToAction("LandingPage");
                }
                else
                {
                    TempData["error"] = "Password Not Matched";
                    return View(registervm);
                }
            }
        }

       /// <summary>
       /// AccessDenied Page Action
       /// </summary>
       /// <returns></returns>
        public IActionResult AccessDenied()
        {
            return View();
        }


        //********************************************************************************************************************************************************************************************
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
