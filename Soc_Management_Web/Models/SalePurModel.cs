using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class SalePurModel
    {
        public int SalVou { get; set; }
        public int SalVNo { get; set; }
        public string SalRefNo { get; set; }
        public string SalDt { get; set; }
        public string StartDt { get; set; }
        public string EndDt { get; set; }
        public int SalDtN { get; set; }
        public long SalDepVou { get; set; }
        public string DepName { get; set; }
        public List<SelectListItem> DepList { get; set; }
        public long SalBookVou { get; set; }
        public string BookName { get; set; }
        public List<CustomDropDown> BookAccList { get; set; }
        public long SalAccVou { get; set; }
        public string AccName { get; set; }
        public List<CustomDropDown> AccNameList { get; set; }
        public decimal SalAmount { get; set; }
        public string SalVouType { get; set; }
        public int SalVouTypeN { get; set; }
        public List<CustomDropDown> VouTypeList { get; set; }
        public string SalRemark { get; set; }
        public string SalDrvNm { get; set; }
        public string SalDrvAdd { get; set; }
        public decimal SalCashAmt { get; set; }
        public decimal SalTaxable { get; set; }
        public decimal SalCGSTAmt { get; set; }
        public decimal SalSGSTAmt { get; set; }
        public decimal SalTaxAmt { get; set; }
        public decimal SalBankAmt { get; set; }
        public decimal SalNetAmt { get; set; }
        public List<CustomDropDown> ProductList { get; set; }
        public SalePurGridModel SalePur { get; set; }
        public List<RecPayGridModel> SPList { get; set; }

        //Compulsory to add for transaction
        public string Data { get; set; }
    }
    public class SalePurGridModel
    {
        public long SalAVou { get; set; }
        public long SalASalVou { get; set; }
        public int SalASrNo { get; set; }
        public int SalAPrdVou { get; set; }
        public string PrdName { get; set; }
        public decimal SalAQty { get; set; }
        public decimal SalARate { get; set; }
        public decimal SalAAmt { get; set; }
        public decimal SalATaxRate { get; set; }
        public decimal SalATaxAmt { get; set; }
        public decimal SalANetAmt { get; set; }
        public string SalARemarks { get; set; }
        public List<CustomDropDown> ProductList { get; set; }
   }
}