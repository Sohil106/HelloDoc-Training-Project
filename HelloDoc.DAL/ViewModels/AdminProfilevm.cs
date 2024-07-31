using HelloDoc.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class AdminProfilevm
    {
        public List<Role> Roles { get; set; }

        public List<Region> Regions { get; set; }

        public List<AdminregionTable> AdminRegions { get; set; }

        public int callId { get; set; }
        public string AspNetUserId { get; set; }


        public int AdminId { get; set; }

        public string UserName {  get; set; }

        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,20}$", ErrorMessage = "Minimum six characters,maximum 20 characters, at least one letter, one number and one special character is mandatory")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,20}$", ErrorMessage = "Minimum six characters,maximum 20 characters and at least one letter, one number and one special character is mandatory")]
        public string? CreateAdminPassword { get; set; }


        public string Status { get; set; }

        [Required(ErrorMessage = "Role Is Required")]
        public int? RoleId { get; set; }

        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "First name must be a string of 3 to 15 letters and Not contain Space")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LasName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "Last name must be a string of 3 to 15 letters and Not contain Space")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        //[RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please Enter Valid Phone Number")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string PhoneNumber { get; set;}

        [Required(ErrorMessage = "Address is Required")]
        public string Address1 { get; set; }

        public string Address2 { get; set; }
        public int RegionId { get; set; }


        [Required(ErrorMessage = "City is Required")]
        [RegularExpression(@"^([a-zA-Z]+)$", ErrorMessage = "Invalid City Name")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is Required")]
        public string State { get; set; }

        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "Invalid Zipcode (must be 4 to 6 numbers)")]
        public string Zipcode { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string AltPhone { get; set; }

    }
    public class AdminregionTable
    {
        public int Adminid { get; set; }

        public int Regionid { get; set; }

        public string Name { get; set; }

        public bool ExistsInTable { get; set; }
    }
}
