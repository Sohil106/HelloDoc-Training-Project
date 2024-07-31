using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface IPatientReqRepo
    {
        public List<Region> GetRegions();
        void AddToUser(PatientReqData model);
        void AddToRequest(PatientReqData model);

        void AddToReqClient(PatientReqData model);

        void AddToReqWIseFile(PatientReqData model);

        void UploadFile(PatientReqData model);

        void AddToReqConcierge(PatientReqData model);

        void AddToConcierge(PatientReqData model);

        bool EmailCheck(String Email);

        int GetUserId(String email);

        void AddToReqBusiness(PatientReqData model);

        void AddToBusiness(PatientReqData model);

        public Task EmailSender(string email, string subject, string message);


        //***************************************************review Agreement *********************************************************************************************************
        public ReviewAgreementvm GetReviewAgreement(int reqId);

        public void AgreeReview(int reqId);

        public void CancelReview(ReviewAgreementvm reviewAgreementvm);

    }
}
