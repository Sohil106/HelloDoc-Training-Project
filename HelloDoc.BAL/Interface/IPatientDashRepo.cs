

using Data_Access.Dash;
using HelloDoc.DAL.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Business_Logic.Interface
{
    public interface IPatientDashRepo 
    {
        List<DashboardData> RequestList(int userId);


        List<DocumentData> DocumentList(int reqId);

        bool DashboardUpload(PatientDashData patientdashdata, int reqid);

        ProfileData GetProfileData(int userid);

        void SetProfileData(ProfileData updatedprofiledata, int userid);

        
    }
}
