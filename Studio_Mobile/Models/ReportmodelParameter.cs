using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class ReportmodelParameter
    {
        public string FileName { get; set; }
        public string occasionfromDate { get; set; }
        public string occasiontoDate { get; set; }
        public long orderFromNo { get; set; }
        public long orderToNo { get; set; }
        public string customer { get; set; }
        public string eventTittle { get; set; }
        public string occasionTitle { get; set; }
        public string job { get; set; }
        public string category { get; set; }
        public string peningstatus { get; set; }
        public string person { get; set; }
        public string sortorder { get; set; }
        public string layout { get; set; }
        public string jobStatus { get; set; }
        public string orderStatus { get; set; }

        public string inqFromDate { get; set; }
        public string inqToDate { get; set; }

        public string eventFromDate { get; set; }
        public string eventToDate { get; set; }
        public string InqStatus { get; set; }
        public List<SelectListItem> lstperson { get; set; }
        public List<SelectListItem> lstcusomer { get; set; }
        public List<SelectListItem> lsteventstittle { get; set; }
        public List<SelectListItem> lstoccassiontittle { get; set; }
        public List<SelectListItem> lstlayout { get; set; }
        public List<SelectListItem> lstjob { get; set; }
        public List<SelectListItem> lslCategory { get; set; }
        public List<SelectListItem> lststatus { get; set; }
        public List<SelectListItem> lstpendingsatus { get; set; }
        public List<SelectListItem> inqstatuslst { get; set; }
        public List<SelectListItem> lstPending { get; set; }
        public List<SelectListItem> lstsortorder { get; set; }

    }

    public class ReportmodelParameterpost
    {
        public string occasionfromDate { get; set; }
        public string occasiontoDate { get; set; }
        public long orderFromNo { get; set; }
        public long orderToNo { get; set; }
        public string customer { get; set; }
        public string eventTittle { get; set; }
        public string occasionTitle { get; set; }
        public string job { get; set; }
        public string category { get; set; }
        public string peningstatus { get; set; }
        public string person { get; set; }
        public string sortorder { get; set; }
        public string layout { get; set; }
        public string jobStatus { get; set; }
        public string orderStatus { get; set; }

        public string inqFromDate { get; set; }
        public string inqToDate { get; set; }

        public string eventFromDate { get; set; }
        public string eventToDate { get; set; }
        public string InqStatus { get; set; }
       

    }
}
