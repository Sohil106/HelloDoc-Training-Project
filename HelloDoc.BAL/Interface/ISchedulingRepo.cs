using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface ISchedulingRepo
    {
        // ******************************************************* Scheduling  ************************************************************
        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday, string type,string aspId); // type = day,week or month

        public ShiftDetailsmodal GetShift(int shiftdetailsid);

        public bool createShift(ScheduleModel scheduleModel, string Aspid);

        public void SetReturnShift(int status, int shiftdetailid, string Aspid);

        public void SetDeleteShift(int shiftdetailid, string Aspid);

        public void SetEditShift(ShiftDetailsmodal shiftDetailsmodal, string Aspid);

        public List<ShiftReview> GetShiftReview(int regionId, int callId);

        public void ApproveSelectedShift(int[] shiftDetailsId, string Aspid);

        public void DeleteShiftReview(int[] shiftDetailsId, string Aspid);

        public OnCallModal GetOnCallDetails(int regionId);

        // ******************************************************* Provider Location  ************************************************************
        public List<PhysicianLocation> GetPhysicianlocations();
    }
}
