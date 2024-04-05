using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Models
{
    public class AccountMasterModel
    {
        public int AccVou { get; set; }
        public string AccDate { get; set; }
        public string AccNm { get; set; }
        public string AccAdd1 { get; set; }
        public string AccAdd2 { get; set; }
        public string AccAdd3 { get; set; }
        public string AccAreaNm { get; set; }
        public string AccCtyNm { get; set; }
        public string AccPhoRes { get; set; }
        public string AccPhoOff { get; set; }
        public string AccEmail { get; set; }
        public int AccCatVou { get; set; }
        public string AccSalu { get; set; }
        public string AccRef { get; set; }
        public string AccRefVou { get; set; }
        public string AccPIN { get; set; }
        public string AccCont { get; set; }
        public float AccMob { get; set; }
        public string AccBirthDt { get; set; }
        public string AccMarriageDt { get; set; }
        public string AccCont2 { get; set; }
        public float AccMob2 { get; set; }
        public string AccBirthDt2 { get; set; }
        public string AccMarriageDt2 { get; set; }
        public string AccCont3 { get; set; }
        public float AccMob3 { get; set; }
        public string AccBirthDt3 { get; set; }

        public string AccMarriageDt3 { get; set; }
        public string AccCont4 { get; set; }
        public float AccMob4 { get; set; }
        public string AccBirthDt4 { get; set; }

        public string AccMarriageDt4 { get; set; }


        public string AccPANNo { get; set; }
        public int AccGrpVou { get; set; }
        public string AccGrp { get; set; }
        public string AccMerge { get; set; }
        public string AccEmail2 { get; set; }
        public float AccBirthDtn { get; set; }
        public float AccBirth2Dtn { get; set; }
        public float AccBirth3Dtn { get; set; }
        public float AccBirth4Dtn { get; set; }
        public float AccMarriageDtn { get; set; }
        public float AccMarriage2Dtn { get; set; }
        public float AccMarriage3Dtn { get; set; }
        public float AccMarriage4Dtn { get; set; }
        public float AccDtn { get; set; }
        public float AccFlag { get; set; }
        public float AccSrvIntVou { get; set; }
        public float AccMobVou { get; set; }
        public float AccPrefix { get; set; }
        public float AccLocId { get; set; }
        public float AccUpdLocId { get; set; }
        public string AccResAdd1 { get; set; }
        public string AccResAdd2 { get; set; }
        public string AccResAdd3 { get; set; }
        public string AccResArea { get; set; }
        public string AccResCity { get; set; }
        public string AccResPin { get; set; }

        public List<SelectListItem> TypeList { get; set; }
       
        public List<SelectListItem> GroupList { get; set; }

        public List<CustomDropDown> CityList { get; set; }

        
      

    }
}
