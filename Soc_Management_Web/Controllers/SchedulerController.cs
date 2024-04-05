using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using Soc_Management_Web.Classes;
using Soc_Management_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Soc_Management_Web.Controllers
{
    public class SchedulerController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SchedulerController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(long id)
        {
            ScheduleModel model = new ScheduleModel();
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
        public JsonResult GetListData(Schedulerequest sc)
        {
            List<scheduledisplaymodel> lst = new List<scheduledisplaymodel>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[8];
                sqlParameters[0] = new SqlParameter("@fromdate", sc.fromDate);
                sqlParameters[1] = new SqlParameter("@todate", sc.toDate);
                sqlParameters[2] = new SqlParameter("@status", sc.pending);
                sqlParameters[3] = new SqlParameter("@personid", sc.personId);
                sqlParameters[4] = new SqlParameter("@category", sc.categoryId);
                sqlParameters[5] = new SqlParameter("@customer", sc.customerId);
                sqlParameters[6] = new SqlParameter("@orderby", sc.orderBy);
                sqlParameters[7] = new SqlParameter("@flg", sc.flag);
                DataTable dt = ObjDBConnection.CallStoreProcedure("usp_GettScheduledata", sqlParameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        scheduledisplaymodel it = new scheduledisplaymodel();
                        it.sl = Convert.ToInt32(dt.Rows[i]["Sl"].ToString());
                        it.id = Convert.ToInt32(dt.Rows[i]["OrtVou"].ToString());
                        it.orderid = Convert.ToInt32(dt.Rows[i]["OrtOrdVou"].ToString());
                        it.customer= dt.Rows[i]["AccNm"].ToString();
                        it.category = dt.Rows[i]["JobCd"].ToString();
                        it.job = dt.Rows[i]["JobNm"].ToString();
                        it.events = dt.Rows[i]["OrtEvnNm"].ToString();
                        it.venue = dt.Rows[i]["OrtVenNm"].ToString();
                        it.fromtime = dt.Rows[i]["OrtFrTm"].ToString();
                        it.totime = dt.Rows[i]["OrtToTm"].ToString();
                        it.fromdate = dt.Rows[i]["OrtOccDt"].ToString();
                        it.todate = dt.Rows[i]["Todate"].ToString();
                        it.status = dt.Rows[i]["OrdEveStat"].ToString();
                        it.remarks = dt.Rows[i]["OrtRem"].ToString();
                        it.spremarks = dt.Rows[i]["OrtSpRem"].ToString();
                        it.person = dt.Rows[i]["Person"].ToString();
                        it.personremarks = dt.Rows[i]["PersonRemark"].ToString();
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

        [HttpGet]
        public JsonResult deletedata(GetAlldetails data)
        {

            List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
            SqlParameter[] sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("@IndId", data.TranId);
            sqlParameters[1] = new SqlParameter("@Id", data.Id);
            sqlParameters[2] = new SqlParameter("@Type", data.Type);
            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SpGet_deletedata", sqlParameters);
           
            return Json(new { etrItmObj });
        }
        [HttpGet]
        public JsonResult GetSmsData(long id, string tosend)
        {

            schedulemessage sm = new schedulemessage();
             SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@id", id);
            sqlParameters[1] = new SqlParameter("@tosend", tosend);
            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("prc_sendsms", sqlParameters);
             if(DtEmp.Rows.Count>0)
            {
                sm.message = DtEmp.Rows[0]["result"].ToString();
                sm.mobile = DtEmp.Rows[0]["mobile"].ToString();
                sm.person = DtEmp.Rows[0]["Person"].ToString();
            }
            return Json(new { sm });
        }

        public JsonResult sendonWhatsApp(string mobile, string data)
        {
           mobile = "9228263943";
            WatsappNotification watsapp = new WatsappNotification();
            var datas= watsapp.SendWhatAppMessage(mobile, data, "", "");
            return Json(new { datas });
        }
    }
}
                                     