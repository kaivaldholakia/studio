using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace PIOAccount.Models
{
    public class GodownOutwardModel
    {
        public int GdnOutwardVou { get; set; }

        public int GdnVNo { get; set; }

        public string GdnOutwardNo { get; set; }

        public string GdnOutwardDt { get; set; }

        public int GdnOutwardAccVou { get; set; }

        public int GdnOutwardGodownVou { get; set; }

        public List<OutwardProductModel> OutputProducts { get; set; }

        public string GdnOutwardVehicleNo { get; set; }

        public string GdnOutwardRemarks { get; set; }

        public List<SelectListItem> AccList { get; set; }

        public List<SelectListItem> GodownList { get; set; }

        public List<SelectListItem> ProductList { get; set; }

        //Compulsory to add for transaction
        public string Data { get; set; }
    }

    public class OutwardProductModel
    {
        public int OutputPrdVou { get; set; }

        public int PrdVou { get; set; }

        public string VakkalNo { get; set; }

        public double Qty { get; set; }

        public string Remarks { get; set; }
    }
}
