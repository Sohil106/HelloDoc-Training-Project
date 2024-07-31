using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using System.Collections;

namespace HelloDoc.BAL.Repo
{
    public class PartnersRepo : IPartnersRepo
    {
        private readonly ApplicationDbContext _db;

        public PartnersRepo(ApplicationDbContext db)
        {
            _db = db;
        }

       /// <summary>
       /// Fetch HealthProfessionalType Table Records
       /// </summary>
       /// <returns></returns>
        public List<HealthProfessionalType> GetProfession()
        {
            var professions = _db.HealthProfessionalTypes.ToList();
            return professions;
        }

        /// <summary>
        /// Fetch HEalthProfessional Table Records Based On Type
        /// </summary>
        /// <param name="professionid"></param>
        /// <returns></returns>
        public List<Partnersdata> GetPartnersdata(int professionid)
        {
            var vendor = _db.HealthProfessionals.Where(r => r.IsDeleted == null).ToList();
            if (professionid != 0)
            {
                vendor = vendor.Where(i => i.Profession == professionid).ToList();
            }
            var Partnersdata = vendor.Select(r => new Partnersdata()
            {
                VendorId = r.VendorId,
                VendorName = r.VendorName,
                ProfessionName = _db.HealthProfessionalTypes.FirstOrDefault(i => i.HealthProfessionalId == r.Profession).ProfessionName,
                VendorEmail = r.Email,
                FaxNo = r.FaxNumber,
                PhoneNo = r.PhoneNumber,
                Businesscontact = r.BusinessContact,
            }).ToList();
            return Partnersdata;
        }

        /// <summary>
        /// Create Accout Of HealthProfessionals On AddBusiness Page
        /// </summary>
        /// <param name="partnersvm"></param>
        /// <param name="LoggerAspnetuserId"></param>
        /// <returns></returns>
        public bool CreateNewBusiness(Partnersvm partnersvm, string LoggerAspnetuserId)
        {
            if (!_db.HealthProfessionals.Any(x => x.Email == partnersvm.Email))
            {
                var healthprof = new HealthProfessional()
                {
                    VendorName = partnersvm.BusinessName,
                    Profession = partnersvm.SelectedhealthprofID,
                    FaxNumber = partnersvm.FAXNumber,
                    PhoneNumber = partnersvm.Phonenumber,
                    Email = partnersvm.Email,
                    BusinessContact = partnersvm.BusinessContact,
                    Address = partnersvm.Street,
                    City = partnersvm.City,
                    RegionId = partnersvm.RegionId,
                    Zip = partnersvm.Zip,
                    State = _db.Regions.FirstOrDefault(i => i.RegionId == partnersvm.RegionId).Name,
                   ModifiedDate = DateTime.Now.Date
            };
                _db.HealthProfessionals.Add(healthprof);
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Fetch Records On Edit Bussiness Page of HealthProfessional Table
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public Partnersvm GetEditBusinessData(int vendorID)
        {
            var vendorDetails = _db.HealthProfessionals.FirstOrDefault(i => i.VendorId == vendorID);
            var partnersvm = new Partnersvm()
            {
                BusinessName = vendorDetails.VendorName,
                SelectedhealthprofID = vendorDetails.Profession,
                FAXNumber = vendorDetails.FaxNumber,
                Phonenumber = vendorDetails.PhoneNumber,
                Email = vendorDetails.Email,
                BusinessContact = vendorDetails.BusinessContact,
                Street = vendorDetails.Address,
                City = vendorDetails.City,
                RegionId = vendorDetails.RegionId,
                Zip = vendorDetails.Zip,

            };
            return partnersvm;
        }

        /// <summary>
        /// Update Records On Edit Business Page Of HeatlhProfeesional Table
        /// </summary>
        /// <param name="partnersvm"></param>
        /// <returns></returns>
        public bool UpdateBusiness(Partnersvm partnersvm)
        {
            var vendorDetails = _db.HealthProfessionals.FirstOrDefault(i => i.VendorId == partnersvm.vendorID);
            if (partnersvm.BusinessName != vendorDetails.VendorName || partnersvm.SelectedhealthprofID != vendorDetails.Profession || partnersvm.FAXNumber != vendorDetails.FaxNumber
            || partnersvm.Phonenumber != vendorDetails.PhoneNumber || /*partnersvm.Email != vendorDetails.Email || */partnersvm.BusinessContact != vendorDetails.BusinessContact
            || partnersvm.Street != vendorDetails.Address || partnersvm.City != vendorDetails.City || partnersvm.RegionId != vendorDetails.RegionId || partnersvm.Zip != vendorDetails.Zip)
            {
                vendorDetails.VendorName = partnersvm.BusinessName;
                vendorDetails.Profession = partnersvm.SelectedhealthprofID;
                vendorDetails.FaxNumber = partnersvm.FAXNumber;
                vendorDetails.PhoneNumber = partnersvm.Phonenumber;
                //vendorDetails.Email = partnersvm.Email;
                vendorDetails.BusinessContact = partnersvm.BusinessContact;
                vendorDetails.Address = partnersvm.Street;
                vendorDetails.City = partnersvm.City;
                vendorDetails.RegionId = partnersvm.RegionId;
                vendorDetails.Zip = partnersvm.Zip;
                vendorDetails.State = _db.Regions.FirstOrDefault(i => i.RegionId == partnersvm.RegionId).Name;
                vendorDetails.ModifiedDate = DateTime.Now.Date;
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Deleat HealthProfeesional Account
        /// </summary>
        /// <param name="VendorID"></param>
        public void DelPartner(int VendorID)
        {
            var vendor = _db.HealthProfessionals.FirstOrDefault(x => x.VendorId == VendorID);
            if (vendor != null)
            {
                vendor.IsDeleted = new BitArray(1, true);
                _db.SaveChanges();
            }
        }
    }
}
