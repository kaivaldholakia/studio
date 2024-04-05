using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using Soc_Management_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Controllers
{
    public class DropdownController : Controller
    {
        MasterDropdownHelper mddhelper = new MasterDropdownHelper();
        // Save Term and condition data
        [HttpPost]
        public JsonResult GetJobddl()
        {
            try
            {
                var data = mddhelper.GetDrop("Job");
                return Json(new { data });
            }
            catch (Exception)
            {

                return null;
            }  
            //var filteredJobs = from job in data
            //                   where job.TextName.ToUpper().StartsWith(Prefix.ToUpper())
            //                   select new DropdownMasterModel
            //                   {
            //                       Value = job.Value,
            //                       TextName = job.TextName
            //                   };

          
        }
        [HttpPost]
        public JsonResult GetEvent()
        {
            try
            {
                var data = mddhelper.GetDrop("Event");
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }
           
           

        }

        [HttpPost]
        public JsonResult GetCustomer()
        {
            try
            {
                var data = mddhelper.GetDrop("Customer");
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }



        }
        [HttpPost]
        public JsonResult GetVenue()
        {
            try
            {
              
                var data = mddhelper.GetDrop("Venue");
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception)
            {

                return null;
            }


        }

        public JsonResult getNoasEditale(long no,string type)
        {
            try
            {

                var data = mddhelper.getAutono(no, type);
                return Json(data.Select(x => new { value = x.Value }));
            }
            catch (Exception)
            {

                return null;
            }


        }

        [HttpPost]
        public JsonResult GetjobDetailsById(int Id)
        {
            try
            {
                var data = mddhelper.GetJobDetails("");
                var filteredJobs = from job in data
                                   where job.jobid==Id
                                   select job;
                return Json(new { filteredJobs });
            }
            catch (Exception)
            {

                return null;
            }



        }

        [HttpPost]
        public JsonResult GetExtraItem()
        {
            try
            {
                var data = mddhelper.GetDrop("ExtraItem");
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult GetTermsAndCOndition(int type)
        {
            try
            {
                var data = mddhelper.GetTermsAndCondition(type);
                return Json(data.Select(x => new { label = x.Value, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }
        }
        [HttpPost]
        public JsonResult GetTermsAndCOndition1(int type)
        {
            try
            {
                var data = mddhelper.GetTermsAndCondition(type);
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }
        }
        [HttpPost]
        public JsonResult GetStatus()
        {
            try
            {
                var data = mddhelper.GetDrop("Status");
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }
        }
        public JsonResult GetPersonwithcategory(string Category,long Id)
        {
            try
            {
                var data = mddhelper.GetpersonbyCategory(Id,Category);
                return Json(data.Select(x => new { label = x.Text, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }
        }
        [HttpPost]
        public JsonResult GetInqSubMobFooter(int type)
        {
            try
            {
                var data = mddhelper.GetInqSubMobFootern(type);
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }
        }

        [HttpPost]
        public JsonResult InQueryTittle()
        {
            try
            {
                var data = mddhelper.GetDrop("InQueryTittle");
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }



        }
        [HttpPost]
        public JsonResult Refrence()
        {
            try
            {
                var data = mddhelper.GetDrop("Refrence");
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }



        }

        [HttpPost]
        public JsonResult InSubQueryTittle()
        {
            try
            {
                var data = mddhelper.GetDrop("InSubQueryTittle");
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }



        }

        [HttpPost]
        public JsonResult InqueryNo()
        {
            try
            {
                var data = mddhelper.GetDrop("InqueryNo");
                return Json(data.Select(x => new { label = x.TextName, value = x.Value }));
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(null);
            }



        }
    }
}
