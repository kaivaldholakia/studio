using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class PostJobDetails
    {
        public int sl { get; set; }
        public int pjobentrychk { get; set; }
        public string Wedingceremonyoptions { get; set; }
        public int addoneday { get; set; }
        public int fullday { get; set; }
        public int Job { get; set; }
        public string JobName { get; set; }
        public string Eventsid { get; set; }
        public string venuelink { get; set; }
        public string venueid { get; set; }
        public string ocasiondate { get; set; }
        public string Todate { get; set; }
        public string fromtime { get; set; }
        public string totime { get; set; }
        public decimal qty { get; set; }
        public decimal rate { get; set; }
        public decimal amount { get; set; }
        public decimal discountpercentage { get; set; }
        public decimal discountamt { get; set; }
        public decimal totalnet { get; set; }
        public string remarks  { get; set; }
        public string spnoes { get; set; }
        public string status { get; set; }
        public string eventhrs { get; set; }
        public int IndId { get; set; }
        public int JobId { get; set; }
        public string date { get; set; }
        public decimal InqAmount { get; set; }

        public string VenueOneAddTo { get; set; }
        public string VenueToAddOne { get; set; }
        public string VenueToAddTwo { get; set; }
        public string VenueToUrl { get; set; }

    }

    public class GetAlldetails
    {
        public int TranId { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
      
    }
    public class OrderJob
    {
        public long OrderId { get; set; }
        public long JobId { get; set; }
        public string Type { get; set; }

        public long flag { get; set; }

    }
    public class ManpowerEntry
    {
        public long id { get; set; }
        public long orderId { get; set; }
        public string remarks { get; set; }
        public string person { get; set; }

        public int flag { get; set; }

    }
}
