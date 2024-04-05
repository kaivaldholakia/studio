using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Soc_Management_Web.Models
{
    public class ProductMaster
    {
        public int productVou { get; set; }
        public string productname { get; set; }
        public string productcode { get; set; }
        public int productgroupid { get; set; }
        public decimal purchaserate { get; set; }
        public decimal salerate { get; set; }
        public string productdescription { get; set; }

        public List<SelectListItem> lstProductGroup { get; set; }
    }
}
