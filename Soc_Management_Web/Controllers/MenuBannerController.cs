
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using Soc_Management_Web.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Soc_Management_Web.Controllers
{
    public class MenuBannerController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        private readonly IWebHostEnvironment _hostingEnvironment;

        public MenuBannerController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(long id)
        {
            MenuBanner model = new MenuBanner();
            bool isreturn = false;
            INIT(ref isreturn);
            if (isreturn)
            {
                return RedirectToAction("index", "dashboard");
            }
            long userId = GetIntSession("UserId");
            long societyid = id;
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            int administrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
            int clientId = 0;
            if (id > 0)
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@id", id);
                sqlParameters[1] = new SqlParameter("@MenuId", "");
               sqlParameters[4] = new SqlParameter("@FileNames", "");
                sqlParameters[5] = new SqlParameter("@FLG", "3");
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_MenuBanner_Insert", sqlParameters);
                if (DtEmp.Rows.Count > 0)
                {
                    model.Id = id;
                   
                    //model.filename = DtEmp.Rows[0]["MenuBanner"].ToString();
                }
            }
            return View(model);
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
        public async System.Threading.Tasks.Task<ActionResult> IndexAsync(long id, Webcategory obj)
        {
            string obj1 = "";
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (obj.file != null && obj.file.Length > 0)
                {

                    var uploadsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "MenuBanner");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + obj.file.FileName;
                    string filePath = Path.Combine(uploadsFolderPath, uniqueFileName);
                    obj.filename = uniqueFileName;
                    // Save the uploaded file with a unique filename
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await obj.file.CopyToAsync(stream);
                    }

                    // Set obj.path to the full path of the saved file
                    obj.path = filePath;



                }
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                if (!string.IsNullOrWhiteSpace(Convert.ToString(obj.Catname)))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@id", obj.Id);
                    sqlParameters[0] = new SqlParameter("@MenuId", obj.Id);
                    sqlParameters[4] = new SqlParameter("@FileNames", obj.filename);
                    sqlParameters[5] = new SqlParameter("@FLG", "1");
                    DataTable DtCat = ObjDBConnection.CallStoreProcedure("usp_MenuBanner_Insert", sqlParameters);
                    if (DtCat != null && DtCat.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtCat.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            id = status;
                            obj1 = "Dulplicate Category Details";
                            return RedirectToAction("index", "DataStorageEntry", new { id = 0 });
                        }
                        else
                        {


                            if (id > 0)
                            {
                                obj1 = "Update Sucessfully";
                                SetSuccessMessage("Update Sucessfully");
                            }
                            else
                            {
                                id = status;
                                obj1 = "Inserted Sucessfully";
                                SetSuccessMessage("Inserted Sucessfully");

                            }

                            //return RedirectToAction("index", "WebCategory", new { id = id });
                        }
                    }
                    else
                    {
                        obj1 = "Inserted Sucessfully";
                    }
                }
                else
                {
                    obj1 = "Inserted Sucessfully";
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return Json(new { obj1 });
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
                    string currentURL = "/Webcategory/Index";
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
                    getReportDataModel.ControllerName = "Webcategory";
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

        public IActionResult Delete(long id)
        {
            try
            {
                CategoryModel catModel = new CategoryModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@id", id);
                    sqlParameters[1] = new SqlParameter("@CatNm", "");
                    sqlParameters[2] = new SqlParameter("@decsription", "");
                    sqlParameters[3] = new SqlParameter("@Filepath", "");
                    sqlParameters[4] = new SqlParameter("@FileNames", "");
                    sqlParameters[5] = new SqlParameter("@FLG", "2");
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("usp_webCatMaster_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Category Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "WebCategory");
        }

    }
}
