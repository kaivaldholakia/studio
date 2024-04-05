using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class OrderManpowerDetailsModel
    {
        public int Sl { get; set; }
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int InqId { get; set; }
        public int ProductId { get; set; }
        public int JobId { get; set; }
        public int Qty { get; set; }
        public int ManpowerId { get; set; }

        public string ProdName { get; set; }
        public string Venue { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string fromTime { get; set; }
        public string ToTime { get; set; }
    }
}
