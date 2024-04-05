using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using Soc_Management_Web.Models;
using System;
using System.Data;
using System.Data.SqlClient;


namespace Soc_Management_Web.Controllers
{
    public class DataStorageEntryController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DataStorageEntryController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(long id)
        {
            LocationOrderModel model = new LocationOrderModel();
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
                SqlParameter[] sqlParameters = new SqlParameter[15];
                sqlParameters[0] = new SqlParameter("@Id", id);
                sqlParameters[1] = new SqlParameter("@orderId", id);
                sqlParameters[2] = new SqlParameter("@RecorType", "");
                sqlParameters[3] = new SqlParameter("@AllPhotos", "");
                sqlParameters[4] = new SqlParameter("@Allvieos", "");
                sqlParameters[5] = new SqlParameter("@VideoLocaton", "");
                sqlParameters[6] = new SqlParameter("@PhotosLocation", "");
                sqlParameters[7] = new SqlParameter("@VideoQty","");
                sqlParameters[8] = new SqlParameter("@PhotosQty","");
                sqlParameters[9] = new SqlParameter("@Remarks","");
                sqlParameters[10] = new SqlParameter("@Fileformatedetails","");
                sqlParameters[11] = new SqlParameter("@flag", 2);
                sqlParameters[12] = new SqlParameter("@Date", 1);
                sqlParameters[13] = new SqlParameter("@CustomerId", 1);
                sqlParameters[14] = new SqlParameter("@EventTittle", 1);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_locationdetails", sqlParameters);
                if(DtEmp.Rows.Count>0)
                {
                    model.AllPhotos = Convert.ToBoolean(DtEmp.Rows[0]["AllPhotos"].ToString());
                    model.Allvieos = Convert.ToBoolean(DtEmp.Rows[0]["AllVideos"].ToString());
                    model.CustomerId = Convert.ToInt64(DtEmp.Rows[0]["CustomerId"].ToString());
                    model.Dates = DtEmp.Rows[0]["Dates"].ToString();
                    model.EventTittle = DtEmp.Rows[0]["EventTittle"].ToString();
                    model.Fileformatedetails = DtEmp.Rows[0]["FileFormateSize"].ToString();
                    model.LocationId = Convert.ToInt64(DtEmp.Rows[0]["OrderId"].ToString());
                    model.OrderId = Convert.ToInt64(DtEmp.Rows[0]["OrderId"].ToString());
                    model.PhotosLocation = DtEmp.Rows[0]["PhotosLocation"].ToString();
                    model.PhotosQty = Convert.ToInt64(DtEmp.Rows[0]["PhotosQty"].ToString());
                    model.RecType = DtEmp.Rows[0]["TypeName"].ToString();
                    model.Remarks = DtEmp.Rows[0]["Remarks"].ToString();
                    model.VideoLocaton = DtEmp.Rows[0]["VideoLocation"].ToString();
                    model.VideoQty = Convert.ToInt64(DtEmp.Rows[0]["VideosQty"].ToString());
                }
            }

            model.TypeList = objProductHelper.GetVideoTypeList();
            model.lstCustomer = objProductHelper.GetCustomerListOnly();
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
        public ActionResult Index(long id, LocationOrderModel obj)
        {
            string obj1 = "";
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);

                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                if (!string.IsNullOrWhiteSpace(Convert.ToString(obj.OrderId)))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[15];
                    sqlParameters[0] = new SqlParameter("@Id", obj.OrderId);
                    sqlParameters[1] = new SqlParameter("@orderId", obj.OrderId);
                    sqlParameters[2] = new SqlParameter("@RecorType", obj.RecType);
                    sqlParameters[3] = new SqlParameter("@AllPhotos", obj.AllPhotos);
                    sqlParameters[4] = new SqlParameter("@Allvieos", obj.Allvieos);
                    sqlParameters[5] = new SqlParameter("@VideoLocaton", obj.VideoLocaton);
                    sqlParameters[6] = new SqlParameter("@PhotosLocation", obj.PhotosLocation);
                    sqlParameters[7] = new SqlParameter("@VideoQty", obj.VideoQty);
                    sqlParameters[8] = new SqlParameter("@PhotosQty", obj.PhotosQty);
                    sqlParameters[9] = new SqlParameter("@Remarks", obj.Remarks);
                    sqlParameters[10] = new SqlParameter("@Fileformatedetails", obj.Fileformatedetails);
                    sqlParameters[11] = new SqlParameter("@flag", 4);
                    sqlParameters[12] = new SqlParameter("@Date", obj.Dates);
                    sqlParameters[13] = new SqlParameter("@CustomerId", obj.CustomerId);
                    sqlParameters[14] = new SqlParameter("@EventTittle", obj.EventTittle);
                    DataTable DtCat = ObjDBConnection.CallStoreProcedure("usp_locationdetails", sqlParameters);
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

                            return RedirectToAction("index", "DataStorageEntry", new { id = id });
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
                    string currentURL = "/DataStorageEntry/Index";
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
                    getReportDataModel.ControllerName = "DataStorageEntry";
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

    }
}
