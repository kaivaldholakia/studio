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
    public class DataReceiveController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DataReceiveController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(long id)
        {
            DatareceiveModel model = new DatareceiveModel();
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


            model.personlst = objProductHelper.GetCustomerList();
            model.lstCustomer = objProductHelper.GetCustomerListOnly();
            model.categorylst = objProductHelper.GetCategorylist();
            model.pendinglst = objProductHelper.Pending();
            model.orderbylst = objProductHelper.OrderBySchedul();
            model.eventlist = objProductHelper.GetEventLst();
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
        public JsonResult GetListData(DatareceiveModel sc)
        {
            List<DataReceivemodel> lst = new List<DataReceivemodel>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[11];
                sqlParameters[0] = new SqlParameter("@fromdate", sc.fromDate);
                sqlParameters[1] = new SqlParameter("@todate", sc.toDate);
                sqlParameters[2] = new SqlParameter("@status", sc.pending);
                sqlParameters[3] = new SqlParameter("@personid", sc.personId);
                sqlParameters[4] = new SqlParameter("@category", sc.categoryId);
                sqlParameters[5] = new SqlParameter("@customer", sc.customerId);
                sqlParameters[6] = new SqlParameter("@events", sc.events);
                sqlParameters[7] = new SqlParameter("@fromorderno", sc.fromorderno);
                sqlParameters[8] = new SqlParameter("@toorderno", sc.toorderno);
                sqlParameters[9] = new SqlParameter("@orderby", sc.orderBy);
                sqlParameters[10] = new SqlParameter("@flag", 1);
                DataTable dt = ObjDBConnection.CallStoreProcedure("usp_Gettdatareceivedata", sqlParameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataReceivemodel it = new DataReceivemodel();
                        it.sl = Convert.ToInt32(dt.Rows[i]["sl"].ToString());
                        it.id = Convert.ToInt32(dt.Rows[i]["id"].ToString());
                        it.orderId = Convert.ToInt32(dt.Rows[i]["orderId"].ToString());
                        it.AccNm = dt.Rows[i]["AccNm"].ToString();
                        it.OrtEvnNm = dt.Rows[i]["OrtEvnNm"].ToString();
                        it.FromDate = dt.Rows[i]["FromDate"].ToString();
                        it.DataYN = Convert.ToInt32(dt.Rows[i]["DataYN"].ToString());
                        it.Person = dt.Rows[i]["Person"].ToString();
                        it.Locations = dt.Rows[i]["Locations"].ToString();
                        it.Size = dt.Rows[i]["Size"].ToString();
                        it.Remarks = dt.Rows[i]["Remarks"].ToString();
                        it.SaveStatus = Convert.ToInt32(dt.Rows[i]["SaveStatus"].ToString());
                        it.JobCd = dt.Rows[i]["JobCd"].ToString();
                        it.venue = dt.Rows[i]["OrtVenNm"].ToString();
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

        [HttpPost]
        public JsonResult SavePersondata(DataReceiverequest obj)
        {
            string result = "";
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@Id", obj.id);
                sqlParameters[1] = new SqlParameter("@DataYN", obj.dataYN);
                sqlParameters[2] = new SqlParameter("@Locations", obj.locations);
                sqlParameters[3] = new SqlParameter("@Remarks", obj.remarks);
                sqlParameters[4] = new SqlParameter("@SaveStatus", obj.saveStatus);
                sqlParameters[5] = new SqlParameter("@Size", obj.size);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("upSp_Savedatareceivedata", sqlParameters);
                result = "Saved";
            }
            catch (Exception)
            {
                result = "Faild";
            }
            return Json(new { success = true, result = result });

        }
    }
}
