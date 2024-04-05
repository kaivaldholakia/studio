using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
	public class Webcategory
	{
		public long Id { get; set; }
		public string Catname { get; set; }
		public string filename { get; set; }
		public string Description { get; set; }
		public string path { get; set; }
		public IFormFile file { get; set; }
	}

	public class Webcategorydetails
	{
		public long Id { get; set; }
		public long catid { get; set; }
		public string Catname { get; set; }
		public string tittle { get; set; }
		public string videopath { get; set; }
		public string filename { get; set; }
		public string Description { get; set; }
		public string iamgepath { get; set; }
		public List<SelectListItem> Caltlst { get; set; }
		public IFormFile file { get; set; }
	}
}
