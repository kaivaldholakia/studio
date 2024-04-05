using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class OrdEventStatus
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public long fromNo { get; set; }
        public long toNo { get; set; }

        public string cusomer { get; set; }
        public string events { get; set; }
        public string occation { get; set; }
        public string job { get; set; }
        public string status { get; set; }

        public List<SelectListItem> lstcusomer { get; set; }
        public List<SelectListItem> lstevents { get; set; }
        public List<SelectListItem> lstoccation { get; set; }
        public List<SelectListItem> lstjob { get; set; }
        public List<SelectListItem> lststatus { get; set; }

    }

    public class OrdEventStatuslist
    {
        public long ordId  { get; set; }
        public long detailsid { get; set; }
        public long sl { get; set; }
        public string OrderTittle { get; set; }
        public string customer { get; set; }
        public string orddate { get; set; }
		public string todate { get; set; }
		public string ordstatus { get; set; }
        public string remarks { get; set; }

        public string events { get; set; }
        public string job { get; set; }
        public string jobcd { get; set; }
        public string venue { get; set; }
        public Int32 photosqty { get; set; }
        public Int32 videoqty { get; set; }
        public string videostatus { get; set; }
        public string photosstatus { get; set; }

        public string eventstatus { get; set; }

    }
}
