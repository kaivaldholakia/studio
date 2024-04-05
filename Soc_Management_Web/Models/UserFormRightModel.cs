using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class UserFormRightModel
    {
        public bool IsAdd { get; set; }

        public bool IsEdit { get; set; }

        public bool IsList { get; set; }

        public bool IsDelete { get; set; }

        public int ModuleId { get; set; }

        public string ModuleNm { get; set; }
    }
}
