using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class ReviewAgreementvm
    {
        public int RequestId { get; set; }

        public string PatientName { get; set; }

        [Required(ErrorMessage = "Cancelation Reason is Required")]
        public string CancellationNotes { get; set; }
    }
}
