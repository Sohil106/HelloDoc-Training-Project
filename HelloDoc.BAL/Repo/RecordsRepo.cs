using HelloDoc.BAL.Interface;
using HelloDoc.DAL.DataContext;
using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels.Records;
using System.Collections;

namespace HelloDoc.BAL.Repo
{
    public class RecordsRepo : IRecordsRepo
    {
        private readonly ApplicationDbContext _db;

        public RecordsRepo(ApplicationDbContext db)
        {
            _db = db;
        }

       /// <summary>
       /// Fetch Records On Patient History Page
       /// </summary>
       /// <param name="UserId"></param>
       /// <param name="model"></param>
       /// <returns></returns>
        public GetRecordsvm GetRecordsTab(int UserId, GetRecordsvm model)
        {
            var usersList = _db.Users.ToList(); 

            if (model != null)
            {
                if (model.searchRecordOne != null)
                {
                    usersList = usersList.Where(r => r.FirstName != null && r.FirstName.Trim().ToLower().Contains(model.searchRecordOne.Trim().ToLower())).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    usersList = usersList.Where(r => r.LastName != null && r.LastName.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).ToList();
                }

                if (model.searchRecordThree != null)
                {
                    usersList = usersList.Where(r => r.Email != null && r.Email.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).ToList();
                }

                if (model.searchRecordFour != null)
                {
                    usersList = usersList.Where(r => r.Mobile != null && r.Mobile.Trim().ToLower().Contains(model.searchRecordFour.Trim().ToLower())).ToList();
                }
            }

            model.users = usersList.ToList();

            if (UserId != null && UserId != 0)
            {
                var recordList = _db.Requests.Where(i => i.UserId == UserId).ToList();
                foreach (var request in recordList)
                {
                    var requestClient = _db.RequestClients.FirstOrDefault(rc => rc.RequestId == request.RequestId);                   

                    if (requestClient != null)
                    {
                        request.FirstName = requestClient.FirstName;
                        request.LastName = requestClient.LastName;
                    }

                    var physician = _db.Physicians.FirstOrDefault(p => p.PhysicianId == request.PhysicianId);

                    if(physician != null)
                    {
                        request.Physician.FirstName = physician.FirstName;
                        request.Physician.LastName = physician.LastName;
                    }
                }

                model.requestList = recordList;
            }
            return model;
        }

        /// <summary>
        /// Fetch Records From RequestType Table
        /// </summary>
        /// <returns></returns>
        public List<RequestType> GetRequesttypes()
        {
            return _db.RequestTypes.ToList();
        }

       /// <summary>
       /// Fetch Records On SearchRecords Page
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        public SearchRecordsvm GetSearchRecords(SearchRecordsvm model)
        {
            var requestQuery = _db.Requests.Where(i => i.IsDeleted != new BitArray(1, true)).ToList();
            var recordList = new List<requests>();

            foreach (var request in requestQuery)
            {
                var requestClient = _db.RequestClients.Where(i => i.RequestId == request.RequestId).FirstOrDefault();
                var physician = _db.Physicians.Where(i => i.PhysicianId == request.PhysicianId).FirstOrDefault();
                var requestNotes = _db.RequestNotes.Where(i => i.RequestId == request.RequestId).FirstOrDefault();

                var newRequest = new requests
                {
                    patientname = (requestClient?.FirstName ?? "") + " " + (requestClient?.LastName ?? ""),
                    requestor = request.FirstName + " " + request.LastName,
                    dateOfService = null,
                    closeCaseDate = null,
                    email = requestClient?.Email,
                    contact = requestClient?.PhoneNumber,
                    address = requestClient?.Address,
                    zip = requestClient?.ZipCode,
                    status = (int)request.Status,
                    physician = (physician?.FirstName ?? "") + " " + (physician?.LastName ?? ""),
                    physicianNote = requestNotes?.PhysicianNotes,
                    providerNote = null,
                    AdminNote = requestNotes?.AdminNotes,
                    pateintNote = requestClient?.Notes,
                    requestid = request.RequestId,
                    userid = request.UserId,
                    requestTypeId = request.RequestTypeId,

                };

                recordList.Add(newRequest);
            }

            if (model != null)
            {
                if (model.searchRecordOne != null && model.searchRecordOne != 0)
                {
                    recordList = recordList.Where(r => r.status == model.searchRecordOne).Select(r => r).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    recordList = recordList.Where(r => r.patientname.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).Select(r => r).ToList();
                }

                if (model.searchRecordThree != null && model.searchRecordThree != 0)
                {
                    recordList = recordList.Where(r => r.requestTypeId == model.searchRecordThree).Select(r => r).ToList();
                }

                if (model.searchRecordSix != null)
                {
                    recordList = recordList.Where(r => r.physician != null && r.physician.Trim().ToLower().Contains(model.searchRecordSix.Trim().ToLower())).Select(r => r).ToList();
                }

                if (model.searchRecordSeven != null)
                {

                    recordList = recordList.Where(r => r.email.Trim().ToLower().Contains(model.searchRecordSeven.Trim().ToLower())).Select(r => r).ToList();
                }

                if (model.searchRecordEight != null)
                {
                    recordList = recordList.Where(r => r.contact != null && r.contact.Trim().ToLower().Contains(model.searchRecordEight.Trim().ToLower())).Select(r => r).ToList();
                }
            }

            model.requestList = recordList;

            return model;
        }

