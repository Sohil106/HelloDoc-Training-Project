using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface IPartnersRepo
    {
        // ******************************************************* Partners *********************************************************

        public List<HealthProfessionalType> GetProfession();

        public List<Partnersdata> GetPartnersdata(int professionid);

        public bool CreateNewBusiness(Partnersvm partnersvm, string LoggerAspnetuserId);

        public Partnersvm GetEditBusinessData(int vendorID);

        public bool UpdateBusiness(Partnersvm partnersCM);

        public void DelPartner(int VendorID);

    }
}
