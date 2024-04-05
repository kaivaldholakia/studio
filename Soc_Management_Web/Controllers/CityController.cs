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
    public class CityController : BaseController
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
                CityModel cityModel = new CityModel();
                if (id > 0)
                {
                    cityModel.CtyVou= Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@CtyVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetCityDetails", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        cityModel.CtyName = DtEmp.Rows[0]["CtyNm"].ToString();
                        cityModel.CtyStaVou = Convert.ToInt32(DtEmp.Rows[0]["CtyStaVou"].ToString());
                    }
                }
                cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator);

                return View(cityModel);
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
        public IActionResult Index(long id, CityModel cityModel)
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
                if (!string.IsNullOrWhiteSpace(cityModel.CtyName) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(cityModel.CtyStaVou).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@CtyName", cityModel.CtyName);
                    sqlParameters[1] = new SqlParameter("@CtyStaVou", cityModel.CtyStaVou);
                    sqlParameters[2] = new SqlParameter("@CtyState", cityModel.State);
                    sqlParameters[3] = new SqlParameter("@CtyVou", id);
                    sqlParameters[4] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[5] = new SqlParameter("@FLG", "1");
                    sqlParameters[6] = new SqlParameter("@CtyCmpCdN", companyId);
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("CityMst_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate City Details");
                            ViewBag.FocusType = "-1";
                            cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(cityModel);
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
                            return RedirectToAction("index", "City", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(cityModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(cityModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new CityModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                CityModel cityModel = new CityModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@CtyName", "");
                    sqlParameters[1] = new SqlParameter("@CtyStaVou", "0");
                    sqlParameters[2] = new SqlParameter("@CtyState", "");
                    sqlParameters[3] = new SqlParameter("@CtyVou", id);
                    sqlParameters[4] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[5] = new SqlParameter("@FLG", "2");
                    sqlParameters[6] = new SqlParameter("@CTYCMPCDN", companyId);
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("CityMst_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("City Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "City");
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
                    string currentURL = "/City/Index";
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
                    getReportDataModel.ControllerName = "City";
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
                    var bytes = Excel(getReportDataModel, "City Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "City.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "City Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "City.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult AddState(string name, string code, int position)
        {
            try
            {

                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(code))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[9];
                    sqlParameters[0] = new SqlParameter("@MscName", name);
                    sqlParameters[1] = new SqlParameter("@MscCode", code);
                    sqlParameters[2] = new SqlParameter("@MscType", "STA");
                    sqlParameters[3] = new SqlParameter("@MscPos", position);
                    sqlParameters[4] = new SqlParameter("@MscActYN", "0");
                    sqlParameters[5] = new SqlParameter("@MscVou", "0");
                    sqlParameters[6] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[7] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[8] = new SqlParameter("@Flg", 1);

                    DataTable DtState = ObjDBConnection.CallStoreProcedure("MscMst_Insert", sqlParameters);
                    if (DtState != null && DtState.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtState.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            return Json(new { result = false, message = "Duplicate Code" });
                        }
                        else if (status == -2)
                        {
                            return Json(new { result = false, message = "Duplicate Name" });
                        }
                        else
                        {
                            var stateList = objProductHelper.GetStateMasterDropdown(companyId, administrator);
                            return Json(new { result = true, message = "Inserted successfully", data = stateList });
                        }
                    }
                    else
                    {
                        return Json(new { result = false, message = "Please Enter the Value" });
                    }
                }
                else
                {
                    return Json(new { result = false, message = "Please Enter the Value" });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public IActionResult GetStateList()
        //{
        //    try
        //    {
        //        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
        //        int administrator = 0;
        //        var stateList = objProductHelper.GetStateMasterDropdown(companyId, administrator);
        //        return Json(new { result = true, data = stateList });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        [Route("/City/GetState-List")]
        public IActionResult StateList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = 0;
            var stateList = objProductHelper.GetStateMasterDropdown(companyId, administrator);

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                stateList = stateList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }
            return Json(new { items = CommonHelpers.BindSelect2Model(stateList) });
        }

    }
}
