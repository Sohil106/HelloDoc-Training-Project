using HelloDoc.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class Partnersvm
    {
        public List<Region> regions { get; set; }

        public List<HealthProfessionalType> Professions { get; set; }

        public List<Partnersdata> Partnersdata { get; set; }

        [Required(ErrorMessage = "Street Is Required")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State Is Required")]
        public int? RegionId { get; set; }

        [Required(ErrorMessage = "Please Select Profession")]
        public int? SelectedhealthprofID { get; set; }

        [Required(ErrorMessage = "Zip Is Required")]
        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "Invalid Zipcode (must be 4 to 6 numbers)")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Business Name Is Required")]
        [StringLength(16, ErrorMessage = "Only 16 Characaters are Accepted")]
        // [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Business Name Accepts Only Text Characters")]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string Phonenumber { get; set; }

        [Required(ErrorMessage = "FAX NO. Is Required")]
        public string FAXNumber { get; set; }

        [Required(ErrorMessage = "Business Contact Is Required")]
        public string? BusinessContact { get; set; }

        public int? vendorID { get; set; }
    }

    public class Partnersdata
    {
        public string VendorName { get; set; }

        public string ProfessionName { get; set; }

        public int? VendorId { get; set; }

        public string PhoneNo { get; set; }

        public string? FaxNo { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^.{1,30}$", ErrorMessage = "Email must not exceed 30 characters")]
        public string? VendorEmail { get; set; }

        public string Businesscontact { get; set; }
    }
}
