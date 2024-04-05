using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class VoucherTypeModel
    {
        public int VchVou { get; set; }
        public int VchTrnMscVou { get; set; }
        public List<CustomDropDown> TransTypeList { get; set; }
        public string TranType { get; set; }
        public string VchTrnMscCd { get; set; }
        public string VchType { get; set; }
        public string VchDesc { get; set; }
    }
}
