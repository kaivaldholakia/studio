using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PIOAccount.Classes;
using PIOAccount.Models;

namespace PIOAccount.Controllers
{
    public class AccountGroupController : BaseController
    {

        DbConnection ObjDBConnection = new DbConnection();
        AccountGroupHelpers ObjaccountGroupHelpers = new AccountGroupHelpers();

        public IActionResult Index(long id)
        {
            try
            {
                bool isreturn = false;
                long companyId = GetIntSession("CompanyId");
                long userId = GetIntSession("UserId");
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                AccountGroupMasterModel accountGroupMasterModel = new AccountGroupMasterModel();
                if (id > 0)
                {
                    accountGroupMasterModel.AgrVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@AgrVOU", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtAccountGroup = ObjDBConnection.CallStoreProcedure("GetAccountGroupDetails", sqlParameters);
                    if (DtAccountGroup != null && DtAccountGroup.Rows.Count > 0)
                    {
                        accountGroupMasterModel.AgrName = DtAccountGroup.Rows[0]["AgrName"].ToString();
                        accountGroupMasterModel.AgrType = DbConnection.ParseInt32(DtAccountGroup.Rows[0]["AgrType"].ToString());
                        accountGroupMasterModel.AgrSrNo = DbConnection.ParseInt32(DtAccountGroup.Rows[0]["AgrSrNo"].ToString());
                        accountGroupMasterModel.AgrCate = DbConnection.ParseInt32(DtAccountGroup.Rows[0]["AgrCate"].ToString());
                        accountGroupMasterModel.AgrCrDr = DbConnection.ParseInt32(DtAccountGroup.Rows[0]["AgrCrDr"].ToString());
                    }
                }
                accountGroupMasterModel.GroupType = ObjaccountGroupHelpers.GetGroupType();
                accountGroupMasterModel.Category = ObjaccountGroupHelpers.GetCategory();
                accountGroupMasterModel.AccountCrDr = ObjaccountGroupHelpers.GetAccountCrDr();
                accountGroupMasterModel.accountgroupList = ObjaccountGroupHelpers.GetOpositeGroupDropdown(Convert.ToInt32(companyId));
                return View(accountGroupMasterModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult Index(long id, AccountGroupMasterModel accountGroupMasterModel)
        {
            try
            {
                bool isreturn = false;
                long companyId = GetIntSession("CompanyId");
                long userId = GetIntSession("UserId");
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                accountGroupMasterModel.GroupType = ObjaccountGroupHelpers.GetGroupType();
                accountGroupMasterModel.Category = ObjaccountGroupHelpers.GetCategory();
                accountGroupMasterModel.AccountCrDr = ObjaccountGroupHelpers.GetAccountCrDr();
                accountGroupMasterModel.accountgroupList = ObjaccountGroupHelpers.GetOpositeGroupDropdown(Convert.ToInt32(companyId));

                if (!string.IsNullOrWhiteSpace(accountGroupMasterModel.AgrName))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("@AgrName", accountGroupMasterModel.AgrName);
                    sqlParameters[1] = new SqlParameter("@AgrType", accountGroupMasterModel.AgrType);
                    sqlParameters[2] = new SqlParameter("@AgrSrNo", accountGroupMasterModel.AgrSrNo);
                    sqlParameters[3] = new SqlParameter("@AgrCate", accountGroupMasterModel.AgrCate);
                    sqlParameters[4] = new SqlParameter("@AgrCrDr", accountGroupMasterModel.AgrCrDr);
                    sqlParameters[5] = new SqlParameter("@AgrVou", id);
                    sqlParameters[6] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[7] = new SqlParameter("@UsrVou", userId);

                    DataTable DtAccountGroup = ObjDBConnection.CallStoreProcedure("AccountGroup_Insert", sqlParameters);
                    if (DtAccountGroup != null && DtAccountGroup.Rows.Count > 0)
                    {
                        int Status = DbConnection.ParseInt32(DtAccountGroup.Rows[0][0].ToString());
                        if (Status == 0)
                        {
                            SetErrorMessage("Duplicate Account Group Details");
                            ViewBag.FocusType = "1";
                            return View(accountGroupMasterModel);
                        }
                        else
                        {
                            if (id > 0)
                            {
                                SetSuccessMessage("Update Successfully");
                            }
                            else
                            {
                                SetSuccessMessage("Inserted Successfully");
                            }
                            return RedirectToAction("index", "AccountGroup", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(accountGroupMasterModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "1";
                    return View(accountGroupMasterModel);
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
                AccountGroupMasterModel accountGroupMasterModel = new AccountGroupMasterModel();
                if (id > 0)
                {
                    long companyId = GetIntSession("CompanyId");
                    long userId = GetIntSession("UserId");
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@AgrVOU", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 1);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtAccountGroup = ObjDBConnection.CallStoreProcedure("GetAccountGroupDetails", sqlParameters);
                    if (DtAccountGroup != null && DtAccountGroup.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtAccountGroup.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Account Group Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "AccountGroup");
        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/AccountGroup/Index";
                userFormRights = GetUserRights(userId, currentURL);
                if (userFormRights == null)
                {
                    SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                }
                ViewBag.userRight = userFormRights;
                #endregion

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
                getReportDataModel.ControllerName = "AccountGroup";
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
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
                    var bytes = Excel(getReportDataModel, "Account Group Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "AccountGroup.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Account Group Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "AccountGroup.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
