using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class MscMstPartialViewModel
    {
        public string MscName { get; set; }

        public string MscCd { get; set; }

        public int MscPos { get; set; }

        public int MscVou { get; set; }

        public bool IsEdit { get; set; }

        public bool IsAdd { get; set; }

        public string RedirectURL { get; set; }

        public string Placeholder { get; set; }

        public string Type { get; set; }

        public int MscActYNVou { get; set; }
        public List<SelectListItem> ActiveYNList { get; set; }
        public string ActiveYN { get; set; }

    }
}
