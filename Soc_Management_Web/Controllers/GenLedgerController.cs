using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Models;

namespace PIOAccount.Controllers
{
    public class GenLedgerController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        
        public IActionResult Index()
        {
            GenLedgerModel genLedgerModel = new GenLedgerModel();
            try
            {
                bool isreturn = false;
                long companyId = GetIntSession("CompanyId");
                long userId = GetIntSession("UserId");
                long yearId = GetIntSession("YearId");
                var yearData = DbConnection.GetYearListByCompanyId(Convert.ToInt32(companyId)).Where(x => x.YearVou == yearId).FirstOrDefault();
                if (yearData != null)
                {
                    genLedgerModel.GenFrDate = yearData.StartDate;
                    genLedgerModel.GenToDate = yearData.EndDate;
                }
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                genLedgerModel.AccountList = ObjTaxMasterHelpers.GetSalesAccountDropdown(Convert.ToInt32(companyId), 0);
                genLedgerModel.DeptList = ObjTaxMasterHelpers.GetDepartmentDropdown(Convert.ToInt32(companyId));
                
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(genLedgerModel);
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
                ViewBag.layoutList = GetGridLayoutDropDown(DbConnection.GridTypeReport, userFormRights.ModuleId);
                ViewBag.pageNoList = GetPageNo();
            }

            #endregion

        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby, string fromDate, string toDate, string accountId, string departmentId)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                int userId = Convert.ToInt32(GetIntSession("UserId"));
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/GenLedger/Index";
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

