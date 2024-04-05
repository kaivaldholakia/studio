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
    public class AccountMasterController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        AccountMasterHelpers ObjaccountMasterHelpers = new AccountMasterHelpers();

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
                AccountMasterModel accountMasterModel = new AccountMasterModel();
                if (id > 0)
                {
                    accountMasterModel.AccVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@AccVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtAcc = ObjDBConnection.CallStoreProcedure("GetAccountDetails", sqlParameters);
                    if (DtAcc != null && DtAcc.Rows.Count > 0)
                    {
                        accountMasterModel.AccCd = (DtAcc.Rows[0]["AccCd"].ToString()).TrimEnd();
                        accountMasterModel.AccName = DtAcc.Rows[0]["AccNm"].ToString();
                        accountMasterModel.AccAdd = DtAcc.Rows[0]["AccAdd"].ToString();
                        accountMasterModel.AccCtyVou = Convert.ToInt32(DtAcc.Rows[0]["AccCtyVou"].ToString());
                        accountMasterModel.AccCity = DtAcc.Rows[0]["AccCity"].ToString();
                        accountMasterModel.AccState = DtAcc.Rows[0]["AccState"].ToString();
                        accountMasterModel.AccPhone = DtAcc.Rows[0]["AccPhone"].ToString();
                        accountMasterModel.AccMob = DtAcc.Rows[0]["AccMob"].ToString();
                        accountMasterModel.AccEmail = DtAcc.Rows[0]["AccEmail"].ToString();
                        accountMasterModel.AccGST = DtAcc.Rows[0]["AccGST"].ToString();
                        accountMasterModel.AccPAN = DtAcc.Rows[0]["AccPAN"].ToString();
                        accountMasterModel.AccGrpVou = Convert.ToInt32(DtAcc.Rows[0]["AccGrpVou"].ToString());
                    }
                }
                else
                {
                    accountMasterModel.AccType = Convert.ToInt32(HttpContext.Request.Cookies["Last_Type_Account"]);
                }
                accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                accountMasterModel.GroupList = ObjaccountMasterHelpers.GetGroupNameDropdown(companyId);
                return View(accountMasterModel);
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
        public IActionResult Index(long id, AccountMasterModel accountMasterModel)
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
                if (!string.IsNullOrWhiteSpace(accountMasterModel.AccName))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[16];
                    sqlParameters[0] = new SqlParameter("@AccName", accountMasterModel.AccName);
                    sqlParameters[1] = new SqlParameter("@AccCd", accountMasterModel.AccCd);
                    sqlParameters[2] = new SqlParameter("@AccAdd", accountMasterModel.AccAdd);
                    sqlParameters[3] = new SqlParameter("@AccCtyVou", accountMasterModel.AccCtyVou);
                    sqlParameters[4] = new SqlParameter("@AccCity", accountMasterModel.AccCity);
                    sqlParameters[5] = new SqlParameter("@AccState", accountMasterModel.AccState);
                    sqlParameters[6] = new SqlParameter("@AccPhone", accountMasterModel.AccPhone);
                    sqlParameters[7] = new SqlParameter("@AccMob", accountMasterModel.AccMob);
                    sqlParameters[8] = new SqlParameter("@AccEmail", accountMasterModel.AccEmail);
                    sqlParameters[9] = new SqlParameter("@AccGST", accountMasterModel.AccGST);
                    sqlParameters[10] = new SqlParameter("@AccPAN", accountMasterModel.AccPAN);
                    sqlParameters[11] = new SqlParameter("@AccVou", id);
                    sqlParameters[12] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[13] = new SqlParameter("@FLG", "1");
                    sqlParameters[14] = new SqlParameter("@AccCmpCdN", companyId);
                    sqlParameters[15] = new SqlParameter("@AccGrpVou", accountMasterModel.AccGrpVou);
                    DataTable DtAccount = ObjDBConnection.CallStoreProcedure("AccountMst_Insert", sqlParameters);
                    if (DtAccount != null && DtAccount.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtAccount.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Account Code");
                            ViewBag.FocusType = "-1";
                            accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                            accountMasterModel.GroupList = ObjaccountMasterHelpers.GetGroupNameDropdown(companyId);
                            return View(accountMasterModel);
                        }
                        else if (status == -2)
                        {
                            SetErrorMessage("Dulplicate Account");
                            ViewBag.FocusType = "-2";
                            accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                            accountMasterModel.GroupList = ObjaccountMasterHelpers.GetGroupNameDropdown(companyId);
                            return View(accountMasterModel);
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
                                HttpContext.Response.Cookies.Append("Last_Type_Account", Convert.ToString(accountMasterModel.AccType));
                            }
                            return RedirectToAction("index", "AccountMaster", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                        accountMasterModel.GroupList = ObjaccountMasterHelpers.GetGroupNameDropdown(companyId);
                        return View(accountMasterModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    accountMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                    accountMasterModel.GroupList = ObjaccountMasterHelpers.GetGroupNameDropdown(companyId);
                    return View(accountMasterModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new AccountMasterModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                AccountMasterModel accountMasterModel = new AccountMasterModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@AccVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", "1");
                    sqlParameters[2] = new SqlParameter("@skiprecord", "0");
                    sqlParameters[3] = new SqlParameter("@pagesize", "0");
                    sqlParameters[4] = new SqlParameter("@searchvalue", "");
                    sqlParameters[5] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtAcc = ObjDBConnection.CallStoreProcedure("GetAccountDetails", sqlParameters);
                    if (DtAcc != null && DtAcc.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtAcc.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Account Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "AccountMaster");
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
                    string currentURL = "/AccountMaster/Index";
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
                    getReportDataModel.ControllerName = "AccountMaster";
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
                    var bytes = Excel(getReportDataModel, "Account Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Account.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Account Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Account.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [Route("/accountmaster/city-list")]
        public IActionResult CityList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                CityList = CityList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(CityList) });
        }

        [Route("/accountmaster/group-list")]
        public IActionResult GroupList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var GroupList = ObjaccountMasterHelpers.GetGroupNameDropdown(companyId);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                GroupList = GroupList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(GroupList) });
        }

    }
}
