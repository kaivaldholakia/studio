using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class InqueryReportModel
    {
        public string InqDt { get; set; }
        public string AccNm { get; set; }
        public string AccAdd1 { get; set; }
        public string InqMobile { get; set; }
        public string FileName { get; set; }
        public string AccEmail { get; set; }
        public string AccMob { get; set; }
        public string InqTitle { get; set; }
        public string IntOccDt { get; set; }
        public string Todate { get; set; }
        public string IntFrTm { get; set; }
        public string IntToTm { get; set; }
        public string IntEvnNm { get; set; }
        public string JobNm { get; set; }
        public string IntVenNm { get; set; }
        public decimal IntQty { get; set; }
        public decimal IntRt { get; set; }
        public decimal IntAmt { get; set; }
        public string Studio { get; set; }
        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public string pinstate { get; set; }
        public string mobile { get; set; }
        public string ownerEmail { get; set; }
        public string OwnerName { get; set; }
        public string phone { get; set; }
        public string Heading { get; set; }
        public string InqFooter { get; set; }
        public string FooterMobile { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmont { get; set; }
        public List<ExtraItemreport> ExtraItem { get; set; }
        public List<Inclusivereport> Inclusive { get; set; }
        public List<Exclusierepoer> exclusive { get; set; }
    }
    public class ExtraItemreport
    {
      
        public string IteEitNm { get; set; }
        public decimal IteEitAmt { get; set; }
    }
    public class Inclusivereport
    {

        public string IncTncNm { get; set; }
        public string IncTncDesc { get; set; }
    }
    public class Exclusierepoer
    {

        public string IExTncNm { get; set; }
        public string IExTncDesc { get; set; }
    }
    public class ReportFileInfo
    {
        public string Sendto { get; set; }
        public string Bodytext { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long Id { get; set; }
    }
}
