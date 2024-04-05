using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class GenSettingModel
    {
        public long GenVou { get; set; }
        public long GenCmpVou { get; set; }
        public string GenEmail { get; set; }
        public string GenPass { get; set; }
        public int GenSMTP { get; set; }
        public string GenHost { get; set; }
        public string GenWhtMob { get; set; }
        public string GenTokenID { get; set; }
        public string GenInstID { get; set; }
        public string GenSkruApi { get; set; }
        public string GenSURL { get; set; }
    }
}
