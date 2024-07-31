using HelloDoc.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class AdminDashvm
    {
       public List<RequestListAdminDash> RequestListAdminDash { get; set; }
        public string reqTypeId { get; set; }
        public int CallId { get; set; }
        public string AspId { get; set; }
        public int StatusForName { get; set; }

       public ViewCaseModel viewCaseModel { get; set; }

       public List<Region> regions { get; set; }

        public int NewRequest { get; set; }

        public int PendingRequest { get; set; }

        public int ActiveRequest { get; set; }

        public int ConcludeRequest { get; set; }

        public int ToCloseRequest { get; set; }

        public int UnpaidRequest { get; set; }

        public int requestid { get; set; }

       //-------------For  popup modals------------------------------------------------

        public ViewNotesModel viewNotesModel { get; set; }

        public CancelCaseModal? cancelCaseModal { get; set; }
        
        public List<CaseTag> caseTags { get; set; }

        public AssignCaseModel? assignCaseModel { get; set; }

               //List of regions

        public List<Physician> physicians { get; set; }

        public BlockCaseModel? blockCaseModel { get; set; }
        
        public ViewUploadsModel? viewUploadsModel { get; set; }

        //-------------For  order partialview------------------------------------------------
        public SendOrderModel? sendOrderModel { get; set; }

        public List<HealthProfessionalType>? healthProfessionalTypes { get; set; }

        public List<HealthProfessional>? healthProfessionals { get; set; }

        //-------------For  transfer model partialview---------------------------------------

        public TransferCaseModel? transferCaseModel { get; set; }

        //-------------For  Clear model partialview---------------------------------------

        public ClearCaseModel? clearCaseModel { get; set; }

        //-------------For  Send Agreemenr model partialview---------------------------------------

        public SendAgreementModel? sendAgreementModel { get; set; }

        //-------------Close Case page Model---------------------------------

        public CloseCaseModel? closeCaseModel { get; set; }

        public List<CloseCaseList>? closeCaseLists { get; set; }


        //-------------Send link for submit request---------------------------------
        public SendLinkModel? sendLinkModel { get; set; }

        //ForProvider

        public CountRequest countRequest { get; set; }

        public int sessionId { get; set; }

        public int AcceptRequestId { get; set; }

        public TransferCaseModal? transferCaseModal { get; set; }

        //-------------Request DTY Support
        
        [Required(ErrorMessage = "Message is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{2,150}$", ErrorMessage = "Transfer Description Accepts Only Alphabets ( Min. 2 & Max. 150 )")]
        public string? RequestSupportMessage { get; set; }
    }

    public class RequestListAdminDash
    {
        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Requestor { get; set; }

        public DateTime RequestDate { get; set; } //Createddate

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string? Notes { get; set; }

        public string? Address { get; set; }

        public string ChatWith { get; set; } //-- nahi aave

        public string Physician { get; set; }

        public DateTime DateOfService { get; set; }

        public string RegionName { get; set; }

        public int Status { get; set; }

        public int RequestTypeId { get; set; }

        public int? RequestId { get; set; }

        public string Email { get; set; }

        public int totalHours { get; set; }

        public int totalMinutes { get; set; }

        public int totalSeconds { get; set; }

        public string transferToPhysicianLog { get; set; }

        public int callType { get; set; }

        public bool isFinalized { get; set; }

        //public int NewRequest { get; set; }
    }


    public class ViewCaseModel
    {
        
        public string? ConformNum {  get; set; }

        public string? PatientNotes { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Enter Valid Email")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? Email {  get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Region { get; set; }

        public string? BusinessAddress { get; set; }

        public string? Room { get; set; }

        public int RequestTypeId { get; set; }

        public int RequestId { get; set; }

        public int? UserId  { get; set; }




    }

    public class ViewNotesModel
    {

        public int RequestId { get; set; }

        [Required(ErrorMessage = "Additional note is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        public string? AdminNotes { get; set;}

        [Required(ErrorMessage = "Additional note is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        public string? PhysicianNotes { get; set; }

        public List<string>? TransferNotes { get; set; }

        
   
    }


    public class CancelCaseModal
    {
        public int RequestId { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "Cancellation notes is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        public string AditionalNotes { get; set; }

        [Required(ErrorMessage = "Cancellation reason is Required")]
        public int CasetagId { get; set; }
    }


    public class AssignCaseModel
    {
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Region is Required")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Physician is Required")]
        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "Assign note is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]

        //[RegularExpression(@"^[a-zA-Z]+(\s[a-zA-Z0-9]+)?$", ErrorMessage = "Invalid Notes")]
        public string AssignNotes { get; set; }

    }

    public class BlockCaseModel
    {
        public int RequestId { get; set;}

        public string Name { get; set; }

        [Required(ErrorMessage = "Block Reason is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        //[RegularExpression(@"^[a-zA-Z]+(\s[a-zA-Z0-9]+)?$", ErrorMessage = "Please Write Valid Reason")]
        public string Reason { get; set;}

    }

    public class  SendOrderModel
    {
       
        public int RequestId { get; set; }

        public int HealthPressionallId { get; set; }

        [Required(ErrorMessage = "Business is Required")]
        public int VendorId { get; set; }

        //public string? Profession { get; set; }

        //public string? Business { get; set; }
        [Required(ErrorMessage = "Business Contact is Required")]
        public string? BusinessContact { get; set; }


        [Required(ErrorMessage = "Business Email is Required")]
        [EmailAddress(ErrorMessage = "Enter valid Email")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Faxnumber is Required")]
        public string? FaxNum { get; set;}


        [Required(ErrorMessage = "Prescreption is Required")]
        public string? Prescription { get; set; }

        public int Refil { get; set; }

    }

    public class TransferCaseModel
    {
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Region is Required")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Physician is Required")]
        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "Transfer note is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        //[RegularExpression(@"^[a-zA-Z]+(\s[a-zA-Z0-9]+)?$", ErrorMessage = "Invalid Notes")]
        public string? TransferNotes { get; set; }

    }

    public class ClearCaseModel
    {
        public int RequestId { get; set; }
    }

    public class SendAgreementModel
    {
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Phone number is Required")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Enter valid Email")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? Email { get; set; }

        public int RequestTypeId { get; set; }
    }


    public class CloseCaseModel
    {
        public int RequestId { get; set;}

        public string ConfirmationNumber { get; set; }

        public string? PatientName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Phone number is Required")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Enter valid Email")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? Email { get; set; }
    }

    public class CloseCaseList
    {

        public int requestWiseFileId { get; set; }

        public int RequestId { get; set; }

        public string DocumentName { get; set; }

        public DateTime UploadDate { get; set; }

    }

    public class SendLinkModel
    {
        [Required(ErrorMessage = "First Name is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "First name must be a string of 3 to 15 letters and Not contain Space")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "Last name must be a string of 3 to 15 letters and Not contain Space")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        //[RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please Enter Valid Phone Number")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Enter valid Email")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? Email { get; set; }

    }


    public class CountRequest
    {
        public int NewRequest { get; set; }

        public int PendingRequest { get; set; }

        public int ActiveRequest { get; set; }

        public int ConcludeRequest { get; set; }

        public int ToCloseRequest { get; set; }

        public int UnpaidRequest { get; set; }
    }



    //For Provider

    public class TransferCaseModal
    {
        public int RequestId { get; set; }

        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "Description is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        public string? TransferDescription { get; set; }
    }

}

         
