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
    public class AdminViewUploadsvm
    {
        public string? ConformationNumber { get; set; }
        
        public int? CallId { get; set; }

        public int statusForName { get; set; }

        public int RequestId { get; set; }

        public int? UserId { get; set; }

        public string Name { get; set; }

        public List<ViewUploadsModel> viewUploadsList { get; set; }

        public IFormFile Document { get; set; }

        [Required(ErrorMessage = "Provider note is Required")]
        [RegularExpression(@"^[a-zA-Z ]{3,400}$", ErrorMessage = "Only allow to write letters and spaces and letters betweem(3 to 400)")]
        public string ProviderNote { get; set; }

    }

    public class ViewUploadsModel
    {
        public int requestWiseFileId { get; set; }

        public int RequestId { get; set; }  
        

        public string DocumentName { get; set; }

        public DateTime UploadDate { get; set; }     

    }
}
