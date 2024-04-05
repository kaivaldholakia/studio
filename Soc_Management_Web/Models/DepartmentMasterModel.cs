using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class DepartmentMasterModel
    {
        public int DepVou { get; set; }

        public string DepName { get; set; }

        public string DepCode { get; set; }

        public string DepAdd { get; set; }

        public string DepCity { get; set; }
        public int DepCityVou { get; set; } = 0;
        public List<CustomDropDown> CityList { get; set; }
        public int DepStateVou { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public string DepState { get; set; }

        public string DepPAN { get; set; }

        public string DepGST { get; set; }

        public string DepPhone2 { get; set; }

        public string DepMob { get; set; }

        public string DepWhtMob { get; set; }

        public string DepEmail { get; set; }

        public string DepBusLine { get; set; }

        public string DepSTax { get; set; }

        public string DepJurd { get; set; }

        public string DepBnkNm { get; set; }

        public string DepBnkBrnch { get; set; }

        public string DepAcNo { get; set; }

        public string DepIFSC { get; set; }
        public List<SelectListItem> departmentList { get; set; }

        public string ProfilePicture { get; set; }

        public IFormFile profileLogo { get; set; }

        public int DepBtyVou { get; set; }

        public List<SelectListItem> BiltyName { get; set; }

        public int DepMemVou { get; set; }

        public List<SelectListItem> MemoName { get; set; }

        public int DepInvVou { get; set; }

        public List<SelectListItem> InvoiceName { get; set; }

        public string lblAdd { get; set; }
    }
}
