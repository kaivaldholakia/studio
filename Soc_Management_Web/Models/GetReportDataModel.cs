using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class GetReportDataModel
    {
        public List<ReportColumnConfiguration> ColumnsData { get; set; }

        public List<object> RowsData { get; set; }

        public int totalRecord { get; set; }

        public int pageIndex { get; set; }
        public int multiYN { get; set; }

        public int pageSize { get; set; }

        public int ReportType { get; set; }
        public string GrdTitle { get; set; }

        public string ControllerName { get; set; }

        public string Query { get; set; }

        public bool IsError { get; set; }
        public int DocumentPageSize { get; set; }
    }

    public class ReportColumnConfiguration
    {
        public string DbFldName { get; set; }
        
        public string GrdANewColNm{ get; set; }

        public string GrdAPosition { get; set; }

        public string GrdAAlign { get; set; }

        public string GrdADataType { get; set; }

        public string GrdAWidth { get; set; }

        public string GrdATotYN { get; set; }

        public string GrdALinkYN { get; set; }

        public string GrdASuppressIfVal { get; set; }

        public string GrdADecUpTo { get; set; }

        public string GrdAHideYN { get; set; }
        public string GrdCanGrow { get; set; }

        public string OrderBy { get; set; }
    }
}
