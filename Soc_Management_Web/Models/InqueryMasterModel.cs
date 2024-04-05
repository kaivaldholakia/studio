using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class InqueryMasterModel
    {
        public int sl { get; set; }
        public int InquiryVou { get; set; }
        public int InquiryNo { get; set; }
        public string InquiryDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
        public string MobileNo { get; set; }
        public string InquiryTitle { get; set; }
        public string Remarks { get; set; }
        public string InqAmount { get; set; }
        public string Address { get; set; }
        public bool manual { get; set; } = false;
        public int ExtraItemId { get; set; }
        public List<SelectListItem> lstExtraitem { get; set; }
        public int TeamrAndConditionId { get; set; }
        public List<SelectListItem> lstTermsAndCondition { get; set; }
        public int Customerid { get; set; }
        public List<SelectListItem> lstCustomer { get; set; }
        public int InqStatusId { get; set; } = 13;
        public List<SelectListItem> lstInqStatus { get; set; }
        public string RefId { get; set; }
        public string OdrNo { get; set; }

        public string Customer { get; set; }
        public List<SelectListItem> lstReference { get; set; }
        public int jobid { get; set; }
        public List<SelectListItem> lstJobmaster { get; set; }
        public int Eventid { get; set; }
        public List<SelectListItem> EventLst { get; set; }

        public string InqSubTitle { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
    }

    public class InquerySubMasterModel
    {
        public List<InqJobModel> inqjobmodel { get; set; }

    }
    public class InqJobModel
    {
        public int No { get; set; }
        public string Job { get; set; }
        public string Event { get; set; }
        public string Venue { get; set; }
        public string OccasionDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Qty { get; set; }
        public int Rate { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public string SpNote { get; set; }
        public int Amount { get; set; }
        public int DiscPerc { get; set; }
        public int DiscAmt { get; set; }
        public int NetAmt { get; set; }
        public int EventHrs { get; set; }
        public string ExtraJob { get; set; }
    }

   
}
