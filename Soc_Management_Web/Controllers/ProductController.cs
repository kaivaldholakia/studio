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
    public class ProductController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
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
                ProductModel ProductModel = new ProductModel();
                if (id > 0)
                {
                    ProductModel.PrdVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@PrdVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetProductDetails", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        ProductModel.PrdName = DtEmp.Rows[0]["PrdName"].ToString();
                        ProductModel.PrdCode = DtEmp.Rows[0]["PrdCode"].ToString();
                        ProductModel.PrdGSTRt = Convert.ToDecimal(DtEmp.Rows[0]["PrdGSTRt"].ToString());
                        ProductModel.PrdSalRt = Convert.ToDecimal(DtEmp.Rows[0]["PrdSalRt"].ToString());
                        ProductModel.PrdPurRt = Convert.ToDecimal(DtEmp.Rows[0]["PrdPurRt"].ToString());
                    }
                }

                return View(ProductModel);
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
        public IActionResult Index(long id, ProductModel ProductModel)
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
                if (!string.IsNullOrWhiteSpace(ProductModel.PrdName) && !string.IsNullOrWhiteSpace(ProductModel.PrdCode))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("@Vou", id);
                    sqlParameters[1] = new SqlParameter("@PrdName", ProductModel.PrdName);
                    sqlParameters[2] = new SqlParameter("@PrdCode", ProductModel.PrdCode);
                    sqlParameters[3] = new SqlParameter("@PrdGSTRt", ProductModel.PrdGSTRt);
                    sqlParameters[4] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[5] = new SqlParameter("@UserId", userId);
                    sqlParameters[6] = new SqlParameter("@SalRt", ProductModel.PrdSalRt);
                    sqlParameters[7] = new SqlParameter("@PurRt", ProductModel.PrdPurRt);
                    DataTable DtProduct = ObjDBConnection.CallStoreProcedure("ProductMst_Insert", sqlParameters);
                    if (DtProduct != null && DtProduct.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtProduct.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Product Name Details");
                            ViewBag.FocusType = "-1";
                        }
                        else if (status == -2)
                        {
                            SetErrorMessage("Dulplicate Product Code Details");
                            ViewBag.FocusType = "-2";
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
                            return RedirectToAction("index", "Product", new { id = 0 });
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new ProductModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                ProductModel ProductModel = new ProductModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@PrdVou", id);
                    sqlParameters[1] = new SqlParameter("@FLG", "1");
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtProduct = ObjDBConnection.CallStoreProcedure("GetProductDetails", sqlParameters);
                    if (DtProduct != null && DtProduct.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtProduct.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Product Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "Product");
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
                    string currentURL = "/Product/Index";
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
                    getReportDataModel.ControllerName = "Product";
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
                    var bytes = Excel(getReportDataModel, "Product Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Product.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Product Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Product.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
