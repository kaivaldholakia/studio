using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Controllers
{
    public class StateMasterController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objproductHelpers = new ProductHelpers();
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
                var userRight = ViewBag.userRight as UserFormRightModel;
                MscMstPartialViewModel mscMstPartialViewModel = new MscMstPartialViewModel();
                mscMstPartialViewModel.Placeholder = "State";
                mscMstPartialViewModel.IsEdit = userRight != null ? userRight.IsEdit : false;
                mscMstPartialViewModel.IsAdd = userRight != null ? userRight.IsAdd : false; ;
                mscMstPartialViewModel.RedirectURL = "/StateMaster/Index";
                mscMstPartialViewModel.Type = "STA";
                if (id > 0)
                {
                    mscMstPartialViewModel.MscVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@VOU", id);
                    sqlParameters[1] = new SqlParameter("@Type", "STA");
                    sqlParameters[2] = new SqlParameter("@Flg", 2);
                    sqlParameters[3] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtMisc = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                    if (DtMisc != null && DtMisc.Rows.Count > 0)
                    {
                        if (DtMisc.Rows[0]["MscActYN"].ToString().TrimEnd() == "No")
                        {
                            mscMstPartialViewModel.MscActYNVou = 1;
                        }
                        else
                        {
                            mscMstPartialViewModel.MscActYNVou = 0;
                        }
                        mscMstPartialViewModel.ActiveYN = DtMisc.Rows[0]["MscActYN"].ToString();

                        mscMstPartialViewModel.MscName = DtMisc.Rows[0]["MscNm"].ToString();
                        mscMstPartialViewModel.MscCd = DtMisc.Rows[0]["MscCd"].ToString();
                        mscMstPartialViewModel.MscPos = Convert.ToInt32(DtMisc.Rows[0]["MscPos"].ToString());
                    }
                }
                mscMstPartialViewModel.ActiveYNList = objproductHelpers.GetProductYesNo();
                return View(mscMstPartialViewModel);
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
                    string currentURL = "/StateMaster/Index";
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
                    getReportDataModel.ControllerName = "StateMaster";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }
        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                //getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, 0, "", 0, 1);
                getReportDataModel = getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, YearId, "", 0, 1);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "State Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "State.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "State Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "State.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IActionResult Delete(long id)
        {
            try
            {
                MscMstPartialViewModel mscMstPartialViewModel = new MscMstPartialViewModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@VOU", id);
                    sqlParameters[1] = new SqlParameter("@Type", "STA");
                    sqlParameters[2] = new SqlParameter("@Flg", 1);
                    sqlParameters[3] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtMscMst = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                    if (DtMscMst != null && DtMscMst.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtMscMst.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("State Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "StateMaster");
        }
    }
}
