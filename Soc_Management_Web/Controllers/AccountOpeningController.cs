using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using PIOAccount.Classes;
using PIOAccount.Models;
using Newtonsoft.Json;

namespace PIOAccount.Controllers
{
    public class AccountOpeningController : BaseController
    {

        DbConnection ObjDBConnection = new DbConnection();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        AccountGroupHelpers ObjAccountGroupHelpers = new AccountGroupHelpers();

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
                long userId = GetIntSession("UserId");
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                var yearList = DbConnection.GetYearList(companyId, YearId);
                AccountOpeningModel accountOpeningModel = new AccountOpeningModel();
                //accountOpeningModel.OpeningBal = new OpeningBalGridModel();
                accountOpeningModel.DepList = ObjTaxMasterHelpers.GetDepartmentDropdown(companyId);
                accountOpeningModel.OblAccountList = ObjTaxMasterHelpers.GetSalesAccountDropdown(companyId,0);
                accountOpeningModel.CrDrList = ObjAccountGroupHelpers.GetAccountCrDr();
                var yearData = DbConnection.GetYearListByCompanyId(Convert.ToInt32(companyId)).Where(x => x.YearVou == YearId).FirstOrDefault();
                //string[] yearSplit = yearList[0].Text.Split('-');
                string[] yearSplit = yearData.StartDate.Split('-');
                if (yearSplit != null && yearSplit.Length > 0)
                {
                    accountOpeningModel.OblDt = new DateTime(Convert.ToInt32(yearSplit[0]), 4, 01).AddDays(-1).ToString("yyyy-MM-dd");
                }
                accountOpeningModel.OblVNo = DbConnection.ParseInt32(ObjDBConnection.GetLatestVoucherNumber("OblMst", 0, companyId));
                if (id > 0)
                {
                    accountOpeningModel.OblVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[4];
                    sqlParameters[0] = new SqlParameter("@OblVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[3] = new SqlParameter("@Yrvou", YearId);
                    DataTable DtAccOpen = ObjDBConnection.CallStoreProcedure("GetAccountOpeningDetails", sqlParameters);
                    if (DtAccOpen != null && DtAccOpen.Rows.Count > 0)
                    {
                        accountOpeningModel.OblVou = DbConnection.ParseInt32(id);
                        accountOpeningModel.OblDepVou = DbConnection.ParseInt32(DtAccOpen.Rows[0]["OblDepVou"].ToString());
                        accountOpeningModel.OblVNo = DbConnection.ParseInt32(DtAccOpen.Rows[0]["OblVNo"].ToString());
                        accountOpeningModel.OblDt = !string.IsNullOrWhiteSpace(DtAccOpen.Rows[0]["OblDate"].ToString()) ? Convert.ToDateTime(DtAccOpen.Rows[0]["OblDate"].ToString()).ToString("yyyy-MM-dd") : "";
                        accountOpeningModel.OblAmount = Convert.ToDecimal(DtAccOpen.Rows[0]["OblAmount"].ToString());

                        List<TransactionGridAddUpdateRootModel> lstobl = new List<TransactionGridAddUpdateRootModel>();
                        List<TransactionGridAddUpdateDataModel> dataLIstAnotherModel = new List<TransactionGridAddUpdateDataModel>();

                        for (int i = 0; i < DtAccOpen.Rows.Count; i++)
                        {
                            List<TransactionGridAddUpdateModel> dataList = new List<TransactionGridAddUpdateModel>();

                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OblAAccVou",
                                Value = Convert.ToString(DtAccOpen.Rows[i]["OblAAccVou"].ToString())
                            });

                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OblAAmt",
                                Value = Convert.ToString(DtAccOpen.Rows[i]["OblAAmount"].ToString())
                            });

                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OblARemark",
                                Value = Convert.ToString(DtAccOpen.Rows[i]["OblARemark"].ToString())
                            });

                            dataList.Add(new TransactionGridAddUpdateModel
                            {
                                Label = "OblACrDr",
                                Value = Convert.ToString(DtAccOpen.Rows[i]["OblACrDr"].ToString())
                            });

                            dataLIstAnotherModel.Add(new TransactionGridAddUpdateDataModel
                            {
                                Data = dataList
                            });
                        }
                        lstobl.Add(new TransactionGridAddUpdateRootModel
                        {
                            MyArray = dataLIstAnotherModel
                        });

                        var jsonString = JsonConvert.SerializeObject(lstobl);
                        accountOpeningModel.Data = jsonString;
                    }
                }
                return View(accountOpeningModel);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //Transaction dynamic grid
                string currentURL = GetCurrentURL();
                ViewBag.LayoutListNew = TransactionGridHelper.GetLayoutList(currentURL);
            }
            return View();

        }

        [HttpPost]
        public IActionResult Index(long id, AccountOpeningModel accountOpeningModel)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                List<TransactionGridAddUpdateRootModel> transactionList = JsonConvert.DeserializeObject<List<TransactionGridAddUpdateRootModel>>(accountOpeningModel.Data);

                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));

                long userId = GetIntSession("UserId");
                //accountOpeningModel.OpeningBal = new OpeningBalGridModel();
                accountOpeningModel.OblAccountList = ObjTaxMasterHelpers.GetSalesAccountDropdown(companyId,0);
                accountOpeningModel.DepList = ObjTaxMasterHelpers.GetDepartmentDropdown(companyId);
                accountOpeningModel.CrDrList = ObjAccountGroupHelpers.GetAccountCrDr();
                if (!string.IsNullOrWhiteSpace(accountOpeningModel.OblDt) && !string.IsNullOrWhiteSpace(Convert.ToInt32(accountOpeningModel.OblDepVou).ToString()) && transactionList.Count > 0) 
                {
                    SqlParameter[] sqlParameters = new SqlParameter[8];
                    sqlParameters[0] = new SqlParameter("@VoucherNumber", accountOpeningModel.OblVNo);
                    sqlParameters[1] = new SqlParameter("@Date", accountOpeningModel.OblDt);
                    sqlParameters[2] = new SqlParameter("@AmountTotal", accountOpeningModel.OblAmount);
                    sqlParameters[3] = new SqlParameter("@Vou", id);
                    sqlParameters[4] = new SqlParameter("@DepVou", accountOpeningModel.OblDepVou);
                    sqlParameters[5] = new SqlParameter("@DateNumber", ObjDBConnection.GetNumericDate(accountOpeningModel.OblDt));
                    sqlParameters[6] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[7] = new SqlParameter("@Yrvou", YearId);
                    DataTable DtAccOpening = ObjDBConnection.CallStoreProcedure("AccountOpeningMst_Insert", sqlParameters);
                    if (DtAccOpening != null && DtAccOpening.Rows.Count > 0)
                    {
                        int masterId = DbConnection.ParseInt32(DtAccOpening.Rows[0][0].ToString());
                        if (masterId > 0)
                        {
                            for (int i = 0; i < transactionList[0].MyArray.Count; i++)
                            {

                                SqlParameter[] parameter = new SqlParameter[8];
                                parameter[0] = new SqlParameter("@AccountOpnId", masterId);
                                parameter[1] = new SqlParameter("@PartyId", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OblAAccVou"));
                                parameter[2] = new SqlParameter("@Amount", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OblAAmt"));
                                parameter[3] = new SqlParameter("@DrCr", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OblACrDr"));
                                parameter[4] = new SqlParameter("@Remarks", TransactionGridHelper.GetValue(transactionList[0].MyArray[i], "OblARemks"));
                                parameter[5] = new SqlParameter("@SrNo", (i + 1));
                                parameter[6] = new SqlParameter("@CmpVou", companyId);
                                parameter[7] = new SqlParameter("@Yrvou", YearId);
                                DtAccOpening = ObjDBConnection.CallStoreProcedure("AccountOpeningTran_Insert", parameter);
                            }
                            int status = DbConnection.ParseInt32(DtAccOpening.Rows[0][0].ToString());
                            if (status == 0)
                            {
                                accountOpeningModel.OblDt = Convert.ToDateTime(accountOpeningModel.OblDt).ToString("yyyy-MM-dd");
                                SetErrorMessage("Dulplicate Vou.No Details");
                                ViewBag.FocusType = "1";
                                return View(accountOpeningModel);
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
                                return RedirectToAction("index", "AccountOpening", new { id = 0 });
                            }
                        }
                        else
                        {
                            accountOpeningModel.OblDt = Convert.ToDateTime(accountOpeningModel.OblDt).ToString("yyyy-MM-dd");
                            SetErrorMessage("Insert error");
                            ViewBag.FocusType = "1";
                            return View(accountOpeningModel);
                        }
                    }
                    else
                    {
                        accountOpeningModel.OblDt = Convert.ToDateTime(accountOpeningModel.OblDt).ToString("yyyy-MM-dd");
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "1";
                        return View(accountOpeningModel);
                    }
                }
                else
                {
                    accountOpeningModel.OblDt = Convert.ToDateTime(accountOpeningModel.OblDt).ToString("yyyy-MM-dd");
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "1";
                    return View(accountOpeningModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new AccountOpeningModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                AccountOpeningModel accountOpeningModel = new AccountOpeningModel();
                if (id > 0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@OblVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 1);
                    DataTable DtAccountOpening = ObjDBConnection.CallStoreProcedure("GetAccountOpeningDetails", sqlParameters);
                    if (DtAccountOpening != null && DtAccountOpening.Rows.Count > 0)
                    {
                        SetSuccessMessage("Opening Entry Deleted Sucessfully");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "AccountOpening");
        }


        public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
        {
            GetReportDataModel getReportDataModel = new GetReportDataModel();
            try
            {
                #region User Rights
                long userId = GetIntSession("UserId");
                UserFormRightModel userFormRights = new UserFormRightModel();
                string currentURL = "/AccountOpening/Index";
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
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@Vou", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                DataTable DtDepartment = ObjDBConnection.CallStoreProcedure("GetDepartmentCheck", sqlParameters);
                if (DtDepartment != null && DtDepartment.Rows.Count > 0)
                {
                    int status = DbConnection.ParseInt32(DtDepartment.Rows[0][0].ToString());
                    if (status == 0)
                    {
                        getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId, 0, YearId);
                        if (getReportDataModel.IsError)
                        {
                            ViewBag.Query = getReportDataModel.Query;
                            return PartialView("_reportView");
                        }
                        getReportDataModel.pageIndex = pageIndex;
                        getReportDataModel.ControllerName = "AccountOpening";
                    }
                    else
                    {
                        SetErrorMessage("First Add Department");
                        ViewBag.FocusType = "1";
                        return View();
                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        public IActionResult GetLatestVouNumber(string vou)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(vou))
                {
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    string value = ObjDBConnection.GetLatestVoucherNumber("OblMst", DbConnection.ParseInt32(vou), companyId);
                    return Json(new { response = true, message = value });
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Json(new { response = true, message = string.Empty });
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
                int YearId = Convert.ToInt32(GetIntSession("YearId"));
                var companyDetails = DbConnection.GetCompanyDetailsById(companyId);
                getReportDataModel = getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, YearId, "", 0, 1);
                if(type==1)
                {
                    var bytes = Excel(getReportDataModel, "Account Opening Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "AccountOpening.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Account Opening Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/pdf",
                          "AccountOpening.pdf");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        [Route("/AccountOpening/GetAccount-List")]
        public IActionResult AccountList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var accountList = new List<CustomDropDown>();
            accountList.Add(CommonHelpers.GetDefaultValue());
            accountList.AddRange(ObjTaxMasterHelpers.GetSalesAccountDropdown(companyId, 0));

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                //accountList = accountList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
                accountList = accountList.Where(x => x.Text.ToLower().Contains(q.ToLower())).ToList();
            }

            return Json(new { items = CommonHelpers.BindSelect2Model(accountList) });
        }

    }
}
