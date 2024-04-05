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
    public class MenuController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();

        public object IconTxt { get; private set; }

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
                ModuleMasterModal menuModel = new ModuleMasterModal();
                menuModel.Icon = "fas fa-list-alt";
                if (id > 0)
                {
                    menuModel.ModuleId = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@Vou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 1);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetMenuMasterDetails", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        menuModel.Name = DtEmp.Rows[0]["Name"].ToString();
                        menuModel.NameOth = DtEmp.Rows[0]["NameOther"].ToString();
                        menuModel.Link = DtEmp.Rows[0]["Link"].ToString();
                        menuModel.Icon = DtEmp.Rows[0]["Icon"].ToString();
                        menuModel.ParentFK = Convert.ToInt32(DtEmp.Rows[0]["ParentFK"].ToString());
                        menuModel.Position = Convert.ToInt32(DtEmp.Rows[0]["Position"].ToString());
                        menuModel.Deshboardpos = Convert.ToInt32(DtEmp.Rows[0]["DashboardPosition"].ToString());
                    }
                }
                menuModel.MenuList = objProductHelper.GetMasterMenuDropdown(companyId);

                return View(menuModel);
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
        public IActionResult Index(long id, ModuleMasterModal menuModel)
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
                if (!string.IsNullOrWhiteSpace(menuModel.Name) && !string.IsNullOrWhiteSpace(menuModel.Link) && !string.IsNullOrWhiteSpace(DbConnection.ParseInt32(menuModel.ParentFK).ToString()))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[10];
                    sqlParameters[0] = new SqlParameter("@Name", menuModel.Name);
                    sqlParameters[1] = new SqlParameter("@Link", menuModel.Link);
                    sqlParameters[2] = new SqlParameter("@Icon", menuModel.Icon);
                    sqlParameters[3] = new SqlParameter("@ParentFK", menuModel.ParentFK);
                    sqlParameters[4] = new SqlParameter("@IsMaster", menuModel.IsMaster);
                    sqlParameters[5] = new SqlParameter("@Position", menuModel.Position);
                    sqlParameters[6] = new SqlParameter("@DashboardPosition", menuModel.Deshboardpos);
                    sqlParameters[7] = new SqlParameter("@ModuleId", id);
                    sqlParameters[8] = new SqlParameter("@NameOth", menuModel.NameOth);
                    sqlParameters[9] = new SqlParameter("@FLG", "1");

                    DataTable DtMenu = ObjDBConnection.CallStoreProcedure("MenuMaster_Insert", sqlParameters);
                    if (DtMenu != null && DtMenu.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtMenu.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Menu Name Details");
                            ViewBag.FocusType = "-1";
                            menuModel.MenuList = objProductHelper.GetMasterMenuDropdown(companyId);
                        }
                        else if (status == -2)
                        {
                            SetErrorMessage("Dulplicate Menu Link Details");
                            ViewBag.FocusType = "-2";
                            //menuModel.MenuList = objProductHelper.GetMasterMenuDropdown(companyId);
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
                            return RedirectToAction("index", "Menu", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        menuModel.MenuList = objProductHelper.GetMasterMenuDropdown(companyId);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    menuModel.MenuList = objProductHelper.GetMasterMenuDropdown(companyId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new ModuleMasterModal());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                ModuleMasterModal menuModel = new ModuleMasterModal();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[9];
                    sqlParameters[0] = new SqlParameter("@Name", "");
                    sqlParameters[1] = new SqlParameter("@Link", "");
                    sqlParameters[2] = new SqlParameter("@Icon", "");
                    sqlParameters[3] = new SqlParameter("@ParentFK", 0);
                    sqlParameters[4] = new SqlParameter("@IsMaster", 0);
                    sqlParameters[5] = new SqlParameter("@Position", 0);
                    sqlParameters[6] = new SqlParameter("@DashboardPosition", 0);
                    sqlParameters[7] = new SqlParameter("@ModuleId", id);
                    sqlParameters[8] = new SqlParameter("@FLG", "2");
                    DataTable DtMenu = ObjDBConnection.CallStoreProcedure("MenuMaster_Insert", sqlParameters);
                    if (DtMenu != null && DtMenu.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtMenu.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Menu Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "Menu");
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
                    string currentURL = "/Menu/Index";
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
                    getReportDataModel.ControllerName = "Menu";
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
                    var bytes = Excel(getReportDataModel, "Menu Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Menu.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Menu Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Menu.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
