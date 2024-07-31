using HelloDoc.DAL.ViewModels;
using HelloDoc.DAL.Models;
using Data_Access.Dash;



namespace HelloDoc.BAL.Interface
{
    public interface IAdminDashRepo
    {
        List<RequestListAdminDash> getRequestData(int[] Status,string requesttypeid,int regionid);

        ViewCaseModel getViewCaseData(int requestid);

        public void setViewCaseData(ViewCaseModel updatedViewCaseData, int requestid);

        List<Region> getRegions();

        List<Region> GetPhysicianRegions(int physicianId);

        public void countRequest(AdminDashvm adminDashvm);

        public ViewNotesModel getViewNotesData(int requestid);

        public void setViewNotesData(ViewNotesModel updatedViewNotesData,int requestid,string aspId,int callId);

        public List<CaseTag> getCaseTags();

        public CancelCaseModal getCancelCaseData(int requestid);

        public void setCancelCaseData(CancelCaseModal updatedCancelCaseData);

        public AssignCaseModel getAssignCaseData(int requestid);

        public List<Physician> getPhysicians(int regionid);

        public List<Physician> GetPhysicians();

        public void setAssignCaseData(AssignCaseModel updatedAssignCaseData);

        public BlockCaseModel getBlockCaseData(int requestid);

        public void setBlockCaseData(BlockCaseModel updatedBlockCaseData);

        public AdminViewUploadsvm  getViewUploadsData(int requestid);

        public List<ViewUploadsModel> getViewUploadsList(int requestid);

        public void setViewUploadsData(AdminViewUploadsvm adminViewUploadsvm);

        public void DeleteFileData(int requestwisefileid);

        public Task sendEmailWithFile(int[] requestwisefileid, int requestid);

        

        //----------------------------------------------------get order data------------------------------------------------------------

        public SendOrderModel getOrderData(int requestid);

        public List<HealthProfessionalType> getHealthProfessionalTypes();

        
        public List<HealthProfessional> getHealthProfessionals(int health_professional_id); /*type_id*/

        public SendOrderModel GetVendordata(int vendorid);

        public void setOrderData(SendOrderModel sendOrderModel,string aspId);

        //----------------------------------------------------transfercasemodel data------------------------------------------------------------

        public TransferCaseModel getTransferCaseData(int requestid);

        public void setTransferCaseData(TransferCaseModel updateTransferCaseData);

        //----------------------------------------------------clearcasemodel data------------------------------------------------------------

        public ClearCaseModel getClearCaseData(int requestid);

        public void setClearCaseData(ClearCaseModel updatedClearCaseData);

        //----------------------------------------------------Send Agreement model data------------------------------------------------------------

        public SendAgreementModel getSendAgreementData(int requestid);

        public Task sendAgreementMail(SendAgreementModel sendAgreementModel,string aspId);

        //public SendAgreementModel getRequestId(int requestid);
        //public void setStatusActive(int rid);

        //----------------------------------------------------Close case page data------------------------------------------------------------

        public CloseCaseModel getCloseCaseData(int requestid);

        public List<CloseCaseList> getCloseCaseList(int requestid);

        public void updateCloseCaseData(CloseCaseModel closeCaseModel);

        public void setCloseCaseData(int requestid,string aspId);


        //----------------------------------------------------Send link model data------------------------------------------------------------

        public Task SubmitRequestMail(SendLinkModel sendLinkModel,string aspId);

        //----------------------------------------------------Request DTY Support------------------------------------------------------------
        public bool RequestSupportViaEmail(string message, int adminId);
        //----------------------------------------------------Send Create Request data------------------------------------------------------------

        public void SendCreateRequestData(AdminCreateRequestvm adminCreateRequestvm, string aspId);

        public void InsertRequestData(AdminCreateRequestvm adminCreateRequestvm, int reqTypeId, string aspId);

        public void InsertRequestClientData(AdminCreateRequestvm adminCreateRequestvm, int requestId);

        public void InsertNotesData(AdminCreateRequestvm adminCreateRequestvm, int requestId, string aspId);

        public bool CheckRegion(int region);

        // ******************************************************* Account Access  ************************************************************

        public List<AccountAccess> GetAccountAccessData();

        public List<AspNetRole> GetAccountType();

        public List<Menu> GetMenu(int accounttype);

        public List<AccountMenu> GetAccountMenu(int accounttype, int roleid);

        public void SetCreateAccessAccount(AccountAccess accountAccess, List<int> AccountMenu);

        public AccountAccess GetEditAccessData(int roleid);

        public void SetEditAccessAccount(AccountAccess accountAccess, List<int> AccountMenu);

        public void DeleteAccountAccess(int roleid);

        // ******************************************************* User Access  ************************************************************

        public List<UserAccess> GetUserAccessData(int accountTypeId);

     
        #region Invoicing
        List<Timesheet> GetTimeSheetDetail(int phyid, string dateSelected);

        List<PhysicianPayrate> GetPayRateForProviderByPhyId(int phyid);

        bool ApproveTimeSheet(int timeSheetId, int bonus, string notes);

        #endregion
    }
}
