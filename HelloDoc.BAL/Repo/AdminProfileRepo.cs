using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections;




namespace HelloDoc.BAL.Repo
{
    public class AdminProfileRepo : IAdminProfileRepo
    {

        private readonly ApplicationDbContext _db;
      
        public AdminProfileRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// RoleTable Fetch Records
        /// </summary>
        /// <returns></returns>
        public List<Role> GetRoles()
        {
            return _db.Roles.Where(x => x.IsDeleted == new BitArray(1, false) && x.AccountType == 1).ToList();
        }

        /// <summary>
        /// RegionTable Fetch Records
        /// </summary>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public List<AdminregionTable> GetAdminregions(string aspId)
        {
            var regions = _db.Regions.ToList();
            var adminRegion = _db.AdminRegions.ToList();
            var adminId = _db.Admins.FirstOrDefault(x => x.AspNetUserId == aspId).AdminId;

            var CheckRegion = regions.Select(r1 => new AdminregionTable
            {
                Regionid = r1.RegionId,
                Name = r1.Name,
                ExistsInTable = adminRegion.Any(r2 => r2.RegionId == r1.RegionId && r2.AdminId == adminId),
            }).ToList();
            return CheckRegion;
        }

        /// <summary>
        /// Fetch Records on Admin Profile Page
        /// </summary>
        /// <param name="aspnetuserid"></param>
        /// <returns></returns>
        public AdminProfilevm getAdminProfileData(string aspnetuserid)
        {
            var admindata = _db.Admins.Include(a => a.AspNetUser).FirstOrDefault(i=> i.AspNetUserId == aspnetuserid);
            var regions = _db.AdminRegions.Include(a => a.Region).FirstOrDefault(i =>i.AdminId == admindata.AdminId);

            AdminProfilevm adminProfilevm = new AdminProfilevm()
            {
                AdminId = admindata.AdminId,
                AspNetUserId = aspnetuserid,
                UserName = admindata.FirstName + admindata.LastName,
                Password = admindata.AspNetUser.PasswordHash,
                //status
                RoleId = admindata.RoleId,
                FirstName = admindata.FirstName,
                LastName = admindata.LastName,
                Email = admindata.Email,
                ConfirmEmail = admindata.Email,
                PhoneNumber = admindata.Mobile,
                Address1 = admindata.Address1,
                Address2 = admindata.Address2,
                RegionId = (int)admindata.RegionId,
                City = admindata.City,
                State = regions.Region.Name,
                Zipcode = admindata.Zip,
                AltPhone = admindata.AltPhone,
            };
            return adminProfilevm;
        }

        /// <summary>
        /// Update Records From Admin Profile Page
        /// </summary>
        /// <param name="updatedAdminProfileData"></param>
        public void setAdminProfileData(AdminProfilevm updatedAdminProfileData)
        {
            var admintableData= _db.Admins.FirstOrDefault(x => x.AspNetUserId == updatedAdminProfileData.AspNetUserId);

            admintableData.Address1 = updatedAdminProfileData.Address1;
            admintableData.Address2 = updatedAdminProfileData.Address2;
            admintableData.City = updatedAdminProfileData.City;
            admintableData.RegionId = updatedAdminProfileData.RegionId;
            admintableData.Zip = updatedAdminProfileData.Zipcode;
            admintableData.AltPhone = updatedAdminProfileData.AltPhone;
            _db.SaveChanges();


        }

