using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Soc_Management_Web.Models
{
    public class termsandcondition
    {
        public int TncTypeId { get; set; }
        public string TncName { get; set; }
        public string TncDesc { get; set; }
        public List<SelectListItem> tncTypeList { get; set; }
    }
}
