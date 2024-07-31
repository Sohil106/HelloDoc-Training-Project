using HelloDoc.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;


namespace HelloDoc.DAL.ViewModels
{
    public class PatientReqData
    {

        public string? ConfirrmationNumber { get; set; }

        public int RequestClientId { get; set; }


        public int RequestId { get; set; }


        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "First name must be a string of 3 to 15 letters and Not contain Space")]
        public string? OtherFirstName { get; set; }


        //[Required(ErrorMessage = "LastName is Required")]
        //[RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid Last Name")]
        [Required(ErrorMessage = "LasName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "Last name must be a string of 3 to 15 letters and Not contain Space")]
        public string? OtherLastName { get; set; }


        [Required(ErrorMessage = "PhoneNumber is Required")]
        //[RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please Enter Valid Phone Number")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? OtherPhoneNumber { get; set; }


        [Required (ErrorMessage = "Email is Required") ]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? OtherEmail { get; set; }


        [Required(ErrorMessage = "RelationName is Required")]
        [RegularExpression(@"^(?=.{3,15}$)[a-zA-Z ]*(?: [a-zA-Z]*)?$", ErrorMessage = "Only letters and up to two consecutive spaces are allowed, with a minimum of 3 and a maximum of 15 characters.")]
        //[RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid RelationName")]
        public string? OtherRelation { get; set; }

        [Required(ErrorMessage = "Property is Required")]
        [RegularExpression(@"^[a-zA-Z0-9]{5,15}$", ErrorMessage = "Only letters and numbers are allowed, with a minimum of 5 and a maximum of 15 characters.")]
        public string? HostelName { get; set; }

        public int? CaseNo { get; set; }

        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "First name must be a string of 3 to 15 letters and Not contain Space")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "LastName Is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "Last name must be a string of 3 to 15 letters and Not contain Space")]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        //[RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please Enter Valid Phone Number")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string PhoneNumber { get; set; }



        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,20}$", ErrorMessage = "Minimum six characters,maximum 20 characters, at least one letter, one number and one special character is mandatory")]
        public string PasswordHash { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,20}$", ErrorMessage = "Minimum six characters,maximum 20 characters, at least one letter, one number and one special character is mandatory")]
        [Compare("PasswordHash", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConformPassword { get; set; }

        [Required(ErrorMessage = "StreetName is Required")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is Required")]
        //[RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        [RegularExpression(@"^[a-zA-Z ]{3,15}$", ErrorMessage = "Invalid City Name (3 to 15 characters)")]
        public string City { get; set; }

        [Required (ErrorMessage = "State is Required")]
        //[RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid State Name")]
        [RegularExpression(@"^[a-zA-Z ]{3,15}$", ErrorMessage = "Invalid State Name (3 to 15 characters)")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zipcode is Required")]
        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "Invalid Zipcode (must be 4 to 6 numbers)")]
        public string ZipCode { get; set; }


        [Required(ErrorMessage = "Region Is Required")]
        public int? RegionId { get; set; }

        public List<Region> Regions { get; set; }

        [RegularExpression(@"^.{5,100}$", ErrorMessage = "Address must be between 5 to 100 characters.")]
        public string? Address { get; set; }

        public string? FileName { get; set; }

        public IFormFile? Upload { get; set; }



        public string? Location { get; set; }

        public string? NotiMobile { get; set; }


        public string? NotiEmail { get; set; }


        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        public string? Notes { get; set; }

       
        public string StrMonth { get; set; }


        public int? IntYear { get; set; }


        public int? IntDate { get; set; }
     

        public short? CommunicationType { get; set; }

        public short? RemindReservationCount { get; set; }

        public short? RemindHouseCallCount { get; set; }

        public short? IsSetFollowupSent { get; set; }

        public string? Ip { get; set; }

        public short? IsReservationReminderSent { get; set; }


        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
       

        public string AspNetUserId { get; set; }

        public int UserId { get; set; }

        public int ConciergeReqId { get; set; }

        public int ConciergeId { get; set; }


        public int BusinessReqId { get; set; }

        public int BusinessId { get; set; }

      

        

        public int RequestTypeId { get; set; }
    }
}
