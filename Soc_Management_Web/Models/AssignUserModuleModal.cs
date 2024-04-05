using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class AssignUserModuleModal
    {
        
        public List<SelectListItem> UserMasterList { get; set; }
        public List<SelectListItem> UserRollList { get; set; }

        public long UserId { get; set; }
        public long UserRollId { get; set; }

        public List<UserModuleModal> ModuleMasterList { get; set; }

        public List<UserSubMenuModal> AssignedModule { get; set; }
    }

    
    public class UserModuleModal
    {
        public long ModuleId { get; set; }

        public string ModuleName { get; set; }

        public bool IsAdd { get; set; }

        public bool IsEdit { get; set; }

        public bool IsDelete { get; set; }

        public bool IsList { get; set; }

        public List<UserSubMenuModal> SubMenuModalList { get; set; }

    }

    public class UserSubMenuModal
    {
        public long ModuleId { get; set; }

        public string ModuleName { get; set; }

        public bool IsAdd { get; set; }

        public bool IsEdit { get; set; }

        public bool IsDelete { get; set; }

        public bool IsList { get; set; }

        public long UserId { get; set; }
        public long UserRollId { get; set; }
    }

    
    public class AssignedUserModuleList
    {
        public long UserId { get; set; }
        public long UserRollId { get; set; }

        public string UserName { get; set; }

        public string Modules { get; set; }
    }

}
