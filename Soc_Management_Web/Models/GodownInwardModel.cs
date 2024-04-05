using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class GodownInwardModel
    {
        public int GdnInwardVou { get; set; }
        public int GdnVNo { get; set; }
        public string GdnInwardNo { get; set; }
        public string GdnInwardDt { get; set; }  
        public int GdnInwardAccVou { get; set; }
        public List<SelectListItem> AccList { get; set; }
        public int GdnInwardGodownVou { get; set; }
        public List<SelectListItem> GodownList { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public InwardProductModel InwardProduct { get; set; }
        public List<InwardProductModel> InwardProducts { get; set; }
        public string GdnInwardVehicleNo { get; set; }
        public string GdnInwardRemarks { get; set; }

        //Compulsory to add for transaction
        public string Data { get; set; }
    }


    public class InwardProductModel
    {
        public int InwardPrdVou { get; set; }
        public int PrdVou { get; set; }
        public string VakkalNo { get; set; }
        public decimal Qty { get; set; }
        public string Remarks { get; set; }
        public List<SelectListItem> ProductList { get; set; }
    }
}
