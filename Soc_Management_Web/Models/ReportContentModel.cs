using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class ReportContentModel
    {
        public string companyAddres { get; set; }
        public string CompanyTitle { get; set; }
        public long Sl { get; set; }
        public long Id { get; set; }
        public string Reportitle { get; set; }
    }
    public class scheduleReportModel : ReportContentModel
    {
        public string Date { get; set; }
        public string Customer { get; set; }
        public string functionaddress { get; set; }
        public string events { get; set; }
        public string fromtime { get; set; }
        public string totime { get; set; }
        public string category { get; set; }
        public string person { get; set; }
        public string address { get; set; }

    }
        public class PersonWiseEventReportModel : ReportContentModel
    {
        public string Customer { get; set; }
        public string EventDate { get; set; }
        public string Person { get; set; }
        public string Category { get; set; }
        public string PersonNote { get; set; }
        public string Tittle { get; set; }
        public string Job { get; set; }
        public string Statuss { get; set; }
        public string OrtToTm { get; set; }
        public string OrdRem { get; set; }
        public string OrdSpRem { get; set; }
        public string CustomerAddress { get; set; }
        public string Venueadd1 { get; set; }
        public string Adddres2 { get; set; }
        public string OrtFrTm { get; set; }
    }
    public class InquerystatusReportModel: ReportContentModel
    {
        public string Customer { get; set; }
        public string InqDate { get; set; }

        public string InqTitle { get; set; }
        public decimal Amount { get; set; }
        public string Statuss { get; set; }

    }
    public class PartyWiseEventReportModel : ReportContentModel
    {
        public string Customer { get; set; }
        public string EventDate { get; set; }

        public string Tittle { get; set; }
        public string Job { get; set; }
        public string Statuss { get; set; }

    }
}
