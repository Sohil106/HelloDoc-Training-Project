using HelloDoc.DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class ProviderProfilevm
    {
        public PayrateData? payrateData { get; set; }

        public List<Region> Regions { get; set; }

        public List<Role> Roles { get; set; }

        public List<PhysicianRegionTable> PhysicianRegionTables { get; set; }

        [Required(ErrorMessage = "Request Message Is Required")]
        public string? RequestMessage { get; set; }

        public int callId { get; set; }

        public string AspId { get; set; }

        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "UserName Is Required")]
        public string? Username { get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,20}$", ErrorMessage = "Minimum six characters,maximum 20 characters, at least one letter, one number and one special character is mandatory")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,20}$", ErrorMessage = "Minimum six characters,maximum 20 characters, at least one letter, one number and one special character is mandatory")]
        public string? CreatePhyPassword { get; set; }

        [Required(ErrorMessage = "Role Is Required")]
        public int? RoleId { get; set; }

        public short? Status { get; set; }


        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "First name must be a string of 3 to 15 letters and Not contain Space")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "LasName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "Last name must be a string of 3 to 15 letters and Not contain Space")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string Phonenumber { get; set; }


        //public string PhoneWithoutCode { get; set; }

        public string? MedicalLicense { get; set; }

        public string? NPINumber { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? SyncEmail { get; set; }

        [Required(ErrorMessage = "Adress Is Required")]
        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "City Accepts Only Text Characters")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        public int? RegionId { get; set; }

        //[RegularExpression(@"^\d{6}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid Zipcode")]
        [Required(ErrorMessage = "Zipcode Is Required")]
        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "Invalid Zipcode (must be 4 to 6 numbers)")]
        public string? Zipcode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        [Required(ErrorMessage = "AltPhone Is Required")]
        public string? AltPhone { get; set; }

        //public string? AltPhoneWithoutCode { get; set; }

        [Required(ErrorMessage = "BusinessName Is Required")]
        [RegularExpression(@"^[a-zA-Z0-9]{5,15}$", ErrorMessage = "Only letters and numbers are allowed, with a minimum of 5 and a maximum of 15 characters.")]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "BusinessWebsite Is Required")]
        [RegularExpression(@"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$", ErrorMessage = "Please enter a valid website URL.")]
        public string? BusinessWebsite { get; set; }

        public IFormFile? Photo { get; set; }

        public string? PhotoValue { get; set; }

        public IFormFile? Signature { get; set; }

        public string? SignatureValue { get; set; }

        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        public string? AdminNotes { get; set; }

        public IFormFile? ContractorAgreement { get; set; }

        public bool IsContractorAgreement { get; set; }

        public IFormFile? BackgroundCheck { get; set; }

        public bool IsBackgroundCheck { get; set; }

        public IFormFile? HIPAA { get; set; }

        public bool IsHIPAA { get; set; }

        public IFormFile? NonDisclosure { get; set; }

        public bool IsNonDisclosure { get; set; }

        public IFormFile? LicenseDocument { get; set; }

        public bool IsLicenseDocument { get; set; }
    }

    public class PhysicianRegionTable
    {
        public int PhysicianId { get; set; }

        public int Regionid { get; set; }

        public string Name { get; set; }

        public bool ExistsInTable { get; set; }
    }

    public class PayrateData
    {
        public int PhysicianId { get; set; }

      
        [RegularExpression(@"^[0-9]{0,3}$", ErrorMessage = "you can write upto three numeric letters and not allowed to write alphabetic letters")]
        public int? NightShiftWeekend { get; set; }

        [RegularExpression(@"^[0-9]{0,3}$", ErrorMessage = "you can write upto three numeric letters and not allowed to write alphabetic letters")]
        public int? Shift { get; set; }

        [RegularExpression(@"^[0-9]{0,3}$", ErrorMessage = "you can write upto three numeric letters and not allowed to write alphabetic letters")]
        public int? HouseCallNightsWeekend { get; set; }

        [RegularExpression(@"^[0-9]{0,3}$", ErrorMessage = "you can write upto three numeric letters and not allowed to write alphabetic letters")]
        public int? PhoneConsults { get; set; }

        [RegularExpression(@"^[0-9]{0,3}$", ErrorMessage = "you can write upto three numeric letters and not allowed to write alphabetic letters")]
        public int? PhoneConsultsNightsWeekend { get; set; }

        [RegularExpression(@"^[0-9]{0,3}$", ErrorMessage = "you can write upto three numeric letters and not allowed to write alphabetic letters")]
        public int? BatchTesting { get; set; }

        [RegularExpression(@"^[0-9]{0,3}$", ErrorMessage = "you can write upto three numeric letters and not allowed to write alphabetic letters")]
        public int? HouseCalls { get; set; }

    }
}
