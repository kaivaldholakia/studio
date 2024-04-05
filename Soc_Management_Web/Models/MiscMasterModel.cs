using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class MiscMasterModel
    {
        public int MscVou { get; set; }

        public string MscName { get; set; }

        public string MscCd { get; set; }

        public int MscPos { get; set; }

        public int MscActYNVou { get; set; }
        public List<SelectListItem> ActiveYNList { get; set; }
        public string Active { get; set; }

    }
}

