using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace PIOAccount.Models
{
    public class JobModel
    {
        public int jobid { get; set; }
        public string jobworkname { get; set; }
        public string shortname { get; set; }
        public string remarks { get; set; }
        public decimal rate { get; set; }
        public bool NoEntVnDate { get; set; }


        public int pno { get; set; }

        public string premarks1 { get; set; }
        public int pno1 { get; set; }
        public string premarks2 { get; set; }

        public int mno { get; set; }

        public string mremarks1 { get; set; }
        public int mno1 { get; set; }
        public string mremarks2 { get; set; }


        public string category { get; set; }
        public decimal qty1 { get; set; }

        public string product { get; set; }
        public decimal qty { get; set; }

        public List<SelectListItem> lstcategory { get; set; }
        public List<SelectListItem> lstproduct { get; set; }
        public List<ProductViewModel> lstProduct1 { get; set; }
        public List<MainPowerViewModel> lstMainPower { get; set; }

    }
    public class ProductViewModel
    {
        public int editSno { get; set; }
        public int SNo { get; set; }
        public int Qty { get; set; }
        public int PrVou { get; set; }
        public string PrNm { get; set; }
        public string Rmk { get; set; }
    }
    public class MainPowerViewModel
    {
        public int editSno { get; set; }
        public int SNo { get; set; }
        public int Qty { get; set; }
        public int CatVou { get; set; }
        public string CatNm { get; set; }
        public string Rmk { get; set; }
    }
}
