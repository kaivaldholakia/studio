using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class ModuleMasterModal
    {
        
        public long ModuleId { get; set; }

        
        public string Name { get; set; }
        public string NameOth { get; set; }

        
        public string Link { get; set; }

        
        public string Icon { get; set; }

        
        public bool IsDelete { get; set; }

        public long? ParentFK { get; set; }

        public int ParentFKVou { get; set; }
        public List<SelectListItem> MenuList { get; set; }

        public int Position { get; set; }
        public int Deshboardpos { get; set; }
        public bool IsMaster { get; set; }

        public List<ModuleMasterViewModal> ModuleMasterViewModalList { get; set; }
    }
}
