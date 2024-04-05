using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using Soc_Management_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Soc_Management_Web.Controllers
{
    public class OrderStatusController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        MasterDropdownHelper master = new MasterDropdownHelper();
        private readonly IWebHostEnvironment _hostingEnvironment;

        public OrderStatusController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(long id)
        {
            OrdEventStatus model = new OrdEventStatus();
            bool isreturn = false;
            INIT(ref isreturn);
            if (isreturn)
            {
                return RedirectToAction("index", "dashboard");
            }
            long userId = GetIntSession("UserId");
            long societyid = id;
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
            int clientId = 0;


            model.lstevents = master.GetDropgen("Event");
            model.lstcusomer = objProductHelper.GetCustomerListOnly();
            model.lstjob = master.GetDropgen("Job");
            model.lstoccation = master.GetDropgen("OrdTiitle");
            model.lststatus = master.GetDropgen("Status");
            return View(model);
        }

        private void INIT(ref bool isReturn)
        {
            #region User Rights
            long userId = GetIntSession("UserId");
            UserFormRightModel userFormRights = new UserFormRightModel();
            string currentURL = GetCurrentURL();
            userFormRights = GetUserRights(userId, currentURL);
            if (userFormRights == null)
            {
                SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                isReturn = true;
            }
            ViewBag.userRight = userFormRights;

            #endregion

            #region Dynamic Report

            if (userFormRights != null)
            {
                ViewBag.layoutList = GetGridLayoutDropDown(DbConnection.GridTypeView, userFormRights.ModuleId);
                ViewBag.pageNoList = GetPageNo();
            }

            #endregion
        }


        [HttpGet]
        public JsonResult GetListData(OrdEventStatus sc)
        {
            List<OrdEventStatuslist> lst = new List<OrdEventStatuslist>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[12];
                sqlParameters[0] = new SqlParameter("@fromdate", sc.fromDate);
                sqlParameters[1] = new SqlParameter("@toDate", sc.toDate);
                sqlParameters[2] = new SqlParameter("@fromno", sc.fromNo);
                sqlParameters[3] = new SqlParameter("@tono", sc.toNo);
                sqlParameters[4] = new SqlParameter("@customer", sc.cusomer);
                sqlParameters[5] = new SqlParameter("@ventTittle", sc.events);
                sqlParameters[6] = new SqlParameter("@ocationTitle", sc.occation);
                sqlParameters[7] = new SqlParameter("@job", sc.job);
                sqlParameters[8] = new SqlParameter("@status", sc.status);
                sqlParameters[9] = new SqlParameter("@type", "");
                sqlParameters[10] = new SqlParameter("@Orderby","");
                sqlParameters[11] = new SqlParameter("@flag", 1);
                DataTable dt = ObjDBConnection.CallStoreProcedure("usp_getorderstatus_eventstatus", sqlParameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        OrdEventStatuslist it = new OrdEventStatuslist();
                        it.sl = Convert.ToInt32(dt.Rows[i]["Sl"].ToString());
                        it.ordId = Convert.ToInt32(dt.Rows[i]["OrdVou"].ToString());
                        it.customer = dt.Rows[i]["AccNm"].ToString();
                        it.OrderTittle = dt.Rows[i]["OrdTitle"].ToString();
                        it.orddate = dt.Rows[i]["OrdDt"].ToString();
                        it.todate = dt.Rows[i]["OrdToDt"].ToString();
                        it.ordstatus = dt.Rows[i]["StatusName"].ToString();
                        it.remarks = dt.Rows[i]["OrdRem"].ToString();
                        lst.Add(it);
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return Json(new { success = false, result = "Faild" });
            }

            return Json(new { success = true, result = lst });

        }
    }
}
