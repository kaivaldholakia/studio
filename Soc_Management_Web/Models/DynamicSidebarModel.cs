using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class DynamicSidebarModel
    {
        public long ModuleId { get; set; }

        public string ModuleName { get; set; }

        public string Link { get; set; }

        public string Icons { get; set; }

        public List<DynamicSidebarDetailModel> sidebarDetail { get; set; }
    }

    public class DynamicSidebarDetailModel
    {
        public long ModuleId { get; set; }

        public string ModuleName { get; set; }

        public string Link { get; set; }

        public string Icons { get; set; }
    }
}
