using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ClientController : BaseController
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

                long companyId = GetIntSession("CompanyId");
                long userId = GetIntSession("UserId");
                ClientMasterModel clientMasterModel = new ClientMasterModel();
                clientMasterModel.ClientTypeList = ClientType();
                if (id > 0)
                {
                    clientMasterModel.ClientVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@ClientVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 3);

                    DataTable dtClient = ObjDBConnection.CallStoreProcedure("ClientMasterInsert", sqlParameters);
                    if (dtClient != null && dtClient.Rows.Count > 0)
                    {
                        clientMasterModel.ClientEmail = dtClient.Rows[0]["CliEmail"].ToString();
                        clientMasterModel.ClientMobile = dtClient.Rows[0]["CliMobile"].ToString();
                        clientMasterModel.ClientName = dtClient.Rows[0]["CliName"].ToString();
                        clientMasterModel.ClientType = Convert.ToInt32(dtClient.Rows[0]["CliTYpe"].ToString());
                        clientMasterModel.ClientIsActive = Convert.ToInt32(dtClient.Rows[0]["CliActive"].ToString());
                        clientMasterModel.ClientVou = DbConnection.ParseInt32(dtClient.Rows[0]["CliVou"].ToString());
                    }
                }
                return View(clientMasterModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        public IActionResult Index(long id, ClientMasterModel clientMasterModel)
        {
            try
            {
                clientMasterModel.ClientTypeList = ClientType();
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                long companyId = GetIntSession("CompanyId");
                long userId = GetIntSession("UserId");
                SqlParameter[] sqlParameters = new SqlParameter[7];
                sqlParameters[0] = new SqlParameter("@ClientName", clientMasterModel.ClientName);
                sqlParameters[1] = new SqlParameter("@ClientEmail", clientMasterModel.ClientEmail);
                sqlParameters[2] = new SqlParameter("@ClientMobile", clientMasterModel.ClientMobile);
                sqlParameters[3] = new SqlParameter("@ClientIsActive", clientMasterModel.ClientIsActive);
                sqlParameters[4] = new SqlParameter("@ClientType", clientMasterModel.ClientType);
                sqlParameters[5] = new SqlParameter("@ClientVou", id);
                sqlParameters[6] = new SqlParameter("@Flg", 1);
                DataTable dtClient = ObjDBConnection.CallStoreProcedure("ClientMasterInsert", sqlParameters);
                if (dtClient != null && dtClient.Rows.Count > 0)
                {
                    int Status = DbConnection.ParseInt32(dtClient.Rows[0][0].ToString());
                    if (Status == -1)
                    {
                        SetErrorMessage("Dulplicate email address");
                        ViewBag.FocusType = "1";
                        return View(clientMasterModel);
                    }
                    if (Status == -2)
                    {
                        SetErrorMessage("Dulplicate client name");
                        ViewBag.FocusType = "1";
                        return View(clientMasterModel);
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
                        return RedirectToAction("index", "Client", new { id = 0 });
                    }
                }
                else
                {
                    SetErrorMessage("Internal error");
                    ViewBag.FocusType = "1";
                    return View(clientMasterModel);
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
                if (id > 0)
                {
                    long companyId = GetIntSession("CompanyId");
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@ClientVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    DataTable dtClient = ObjDBConnection.CallStoreProcedure("ClientMasterInsert", sqlParameters);
                    if (dtClient != null && dtClient.Rows.Count > 0)
                    {
                        int value = DbConnection.ParseInt32(dtClient.Rows[0][0].ToString());
                        if (value > 0)
                        {
                            SetSuccessMessage("Client Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "Client");
        }

        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            string query = string.Empty;
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/Client/Index";
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
                getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, clientId, 0, "Client", isadministrator);
                if (getReportDataModel.IsError)
                {
                    ViewBag.Query = getReportDataModel.Query;
                    return PartialView("_reportView");
                }
                getReportDataModel.pageIndex = pageIndex;
                getReportDataModel.ControllerName = "Client";
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


        private List<SelectListItem> ClientType()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = "Demo",
                Value = "1"
            });

            list.Add(new SelectListItem
            {
                Text = "1 Year",
                Value = "2"
            });
            return list;
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
                    var bytes = Excel(getReportDataModel, "Client Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "Client.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Client Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "Client.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
