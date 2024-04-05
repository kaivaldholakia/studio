using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
 

namespace Soc_Management_Web.Models
{
    public class ProductGroupMaster
    {
        public int PgrVou { get; set; }
        public string GroupName { get; set; }
        public string ShortName { get; set; }
    }
}
