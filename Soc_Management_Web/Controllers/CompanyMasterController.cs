using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
    public class CompanyMasterController : BaseController
    {

        DbConnection ObjDBConnection = new DbConnection();

        public IActionResult Index(long id)
        {
            try
            {
                int companyid =  Convert.ToInt32(GetIntSession("CompanyId"));
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                CompanyMasterModel companyMasterModel = new CompanyMasterModel();
                if (id > 0)
                {
                    companyMasterModel.CmpVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@CmpVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    DataTable DtCompany = ObjDBConnection.CallStoreProcedure("GetCompanyDetails", sqlParameters);
                    if (DtCompany != null && DtCompany.Rows.Count > 0)
                    {
                        companyMasterModel.CmpCode = DtCompany.Rows[0]["CmpCode"].ToString();
                        companyMasterModel.CmpName = DtCompany.Rows[0]["CmpName"].ToString();
                        companyMasterModel.ClientId = Convert.ToInt32(DtCompany.Rows[0]["CLientId"].ToString());
                        companyMasterModel.MainLngID = Convert.ToInt32(DtCompany.Rows[0]["CmpLang"].ToString());
                        companyMasterModel.DataLngID = Convert.ToInt32(DtCompany.Rows[0]["CmpDataLang"].ToString());
                        companyMasterModel.StartDate = !string.IsNullOrWhiteSpace(DtCompany.Rows[0]["CmpStDt"].ToString()) ? Convert.ToDateTime(DtCompany.Rows[0]["CmpStDt"].ToString()).ToString("yyyy-MM-dd") : "";
                        companyMasterModel.EndDate = !string.IsNullOrWhiteSpace(DtCompany.Rows[0]["CmpEndDt"].ToString()) ? Convert.ToDateTime(DtCompany.Rows[0]["CmpEndDt"].ToString()).ToString("yyyy-MM-dd") : "";
                    }
                }
                companyMasterModel.GetClientlist = DbConnection.GetClientList(companyid, isadministrator);
                companyMasterModel.GetMainLnglist = DbConnection.GetLangList();
                companyMasterModel.GetDataLnglist = DbConnection.GetLangList();
                return View(companyMasterModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult Index(long id, CompanyMasterModel companyMasterModel)
        {
            try
            {
                int companyid = Convert.ToInt32(GetIntSession("CompanyId"));
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                companyMasterModel.GetClientlist = DbConnection.GetClientList(companyid, isadministrator);
                companyMasterModel.GetMainLnglist = DbConnection.GetLangList();
                companyMasterModel.GetDataLnglist = DbConnection.GetLangList();
                if ((id > 0 && !string.IsNullOrWhiteSpace(companyMasterModel.CmpName)) && (id < 0 && !string.IsNullOrWhiteSpace(companyMasterModel.CmpName) && !string.IsNullOrWhiteSpace(companyMasterModel.CmpCode)))
                {
                    companyMasterModel.StartDate = Convert.ToDateTime(companyMasterModel.StartDate).ToString("yyyy-MM-dd");
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "1";
                    return View(companyMasterModel);
                }
                else { 

                    SqlParameter[] sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("@CmpCode", companyMasterModel.CmpCode);
                    sqlParameters[1] = new SqlParameter("@CmpName", companyMasterModel.CmpName);
                    sqlParameters[2] = new SqlParameter("@CmpVou", id);
                    sqlParameters[3] = new SqlParameter("@ClientId", companyMasterModel.ClientId);
                    sqlParameters[4] = new SqlParameter("@StDate", companyMasterModel.StartDate);
                    sqlParameters[5] = new SqlParameter("@EndDate", companyMasterModel.EndDate);
                    sqlParameters[6] = new SqlParameter("@MainLngID", companyMasterModel.MainLngID);
                    sqlParameters[7] = new SqlParameter("@DataLngID", companyMasterModel.DataLngID);
                    DataTable DtCmpMst = ObjDBConnection.CallStoreProcedure("CompanyMst_Insert", sqlParameters);
                    if (DtCmpMst != null && DtCmpMst.Rows.Count > 0)
                    {
                        int Status = DbConnection.ParseInt32(DtCmpMst.Rows[0][0].ToString());
                        if (Status == -1)
                        {
                            companyMasterModel.StartDate= Convert.ToDateTime(companyMasterModel.StartDate).ToString("yyyy-MM-dd");
                            SetErrorMessage("Dulplicate Company code");
                            ViewBag.FocusType = "1";
                            return View(companyMasterModel);
                        }
                        else if (Status == -2)
                        {
                            companyMasterModel.StartDate = Convert.ToDateTime(companyMasterModel.StartDate).ToString("yyyy-MM-dd");
                            SetErrorMessage("Dulplicate Company name");
                            ViewBag.FocusType = "1";
                            return View(companyMasterModel);
                        }
                        else
                        {
                            if (id > 0)
                            {
                                SetSuccessMessage("Updated Sucessfully");
                            }
                            else
                            {
                                SetSuccessMessage("Inserted Sucessfully");
                            }
                            return RedirectToAction("index", "CompanyMaster", new { id = 0 });
                        }
                    }
                    else
                    {
                        companyMasterModel.StartDate = Convert.ToDateTime(companyMasterModel.StartDate).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(companyMasterModel);
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new CompanyMasterModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                CompanyMasterModel companyMasterModel = new CompanyMasterModel();
                if (id > 0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@CmpVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 1);
                    DataTable DtCompanyMst = ObjDBConnection.CallStoreProcedure("GetCompanyDetails", sqlParameters);
                    if (DtCompanyMst != null && DtCompanyMst.Rows.Count > 0)
                    {
                        SetSuccessMessage("Company Deleted Sucessfully");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "CompanyMaster");
        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/CompanyMaster/Index";
                userFormRights = GetUserRights(userId, currentURL);
                if (userFormRights == null)
                {
                    SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                }
                ViewBag.userRight = userFormRights;
                #endregion

                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int clientId = Convert.ToInt32(GetIntSession("ClientId"));
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));

                double startRecord = 0;
                if (pageIndex > 0)
                {
                    startRecord = (pageIndex - 1) * pageSize;
                }
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, 0, clientId, 0, "Company", isadministrator);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "CompanyMaster";
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
                    var bytes = Excel(getReportDataModel, "Company Master Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "CmpanyMaster.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Company Master Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "CompanyMaster.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
