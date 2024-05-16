using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using Soc_Management_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Soc_Management_Web.Controllers
{
	public class MobileUserController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        public IActionResult Index(long id)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                MobileUsermodel mu = new MobileUsermodel();
                if (id > 0)
                {
                    mu.userid = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@userid", id);
                    sqlParameters[1] = new SqlParameter("@addby", 0);
                    sqlParameters[2] = new SqlParameter("@mob", "");
                    sqlParameters[3] = new SqlParameter("@email", "");
                    sqlParameters[4] = new SqlParameter("@name", "");
                    sqlParameters[5] = new SqlParameter("@userType", "");
                    sqlParameters[6] = new SqlParameter("@FLG", "2");
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("MobileuserMst_Insert", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        mu.email = DtEmp.Rows[0]["Email"].ToString();
                        mu.mob = DtEmp.Rows[0]["Mobile"].ToString();
                        mu.userid = id;
                        mu.userType = Convert.ToInt32(DtEmp.Rows[0]["UserType"].ToString());
                    }
                }
                mu.lstCustomer = objProductHelper.GetCustomerList();
                mu.usertypelst = objProductHelper.UserType();
                return View(mu);
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
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

        [HttpPost]
        public IActionResult Index(long id, MobileUsermodel cm)
        {
            try
            {
                string message = "";
                bool datastatus = false;
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                if (cm.userid>0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@userid", cm.userid);
                    sqlParameters[1] = new SqlParameter("@addby", userId);
                    sqlParameters[2] = new SqlParameter("@mob", cm.mob);
                    sqlParameters[3] = new SqlParameter("@email", cm.email);
                    sqlParameters[4] = new SqlParameter("@name", cm.name);
                    sqlParameters[5] = new SqlParameter("@userType", cm.userType);
                    sqlParameters[6] = new SqlParameter("@FLG", "1");
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("MobileuserMst_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Mobile User Details");
                            ViewBag.FocusType = "-1";
                        }
                        else
                        {
                            if (id > 0)
                            {
                                SetSuccessMessage("Update Sucessfully");
                                message = "Update Sucessfully";
                                datastatus = true;
                            }
                            else
                            {
                                SetSuccessMessage("Inserted Sucessfully");
                                message = "Inserted Sucessfully";
                                datastatus = true;
                            }
                            return Json(new { status= datastatus, msg= message });
                            //return RedirectToAction("index", "MobileUser", new { id =0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new MobileUsermodel());
        }
        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                if (gridMstId > 0)
                {
                    #region User Rights
                    long userId = GetIntSession("UserId");
                    UserFormRightModel userFormRights = new UserFormRightModel();
                    string currentURL = "/MobileUser/Index";
                    userFormRights = GetUserRights(userId, currentURL);
                    if (userFormRights == null)
                    {
                        SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                    }
                    ViewBag.userRight = userFormRights;
                    #endregion

                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));

                    double startRecord = 0;
                    if (pageIndex > 0)
                    {
                        startRecord = (pageIndex - 1) * pageSize;
                    }
                    getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId);
                    if (getReportDataModel.IsError)
                    {
                        ViewBag.Query = getReportDataModel.Query;
                        return PartialView("_reportView");
                    }
                    getReportDataModel.pageIndex = pageIndex;
                    getReportDataModel.ControllerName = "MobileUser";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }
    
      
        
    }
}
