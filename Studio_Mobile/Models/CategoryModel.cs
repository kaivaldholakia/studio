using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studio_Mobile.Models
{
    public class CategoryModel
    {
        public int id { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
        public string addDate { get; set; }
        public string fileNames { get; set; }
        public string FilePath { get; set; }
    }

    public class CategorydetailsModel
    {
        public int id { get; set; }
        public int categoryid { get; set; }
        public string categoryName { get; set; }
        public string catdescription { get; set; }
        public string categorydetails { get; set; }
        public string tittle { get; set; }
        public string videoPath { get; set; }
        public string addDate { get; set; }
        public string fileNames { get; set; }
        public string FilePath { get; set; }

        public string catdetails { get; set; }
        public string cattittle { get; set; }
        public string catbanner { get; set; }
    }
}
