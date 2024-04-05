using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class TransactionGridModel
    {
        public int GridTransactionId { get; set; }

        public int FKMenuId { get; set; }

        public string LayoutName { get; set; }

        public string Style { get; set; }

        public string TableId { get; set; }

        public List<TransactionGridTransactionModel> TransactionList { get; set; }

        public List<SelectListItem> AlignmentList { get; set; }

        public List<SelectListItem> TypeList { get; set; }

        public List<SelectListItem> MenuList { get; set; }

        public bool IsTotal { get; set; }
    }

    public class TransactionGridTransactionModel
    {
        public int TransactionId { get; set; }

        public string TableHeaderName { get; set; }

        public string FieldName { get; set; }

        public int? Position { get; set; }

        public int? Width { get; set; }

        public int? Decimal { get; set; }

        public int? Align { get; set; }

        public bool TotalYN { get; set; }

        public bool HideYN { get; set; }

        public bool CanGridYN { get; set; }

        public string SupressValue { get; set; }

        public string Style { get; set; }

        public int? Type { get; set; }
    }

    public class TransactionGridDetailModel
    {
        public int GridTransactionId { get; set; }

        public int FKMenuId { get; set; }

        public string MenuName { get; set; }

        public string LayoutName { get; set; }

        public string Style { get; set; }

    }

    public class TransactionGridAddUpdateModel
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public string DisplayValue { get; set; }
    }

    public class TransactionGridAddUpdateDataModel
    {
        public List<TransactionGridAddUpdateModel> Data { get; set; }
    }

    public class TransactionGridAddUpdateRootModel
    {
        public List<TransactionGridAddUpdateDataModel> MyArray { get; set; }
    }


}
