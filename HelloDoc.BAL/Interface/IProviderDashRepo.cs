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
    public interface IProviderDashRepo
    {
        // ***************************************************** Dashboard  ************************************************************

        // *********************** Count request *******************
        public CountRequest GetCountRequest(int physicianId);


        // *********************** Table data ***********************
        public List<RequestListAdminDash> getRequestData(int[] Status, string requesttypeid, int physicianId);


        // ***************************** Accept Case Data *****************************

        public void SetAcceptCaseData(int requestId, int physicianId);


        // ******************** Transfer Case  **********************

        public void TransferCaseData(TransferCaseModal transferCaseModal);

        // ******************** Encounter Case  **********************

        public void SetEncounterCareType(Encountervm encountervm);

        public void FinalizeEncounterCase(int requestId,string aspId);

        public void HouseCallConclude(int requestId);

        // ******************** Conclude Case  **********************

        public AdminViewUploadsvm GetViewDocumentsData(int requestid);

        List<ViewUploadsModel> GetViewDocumentsList(int requestid);

        public void SetViewDocumentData(AdminViewUploadsvm adminViewUploadsvm);

        public void DeleteFileData(int requestwisefileid);

        public void ConfirmConcludeCare(AdminViewUploadsvm adminViewUploadsvm);

        // ************************** Request To Admin **************************

        public void RequestForEdit(ProviderProfilevm providerProfilevm);

        // ************************** Invoicing **************************

        List<TimesheetDetail> GetTimeSheetDetails(int phyid, string dateSelected);

        List<TimesheetDetailReimbursement> GetTimeSheetDetailsReimbursements(int phyid, string dateSelected);

        List<ProviderTimesheetDetails> GetFinalizeTimeSheetDetails(int phyid, string dateSelected);

        bool PostFinalizeTimesheet(List<ProviderTimesheetDetails> providerTimesheetDetails);

        List<AddReceiptsDetails> GetAddReceiptsDetails(int[] timeSheetDetailId, string AspId);

        bool EditReceipt(string aspId, int timeSheetDetailId, string item, int amount, IFormFile file);

        bool DeleteReceipt(string aspId, int timeSheetDetailId);

        bool FinalizeTimeSheet(int timeSheetId);




    }
}
