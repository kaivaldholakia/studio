using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Controllers
{
    public class DepartmentController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        AccountMasterHelpers ObjaccountMasterHelpers = new AccountMasterHelpers();
        private readonly IWebHostEnvironment hostingEnvironment;

        public DepartmentController(IWebHostEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
        }


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

                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int isAdministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                long userId = GetIntSession("UserId");
                int lang = Convert.ToInt32(GetIntSession("Lang"));
                int langData = Convert.ToInt32(GetIntSession("LangData"));
                DepartmentMasterModel departmentMasterModel = new DepartmentMasterModel();
                departmentMasterModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, isAdministrator);
                departmentMasterModel.BiltyName = ObjTaxMasterHelpers.GetBiltyLayoutDropdown();
                departmentMasterModel.MemoName = ObjTaxMasterHelpers.GetMemoLayoutDropdown();
                departmentMasterModel.InvoiceName = ObjTaxMasterHelpers.GetInvoiceLayoutDropdown();
                departmentMasterModel.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                if (id > 0)
                {
                    departmentMasterModel.DepVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@VOU", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtDepartment = ObjDBConnection.CallStoreProcedure("GetDepartmentDetails", sqlParameters);
                    if (DtDepartment != null && DtDepartment.Rows.Count > 0)
                    {
                        departmentMasterModel.DepCode = (DtDepartment.Rows[0]["DepCode"].ToString()).TrimEnd();
                        departmentMasterModel.DepName = DtDepartment.Rows[0]["DepName"].ToString();
                        departmentMasterModel.DepAdd = DtDepartment.Rows[0]["DepAdd"].ToString();
                        departmentMasterModel.DepCity = DtDepartment.Rows[0]["DepCity"].ToString();
                        departmentMasterModel.DepStateVou = DbConnection.ParseInt32(DtDepartment.Rows[0]["DepStateVou"].ToString());
                        departmentMasterModel.DepPAN = DtDepartment.Rows[0]["DepPAN"].ToString();
                        departmentMasterModel.DepGST = DtDepartment.Rows[0]["DepGST"].ToString();
                        departmentMasterModel.DepPhone2 = DtDepartment.Rows[0]["DepPhone2"].ToString();
                        departmentMasterModel.DepMob = DtDepartment.Rows[0]["DepMobile"].ToString();
                        departmentMasterModel.DepEmail = DtDepartment.Rows[0]["DepEmail"].ToString();
                        departmentMasterModel.DepJurd = DtDepartment.Rows[0]["DepJurisd"].ToString();
                        departmentMasterModel.DepBnkNm = DtDepartment.Rows[0]["DepBankNm"].ToString();
                        departmentMasterModel.DepAcNo = DtDepartment.Rows[0]["DepBnkAcNo"].ToString();
                        departmentMasterModel.DepIFSC = DtDepartment.Rows[0]["DepBnkIFSC"].ToString();
                        departmentMasterModel.DepBnkBrnch = DtDepartment.Rows[0]["DepBnkBrnch"].ToString();
                        departmentMasterModel.DepWhtMob = DtDepartment.Rows[0]["DepWhtMob"].ToString();
                        departmentMasterModel.DepBusLine = DtDepartment.Rows[0]["DepBusLine"].ToString();
                        departmentMasterModel.ProfilePicture = DtDepartment.Rows[0]["DepLogo"].ToString();
                        departmentMasterModel.DepCityVou = Convert.ToInt32(DtDepartment.Rows[0]["DepCtyvou"].ToString());
                        departmentMasterModel.DepState= DtDepartment.Rows[0]["DepState"].ToString();
                    }
                }
                ViewBag.lang = lang;
                return View(departmentMasterModel);
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new DepartmentMasterModel());
        }

        [HttpPost]
        public IActionResult Index(long id, DepartmentMasterModel departmentMaster)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                #region Upload File
                if (departmentMaster.profileLogo != null)
                {
                    var uniqueFileName = GetUniqueFileName(departmentMaster.profileLogo.FileName);
                    var uploads = Path.Combine(hostingEnvironment.WebRootPath, "Uploads");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    departmentMaster.profileLogo.CopyTo(new FileStream(filePath, FileMode.Create));
                    departmentMaster.ProfilePicture = uniqueFileName;
                    //to do : Save uniqueFileName  to your db table   
                }

                #endregion

                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int isAdministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                departmentMaster.StateList = objProductHelper.GetStateMasterDropdown(companyId, isAdministrator);
                departmentMaster.BiltyName = ObjTaxMasterHelpers.GetBiltyLayoutDropdown();
                departmentMaster.MemoName = ObjTaxMasterHelpers.GetMemoLayoutDropdown();
                departmentMaster.InvoiceName = ObjTaxMasterHelpers.GetInvoiceLayoutDropdown();
                if (!string.IsNullOrWhiteSpace(departmentMaster.DepCode) && !string.IsNullOrWhiteSpace(departmentMaster.DepName))
                {
                    long userId = GetIntSession("UserId");
                    SqlParameter[] sqlParameters = new SqlParameter[23];
                    sqlParameters[0] = new SqlParameter("@DepCode", departmentMaster.DepCode);
                    sqlParameters[1] = new SqlParameter("@DepName", departmentMaster.DepName);
                    sqlParameters[2] = new SqlParameter("@DepAdd", departmentMaster.DepAdd);
                    sqlParameters[3] = new SqlParameter("@DepCity", departmentMaster.DepCity);
                    sqlParameters[4] = new SqlParameter("@DepStateVou", departmentMaster.DepStateVou);
                    sqlParameters[5] = new SqlParameter("@DepPAN", departmentMaster.DepPAN);
                    sqlParameters[6] = new SqlParameter("@DepGST", departmentMaster.DepGST);
                    sqlParameters[7] = new SqlParameter("@DepPh2", departmentMaster.DepPhone2);
                    sqlParameters[8] = new SqlParameter("@DepMob", departmentMaster.DepMob);
                    sqlParameters[9] = new SqlParameter("@DepEmail", departmentMaster.DepEmail);
                    sqlParameters[10] = new SqlParameter("@DepJurd", departmentMaster.DepJurd);
                    sqlParameters[11] = new SqlParameter("@DepBnkNm", departmentMaster.DepBnkNm);
                    sqlParameters[12] = new SqlParameter("@DepAcNo", departmentMaster.DepAcNo);
                    sqlParameters[13] = new SqlParameter("@DepIFSC", departmentMaster.DepIFSC);
                    sqlParameters[14] = new SqlParameter("@DepBnkBrnch", departmentMaster.DepBnkBrnch);
                    sqlParameters[15] = new SqlParameter("@DepWhtMob", departmentMaster.DepWhtMob);
                    sqlParameters[16] = new SqlParameter("@DepBusLine", departmentMaster.DepBusLine);
                    sqlParameters[17] = new SqlParameter("@DepVou", id);
                    sqlParameters[18] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[19] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[20] = new SqlParameter("@DepLogo", departmentMaster.ProfilePicture);
                    sqlParameters[21] = new SqlParameter("@DepCtyVou", departmentMaster.DepCityVou);
                    sqlParameters[22] = new SqlParameter("@DepState", departmentMaster.DepState);
                    DataTable DtDepartment = ObjDBConnection.CallStoreProcedure("Department_Insert", sqlParameters);
                    if (DtDepartment != null && DtDepartment.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtDepartment.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Code Details");
                            departmentMaster.StateList = objProductHelper.GetStateMasterDropdown(companyId, isAdministrator);
                            departmentMaster.BiltyName = ObjTaxMasterHelpers.GetBiltyLayoutDropdown();
                            departmentMaster.MemoName = ObjTaxMasterHelpers.GetMemoLayoutDropdown();
                            departmentMaster.InvoiceName = ObjTaxMasterHelpers.GetInvoiceLayoutDropdown();
                            departmentMaster.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                            ViewBag.FocusType = "1";
                            return View(departmentMaster);
                        }
                        else if (status == -2)
                        {
                            SetErrorMessage("Dulplicate Department Details");
                            departmentMaster.StateList = objProductHelper.GetStateMasterDropdown(companyId, isAdministrator);
                            departmentMaster.BiltyName = ObjTaxMasterHelpers.GetBiltyLayoutDropdown();
                            departmentMaster.MemoName = ObjTaxMasterHelpers.GetMemoLayoutDropdown();
                            departmentMaster.InvoiceName = ObjTaxMasterHelpers.GetInvoiceLayoutDropdown();
                            departmentMaster.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                            ViewBag.FocusType = "1";
                            return View(departmentMaster);
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
                            return RedirectToAction("index", "department", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        departmentMaster.StateList = objProductHelper.GetStateMasterDropdown(companyId, isAdministrator);
                        departmentMaster.BiltyName = ObjTaxMasterHelpers.GetBiltyLayoutDropdown();
                        departmentMaster.MemoName = ObjTaxMasterHelpers.GetMemoLayoutDropdown();
                        departmentMaster.InvoiceName = ObjTaxMasterHelpers.GetInvoiceLayoutDropdown();
                        departmentMaster.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                        ViewBag.FocusType = "1";
                        return View(departmentMaster);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    departmentMaster.StateList = objProductHelper.GetStateMasterDropdown(companyId, isAdministrator);
                    departmentMaster.BiltyName = ObjTaxMasterHelpers.GetBiltyLayoutDropdown();
                    departmentMaster.MemoName = ObjTaxMasterHelpers.GetMemoLayoutDropdown();
                    departmentMaster.InvoiceName = ObjTaxMasterHelpers.GetInvoiceLayoutDropdown();
                    departmentMaster.CityList = ObjaccountMasterHelpers.GetCityNameCustomDropdown(companyId);
                    ViewBag.FocusType = "1";
                    return View(departmentMaster);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new DepartmentMasterModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                DepartmentMasterModel departmentMasterModel = new DepartmentMasterModel();
                if (id > 0)
                {
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@VOU", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 1);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtDepartment = ObjDBConnection.CallStoreProcedure("GetDepartmentDetails", sqlParameters);
                    if (DtDepartment != null && DtDepartment.Rows.Count > 0)
                    {
                        SetSuccessMessage("Department deleted successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "department");
        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/Department/Index";
                userFormRights = GetUserRights(userId, currentURL);
                if (userFormRights == null)
                {
                    SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                }
                ViewBag.userRight = userFormRights;
                #endregion

                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int isAdministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                double startRecord = 0;
                if (pageIndex > 0)
                {
                    startRecord = (pageIndex - 1) * pageSize;
                }
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, 0, "Department", isAdministrator);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "Department";
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

        private string GetUniqueFileName(string fileName)
        {
            return Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
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
                    var bytes = Excel(getReportDataModel, "Department Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Department.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Department Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Department.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
