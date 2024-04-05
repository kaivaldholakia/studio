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
    public class SubcategoryController : BaseController
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
                Subcategory subcatmodel = new Subcategory();
                if (id > 0)
                {
                    subcatmodel.SubCatVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@SubCatMstVou", id);
                    sqlParameters[1] = new SqlParameter("@FLG", 3);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_SubCatMaster_Insert", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        subcatmodel.CatVou = Convert.ToInt32(DtEmp.Rows[0]["intCatVou"].ToString());
                        subcatmodel.SubCatNm = DtEmp.Rows[0]["SubCatNm"].ToString();
                        subcatmodel.SubCatCd = DtEmp.Rows[0]["SubCatCd"].ToString();
                        subcatmodel.Remarks = DtEmp.Rows[0]["SubRemk"].ToString();
                    }
                }
                subcatmodel.LstCategory = objProductHelper.GetSubCategoryDropdown().OrderBy(X => X.Text).ToList();

                return View(subcatmodel);
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
        public IActionResult Index(long id, Subcategory subcatModel)
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
                if (!string.IsNullOrWhiteSpace(subcatModel.SubCatNm) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(subcatModel.SubCatVou).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@SubCatMstVou", id);
                    sqlParameters[1] = new SqlParameter("@intCatVou", subcatModel.CatVou);
                    sqlParameters[2] = new SqlParameter("@SubCatNm", subcatModel.SubCatNm);
                    sqlParameters[3] = new SqlParameter("@SubCatCd", subcatModel.SubCatCd);
                    sqlParameters[4] = new SqlParameter("@SubRemk", subcatModel.Remarks);
                    sqlParameters[5] = new SqlParameter("@FLG", "1");
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("usp_SubCatMaster_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate SubCategory Details");
                            ViewBag.FocusType = "-1";
                            subcatModel.LstCategory = objProductHelper.GetSubCategoryDropdown().OrderBy(x => x.Text).ToList();
                            return View(subcatModel);
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
                            return RedirectToAction("index", "Subcategory", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        subcatModel.LstCategory = objProductHelper.GetSubCategoryDropdown().OrderBy(x => x.Text).ToList();
                        return View(subcatModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    subcatModel.LstCategory = objProductHelper.GetSubCategoryDropdown().OrderBy(x => x.Text).ToList();
                    return View(subcatModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new Subcategory());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                //Subcategory subcatModel = new Subcategory();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@SubCatMstVou", id);
                    sqlParameters[1] = new SqlParameter("@FLG", 2);
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("usp_SubCatMaster_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("SubCategory Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "Subcategory");
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
                    string currentURL = "/Subcategory/Index";
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
                    getReportDataModel.ControllerName = "Subcategory";
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
