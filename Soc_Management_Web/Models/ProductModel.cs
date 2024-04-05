using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class ProductModel
    {
        public int PrdVou { get; set; }
        public string PrdName { get; set; }
        public string PrdCode { get; set; }
        public decimal PrdGSTRt { get; set; }
        public decimal PrdSalRt { get; set; }
        public decimal PrdPurRt { get; set; }

    }
}
