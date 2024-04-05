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
        public string Person { get; set; }
        public int ManpowerId { get; set; }
        public int MyProperty { get; set; }
        public string ProdName { get; set; }
        public string Venue { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string fromTime { get; set; }
        public string ToTime { get; set; }
        public string StatusName { get; set; }
        public long Statusid { get; set; }
        public string? job { get; set; }
        public string CustomerName { get; set; }
        public string? ordSubTittle { get; set; }
        public string? OrdTitle { get; set; }
        public string? cusmobile { get; set; }
        public string? ordremarks { get; set; }

    }

    public class Schedulelistord
    {
       
        public string fromtime { get; set; }
        public string totime { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string Person { get; set; }
        public string OrdTitle { get; set; }
        public string events { get; set; }
        public long Statusid { get; set; }
        public string? CatNm { get; set; }
    }

    public class notification
    {

        public string fromtime { get; set; }
        public string totime { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string events { get; set; }
        public long IsViewbyPerson { get; set; }
        public long IsViewByAdmin { get; set; }
        public long id { get; set; }
        public string? person { get; set; }
    }
}
