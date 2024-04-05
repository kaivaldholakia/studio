using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studio_Mobile.Models
{
	public class MobileUsermodel
	{
		public long userid { get; set; }
		public string mob { get; set; }
		public string email { get; set; }
		public long userType { get; set; }
		public string name { get; set; }

		public string deviceId { get; set; }
	}
}
