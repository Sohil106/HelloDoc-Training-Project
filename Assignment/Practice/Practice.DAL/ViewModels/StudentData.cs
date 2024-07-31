using Practice.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.DAL.ViewModels
{
    public class StudentData
    {
        public List<StudentList> studentList {  get; set; }

        public StudentList? StudentInfo {  get; set; }

        public List<Course> courses { get; set; }
    }

    public class StudentList
    {

        public int StudentId { get; set; }

        [Required(ErrorMessage = "FirstName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "First name must be a string of 3 to 15 letters and Not contain Space")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LasName is Required")]
        [RegularExpression(@"^([a-zA-Z]{3,15})$", ErrorMessage = "Last name must be a string of 3 to 15 letters and Not contain Space")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]     
        public string Email { get; set; }

        [Required(ErrorMessage = "Age is Required")]
        public short? Age { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Course is Required")]
        public int? CourseId { get; set; }

        public string Course { get; set; }

        [Required(ErrorMessage = "Grade is Required")]
        public string Grade { get; set; }
    }
}