                if (gridMstId == 59)
                {
                    if (!string.IsNullOrWhiteSpace(toDate) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(accountId).ToString()))
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[6];
                        sqlParameters[0] = new SqlParameter("@FromDate", fromDate);
                        sqlParameters[1] = new SqlParameter("@ToDate", toDate);
                        sqlParameters[2] = new SqlParameter("@AccVou", accountId);
                        sqlParameters[3] = new SqlParameter("@DeptVou", departmentId);
                        sqlParameters[4] = new SqlParameter("@UserID", userId);
                        sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                        DataTable DtGenLed = ObjDBConnection.CallStoreProcedure("Get_GENLEDGER_REPORT", sqlParameters);
                        if (DtGenLed != null && DtGenLed.Rows.Count > 0)
                        {
                            int Status = DbConnection.ParseInt32(DtGenLed.Rows[0][0].ToString());
                            if (Status == 1)
                            {
                                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, userId, 0, "", 0, 0, "");
                                if (getReportDataModel.IsError)
                                {
                                    ViewBag.Query = getReportDataModel.Query;
                                    return PartialView("_reportView");
                                }
                                getReportDataModel.pageIndex = pageIndex;
                                getReportDataModel.ControllerName = "GenLedger";
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(toDate) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(accountId).ToString()))
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[6];
                        sqlParameters[0] = new SqlParameter("@FromDate", fromDate);
                        sqlParameters[1] = new SqlParameter("@ToDate", toDate);
                        sqlParameters[2] = new SqlParameter("@AccVou", accountId);
                        sqlParameters[3] = new SqlParameter("@DeptVou", departmentId);
                        sqlParameters[4] = new SqlParameter("@UserID", userId);
                        sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                        DataTable DtGenLed = ObjDBConnection.CallStoreProcedure("Get_GENLEDGER_DETAIL", sqlParameters);
                        if (DtGenLed != null && DtGenLed.Rows.Count > 0)
                        {
                            int Status = DbConnection.ParseInt32(DtGenLed.Rows[0][0].ToString());
                            if (Status == 1)
                            {
                                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, userId, 0, "", 0, 0, "");
                                if (getReportDataModel.IsError)
                                {
                                    ViewBag.Query = getReportDataModel.Query;
                                    return PartialView("_reportView");
                                }
                                getReportDataModel.pageIndex = pageIndex;
                                getReportDataModel.ControllerName = "GenLedger";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        public IActionResult ExportToExcelPDF(int gridMstId, string searchValue, int type,string toDate,string accountId,string fromDate,string departmentId)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                int userId = Convert.ToInt32(GetIntSession("UserId"));
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);

                if (gridMstId == 59)
                {
                    if (!string.IsNullOrWhiteSpace(toDate) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(accountId).ToString()))
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[6];
                        sqlParameters[0] = new SqlParameter("@FromDate", fromDate);
                        sqlParameters[1] = new SqlParameter("@ToDate", toDate);
                        sqlParameters[2] = new SqlParameter("@AccVou", accountId);
                        sqlParameters[3] = new SqlParameter("@DeptVou", departmentId);
                        sqlParameters[4] = new SqlParameter("@UserID", userId);
                        sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                        DataTable DtGenLed = ObjDBConnection.CallStoreProcedure("Get_GENLEDGER_REPORT", sqlParameters);
                        if (DtGenLed != null && DtGenLed.Rows.Count > 0)
                        {
                            int Status = DbConnection.ParseInt32(DtGenLed.Rows[0][0].ToString());
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(toDate) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(accountId).ToString()))
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[6];
                        sqlParameters[0] = new SqlParameter("@FromDate", fromDate);
                        sqlParameters[1] = new SqlParameter("@ToDate", toDate);
                        sqlParameters[2] = new SqlParameter("@AccVou", accountId);
                        sqlParameters[3] = new SqlParameter("@DeptVou", departmentId);
                        sqlParameters[4] = new SqlParameter("@UserID", userId);
                        sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                        DataTable DtGenLed = ObjDBConnection.CallStoreProcedure("Get_GENLEDGER_DETAIL", sqlParameters);
                        if (DtGenLed != null && DtGenLed.Rows.Count > 0)
                        {
                            int Status = DbConnection.ParseInt32(DtGenLed.Rows[0][0].ToString());
                        }
                    }
                }

                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, userId, 0, "", 0, 1);
                if (type == 1)
                {
                    if (!string.IsNullOrWhiteSpace(departmentId))
                    {
                        var departmentDetails = DbConnection.GetDepartmentDetailsById(Convert.ToInt32(departmentId), companyId);
                        var bytes = Excel(getReportDataModel, "General Ledger Report", departmentDetails.DepName, departmentId);
                        return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "GenLedger.xlsx");
                    }
                    else
                    {
                        var bytes = Excel(getReportDataModel, "General Ledger Report", companyDetails.CmpName);
                        return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "GenLedger.xlsx");
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(departmentId))
                    {
                        List<string> headerValues = new List<string>();
                        headerValues.Add(string.Concat("From : ", Convert.ToDateTime(fromDate).ToString("dd-MM-yyyy"), " To : ", Convert.ToDateTime(toDate).ToString("dd-MM-yyyy")));
                        var accountDetails = DbConnection.GetAccountDetailsById(Convert.ToInt32(companyId), accountId);
                        headerValues.Add(string.Concat("Account Name : ", accountDetails.AccName+" ("+accountDetails.AccCd));

                        var departmentDetails = DbConnection.GetDepartmentDetailsById(Convert.ToInt32(departmentId), companyId);
                        //HttpResponseWritingExtensions.WriteAsync(this.Response, "Before PDF Genrate ");
                        var bytes = PDF(getReportDataModel, "General Ledger Report", departmentDetails.DepName, Convert.ToInt32(companyDetails.CmpVou).ToString(), departmentId, headerValues);
                        return File(
                              bytes,
                              "application/pdf",
                              "GenLedger.pdf");
                    }
                    else
                    {
                        List<string> headerValues = new List<string>();
                        headerValues.Add(string.Concat("From : ", Convert.ToDateTime(fromDate).ToString("dd-MM-yyyy"), " To : ", Convert.ToDateTime(toDate).ToString("dd-MM-yyyy")));
                        var accountDetails = DbConnection.GetAccountDetailsById(Convert.ToInt32(companyId), accountId);
                        headerValues.Add(string.Concat("Account Name : ", accountDetails.AccName + " (" + accountDetails.AccCd)+")");

                        //HttpResponseWritingExtensions.WriteAsync(this.Response, "Before PDF Genrate ");

                        var bytes = PDF(getReportDataModel, "General Ledger Report", companyDetails.CmpName, Convert.ToInt32(companyDetails.CmpVou).ToString(),"0", headerValues);
                        return File(
                              bytes,
                              "application/pdf",
                              "GenLedger.pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
