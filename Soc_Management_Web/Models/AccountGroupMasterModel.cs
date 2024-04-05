using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class AccountGroupMasterModel
    {

        public int AgrVou { get; set; }

        public string AgrName { get; set; }

        public int AgrType { get; set; }

        public List<SelectListItem> GroupType { get; set; }

        public int AgrOpGrp { get; set; }

        public List<SelectListItem> OpositeGroup { get; set; }

        public int AgrSrNo { get; set; }

        public int AgrSumm { get; set; }

        public List<SelectListItem> SummaryYesNo { get; set; }

        public int AgrCate { get; set; }

        public List<SelectListItem> Category { get; set; }

        public int AgrCrDr { get; set; }

        public List<SelectListItem> AccountCrDr { get; set; }

        public List<SelectListItem> accountgroupList { get; set; }

    }
}
