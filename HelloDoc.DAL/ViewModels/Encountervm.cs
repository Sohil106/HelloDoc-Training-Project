using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class Encountervm
    {
        public int EncounterFormId { get; set; }

        public int RequestId { get; set; }
        public int CallId { get; set; }

        public int PhysicianId { get; set; }

        [Required(ErrorMessage = "Please Select Any Care Type")]
        public int? Option { get; set; }

        public int StatusForName { get; set; }

        [Required(ErrorMessage = "First Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{3,15}$", ErrorMessage = "Firstname Accepts Only Alphabets min of 3 and  max of 16 Characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{3,15}$", ErrorMessage = "Lastname Accepts Only Alphabets min of 3 and  max of 16 Characters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Location Is Required")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "BirthDate Is Required")]
        public DateTime BirthDate { get; set; }

        public DateTime? ServiceDate { get; set; }

        [Required(ErrorMessage = "PhoneNumber Is Required")]
        [RegularExpression(@"^(?!(\d)\1{9})[6,7,8,9]\d{9}$", ErrorMessage = "Please Enter Valid Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? HistoryOfPresentIllness { get; set; }

        public string? MedicalHistory { get; set; }

        public string? Medications { get; set; }

        public string? Allergies { get; set;}

        public string? Temperature { get; set;}

        public string? HR { get; set;}

        public string? RR { get; set;}

        public string? BloodPressureSystolic { get; set;}

        public string? BloodPressureDiastolic { get; set;}

        public string? O2 { get; set;}
        public string? Pain { get; set;}

        public string? HEENT { get; set;}

        public string? CV { get; set;}

        public string? Chest { get; set;}

        public string? ABD { get; set;}

        public string? Extr { get; set;}

        public string? Skin { get; set;}

        public string? Neuro { get; set;}

        public string? Other { get; set;}

        public string? Diagnosis { get; set;}

        public string? TreatmentPlan { get; set;}

        public string? MedicationDispensed { get; set;}

        public string? Procedures { get; set;}

        public string? FollowUp  { get; set;}


    }
}
