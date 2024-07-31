using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface IAdminProfileRepo
    {
        public List<Role> GetRoles();

        public List<AdminregionTable> GetAdminregions(string aspId);

        public AdminProfilevm getAdminProfileData(String aspnetuserid);

        public void setAdminProfileData(AdminProfilevm updatedAdminProfileData);

        public void setAdminInfoData(AdminProfilevm updateAdminInfoData, List<int> regions);

        public void setPasswordData(AdminProfilevm updatedAdminPassword);

        public void RemoveAdmin(int adminId);

        // ******************************************************* Create Account  ************************************************************

        public bool CreateAdminAccount(AdminProfilevm adminProfilevm, List<int> adminRegions);

      


    }
}
