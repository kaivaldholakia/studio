using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class DataStorageEntryModel
    {
        public int TypeID { get; set; }
        public string DSEDate { get; set; }
        public string OdrNo { get; set; }
        public string CustNm { get; set; }
        public string EvntTit { get; set; }
        public int VidLocID { get; set; }
        public int PtoLocID { get; set; }
        public decimal VidQty { get; set; }
        public decimal PtoQty { get; set; }
        public string FileFrmtAndSize { get; set; }
        public bool AllVid { get; set; }
        public bool AllPto { get; set; }
        public string Remark { get; set; }
        public List<DropType> droptypelst { get; set; }
        public List<DropVidLoc> dropvidloclst { get; set; }
        public List<DropPtoLoc> dropptoloclst { get; set; }
        public List<SelectListItem> lstCustomer { get; set; }

    }

    public class DropType 
    { 
        public int TypeId { get; set; }
        public string TypeName { get; set; }
    }

    public class DropVidLoc
    { 
        public int VidLocId { get; set; }
        public string VidLocNm { get; set; }
    }

    public class DropPtoLoc
    {
        public int PtoLocId { get; set; }
        public string PtoLocNm { get; set; }
    }
}
