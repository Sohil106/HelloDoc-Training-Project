using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using System.Collections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;

namespace HelloDoc.BAL.Repo
{
    public class PatientReqRepo : IPatientReqRepo
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _environment;
        public PatientReqRepo(ApplicationDbContext db, IHostingEnvironment environment) 
        {
            _db = db;
            _environment = environment;
        }

        /// <summary>
        /// Fetch Records From RegionTable
        /// </summary>
        /// <returns></returns>
        public List<Region> GetRegions()
        {
            return _db.Regions.ToList();
        }

        /// <summary>
        /// Add Records To AspNetUser table And User Table
        /// </summary>
        /// <param name="model"></param>
        public void AddToUser(PatientReqData model)
        {
            AspNetUser anu = new AspNetUser();
            AspNetUserRole anur = new AspNetUserRole();
            User u = new User();

            anu.UserName = model.FirstName + model.LastName;
            anu.PasswordHash = model.PasswordHash;
            anu.Email = model.Email;
            anu.PhoneNumber = model.PhoneNumber;
            anu.CreatedDate = DateTime.Now;

            _db.AspNetUsers.Add(anu);
            _db.SaveChanges();
            model.AspNetUserId = anu.Id;

            anur.UserId = model.AspNetUserId;
            anur.RoleId = "3";
            _db.AspNetUserRoles.Add(anur);
            _db.SaveChanges();


            u.AspNetUserId = model.AspNetUserId;
            u.FirstName = model.FirstName;
            u.LastName = model.LastName;
            u.Email = model.Email;
            u.Mobile = model.PhoneNumber;
            u.Street = model.Street;
            u.City = model.City;
            u.State = model.State;
            u.ZipCode = model.ZipCode;
            u.CreatedBy = model.AspNetUserId;
            u.CreatedDate = DateTime.Now;
            var dt = model.BirthDate;
            u.IntDate = dt.Day;
            u.StrMonth = dt.Month.ToString();
            u.IntYear = dt.Year;
            _db.Users.Add(u);
            _db.SaveChanges();

            model.UserId = u.UserId;
        }

        /// <summary>
        /// Add Records To RequestTable
        /// </summary>
        /// <param name="model"></param>
        public void AddToRequest(PatientReqData model)
        {
            User? user = _db.Users.FirstOrDefault(i => i.Email == model.Email);
           
            Request r = new Request();

            r.UserId = user == null ? null : user.UserId;
            r.FirstName = model.OtherFirstName;
            r.LastName = model.OtherLastName;
            r.Email = model.OtherEmail;
            r.PhoneNumber = model.OtherPhoneNumber;
            r.RequestTypeId = model.RequestTypeId;
            r.Status = 1;
            r.CreatedDate = DateTime.Now;
            r.IsUrgentEmailSent = new BitArray(new bool[1] { true });

            int requestCount = _db.Requests.Count(i => i.CreatedDate.Date == DateTime.Now.Date) + 1;
            string confirmation = "MD" + DateTime.Now.Day.ToString("D2") + DateTime.Now.Month.ToString("D2") + DateTime.Now.Year.ToString().Substring(2, 2) + r.LastName.Remove(2).ToUpper() + r.FirstName.Remove(2).ToUpper() + requestCount.ToString("D4");

            r.ConfirmationNumber = confirmation;
            r.RequestId = model.RequestId;

            _db.Requests.Add(r);
            _db.SaveChanges();

            model.RequestId = r.RequestId;
            model.ConfirrmationNumber = confirmation;
        }

        /// <summary>
        /// Add Records To RequestClient Table
        /// </summary>
        /// <param name="model"></param>
        public void AddToReqClient(PatientReqData model)
        {
            RequestClient rc = new RequestClient();

            rc.Email = model.Email;
            rc.FirstName = model.FirstName;
            rc.LastName = model.LastName;
            rc.PhoneNumber = model.PhoneNumber;
            rc.Notes = model.Notes;
            rc.RequestId = model.RequestId;
            rc.StrMonth = model.StrMonth;
            rc.Street = model.Street;
            rc.City = model.City;
            rc.State = model.State;
            rc.ZipCode = model.ZipCode;
            rc.Address = model.Address;
            var dt = model.BirthDate;
            rc.IntDate = dt.Day;
            rc.StrMonth = dt.Month.ToString();
            rc.IntYear = dt.Year;
            rc.RegionId = model.RegionId;

            _db.RequestClients.Add(rc);
            _db.SaveChanges();
        }

        /// <summary>
        /// Add Records To RequestClient Table
        /// </summary>
        /// <param name="model"></param>
        public void AddToReqWIseFile(PatientReqData model)
        {
            RequestWiseFile rwf = new RequestWiseFile();

            rwf.RequestId = model.RequestId;
            rwf.FileName = model.FileName;
            rwf.CreatedDate = DateTime.Now;

            _db.RequestWiseFiles.Add(rwf);
            _db.SaveChanges();
        }

