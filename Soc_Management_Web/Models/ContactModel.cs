using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class ContactModel
    {
        //internal List<SelectListItem> lstcategory;
        public int SocietyId { get; set; }
        public int Code { get; set; }
        public string Date { get; set; }
        public Int32 Category { get; set; }
        public string Name { get; set; }
        public string RefBy { get; set; }
        public Int32 Group { get; set; }
        public string Person1 { get; set; }
        public string Person2 { get; set; }
        public string Person3 { get; set; }
        public string Person4 { get; set; }
        public string Mobileno1 { get; set; }
        public string Mobileno2 { get; set; }
        public string Mobileno3 { get; set; }
        public string Mobileno4 { get; set; }
        public string Birthdate1 { get; set; }
        public string Birthdate2 { get; set; }
        public string Birthdate3 { get; set; }
        public string Birthdate4 { get; set; }
        public string Marrigedate1 { get; set; }
        public string Marrigedate2 { get; set; }
        public string Marrigedate3 { get; set; }
        public string Marrigedate4 { get; set; }
        public string Phone_R { get; set; }
        public string Phone_O { get; set; }
        public string Mail_id1 { get; set; }
        public string Mail_id2 { get; set; }
        public string Address_line1 { get; set; }
        public string Address_line2 { get; set; }
        public string Address_line3 { get; set; }
        public string Area { get; set; }
        public string drpArea { get; set; }
        public Int32 City { get; set; }
        public Int32 Pin_code { get; set; }
        public string Pan_no { get; set; }
        public List<SelectListItem> lstcategory { get; set; }
        public List<SelectListItem> lstGroup { get; set; }
        public List<SelectListItem> GetClientlist { get; set; }
        public List<SelectListItem> lstRefBy { get; set; }

        public List<SelectListItem> GetStateMasterDropdown { get; set; }
        public List<SelectListItem> GetCityMasterDropdown1 { get; set; }

    }
}
