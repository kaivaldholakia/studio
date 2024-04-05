using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class GenLedgerModel
    {

        public string GenFrDate { get; set; }

        public string GenToDate { get; set; }

        public long GenAccVou { get; set; }

        public string AccountName { get; set; }

        public List<CustomDropDown> AccountList { get; set; }

        public long GenAccTVou { get; set; }

        public string AccountTName { get; set; }

        public List<SelectListItem> AccountTypeList { get; set; }

        public long GenDepVou { get; set; }

        public string Department { get; set; }

        public List<SelectListItem> DeptList { get; set; }

        public long GenVehVou { get; set; }

        public string Vehicle { get; set; }

        public List<SelectListItem> VehList { get; set; }


    }
}
