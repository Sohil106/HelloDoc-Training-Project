using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface IProviderDetailsRepo
    {
        // ******************************************************* Provider  ************************************************************

        // ******************************** Provider  Edit*******************************

        //public List<Physician> GetPhysicians();

        public List<Provider> GetProviders(int regionId);

        public List<Region> getRegions();

        public void ContactProvider(ProviderDetailsvm providerDetailsvm, string aspId);

        public bool stopNotification(int PhysicianID);
        public void ContactSMSProvider(ProviderDetailsvm providerDetailsvm, string aspId);

        public List<PhysicianRegionTable> GetPhysicianRegionTables(string aspId);

        public ProviderProfilevm GetProviderProfileData(string aspId);

        List<Role> GetRoles();

        public void PhysicianResetPassword(String password, string aspId);

        public void PhysicianAccountUpdate(short status, int roleId, string aspId,string username);

        public void PhysicianAdministratorDataUpdate(ProviderProfilevm providerProfilevm,List<int> physicianRegions);

        public void PhysicianMailingDataUpdate(ProviderProfilevm providerProfilevm);

        public void PhysicianBusinessInfoUpdate(ProviderProfilevm providerProfilevm);

        public void AddProviderBusinessPhotos(IFormFile photo, IFormFile signature, string aspId);

        public void EditOnBoardingData(ProviderProfilevm providerProfilevm);

        public void RemovePhysician(int physicianId);

        public PayrateData GetPayrateData(int PhysicianId);
        public void SetPayrateData(PayrateData payrateData);
 
           // ******************************** Provider  Create*******************************


        public bool CreatePhysicianAccount(ProviderProfilevm providerProfilevm, List<int> physicianRegions);

        public void AddProviderDocuments(int Physicianid, IFormFile Photo, IFormFile ContractorAgreement, IFormFile BackgroundCheck, IFormFile HIPAA, IFormFile NonDisclosure);

    }
}