        /// <summary>
        /// Add Records To RequestWiseFile Table
        /// </summary>
        /// <param name="model"></param>
        public void UploadFile(PatientReqData model)
        {
            string path = _environment.WebRootPath;
            string filePath = "content/" + model.FileName;
            string fullPath = Path.Combine(path, filePath);

            IFormFile file1 = model.Upload;

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                file1.CopyTo(stream);
            }

            RequestWiseFile rwf = new RequestWiseFile();

            rwf.RequestId = model.RequestId;
            rwf.FileName = !string.IsNullOrEmpty(model.FileName) ? model.FileName : null;
            rwf.CreatedDate = DateTime.Now;

            _db.Add(rwf);
            _db.SaveChanges();
        }

        /// <summary>
        /// Add Records to Concierge Table
        /// </summary>
        /// <param name="model"></param>
        public void AddToConcierge(PatientReqData model)
        {
            Concierge con = new Concierge();

            con.ConciergeName = model.OtherFirstName + model.OtherLastName;
            con.Address = model.HostelName;
            con.Street = model.Street;
            con.State = model.State;
            con.City = model.City;
            con.State = model.State;
            con.ZipCode = model.ZipCode;
            con.CreatedDate = DateTime.Now;
            con.RegionId = 1;

            _db.Concierges.Add(con);
            _db.SaveChanges();
            model.ConciergeId= con.ConciergeId;
        }
        /// <summary>
        /// Add Records To RequestConcierge Table
        /// </summary>
        /// <param name="model"></param>
        public void AddToReqConcierge(PatientReqData model)
        {
            RequestConcierge rcon = new RequestConcierge();

            rcon.ConciergeId = model.ConciergeId;
            rcon.RequestId = model.RequestId;

            _db.RequestConcierges.Add(rcon);
            _db.SaveChanges();

            model.ConciergeReqId = rcon.Id;
        }
        
        /// <summary>
        /// Email check method if Email is Exist Or Not in User Table
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public bool EmailCheck(String Email)
        {
            return _db.Users.Any(x => x.Email == Email); 
        }

        /// <summary>
        /// fetch UserId From User Table
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public int GetUserId(String email)

        {
            var user= _db.Users.FirstOrDefault(x => x.Email == email);
            return user.UserId;
        }

        /// <summary>
        /// Add Records To RequestBusiness Table
        /// </summary>
        /// <param name="model"></param>
        public void AddToReqBusiness(PatientReqData model)
        {
            var rbus = new RequestBusiness()
            {
                BusinessId = model.BusinessId,
                RequestId = model.RequestId,

          
            };
            _db.RequestBusinesses.Add(rbus);
            _db.SaveChanges(); 
            model.BusinessId=rbus.BusinessId;

        }

        /// <summary>
        /// Add Records To Business Table
        /// </summary>
        /// <param name="model"></param>
        public void AddToBusiness(PatientReqData model)
        {

            var bus = new Business()
            {
                Name = model.OtherFirstName + model.OtherLastName,
                CreatedDate = DateTime.Now,
                RegionId = 1,
                Address1 = model.HostelName

            };
            _db.Businesses.Add(bus);
            _db.SaveChanges();
            model.BusinessId = bus.BusinessId;
        }

        /// <summary>
        /// Send Mail Method For Create Account
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task  EmailSender(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.sohilvekariya123@outlook.com";
            var password = "Devjibapa@2023";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };
            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }

        /// <summary>
        /// Fetch Records From RequestClient Table On ReviewAgreemen Page
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public ReviewAgreementvm GetReviewAgreement(int reqId)
        {
            if(_db.Requests.Any(i=>i.RequestId == reqId))
            {
                var patient = _db.RequestClients.FirstOrDefault(i =>i.RequestId == reqId);

                var reviewData = new ReviewAgreementvm()
                {
                    RequestId = reqId,
                    PatientName = patient.FirstName + " " + patient.LastName,
                };
                return reviewData;
            }
            return null;
        }

        /// <summary>
        /// Update Records On RequestStatusLog to changes status after Agree on Agreement
        /// </summary>
        /// <param name="reqId"></param>
        public void AgreeReview(int reqId)
        {
            var request = _db.Requests.FirstOrDefault(i=>i.RequestId == reqId);

            var agreeData = new RequestStatusLog()
            {
                RequestId = reqId,
                Status = 4,
                Notes = "Review is Agreed",
                CreatedDate = DateTime.Now,

            };
            request.Status = 4;
            _db.RequestStatusLogs.Add(agreeData);
            _db.SaveChanges();

        }

        /// <summary>
        /// Update Records On RequestStatusLog to changes status after Cancel Agreement
        /// </summary>
        /// <param name="reviewAgreementvm"></param>
        public void CancelReview(ReviewAgreementvm reviewAgreementvm)
        {
            var request = _db.Requests.FirstOrDefault(i => i.RequestId == reviewAgreementvm.RequestId);

            var cancelData = new RequestStatusLog()
            {
                RequestId = request.RequestId,
                Status = 7,
                Notes = reviewAgreementvm.CancellationNotes,
                CreatedDate = DateTime.Now,
            };

            request.Status = 7;
            _db.RequestStatusLogs.Add(cancelData);
            _db.SaveChanges();
        }
    }
}

