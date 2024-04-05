using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class ClientMasterModel
    {
        public int ClientVou { get; set; }

        public string ClientName { get; set; }

        public string ClientMobile { get; set; }

        public string ClientEmail { get; set; }

        public int   ClientIsActive{ get; set; }

        public int ClientType{ get; set; }

        public List<SelectListItem> ClientTypeList { get; set; }
    }
}
