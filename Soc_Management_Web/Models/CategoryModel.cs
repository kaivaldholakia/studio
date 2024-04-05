using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ShortCode { get; set; }
        public int CategoryType { get; set; }
        public List<SelectListItem> lstcategory { get; set; }
    }
}
