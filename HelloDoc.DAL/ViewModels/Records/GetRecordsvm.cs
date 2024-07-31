using HelloDoc.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels.Records
{
    public class GetRecordsvm
    {
        public List<User>? users { get; set; }
        public List<Request>? requestList { get; set; }
        public List<RequestClient>? requestClient { get; set; }
        public string? searchRecordOne { get; set; }
        public string? searchRecordTwo { get; set; }
        public string? searchRecordThree { get; set; }
        public string? searchRecordFour { get; set; }
    }
}
