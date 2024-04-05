using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class LocationOrderModel
    {

        public long LocationId { get; set; }
        public long OrderId { get; set; }
        public string RecType { get; set; }
        public bool AllPhotos { get; set; }
        public bool Allvieos { get; set; }
        public string VideoLocaton { get; set; }
        public string PhotosLocation { get; set; }
        public List<SelectListItem> lstCustomer { get; set; }
        public long VideoQty { get; set; }
        public long PhotosQty { get; set; }

        public string Fileformatedetails { get; set; }

        public string Remarks { get; set; }

        public string Dates { get; set; }
        public long CustomerId { get; set; }
        public string EventTittle { get; set; }

        public List<SelectListItem> TypeList { get; set; }

    }
}
