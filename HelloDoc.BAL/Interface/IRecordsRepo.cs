using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface IRecordsRepo
    {
        // ******************************************************* Patient Records History *******************************************************

        public GetRecordsvm GetRecordsTab(int UserId, GetRecordsvm model);

        public SearchRecordsvm GetSearchRecords(SearchRecordsvm model);

        public List<RequestType> GetRequesttypes();

        public void deletRequest(int requestId);

        public EmailSmsLogvm GetEmailSmsLog(int tempId, EmailSmsLogvm model);

        public BlockRequestvm GetBlockedRequest(BlockRequestvm model);

        public void UnblockRequest(int requestId);

    }
}
