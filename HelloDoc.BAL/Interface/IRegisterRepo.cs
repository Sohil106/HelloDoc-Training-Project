using HelloDoc.DAL.Models;
using HelloDoc.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.BAL.Interface
{
    public interface IRegisterRepo
    {
        public void RegisterUser(Registervm registerVm);

        public void ResetPassword(Registervm registerVm);

        public AspNetUser GetUserRole(string Email);

    }
}

