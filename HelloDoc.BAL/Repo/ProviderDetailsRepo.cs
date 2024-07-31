using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using System.Collections;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace HelloDoc.BAL.Repo
{
    public class ProviderDetailsRepo : IProviderDetailsRepo
    {

        private readonly ApplicationDbContext _db;

        public ProviderDetailsRepo(ApplicationDbContext db)
        {
            _db = db;

        }

        /// <summary>
        /// Fetch Records From Region Table
        /// </summary>
        /// <returns></returns>
        public List<Region> getRegions()
        {
            var regions = _db.Regions.ToList();
            return regions;
        }

        /// <summary>
        /// Fetch Records On Providers Page
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public List<Provider> GetProviders(int regionId)
        {
            //var physicians = _db.Physicians.ToList();
            var physicians = (from p in _db.Physicians
                              join r in _db.Roles.Where(x => x.IsDeleted == new BitArray(1, false))
                              on p.RoleId equals r.RoleId
                              select p).ToList();

            physicians = physicians.Where(x => x.IsDeleted == null).ToList();

            if (regionId != 0)
            {
                physicians = physicians.Where(x => x.RegionId == regionId).ToList();
            }

            var providerList = physicians.Select(x => new Provider()
            {
                PhysicianId = x.PhysicianId,
                Email = x.Email,
                aspId = x.AspNetUserId,
                Name = x.FirstName + " " + x.LastName,
                Role = _db.Roles.FirstOrDefault(i => i.RoleId == x.RoleId).Name,
                CallStatus = "Un Available",
                Status = (short)x.Status,
                IsNotificatonStopped = _db.PhysicianNotifications.FirstOrDefault(i=> i.PhysicianId == x.PhysicianId)?.IsNotificationStopped != null && _db.PhysicianNotifications.FirstOrDefault(i => i.PhysicianId == x.PhysicianId)?.IsNotificationStopped[0] == true,
            }).ToList();

            return providerList;
        }

        /// <summary>
        /// for stopping  mail or sms to provider
        /// </summary>
        /// <param name="PhysicianId"></param>
        /// <returns></returns>
        public bool stopNotification(int PhysicianId)
        {
            var phynotification = _db.PhysicianNotifications.FirstOrDefault(x=>x.PhysicianId == PhysicianId);

            if(phynotification == null)
            {
                PhysicianNotification pn = new PhysicianNotification();
                pn.PhysicianId = PhysicianId;
                pn.IsNotificationStopped = new BitArray(1, true);
                _db.PhysicianNotifications.Add(pn);
                _db.SaveChanges();
                return true;
            }
            else
            {
                if (phynotification.IsNotificationStopped[0] == true)
                {
                    phynotification.IsNotificationStopped = new BitArray(1,false);
                    _db.SaveChanges();
                    return false;
                }
                else
                {
                    phynotification.IsNotificationStopped = new BitArray(1, true);
                    _db.SaveChanges();
                    return true;
                }
            }
        }

        /// <summary>
        /// Send Mail to Selected Provider and Add Records to EmailLog Table
        /// </summary>
        /// <param name="providerDetailsvm"></param>
        /// <param name="aspId"></param>
        public void ContactProvider(ProviderDetailsvm providerDetailsvm, string aspId)
        {
            var admin = _db.Admins.FirstOrDefault(x => x.AspNetUserId == aspId);
            var physician = _db.Physicians.FirstOrDefault(x => x.Email == providerDetailsvm.Email);

            var mail = "tatva.dotnet.sohilvekariya123@outlook.com";
            var password = "Devjibapa@2023";
            var email = providerDetailsvm.Email;
            var subject = "Message From Admin";
            var message = $"Hey, {providerDetailsvm.Email.Substring(0, providerDetailsvm.Email.IndexOf('@'))} <br><br>" +
                $"Here is the message from {admin.FirstName} {admin.LastName} : <br>" +
                $"{providerDetailsvm.ContactMessage}";

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
                RoleId = 2,
                AdminId = admin.AdminId,
                PhysicianId = physician.PhysicianId,
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsEmailSent = new BitArray(1, true),
                SentTries = 1,
            };

            _db.EmailLogs.Add(emailLog);
            _db.SaveChanges();
        }

        /// <summary>
        /// Add Recrods SMSLog Table 
        /// </summary>
        /// <param name="providerDetailsvm"></param>
        /// <param name="aspId"></param>
        public void ContactSMSProvider(ProviderDetailsvm providerDetailsvm, string aspId)
        {
            var admin = _db.Admins.FirstOrDefault(x => x.AspNetUserId == aspId);
            var physician = _db.Physicians.FirstOrDefault(x => x.Email == providerDetailsvm.Email);
            
            var smsLog = new Smslog()
            {

               
                Smstemplate = "sms",
                MobileNumber = physician.Mobile,
                RoleId = 2,
                AdminId = admin.AdminId,
                PhysicianId = physician.PhysicianId,
                CreateDate = DateTime.Now,
                SentDate = DateTime.Now,
                IsSmssent = new BitArray(1, true),
                SentTries = 1,
            };

            _db.Smslogs.Add(smsLog);
            _db.SaveChanges();
        }

        /// <summary>
        /// Fetch Records From PhysicianRegion Table,Region table and Physician Table
        /// </summary>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public List<PhysicianRegionTable> GetPhysicianRegionTables(string aspId)
        {
            var region = _db.Regions.ToList();
            var physicianRegion = _db.PhysicianRegions.ToList();
            var physicanId = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId).PhysicianId;

            var checkedRegion = region.Select(r1 => new PhysicianRegionTable
            {
                Regionid = r1.RegionId,
                Name = r1.Name,
                ExistsInTable = physicianRegion.Any(r2 => r2.RegionId == r1.RegionId && r2.PhysicianId == physicanId)

            }).ToList();

            return checkedRegion;
        }

        /// <summary>
        /// Fetch Records From Role Table
        /// </summary>
        /// <returns></returns>
        public List<Role> GetRoles()
        {

            return _db.Roles.Where(x => x.IsDeleted == new BitArray(1, false) && x.AccountType == 2).ToList();
        }

        /// <summary>
        /// Fetch Records of Physician Table On Edit Provider Page
        /// </summary>
        /// <param name="aspId"></param>
        /// <returns></returns>
        public ProviderProfilevm GetProviderProfileData(string aspId)
        {
            var aspData = _db.AspNetUsers.FirstOrDefault(x => x.Id == aspId);

            if (aspData != null)
            {
                var provider = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId);

                ProviderProfilevm providerData = new ProviderProfilevm()
                {
                    AspId = provider.AspNetUserId,
                    PhysicianId = provider.PhysicianId,
                    Username = aspData.UserName,
                    Status = provider.Status,
                    RoleId = provider.RoleId,
                    FirstName = provider.FirstName,
                    LastName = provider.LastName,
                    Email = provider.Email,
                    Phonenumber = provider.Mobile,
                    MedicalLicense = provider.MedicalLicense,
                    NPINumber = provider.Npinumber,
                    SyncEmail = provider.SyncEmailAddress,
                    Address1 = provider.Address1,
                    Address2 = provider.Address2,
                    City = provider.City,
                    RegionId = provider.RegionId,
                    Zipcode = provider.Zip,
                    AltPhone = provider.AltPhone,
                    BusinessName = provider.BusinessName,
                    BusinessWebsite = provider.BusinessWebsite,
                    PhotoValue = provider.Photo,
                    SignatureValue = provider.Signature,
                    AdminNotes = provider.AdminNotes,
                    IsContractorAgreement = provider.IsAgreementDoc == null ? false : true,
                    IsBackgroundCheck = provider.IsBackgroundDoc == null ? false : true,
                    IsHIPAA = provider.IsTrainingDoc == null ? false : true,
                    IsNonDisclosure = provider.IsNonDisclosureDoc == null ? false : true,
                    IsLicenseDocument = provider.IsLicenseDoc == null ? false : true,
                };
                return providerData;
            }
            return null;
        }
        
        /// <summary>
        /// Reset Password Of Provider
        /// </summary>
        /// <param name="password"></param>
        /// <param name="aspId"></param>
        public void PhysicianResetPassword(String password, string aspId)
        {
            var Aspnetuser = _db.AspNetUsers.FirstOrDefault(x => x.Id == aspId);

            Aspnetuser.PasswordHash = password;
            Aspnetuser.ModifiedDate = DateTime.Now;

            _db.SaveChanges();
        }

        /// <summary>
        /// Update Account Records on Physicain table From Edit Provider Page
        /// </summary>
        /// <param name="status"></param>
        /// <param name="roleId"></param>
        /// <param name="aspId"></param>
        /// <param name="username"></param>
        public void PhysicianAccountUpdate(short status, int roleId, string aspId, string username)
        {
            var physician = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId);
            var aspuser = _db.AspNetUsers.FirstOrDefault(x => x.Id == aspId);

            if (physician != null)
            {
                //aspuser.UserName = username;
                physician.Status = status;
                physician.RoleId = roleId;
                physician.ModifiedDate = DateTime.Now;
                physician.ModifiedBy = aspId;
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Update Admministrator Records on Physicain table From Edit Provider Page
        /// </summary>
        /// <param name="providerProfilevm"></param>
        /// <param name="physicianRegions"></param>
        public void PhysicianAdministratorDataUpdate(ProviderProfilevm providerProfilevm, List<int> physicianRegions)
        {
            var physician = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == providerProfilevm.AspId);

            if (physician != null)
            {
                physician.FirstName = providerProfilevm.FirstName;
                physician.LastName = providerProfilevm.LastName;
                physician.Mobile = providerProfilevm.Phonenumber;
                physician.MedicalLicense = providerProfilevm.MedicalLicense;
                physician.Npinumber = providerProfilevm.NPINumber;
                physician.SyncEmailAddress = providerProfilevm.SyncEmail;
                physician.ModifiedDate = DateTime.Now;
                physician.ModifiedBy = providerProfilevm.AspId;

                if (_db.PhysicianRegions.Any(x => x.PhysicianId == physician.PhysicianId))
                {
                    var physicianRegion = _db.PhysicianRegions.Where(x => x.PhysicianId == physician.PhysicianId).ToList();

                    _db.PhysicianRegions.RemoveRange(physicianRegion);
                    _db.SaveChanges();
                }

                foreach (int regionId in physicianRegions)
                {
                    var region = _db.Regions.FirstOrDefault(x => x.RegionId == regionId);

                    _db.PhysicianRegions.Add(new PhysicianRegion
                    {
                        PhysicianId = providerProfilevm.PhysicianId,
                        RegionId = region.RegionId,
                    });
                }
                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Update Miling Records on Physicain table From Edit Provider Page
        /// </summary>
        /// <param name="providerProfilevm"></param>
        public void PhysicianMailingDataUpdate(ProviderProfilevm providerProfilevm)
        {
            var physician = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == providerProfilevm.AspId);
            var phylocation = _db.PhysicianLocations.FirstOrDefault(x => x.PhysicianId == providerProfilevm.PhysicianId);

            if (physician != null)
            {
                physician.Address1 = providerProfilevm.Address1;
                physician.Address2 = providerProfilevm.Address2;
                physician.City = providerProfilevm.City;
                physician.RegionId = providerProfilevm.RegionId;

                physician.Zip = providerProfilevm.Zipcode;
                physician.AltPhone = providerProfilevm.AltPhone;
                physician.ModifiedBy = providerProfilevm.AspId;
                physician.ModifiedDate = DateTime.Now;

                _db.SaveChanges();
            }

            if (phylocation != null)
            {
                phylocation.Latitude = providerProfilevm.Latitude;
                phylocation.Longitude = providerProfilevm.Longitude;
                phylocation.Address = providerProfilevm.Address1 + ", " + providerProfilevm.City;
                _db.SaveChanges();
            }

            if (phylocation == null)
            {
                var physicianLocation = new PhysicianLocation()
                {
                    PhysicianId = providerProfilevm.PhysicianId,
                    Latitude = providerProfilevm.Latitude,
                    Longitude = providerProfilevm.Longitude,
                    CreatedDate = DateTime.Now,
                    PhysicianName = _db.Physicians.FirstOrDefault(i => i.PhysicianId == providerProfilevm.PhysicianId).FirstName + " " + _db.Physicians.FirstOrDefault(i => i.PhysicianId == providerProfilevm.PhysicianId).LastName,
                    Address = providerProfilevm.Address1 + ", " + providerProfilevm.City,
                };

                _db.Add(physicianLocation);
                _db.SaveChanges();
            }
        }

        /// <summary>
        ///  Update Business Records on Physicain table From Edit Provider Page
        /// </summary>
        /// <param name="providerProfilevm"></param>
        public void PhysicianBusinessInfoUpdate(ProviderProfilevm providerProfilevm)
        {
            var physician = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == providerProfilevm.AspId);

            if (physician != null)
            {
                physician.BusinessName = providerProfilevm.BusinessName;
                physician.BusinessWebsite = providerProfilevm.BusinessWebsite;
                physician.AdminNotes = providerProfilevm.AdminNotes;
                physician.ModifiedBy = providerProfilevm.AspId;
                physician.ModifiedDate = DateTime.Now;

                _db.SaveChanges();

                if (providerProfilevm.Photo != null || providerProfilevm.Signature != null)
                {
                    AddProviderBusinessPhotos(providerProfilevm.Photo, providerProfilevm.Signature, providerProfilevm.AspId);
                }
            }
        }

        /// <summary>
        ///  Edit Photo From Edit Provider Page
        /// </summary>
        /// <param name="photo"></param>
        /// <param name="signature"></param>
        /// <param name="aspId"></param>
        public void AddProviderBusinessPhotos(IFormFile photo, IFormFile signature, string aspId)
        {
            var physician = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == aspId);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", physician.PhysicianId.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }


            if (photo != null)
            {
                string path = Path.Combine(directory, "Profile" + Path.GetExtension(photo.FileName));
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    photo.CopyTo(fileStream);
                }
                physician.Photo = photo.FileName;
            }

            if (signature != null)
            {
                string path = Path.Combine(directory, "Signature" + Path.GetExtension(signature.FileName));
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    signature.CopyTo(fileStream);
                }

                physician.Signature = signature.FileName;
            }

            _db.SaveChanges();


        }

        /// <summary>
        ///  Update Boarding Data on Physicain table From Edit Provider Page
        /// </summary>
        /// <param name="providerProfilevm"></param>
        public void EditOnBoardingData(ProviderProfilevm providerProfilevm)
        {
            var physicianData = _db.Physicians.FirstOrDefault(x => x.AspNetUserId == providerProfilevm.AspId);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", physicianData.PhysicianId.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (providerProfilevm.ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(providerProfilevm.ContractorAgreement.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfilevm.ContractorAgreement.CopyTo(fileStream);
                }

                physicianData.IsAgreementDoc = new BitArray(1, true);
            }

            if (providerProfilevm.BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(providerProfilevm.BackgroundCheck.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfilevm.BackgroundCheck.CopyTo(fileStream);
                }

                physicianData.IsBackgroundDoc = new BitArray(1, true);
            }

            if (providerProfilevm.HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(providerProfilevm.HIPAA.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfilevm.HIPAA.CopyTo(fileStream);
                }

                physicianData.IsTrainingDoc = new BitArray(1, true);
            }

            if (providerProfilevm.NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(providerProfilevm.NonDisclosure.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfilevm.NonDisclosure.CopyTo(fileStream);
                }

                physicianData.IsNonDisclosureDoc = new BitArray(1, true);
            }

            if (providerProfilevm.LicenseDocument != null)
            {
                string path = Path.Combine(directory, "Licence" + Path.GetExtension(providerProfilevm.LicenseDocument.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfilevm.LicenseDocument.CopyTo(fileStream);
                }

                physicianData.IsLicenseDoc = new BitArray(1, true);
            }
            _db.SaveChanges();

        }

        /// <summary>
        /// Delete Physician
        /// </summary>
        /// <param name="physicianId"></param>
        public void RemovePhysician(int physicianId)
        {
            var physician = _db.Physicians.FirstOrDefault(x => x.PhysicianId == physicianId);

            physician.IsDeleted = new BitArray(1, true);
            _db.SaveChanges();

        }

        public PayrateData GetPayrateData(int PhysicianId)
        {
            var payrateTabledata = _db.PhysicianPayrates.FirstOrDefault(x => x.PhysicianId==PhysicianId);

            if(payrateTabledata != null)
            {
                PayrateData payrateModalData = new PayrateData()
                {
                    PhysicianId = PhysicianId,
                    NightShiftWeekend = payrateTabledata.NightShiftWeekend,
                    Shift = payrateTabledata.Shift,
                    HouseCallNightsWeekend = payrateTabledata.HouseCallNightsWeekend,
                    PhoneConsults = payrateTabledata.PhoneConsults,
                    PhoneConsultsNightsWeekend = payrateTabledata.PhoneConsultsNightsWeekend,
                    BatchTesting = payrateTabledata.BatchTesting,
                    HouseCalls = payrateTabledata.HouseCalls,               
                };
                return payrateModalData;
            }
            else
            {
                PayrateData payrateModalData = new PayrateData()
                {
                    PhysicianId = PhysicianId,
                };
                return payrateModalData;
            }
        }

        public void SetPayrateData(PayrateData payrateModelData)
        {
            var payrateTableData = _db.PhysicianPayrates.FirstOrDefault(x => x.PhysicianId == payrateModelData.PhysicianId);

            if(payrateTableData != null)
            {
                payrateTableData.PhysicianId = payrateModelData.PhysicianId;
                payrateTableData.NightShiftWeekend = payrateModelData.NightShiftWeekend;
                payrateTableData.Shift = payrateModelData.Shift;
                payrateTableData.HouseCallNightsWeekend = payrateModelData.HouseCallNightsWeekend;
                payrateTableData.PhoneConsults = payrateModelData.PhoneConsults;
                payrateTableData.PhoneConsultsNightsWeekend = payrateModelData.PhoneConsults;
                payrateTableData.BatchTesting = payrateModelData.BatchTesting;
                payrateTableData.HouseCalls = payrateModelData.HouseCalls;

                _db.SaveChanges();
            }

            else
            {
                PhysicianPayrate? physicianPayrateTable = new PhysicianPayrate()
                {
                    PhysicianId = payrateModelData.PhysicianId,
                    NightShiftWeekend = payrateModelData.NightShiftWeekend,
                    Shift = payrateModelData.Shift,
                    HouseCallNightsWeekend = payrateModelData.HouseCallNightsWeekend,
                    PhoneConsults = payrateModelData.PhoneConsults,
                    PhoneConsultsNightsWeekend = payrateModelData.PhoneConsultsNightsWeekend,
                    BatchTesting = payrateModelData.BatchTesting,
                    HouseCalls = payrateModelData.HouseCalls,
                };
                _db.PhysicianPayrates.Add(physicianPayrateTable);
                _db.SaveChanges();
            }

        }


        //**********************************************************************--- Create Provider Account---********************************************************************

        public bool CreatePhysicianAccount(ProviderProfilevm providerProfilevm, List<int> physicianRegions)
        {
            if (!_db.AspNetUsers.Any(x => x.Email == providerProfilevm.Email))
            {
                var aspData = new AspNetUser()
                {
                    //Id = 25,
                    UserName = "MD" + providerProfilevm.LastName + providerProfilevm.FirstName.Substring(0, 1).ToUpper(),
                    PasswordHash = providerProfilevm.CreatePhyPassword,
                    Email = providerProfilevm.Email,
                    PhoneNumber = providerProfilevm.Phonenumber,
                    CreatedDate = DateTime.Now,
                    //RoleId = 2,
                };
                _db.AspNetUsers.Add(aspData);
                _db.SaveChanges();


                var physicianData = new Physician()
                {
                    //Physicianid = 6,
                    AspNetUserId = aspData.Id,
                    FirstName = providerProfilevm.FirstName,
                    LastName = providerProfilevm.LastName,
                    Email = providerProfilevm.Email,
                    Mobile = providerProfilevm.Phonenumber,
                    MedicalLicense = providerProfilevm.MedicalLicense,
                    Npinumber = providerProfilevm.NPINumber,
                    AdminNotes = providerProfilevm.AdminNotes,
                    Address1 = providerProfilevm.Address1,
                    Address2 = providerProfilevm.Address2,
                    City = providerProfilevm.City,
                    RegionId = providerProfilevm.RegionId,
                    Zip = providerProfilevm.Zipcode,
                    AltPhone = providerProfilevm.AltPhone,
                    CreatedBy = providerProfilevm.AspId,
                    CreatedDate = DateTime.Now,
                    Status = 1,
                    BusinessName = providerProfilevm.BusinessName,
                    BusinessWebsite = providerProfilevm.BusinessWebsite,
                    RoleId = providerProfilevm.RoleId,
                };
                _db.Physicians.Add(physicianData);
                _db.SaveChanges();

                foreach (int regionId in physicianRegions)
                {
                    var region = _db.Regions.FirstOrDefault(x => x.RegionId == regionId);

                    _db.PhysicianRegions.Add(new PhysicianRegion
                    {
                        PhysicianId = physicianData.PhysicianId,
                        RegionId = region.RegionId,
                    });
                }
                _db.SaveChanges();

                AspNetUserRole anur = new AspNetUserRole();
                anur.UserId = aspData.Id;
                anur.RoleId = "2";
                _db.AspNetUserRoles.Add(anur);
                _db.SaveChanges();


                var phyLocation = new PhysicianLocation()
                {
                    PhysicianId = physicianData.PhysicianId,
                    Latitude = providerProfilevm.Latitude,
                    Longitude = providerProfilevm.Longitude,
                    CreatedDate = DateTime.Now.Date,
                    PhysicianName = providerProfilevm.FirstName + " " + providerProfilevm.LastName,
                    Address = providerProfilevm.City + "," + _db.Regions.FirstOrDefault(i => i.RegionId == providerProfilevm.RegionId).Name,
                };
                _db.Add(phyLocation);
                _db.SaveChanges();

                AddProviderDocuments(physicianData.PhysicianId, providerProfilevm.Photo, providerProfilevm.ContractorAgreement, providerProfilevm.BackgroundCheck, providerProfilevm.HIPAA, providerProfilevm.NonDisclosure);

                return true;
            }

            return false;
        }

        public void AddProviderDocuments(int Physicianid, IFormFile Photo, IFormFile ContractorAgreement, IFormFile BackgroundCheck, IFormFile HIPAA, IFormFile NonDisclosure)
        {
            var physicianData = _db.Physicians.FirstOrDefault(x => x.PhysicianId == Physicianid);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (Photo != null)
            {
                string path = Path.Combine(directory, "Profile" + Path.GetExtension(Photo.FileName));
                //string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "content", Photo.FileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    Photo.CopyTo(fileStream);
                }

                physicianData.Photo = Photo.FileName;
            }


            if (ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(ContractorAgreement.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    ContractorAgreement.CopyTo(fileStream);
                }

                physicianData.IsAgreementDoc = new BitArray(1, true);
            }

            if (BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(BackgroundCheck.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    BackgroundCheck.CopyTo(fileStream);
                }

                physicianData.IsBackgroundDoc = new BitArray(1, true);
            }

            if (HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(HIPAA.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    HIPAA.CopyTo(fileStream);
                }

                physicianData.IsTrainingDoc = new BitArray(1, true);
            }

            if (NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(NonDisclosure.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    NonDisclosure.CopyTo(fileStream);
                }

                physicianData.IsNonDisclosureDoc = new BitArray(1, true);
            }

            _db.SaveChanges();
        }

      

    }
}
