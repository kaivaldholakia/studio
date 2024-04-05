using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
	public class MobileUsermodel
	{
		public long userid { get; set; }
		public string mob { get; set; }
		public string email { get; set; }
		public long userType { get; set; }
		public string name { get; set; }
		public long addby { get; set; }
		public List<SelectListItem> usertypelst { get; set; }
		public List<SelectListItem> lstCustomer { get; set; }
	}
}
