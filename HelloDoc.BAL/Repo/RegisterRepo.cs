using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HelloDoc.BAL.Repo
{
    public class RegisterRepo : IRegisterRepo
    {
        private readonly ApplicationDbContext _db;

        public RegisterRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Add Records in AspNetUser,User Table and Update Data In Request Table Via Register Page
        /// </summary>
        /// <param name="registerVm"></param>
        public void RegisterUser(Registervm registerVm)
        {
            var data = new AspNetUser()
            {
                UserName = registerVm.Email.Length <= 4 ? registerVm.Email : registerVm.Email.Substring(0, 4),
                Email = registerVm.Email,
                PasswordHash = registerVm.Password,
                CreatedDate = DateTime.Now,
            };
            _db.AspNetUsers.Add(data);
            _db.SaveChanges();

            AspNetUser? aspnetuser = _db.AspNetUsers.FirstOrDefault(i => i.Email == registerVm.Email);
            string AspnetuserId = aspnetuser.Id;
            string Aspnetusername = aspnetuser.UserName;


            var clientData = _db.RequestClients.FirstOrDefault(u => u.Email == registerVm.Email);
            


            var data1 = new User()
            {
                FirstName = clientData.FirstName,
                LastName = clientData.LastName,
                Email = registerVm.Email,
                Mobile = clientData.PhoneNumber,
                Street = clientData.Street,
                City = clientData.City,
                State = clientData.State,
                ZipCode = clientData.ZipCode,
                StrMonth = clientData.StrMonth,
                IntDate = clientData.IntDate,
                IntYear = clientData.IntYear,
                AspNetUserId = AspnetuserId,
                CreatedBy = clientData.FirstName +" "+ clientData.LastName,
                CreatedDate = DateTime.Now,
                

        };
            _db.Users.Add(data1);
            _db.SaveChanges();

            User? user = _db.Users.FirstOrDefault(i => i.Email == registerVm.Email);
            int userId = user.UserId;

            RequestClient? request = _db.RequestClients.FirstOrDefault(i => i.Email == registerVm.Email);
            int Requestid = request.RequestId;
            string PhoneNo = request.PhoneNumber;

            Request? request1 = _db.Requests.FirstOrDefault(i => i.RequestId == Requestid);
         
             request1.UserId = userId;
           
            _db.SaveChanges();
            _db.Update(request1);

            AspNetUser? asp1 =_db.AspNetUsers.FirstOrDefault(i => i.Email == registerVm.Email);
            asp1.PhoneNumber = PhoneNo;

            _db.SaveChanges();
            _db.Update(asp1);


            AspNetUser? aspuser = _db.AspNetUsers.FirstOrDefault(i => i.Email == registerVm.Email);

            AspNetUserRole anur = new AspNetUserRole();
            anur.UserId = aspuser.Id;
            anur.RoleId = "3";
            _db.AspNetUserRoles.Add(anur);
            _db.SaveChanges();



        }

        /// <summary>
        /// Update Password 
        /// </summary>
        /// <param name="registerVm"></param>
        public void ResetPassword(Registervm registerVm)
        {
            AspNetUser? aspnetuser = _db.AspNetUsers.FirstOrDefault(f => f.Email == registerVm.Email);
            if (aspnetuser != null)
            {
                aspnetuser.PasswordHash = registerVm.Password;
                aspnetuser.Otp = null;
            }

            _db.SaveChanges();
            _db.Update(aspnetuser);
        }

        /// <summary>
        /// Fetch Records by Joining three tables AspNetUSers,AspNetUserRole And Role 
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public AspNetUser GetUserRole(string Email)
        {
            var role = _db.AspNetUsers.Include(x => x.AspNetUserRole).ThenInclude(y => y.Role).FirstOrDefault(x => x.Email == Email);
            return role;
        }
    }
}
