using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class Subcategory
    {
        public int SubCatVou { get; set; }
        public int CatVou { get; set; }
        public string SubCatNm { get; set; }
        public string SubCatCd { get; set; }
        public string Remarks { get; set; }
        public List<SelectListItem> LstCategory { get; set; }
    }
}
