using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class ModuleMasterViewModal
    {
        public long ModuleId { get; set; }

        public int Position { get; set; }

        public int DashboardPosition { get; set; }

        public string Name { get; set; }

        public string Link { get; set; }

        public string Icon { get; set; }

        public bool IsDelete { get; set; }

        public long? ParentFK { get; set; }

        public bool IsMaster { get; set; }
    }
}
