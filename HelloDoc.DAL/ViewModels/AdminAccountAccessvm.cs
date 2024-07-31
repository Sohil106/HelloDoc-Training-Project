using HelloDoc.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDoc.DAL.ViewModels
{
    public class AdminAccountAccessvm
    {
        public List<AccountAccess> AccountAccess { get; set; }

        public AccountAccess CreateAccountAccess { get; set; }

        public List<AspNetRole> Aspnetroles { get; set; } //database

        public List<Menu> Menus { get; set; } //database

        public List<AccountMenu> AccountMenu { get; set; }
    }

    public class AccountAccess
    {
        public int roleid { get; set; }

        public int Adminid { get; set; }


        [Required(ErrorMessage = "Role Name Is Required")]
        public string name { get; set; }

        public string accounttype { get; set; }

        [Required(ErrorMessage = "Account Type Is Required")]
        public int accounttypeid { get; set; }

    }

    public class AccountMenu
    {
        public int menuid { get; set; }

        public int roleid { get; set; }

        public string name { get; set; }

        public int accounttype { get; set; }

        public bool ExistsInTable { get; set; }

    }
}
