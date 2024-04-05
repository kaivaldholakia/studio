using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class schedulemessage
	{
		public string mobile { get; set; }
        public string person { get; set; }
        public string message { get; set; }
    }
    public class ScheduleModel
    {
        public string fromDate { get; set; } 
        public string toDate { get; set; }
        public string pending { get; set; }
        public long personId { get; set; }
        public long categoryId { get; set; }
        public string orderBy { get; set; }
        public long customerId { get; set; }
        public List<SelectListItem> lstCustomer { get; set; }
        public List<SelectListItem> orderbylst { get; set; }
        public List<SelectListItem> categorylst { get; set; }
        public List<SelectListItem> personlst { get; set; }
        public List<SelectListItem> pendinglst { get; set; }



    }
    public class DataReceiverequest
    {
        public long id { get; set; }
        public long dataYN { get; set; }

        public string locations { get; set; }
        public string size { get; set; }
        public string remarks { get; set; }
        public long saveStatus { get; set; }
    }
    public class DataReceivemodel
    {
        public long sl { get; set; }
        public long orderId { get; set; }
        public long id { get; set; }
        public string venue { get; set; }
        public string AccNm { get; set; }
        public string OrtEvnNm { get; set; }
        public string FromDate { get; set; }
        public long DataYN { get; set; }
        public string Person { get; set; }
        public string Locations { get; set; }
        public string Size { get; set; }
        public string Remarks { get; set; }
        public long SaveStatus { get; set; }
        public string JobCd { get; set; }
    }
    public class DatareceiveModel
    {
        public string fromDate { get; set; }
        public string toDate { get; set; } 
        public string pending { get; set; }
        public string events { get; set; }
        public string personId { get; set; }
        public string categoryId { get; set; }
        public long fromorderno { get; set; } = 0;
        public long toorderno { get; set; } = 0;
        public string orderBy { get; set; }
        public string customerId { get; set; }
        public List<SelectListItem> lstCustomer { get; set; }
        public List<SelectListItem> orderbylst { get; set; }
        public List<SelectListItem> categorylst { get; set; }
        public List<SelectListItem> personlst { get; set; }
        public List<SelectListItem> pendinglst { get; set; }
        public List<SelectListItem> eventlist { get; set; }


    }
    public class Schedulerequest
    {
        public string fromDate { get; set; } 
        public string toDate { get; set; } 
        public string pending { get; set; }
        public string personId { get; set; }
        public string categoryId { get; set; }
        public string orderBy { get; set; }
        public string customerId { get; set; }
        public string flag { get; set; }
    }
    public class scheduledisplaymodel
    {
        public long sl { get; set; }
        public long id { get; set; }
        public long orderid { get; set; }
        public string customer { get; set; }
        public string category { get; set; }
        public string job { get; set; }
        public string events { get; set; }
        public string venue { get; set; }
        public string fromtime { get; set; }
        public string totime { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
        public string spremarks { get; set; }
        public string person { get; set; }
        public string personremarks { get; set; }


    }
}
