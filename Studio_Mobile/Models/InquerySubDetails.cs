using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Models
{
    public class InquerySubDetails
    {
    }
    public class InqTermandCondition
    {
        public int InqId { get; set; }
        public int TermInqId { get; set; }
        public int SlNo { get; set; }
        public string Terms { get; set; }
        public string Description { get; set; }
        public int status { get; set; }
    }
}
