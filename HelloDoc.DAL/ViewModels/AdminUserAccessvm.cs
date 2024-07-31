using HelloDoc.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class AdminUserAccessvm
    {
        public List<AspNetRole> Aspnetroles { get; set; }

        public List<UserAccess> UserAccesses { get; set; }
    }

    public class UserAccess
    {
        public string AspId { get; set; }

        public int AccountTypeId { get; set; }

        public string AccountType { get; set; }

        public string AccountHolderName { get; set; }

        public string AccountPhone { get; set; }

        public short AccountStatus { get; set; }

        public int AccountRequests { get; set; }
    }
}
