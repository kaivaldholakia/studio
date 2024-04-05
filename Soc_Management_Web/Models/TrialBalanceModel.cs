using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class TrialBalanceModel
    {
        public string GenFrDate { get; set; }

        public string GenToDate { get; set; }

        public long GenAgrVou { get; set; }

        public string GroupName { get; set; }

        public List<SelectListItem> GroupList { get; set; }

        public long GenAgrTVou { get; set; }

        public string GroupTyName { get; set; }

        public List<SelectListItem> GroupTypeList { get; set; }
        public long GenDepVou { get; set; }

        public string Department { get; set; }

        public List<SelectListItem> DeptList { get; set; }

    }
}