        /// <summary>
        /// Delete Request From Search Records Page
        /// </summary>
        /// <param name="requestId"></param>
        public void deletRequest(int requestId)
        {
            var request = _db.Requests.Where(i => i.RequestId == requestId).FirstOrDefault();

            if (request != null)
            {
                request.IsDeleted = new BitArray(1, true);
                request.ModifiedDate = DateTime.Now;

                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Fetch Records On EmailLogs Page and SMSLogs Page
        /// </summary>
        /// <param name="tempId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailSmsLogvm GetEmailSmsLog(int tempId, EmailSmsLogvm model)
        {
            model.tempid = tempId;
            var recordList = new List<emailSmsRecords>();

            if (tempId == 0)
            {
                var records = _db.EmailLogs.ToList();
                var rolelist = _db.AspNetRoles.ToList();

                foreach (var item in records)
                {
                    var newRecord = new emailSmsRecords
                    {
                        roleid = item.RoleId != null ? item.RoleId : null,
                        email = item.EmailId != null ? item.EmailId : "-",
                        createddate = item.CreateDate.ToString("yyyy-MM-dd"),
                        sentdate = item.SentDate?.ToString("yyyy-MM-dd"),
                        sent = item.IsEmailSent[0] ? "Yes" : "No",
                        recipient = item.EmailId.Substring(0, item.EmailId.IndexOf("@")),
                        rolename = item.RoleId != null ? rolelist.FirstOrDefault(i=> i.Id  == item.RoleId.ToString()).Name : "null" ,
                        senttries = item.SentTries,
                        confirmationNumber = item.ConfirmationNumber != null ? item.ConfirmationNumber : "-",
                        action = item.Action != null ? item.Action.ToString() : "-",
                    };

                    recordList.Add(newRecord);
                }
                if (model != null)
                {
                    if (model.searchRecordOne != null && model.searchRecordOne != 0)
                    {
                        recordList = recordList.Where(r => r.roleid == model.searchRecordOne).Select(r => r).ToList();
                    }

                    if (model.searchRecordTwo != null)
                    {
                        recordList = recordList.Where(r => r.recipient != null && r.recipient.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).Select(r => r).ToList();
                    }

                    if (model.searchRecordThree != null)
                    {
                        recordList = recordList.Where(r => r.email.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).Select(r => r).ToList();
                    }

                    if (model.searchRecordFour != null)
                    {
                        recordList = recordList.Where(r => r.createddate.Equals(model.searchRecordFour?.ToString("yyyy-MM-dd"))).Select(r => r).ToList();
                    }

                    if (model.searchRecordFive != null)
                    {

                        recordList = recordList.Where(r => r.sentdate.Equals(model.searchRecordFive?.ToString("yyyy-MM-dd"))).Select(r => r).ToList();
                    }
                }
                model.recordslist = recordList;

            }
            else
            {
                var records = _db.Smslogs.ToList();
                var rolelist = _db.AspNetRoles.ToList();
                foreach (var item in records)
                {

                    var newRecord = new emailSmsRecords
                    {
                        roleid = item.RoleId,
                        contact = item.MobileNumber,
                        createddate = item.CreateDate.ToString("yyyy-MM-dd"),
                        sentdate = item.SentDate?.ToString("yyyy-MM-dd"),
                        sent = item.IsSmssent[0] ? "Yes" : "No",
                        rolename = rolelist.FirstOrDefault(i => i.Id == item.RoleId.ToString()).Name,
                        senttries = item.SentTries,
                        confirmationNumber = item.ConfirmationNumber != null ? item.ConfirmationNumber : "-",
                        action = item.Action != null ? item.Action.ToString() : "-",

                    };
                    if (item.RequestId == null)
                    {
                        newRecord.recipient = _db.Physicians.FirstOrDefault(i => i.PhysicianId == item.PhysicianId).FirstName + " " + _db.Physicians.FirstOrDefault(i => i.PhysicianId == item.PhysicianId).LastName;
                            }
                    else
                    {
                        newRecord.recipient = _db.RequestClients.FirstOrDefault(i => i.RequestId == item.RequestId).FirstName + " " + _db.RequestClients.FirstOrDefault(i => i.RequestId == item.RequestId).LastName;

                        }
                    recordList.Add(newRecord);

                }
                if (model != null)
                {
                    if (model.searchRecordOne != null && model.searchRecordOne != 0)
                    {
                        recordList = recordList.Where(r => r.roleid == model.searchRecordOne).Select(r => r).ToList();
                    }

                    if (model.searchRecordTwo != null)
                    {
                        recordList = recordList.Where(r => r.recipient.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).Select(r => r).ToList();
                    }

                    if (model.searchRecordThree != null)
                    {
                        recordList = recordList.Where(r => r.contact.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).Select(r => r).ToList();
                    }

                    if (model.searchRecordFour != null)
                    {
                        recordList = recordList.Where(r => r.createddate.Equals(model.searchRecordFour?.ToString("yyyy-MM-dd"))).Select(r => r).ToList();
                    }

                    if (model.searchRecordFive != null)
                    {

                        recordList = recordList.Where(r => r.sentdate.Equals(model.searchRecordFive?.ToString("yyyy-MM-dd"))).Select(r => r).ToList();
                    }
                }
                model.recordslist = recordList;

            }
            return model;
        }

        /// <summary>
        ///  Fetch Records On Block History Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BlockRequestvm GetBlockedRequest(BlockRequestvm model)
        {

            var blockrequest = _db.BlockRequests.Where(i => i.IsActive == new BitArray(1, true)).ToList();

            var recordList = new List<blockedRequest>();
            foreach (var m in blockrequest)
            {
                var request = _db.RequestClients.Where(i => i.RequestId == int.Parse(m.RequestId));
                bool check;
                if (m.IsActive != null && m.IsActive.Length > 0)
                {
                    check = m.IsActive[0];
                }
                else
                {
                    check = false;
                }

                var newRecord = new blockedRequest
                {
                    patientname = request.Select(i => i.FirstName).FirstOrDefault() + " " + request.Select(i => i.LastName).FirstOrDefault(),
                    contact = m.PhoneNumber,
                    email = m.Email,
                    requestid = int.Parse(m.RequestId),
                    createddate = m.CreatedDate,
                    notes = m.Reason,
                    isactive = check,
                };
                recordList.Add(newRecord);
            }
            if (model != null)
            {
                if (model.searchRecordOne != null)
                {
                    recordList = recordList.Where(r => r.patientname != null && r.patientname.Trim().ToLower().Contains(model.searchRecordOne.Trim().ToLower())).ToList();
                }

                if (model.searchRecordTwo != null)
                {
                    recordList = recordList.Where(r => r.patientname != null && r.patientname.Trim().ToLower().Contains(model.searchRecordTwo.Trim().ToLower())).ToList();
                }

                if (model.searchRecordThree != null)
                {
                    recordList = recordList.Where(r => r.email != null && r.patientname.Trim().ToLower().Contains(model.searchRecordThree.Trim().ToLower())).ToList();
                }

                if (model.searchRecordFour != null)
                {
                    recordList = recordList.Where(r => r.contact != null && r.contact.Trim().ToLower().Contains(model.searchRecordFour.Trim().ToLower())).ToList();
                }
            }
            model.blockrequestList = recordList;
            return model;
        }

        /// <summary>
        /// Unblock Request 
        /// </summary>
        /// <param name="requestId"></param>
        public void UnblockRequest(int requestId)
        {
            string requestid = requestId.ToString();
            var blockrequest = _db.BlockRequests.Where(i => i.RequestId == requestid).FirstOrDefault();
            var request = _db.Requests.Where(i => i.RequestId == requestId).FirstOrDefault();
            if (blockrequest != null && request != null)
            {
                request.Status = 1;
                blockrequest.IsActive = new BitArray(1, false);
                blockrequest.ModifiedDate = DateTime.Now;
            }
            _db.SaveChanges();

        }

    }
}
