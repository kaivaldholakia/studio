using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class YearMasterModel
    {
        public int YearVou { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int IsDefault { get; set; }

        public int CompanyVou { get; set; }

        public List<SelectListItem> GetCompanyList { get; set; }
    }
}
