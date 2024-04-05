using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class RecPayModel
    {
        public int VcmVou { get; set; }
        public int VcmVNo { get; set; }
        public string VcmRefNo { get; set; }
        public string VcmDt { get; set; }
        public string StartDt { get; set; }
        public string EndDt { get; set; }
        public int VcmDtN { get; set; }
        public long VcmDepVou { get; set; }
        public string DepName { get; set; }
        public List<SelectListItem> DepList { get; set; }
        public long VcmBookVou { get; set; }
        public string BookName { get; set; }
        public List<CustomDropDown> BookAccList { get; set; }
        public long VcmDrCrVou { get; set; }
        public List<SelectListItem> DCList { get; set; }
        public string VcmDrCr { get; set; }
        public decimal VcmAmount { get; set; }
        public string VcmVouType { get; set; }
        public int VcmVouTypeN { get; set; }
        public List<CustomDropDown> VouTypeList { get; set; }
        public string VcmRemark { get; set; }
        public List<CustomDropDown> VcmAccountList { get; set; }
        public RecPayGridModel RecPay { get; set; }
        public List<RecPayGridModel> RPList { get; set; }

        //Compulsory to add for transaction
        public string Data { get; set; }
    }

    public class RecPayGridModel
    {
        public long VcmAVou { get; set; }
        public long VcmAVcmVou { get; set; }
        public int VcmASrNo { get; set; }
        public int VcmAAccVou { get; set; }

        public string AccName { get; set; }
        public decimal VcmAAmt { get; set; }
        public string VcmARefNo { get; set; }
        public string VcmARemarks { get; set; }

        public List<CustomDropDown> VcmAccountList { get; set; }
    }
}
