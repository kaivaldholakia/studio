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
    public class ProductGroupMasterController : BaseController
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
                ProductGroupMaster pgrModel = new ProductGroupMaster();
                if (id > 0)
                {
                    pgrModel.PgrVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@PgrVou", id);
                    sqlParameters[1] = new SqlParameter("@FLG", 3);
                    sqlParameters[2] = new SqlParameter("@PgrNm", "");
                    sqlParameters[3] = new SqlParameter("@PgrCd", "");
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Usp_ProductGroup_Insert", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        pgrModel.GroupName = DtEmp.Rows[0]["PgrNm"].ToString();
                        pgrModel.ShortName = DtEmp.Rows[0]["PgrCd"].ToString();
                    }
                }

                return View(pgrModel);
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
        public IActionResult Index(long id, ProductGroupMaster pgrModel)
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
                if (!string.IsNullOrWhiteSpace(pgrModel.GroupName) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(pgrModel.PgrVou).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@PgrVou", id);
                    sqlParameters[1] = new SqlParameter("@PgrNm", pgrModel.GroupName);
                    sqlParameters[2] = new SqlParameter("@PgrCd", pgrModel.ShortName);
                    sqlParameters[3] = new SqlParameter("@FLG", "1");

                    DataTable DtPgr = ObjDBConnection.CallStoreProcedure("Usp_ProductGroup_Insert", sqlParameters);
                    if (DtPgr != null && DtPgr.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtPgr.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate ProductGroup Details");
                            ViewBag.FocusType = "-1";
                            return View(pgrModel);
                        }
                        else
                        {
                            if (id > 0)
                            {
                                SetSuccessMessage("Update Sucessfully");
                            }
                            else
                            {
                                SetSuccessMessage("Inserted Sucessfully");
                            }
                            return RedirectToAction("index", "ProductGroupMaster", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        return View(pgrModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    return View(pgrModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new ProductGroupMaster());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                ProductGroupMaster pgrModel = new ProductGroupMaster();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@PgrVou", id);
                    sqlParameters[1] = new SqlParameter("@PgrNm", "");
                    sqlParameters[2] = new SqlParameter("@PgrCd", "");
                    sqlParameters[3] = new SqlParameter("@FLG", "2");
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("Usp_ProductGroup_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Product Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "ProductGroupMaster");
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
                    string currentURL = "/ProductGroupMaster/Index";
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
                    getReportDataModel.ControllerName = "ProductGroupMaster";
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

