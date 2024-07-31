using Business_Logic.Interface;
using Data_Access.Dash;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HelloDoc.DAL.Repo
{ 
    public class PatientDashRepo : IPatientDashRepo
    {
        public readonly ApplicationDbContext _Context;
        public readonly PatientDashData _patieniDashData;
        public readonly IHostingEnvironment _environment; 

        public PatientDashRepo(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _Context = context;
            _environment = environment;

        }

        /// <summary>
        /// Featch Request Table Records On Patient Dashboard
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<DashboardData> RequestList(int userid)
        {
            var request = _Context.Requests.Where(r => r.UserId == userid).AsNoTracking();
            
            var RequestList = request.Select(r => new DashboardData()
            {
                CreatedDate = r.CreatedDate,
                Status = r.Status,
                DocumentCount = r.RequestWiseFiles.Select(f => f.FileName).Count(),
                RequestId = r.RequestId,
                PhysicianId = r.PhysicianId,
                Provider = r.Physician.FirstName + " " + r.Physician.LastName
            }).ToList();
            return RequestList;
        }

        /// <summary>
        /// Featch RequestWiseFile Table Records On ViewDocs Page
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public List<DocumentData> DocumentList(int reqId)
        {
            var requestWiseFile = _Context.RequestWiseFiles.Where(rwf => rwf.RequestId == reqId).AsNoTracking();

            var DocumentList = requestWiseFile.Select(rwf => new DocumentData()
            {
                //CreatedDate = rwf.CreatedDate,
                CreatedDate = DateTime.Now,
                DocumentName = rwf.FileName,
            }).ToList();

            return DocumentList;
        }

        /// <summary>
        /// Upload Files On VieDocumets Page  
        /// </summary>
        /// <param name="patientDashData"></param>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public bool DashboardUpload(PatientDashData patientDashData, int reqId)
        {
            string path = _environment.WebRootPath;
            string filePath = "content/" + patientDashData.Upload.FileName;
            string fullPath = Path.Combine(path, filePath);

            IFormFile file1 = patientDashData.Upload;
            FileStream stream = new FileStream(fullPath, FileMode.Create);
            file1.CopyTo(stream);

            var fileName = patientDashData.Upload?.FileName;
            var doctType = patientDashData.Upload?.ContentType;

            var file = new RequestWiseFile()
            {
                RequestId = reqId,
                FileName = fileName,
                Ip = doctType,
            };
            _Context.Add(file);
            _Context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Fetch Records On Patient Profie Page
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ProfileData GetProfileData(int userId)
        {
            var userData = _Context.Users.FirstOrDefault(u => u.UserId == userId);

                var BirthDay = Convert.ToInt32(userData.IntDate);
                var BirthMonth = Convert.ToInt32(userData.StrMonth);
                var BirthYear = Convert.ToInt32(userData.IntYear);


            var profileData = new ProfileData()
            {
                Firstname = userData.FirstName,
                LastName = userData.LastName,
                PhoneNumber = userData.Mobile,
                Email = userData.Email,
                Street = userData.Street,
                City = userData.City,
                State = userData.State,
                Zipcode = userData.ZipCode,
                BirthDate = new DateTime(BirthYear, BirthMonth, BirthDay),
            };
            return profileData;
        }

        /// <summary>
        /// Update Records On Patient Profile Page
        /// </summary>
        /// <param name="updatedProfileData"></param>
        /// <param name="userId"></param>
        public void SetProfileData(ProfileData updatedProfileData, int userId)
        {
            var profileRecord = _Context.Users.FirstOrDefault(u => u.UserId == userId);

            if (profileRecord != null)
            {
                profileRecord.FirstName = updatedProfileData.Firstname;
                profileRecord.LastName = updatedProfileData.LastName;
                profileRecord.Mobile = updatedProfileData.PhoneNumber;
                profileRecord.Street = updatedProfileData.Street;
                profileRecord.City = updatedProfileData.City;
                profileRecord.State = updatedProfileData.State;
                profileRecord.ZipCode = updatedProfileData.Zipcode;
                var dt = updatedProfileData.BirthDate;
                profileRecord.IntDate = dt.Day;
                profileRecord.StrMonth = dt.Month.ToString();
                profileRecord.IntYear = dt.Year;
            }
            _Context.SaveChanges();
        }
    }
}