using HelloDoc.DAL.Models;
using Microsoft.DotNet.Scaffolding.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HelloDoc.DAL.ViewModels
{
    public class ProviderDetailsvm
    {
        public int RegionId { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Message Is Required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Message Accepts Only Alphabets")]
        public string ContactMessage { get; set; }

        public List<Region> Regions { get; set; }

        public List<Provider> Providers { get; set; }

        //public List<ProviderList> ProviderList { get; set; }


    }

    public class Provider
    {
        public string aspId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public string CallStatus { get; set; }

        public short Status { get; set; }

        public bool IsNotificatonStopped {  get; set; } 

        public int PhysicianId { get; set; }
    }
}
