using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class ProductSupplierMaster
    {
        public int MacVou { get; set; }
        public int  productnameid { get; set; }
        public int supplierid { get; set; }
        public decimal rate { get; set; }
        public string remarks { get; set; }
        public List<SelectListItem> lstProductList { get; set; }
        public List<SelectListItem> lstSupplierList { get; set; }
    }
}
