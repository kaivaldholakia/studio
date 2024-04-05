using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class GenSettingMasterModel
    {
        public int GENVou { get; set; }
        public int GenCmpVou { get; set; }
        public int SalAccVou { get; set; }
        public int PurAccVou { get; set; }

        public List<CustomDropDown> SalesList { get; set; }
        public List<CustomDropDown> PurchaseList { get; set; }

        // New added by chirag on 07/02/2023 START
        //public string GenEmail { get; set; }
        //public string GenPass { get; set; }
        //public int GenSMTP { get; set; }
        //public string GenHost { get; set; }
        //public string GenWhtMob { get; set; }
        //public string GenTokenID { get; set; }
        //public string GenInstID { get; set; }
        //public string GenSkruApi { get; set; }
        //public string GenSURL { get; set; }
        // New added by chirag on 07/02/2023 END
    }
}
