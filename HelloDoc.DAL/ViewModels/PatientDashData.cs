using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Data_Access.Dash 
{
    public class PatientDashData
    { 
        public List<DashboardData> dashboardData { get; set; }

        public List<DocumentData> documentData { get; set; }

        public IFormFile Upload { get; set; }

        public ProfileData? profileData { get; set; }
    }

    public class DashboardData
    {
        public int RequestId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

        public int DocumentCount { get; set; }

        public string? Provider { get; set; }

        public int?  PhysicianId { get; set; }


    }

    public class DocumentData
    {
        public DateTime CreatedDate { get; set; }

        public string DocumentName { get; set; }

    }

    public class ProfileData
    {

        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "First name must be a string of 3 to 15 letters and Not contain Space")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "LastName Is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "Last name must be a string of 3 to 15 letters and Not contain Space")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        //[RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please Enter Valid Phone Number")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "StreetName is Required")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,15}$", ErrorMessage = "Invalid City Name (3 to 15 characters)")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,15}$", ErrorMessage = "Invalid State Name (3 to 15 characters)")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zipcode is Required")]
        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "Invalid Zipcode (must be 4 to 6 numbers)")]
        public string Zipcode { get; set; } = string.Empty;
    }

}