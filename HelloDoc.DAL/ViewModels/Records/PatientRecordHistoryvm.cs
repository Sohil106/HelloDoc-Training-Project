using HelloDoc.DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels.Records
{
    public class PatientRecordHistoryvm
    {
        public List<PatientRecordsList>? patientRecordsLists { get; set; }

        public List<PatientRecordExploreList>? patientRecordExploreLists { get; set; }

        public List<SearchRecordList>? searchRecordsList { get; set; }

        public List<RequestType>? requesttypes { get; set; }

        public List<BlockedHistoryeRecordsList>? blockedHistoryeRecordsLists { get; set; }

    }

    public class PatientRecordsList
    {
        public int RequestId { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string? LastName { get; set; }

        public string Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }


    }

    public class PatientRecordExploreList
    {
        public int RequestId { get; set; }

        public string? ClientName { get; set; }

        public string? ConfirmationNo { get; set; }

        public string? ProviderName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ConcludedDate { get; set; }

        public string? Status { get; set; }

    }

    public class SearchRecordList
    {
        public int requestid { get; set; }

        public string? PatientName { get; set; }

        public string? Requestor { get; set; }

        public DateTime? DateOfService { get; set; }

        public DateTime? CloseCaseDate { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Zip { get; set; }

        public int? RequestStatus { get; set; }

        public string? Physician { get; set; }

        public string? PhysicianNotes { get; set; }

        public string? CancelledbyProviderNote { get; set; }

        public string? AdminNote { get; set; }

        public string? PatientNote { get; set; }

        public string RequestType { get; set; }

    }

    public class BlockedHistoryeRecordsList
    {
        public int BlockRequestId { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Notes { get; set; }

        public BitArray isActive { get; set; }
    }

}
