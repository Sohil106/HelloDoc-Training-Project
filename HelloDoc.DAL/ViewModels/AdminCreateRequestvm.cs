using HelloDoc.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class AdminCreateRequestvm
    {
        public int? CallId { get; set; }
        public int RequestId { get; set; }
        public int StatusForName { get; set; }

        [Required(ErrorMessage = "First Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{3,15}$", ErrorMessage = "Firstname Accepts Only Alphabets min of 3 and  max of 15 Characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{3,15}$", ErrorMessage = "Lastname Accepts Only Alphabets min of 3 and  max of 15 Characters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Birthdate is required")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string Email { get; set; }

        //[StringLength(10, MinimumLength = 10, ErrorMessage = "Phone Number should be of 10 Numbers")]
        [Required(ErrorMessage = "PhoneNumber Is Required")]
        //[RegularExpression(@"^[0-9]{10}$", ErrorMessage = "please enter valid phone number")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string PhoneNo { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [RegularExpression(@"^[a-zA-Z ]{3,15}$", ErrorMessage = "Invalid Street Name (3 to 15 characters)")]
        public string? Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [RegularExpression(@"^[a-zA-Z ]{3,15}$", ErrorMessage = "Invalid City Name (3 to 15 characters)")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        public int? RegionId { get; set; }

        [Required(ErrorMessage = "Zipcode Is Required")]
        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "Invalid Zipcode (must be 4 to 6 numbers)")]
        public string Zipcode { get; set; }

        [RegularExpression(@"^.{5,100}$", ErrorMessage = "Address must be between 5 to 100 characters.")]
        public string? Room { get; set; }

        [Required(ErrorMessage = "AdmineNotes Is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        public string? AdminNotes { get; set; }

        public List<Region> Regions { get; set; }
    }
}
