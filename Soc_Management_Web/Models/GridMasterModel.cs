using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class GridMasterModel
    {
        public int GrdVou { get; set; }
        public int GrdFlg { get; set; }
        public int GrdMnuVou { get; set; }
        public string MnuName { get; set; }
        public List<SelectListItem> MenuList { get; set; }
        public string GrdType { get; set; }
        public List<SelectListItem> TypeList { get; set; }
        public string GrdName { get; set; }
        public string GrdTitle { get; set; }
        public int GrdMultiSelYN { get; set; }
        public List<SelectListItem> MultiYNList { get; set; }
        public string GrdQryFields { get; set; }
        public string GrdQryJoin { get; set; }
        public string GrdQryOrderBy { get; set; }
        public string GrdDate { get; set; }

        public string GrdATotYNString { get; set; }

        public string GrdALinkYNString { get; set; }

        public string GrdAHideYNString { get; set; }
        public string canGrowString { get; set; }
        public string GrdSaveAs { get; set; } = "0";

        public GridTransactionGridModel Gridtransaction { get; set; }
        public GridShortGridModel Gridshort { get; set; }

        public int GrdDftYNo { get; set; }
        public int PageSize { get; set; }

    }
    public class GridTransactionGridModel
    {
        public int GrdAVou { get; set; }
        public int GrdAGrdVou { get; set; }
        public int GrdASrNo { get; set; }

        public bool[] GrdAHideYN { get; set; }
        public string GrdAHideYNStr { get; set; }
        public List<SelectListItem> HideList { get; set; }
        public int[] GrdAPosition { get; set; }
        public string GrdAPositionStr { get; set; }
        public string[] GrdAColNm { get; set; }
        public string GrdAColNmStr { get; set; }
        public string[] GrdANewColNm { get; set; }
        public string GrdANewColNmStr { get; set; }
        public string[] GrdADbFld { get; set; }
        public string GrdADbFldStr { get; set; }
        public string[] GrdADbFld2 { get; set; }
        public string GrdADbFld2Str { get; set; }
        public int[] GrdADataType { get; set; }
        public string GrdADataTypeStr { get; set; }
        public List<SelectListItem> DataTypeList { get; set; }
        public int[] GrdAWidth { get; set; }
        public string GrdAWidthStr { get; set; }
        public int[] GrdADecUpTo { get; set; }
        public string GrdADecUpToStr { get; set; }
        public int[] GrdAAlign { get; set; }
        public string GrdAAlignStr { get; set; }
        public List<SelectListItem> AlignList { get; set; }
        public bool[] GrdATotYN { get; set; }
        public string GrdATotYNStr { get; set; }
        public List<SelectListItem> TotalYNList { get; set; }
        public bool[] GrdALinkYN { get; set; }
        public string GrdALinkYNStr { get; set; }
        public List<SelectListItem> LinkYNList { get; set; }
        public string[] GrdASuppressIFVal { get; set; }
        public string GrdASuppressIFValStr { get; set; }
        public bool[] GrdCanGrow { get; set; }


    }

    public class GridShortGridModel
    {
        public int GrdBVou { get; set; }
        public int GrdbGrdVou { get; set; }
        public int GrdbSrNo { get; set; }
        public string[] GrdBDbFld { get; set; }
        public string GrdBDbFldStr { get; set; }
        public string[] GrdBColNm { get; set; }
        public string GrdBColNmStr { get; set; }
        public bool[] GrdBDefYN { get; set; }
        public List<SelectListItem> DefaultYNList { get; set; }
        public string GrdBDefYNStr { get; set; }

    }


    public class DynamicRunqueryParaModel
    {
        public string Query { get; set; }
        public string ColumnName { get; set; }
    }
    public class GridRunQueryModel
    {
        public string Response { get; set; }
        public string Message { get; set; }
        public List<string> ColumnName { get; set; }

        public List<Object> RowsData { get; set; }
    }
}

