using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class OrderModel
    {
        public int sl { get; set; }
        public int OrderId { get; set; }
        public string OrderDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
        public string MobileNo { get; set; }
        public string OrderTittle { get; set; }
        public string Remarks { get; set; }
        public string ordAmount { get; set; }
        public string Address { get; set; }
        public int Customerid { get; set; }
        public string Customer { get; set; }
        public string ordSubTitle { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string refby { get; set; }
        public string OrdStatus { get; set; }
    }

   
}
