using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class CityModel
    {
        public int CtyVou { get; set; }
        public string CtyName { get; set; }
        public int CtyStaVou { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public string State { get; set; }
    }
}