        /// <summary>
        /// Update Mailing InFo Records From Adminn Profile Page
        /// </summary>
        /// <param name="updateAdminInfoData"></param>
        /// <param name="regions"></param>
        public void setAdminInfoData(AdminProfilevm updateAdminInfoData,List<int> regions)
        {
            var admintableData = _db.Admins.FirstOrDefault(x => x.AspNetUserId == updateAdminInfoData.AspNetUserId);
            var aspData = _db.AspNetUsers.FirstOrDefault(x => x.Id == updateAdminInfoData.AspNetUserId);

            if (aspData != null)
            {
                aspData.Email = updateAdminInfoData.Email;
                aspData.PhoneNumber = updateAdminInfoData.PhoneNumber;
                aspData.ModifiedDate = DateTime.Now;
            }

            if (admintableData.AdminId != null)
            {
                var Adminregion = _db.AdminRegions.Where(i => i.AdminId == updateAdminInfoData.AdminId).ToList();
                _db.AdminRegions.RemoveRange(Adminregion);
            }

            if (admintableData != null)
            {
                admintableData.Email = updateAdminInfoData.Email;
                admintableData.Mobile = updateAdminInfoData.PhoneNumber;
                admintableData.ModifiedBy = updateAdminInfoData.AspNetUserId;
                admintableData.ModifiedDate = DateTime.Now;
                _db.SaveChanges();
            }

            foreach (int regionid in regions)
            {
                Region? region = _db.Regions.FirstOrDefault(r => r.RegionId == regionid);

                _db.AdminRegions.Add(new AdminRegion
                {
                    AdminId = updateAdminInfoData.AdminId,
                    RegionId = regionid,
                });
            }
            _db.SaveChanges();

            
        }

        /// <summary>
        /// Update Password From Admin Profile Page
        /// </summary>
        /// <param name="updatedAdminPassword"></param>
        public void setPasswordData(AdminProfilevm updatedAdminPassword)
        {
            var aspusertableData = _db.AspNetUsers.FirstOrDefault(x => x.Id == updatedAdminPassword.AspNetUserId);

            aspusertableData.PasswordHash = updatedAdminPassword.Password;

            _db.SaveChanges();
        }

        /// <summary>
        /// Delee Admin
        /// </summary>
        /// <param name="adminId"></param>
        public void RemoveAdmin(int adminId)
        {
            var admin = _db.Admins.FirstOrDefault(x => x.AdminId == adminId);

            admin.IsDeleted = new BitArray(1,true);
            _db.SaveChanges();
        }

        /// <summary>
        /// Create Admin Account 
        /// </summary>
        /// <param name="adminProfilevm"></param>
        /// <param name="adminRegions"></param>
        /// <returns></returns>
        public bool CreateAdminAccount(AdminProfilevm adminProfilevm, List<int> adminRegions)

        {
            if (!_db.AspNetUsers.Any(x => x.Email == adminProfilevm.Email))
            {
                var aspData = new AspNetUser()
                {
                    //Id = 27,
                    UserName = adminProfilevm.LastName + adminProfilevm.FirstName.Substring(0, 1).ToUpper(),
                    PasswordHash = adminProfilevm.CreateAdminPassword,
                    Email = adminProfilevm.Email,
                    PhoneNumber = adminProfilevm.PhoneNumber,
                    CreatedDate = DateTime.Now,
                   
                };
                _db.AspNetUsers.Add(aspData);
                _db.SaveChanges();

                var adminData = new Admin()
                {
                    //Adminid = 3,
                    AspNetUserId = aspData.Id,
                    FirstName = adminProfilevm.FirstName,
                    LastName = adminProfilevm.LastName,
                    Email = adminProfilevm.Email,
                    Mobile = adminProfilevm.PhoneNumber,
                    Address1 = adminProfilevm.Address1,
                    Address2 = adminProfilevm.Address2,
                    City = adminProfilevm.City,
                    RegionId = adminProfilevm.RegionId,
                    Zip = adminProfilevm.Zipcode,
                    AltPhone = adminProfilevm.AltPhone,
                    CreatedBy = adminProfilevm.AspNetUserId,
                    CreatedDate = DateTime.Now,
                    Status = 1,
                    RoleId = adminProfilevm.RoleId,              
                };
                _db.Admins.Add(adminData);
                _db.SaveChanges();

                AspNetUserRole anur = new AspNetUserRole();
                anur.UserId = aspData.Id;
                anur.RoleId = "1";
                _db.AspNetUserRoles.Add(anur);
                _db.SaveChanges();

                foreach (int regionId in adminRegions)
                {
                    var region = _db.Regions.FirstOrDefault(x => x.RegionId == regionId);

                    _db.AdminRegions.Add(new AdminRegion
                    {
                        AdminId = adminData.AdminId,
                        RegionId = region.RegionId,
                    });
                }
                _db.SaveChanges();

                return true;
            }
            return false;
        }

    }
}


