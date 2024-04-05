using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class QueryMasterModel
    {
        public  List<string> ColumnsList { get; set; }

        public List<object> RowsList { get; set; }

        public  string IsDisallow { get; set; }

        
    }

    public class SaveQuery
    {
        public string QueryId { get; set; }

        public string QueryCode { get; set; }

        public string QueryName { get; set; }

        public string QueryDesc { get; set; }

        public string QueryPrefix { get; set; }

        public string QuerySufix { get; set; }

        public string QueryFields { get; set; }

        public string QueryFlg { get; set; }

        public string IsDisallow { get; set; }

        public bool IsExists { get; set; }
    }
}
