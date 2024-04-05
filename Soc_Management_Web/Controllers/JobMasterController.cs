using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using Soc_Management_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Soc_Management_Web.Controllers
{
    public class JobMaster : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        public IActionResult Index(long id)
        {
            try
            {
                if (TempData["reload"] != null)
                {
                    id = Convert.ToInt32(TempData["reload"]);
                    TempData["reload"] = null;
                }
                bool isreturn = false;
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                JobModel jobmdel = new JobModel();
                if (id > 0)
                {
                    //cityModel.CategoryId = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@JobVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 4);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Usp_JobMst_Main_Insert", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        jobmdel.jobid = Convert.ToInt32(DtEmp.Rows[0]["JobVou"].ToString());
                        jobmdel.jobworkname = DtEmp.Rows[0]["JobNm"].ToString();
                        jobmdel.shortname = DtEmp.Rows[0]["JobCd"].ToString();
                        jobmdel.remarks = DtEmp.Rows[0]["JobRem"].ToString();
                        jobmdel.rate = Convert.ToDecimal(DtEmp.Rows[0]["JobRt"].ToString());
                        jobmdel.NoEntVnDate = Convert.ToBoolean(DtEmp.Rows[0]["JobEventYN"].ToString() == "" ? 0 : Convert.ToInt32(DtEmp.Rows[0]["JobEventYN"].ToString()));
                    }
                    SqlParameter[] PrsqlParameters = new SqlParameter[2];
                    PrsqlParameters[0] = new SqlParameter("@JobVou", id);
                    PrsqlParameters[1] = new SqlParameter("@Flg", 5);
                    DataTable PrDtEmp = ObjDBConnection.CallStoreProcedure("Usp_JobMst_Main_Insert", PrsqlParameters);
                    if (PrDtEmp != null && PrDtEmp.Rows.Count > 0)
                    {
                        List<ProductViewModel> prLst = new List<ProductViewModel>();
                        for (var i=0; i < PrDtEmp.Rows.Count; i++)
                        {
                            ProductViewModel obj = new ProductViewModel();
                            obj.SNo = Convert.ToInt32(PrDtEmp.Rows[i]["JprSrNo"].ToString());
                            obj.PrVou = Convert.ToInt32(PrDtEmp.Rows[i]["JprPrdVou"].ToString());
                            obj.PrNm = PrDtEmp.Rows[i]["ProdName"].ToString();
                            obj.Qty = Convert.ToInt32(PrDtEmp.Rows[i]["JprQty"].ToString());
                            obj.Rmk = PrDtEmp.Rows[i]["JprRem"].ToString();
                            obj.editSno = i;
                            prLst.Add(obj);
                        }
                        jobmdel.lstProduct1 = prLst;
                    }
                    SqlParameter[] MnsqlParameters = new SqlParameter[2];
                    MnsqlParameters[0] = new SqlParameter("@JobVou", id);
                    MnsqlParameters[1] = new SqlParameter("@Flg", 6);
                    DataTable MnDtEmp = ObjDBConnection.CallStoreProcedure("Usp_JobMst_Main_Insert", MnsqlParameters);
                    if (MnDtEmp != null && MnDtEmp.Rows.Count > 0)
                    {
                        List<MainPowerViewModel> mnLst = new List<MainPowerViewModel>();
                        for (var i = 0; i < MnDtEmp.Rows.Count; i++)
                        {
                            MainPowerViewModel obj = new MainPowerViewModel();
                            obj.SNo = Convert.ToInt32(MnDtEmp.Rows[i]["JmnSrNo"].ToString());
                            obj.CatVou = Convert.ToInt32(MnDtEmp.Rows[i]["JmnCatVou"].ToString());
                            obj.CatNm = MnDtEmp.Rows[i]["CatNm"].ToString();
                            obj.Qty = Convert.ToInt32(MnDtEmp.Rows[i]["JmnQty"].ToString());
                            obj.Rmk = MnDtEmp.Rows[i]["JmnRem"].ToString();
                            obj.editSno = i;
                            mnLst.Add(obj);
                        }
                        jobmdel.lstMainPower = mnLst;
                    }
                }
                jobmdel.lstcategory = objProductHelper.GetCategoryDropdown().OrderBy(x => x.Text).ToList();
                jobmdel.lstproduct = objProductHelper.GetlstProdDropdown().OrderBy(x => x.Text).ToList();
                ViewBag.FocusType = "0";

                return View(jobmdel);
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
       //[HttpPost]
       // public JsonResult PassThings()
       // {
       //     var result = HttpContext.Request.Form["things"];

       //     var requestParameters = new Dictionary<string, object>();
       //     foreach (var formVariable in HttpContext.Request.Form)
       //     {
       //         requestParameters.Add(formVariable.Key, formVariable.Value.ToString());
       //     }
       //     var json = JsonConvert.SerializeObject(requestParameters);
       //     var myModel = JsonConvert.DeserializeObject(json, typeof(Thing));


       //     return Json("");
       // }

        public class Thing
        {
            public int Id { get; set; }
            public string Color { get; set; }
        }
        [HttpPost]
        public ActionResult Index(JobModel _customer)//long id,
        {
            try
            {
                
                //long id = 111111;
                bool isreturn = false;
                INIT(ref isreturn);
                //long jobid = id;
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                if (!string.IsNullOrWhiteSpace(_customer.jobworkname))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@JobVou", _customer.jobid);
                    sqlParameters[1] = new SqlParameter("@JobNm", _customer.jobworkname);
                    sqlParameters[2] = new SqlParameter("@JobCd", _customer.shortname);
                    sqlParameters[3] = new SqlParameter("@JobRem", _customer.remarks);
                    sqlParameters[4] = new SqlParameter("@JobRt", _customer.rate);
                    sqlParameters[5] = new SqlParameter("@JobEventYN", _customer.NoEntVnDate == true ? 1:0);
                    sqlParameters[6] = new SqlParameter("@FLG", "1");

                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("Usp_JobMst_Main_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (status == -1)
                        {
                           // TempData["reload"] = 0;
                            SetErrorMessage("Duplicate Job Details");
                            ViewBag.FocusType = "-1";
                            return Json(new { msg = "failed" });
                            //cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(JobModel);
                        }
                        else
                        {
                            var jobVou = 0;
                            if (_customer.jobid == 0)
                                jobVou = DbConnection.ParseInt32(DtCity.Rows[0][1].ToString());
                            else
                                jobVou = _customer.jobid;
                            //var SNo = 1;
                            if (_customer.lstProduct1 != null)
                            {
                                List<ProductViewModel> ItemList = _customer.lstProduct1.OrderBy(x => x.SNo).ToList();
                                var srlng = ItemList.Count();
                                var SNo = srlng;
                                foreach (var item in ItemList)
                                {
                                    if (item.SNo != 0)
                                        SNo = item.SNo;
                                    
                                    SqlParameter[] PrsqlParameters = new SqlParameter[6];
                                    PrsqlParameters[0] = new SqlParameter("@JprJobVou", jobVou);
                                    PrsqlParameters[1] = new SqlParameter("@JprSrNo", SNo);
                                    PrsqlParameters[2] = new SqlParameter("@JprPrdVou", item.PrVou);
                                    PrsqlParameters[3] = new SqlParameter("@JprQty", item.Qty);
                                    PrsqlParameters[4] = new SqlParameter("@JprRem", item.Rmk);
                                    PrsqlParameters[5] = new SqlParameter("@FLG", "1");
                                    DataTable PrJb = ObjDBConnection.CallStoreProcedure("Usp_JobMst_PrdDtl_Insert", PrsqlParameters);
                                    SNo--;
                                }
                                SNo = 1;
                            }
                            if (_customer.lstMainPower != null)
                            {
                                List<MainPowerViewModel> ItemList1 = _customer.lstMainPower.OrderBy(x => x.SNo).ToList();
                                var Snlng = ItemList1.Count();
                                var SNo = Snlng;
                                foreach (var item in ItemList1)
                                {
                                    if (item.SNo != 0)
                                        SNo = item.SNo;
                                        
                                    SqlParameter[] MainsqlParameters = new SqlParameter[6];
                                    MainsqlParameters[0] = new SqlParameter("@JmnJobVou", jobVou);
                                    MainsqlParameters[1] = new SqlParameter("@JmnSrNo", SNo);
                                    MainsqlParameters[2] = new SqlParameter("@JmnCatVou", item.CatVou);
                                    MainsqlParameters[3] = new SqlParameter("@JmnQty", item.Qty);
                                    MainsqlParameters[4] = new SqlParameter("@JmnRem", item.Rmk);
                                    MainsqlParameters[5] = new SqlParameter("@FLG", "1");
                                    DataTable MainJb = ObjDBConnection.CallStoreProcedure("Usp_JmnMst_ManPower_Insert", MainsqlParameters);
                                    SNo--;
                                }
                                SNo = 1;
                            }
                            if (_customer.jobid > 0)
                            {
                                TempData["reload"] = 0;
                                SetSuccessMessage("Update Sucessfully");
                            }
                            else
                            {
                                SetSuccessMessage("Inserted Sucessfully");
                            }
                            //return RedirectToAction("index", "JOB", new { id = 0 });
                            return Json(new { msg = "Success"});
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        //cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(cityModel);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    //cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(cityModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            //return View(new JobModel());
            return Json(_customer);
        }
        public ActionResult IndexProd(List<JobModel> customers)//long id,
        {
            try
            {
                long id = 1;
                bool isreturn = false;
                INIT(ref isreturn);
                long jobid = id;
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                foreach (var varProduct in customers)
                {
                    if (!string.IsNullOrWhiteSpace(varProduct.jobworkname))
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[6];
                        sqlParameters[0] = new SqlParameter("@JprJobVou", 0);
                        sqlParameters[1] = new SqlParameter("@JprSrNo", varProduct.pno);
                        sqlParameters[2] = new SqlParameter("@JprPrdVou", varProduct.product);
                        sqlParameters[3] = new SqlParameter("@JprQty", varProduct.qty);
                        sqlParameters[4] = new SqlParameter("@JprRem", varProduct.premarks1);
                        sqlParameters[5] = new SqlParameter("@FLG", "1");

                        DataTable Dtprd = ObjDBConnection.CallStoreProcedure("Usp_JobMst_PrdDtl_Insert", sqlParameters);
                        if (Dtprd != null && Dtprd.Rows.Count > 0)
                        {
                            int status = DbConnection.ParseInt32(Dtprd.Rows[0][0].ToString());
                            if (status == -1)
                            {
                                SetErrorMessage("Duplicate Product Details");
                                ViewBag.FocusType = "-1";

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
                                return RedirectToAction("index", "JOB", new { id = 0 });
                            }
                        }
                        else
                        {
                            SetErrorMessage("Please Enter the Value");
                            ViewBag.FocusType = "-1";
                            //cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(cityModel);
                        }
                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        //cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(cityModel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            //return View(new JobModel());
            return Json(customers);
        }

        public IActionResult Indexmanpower(List<JobModel> _manpower)//long id,(long id, JobModel _manpower)
        {
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);
                long id = 12;
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                foreach (var varmanpower in _manpower)
                {
                    if (!string.IsNullOrWhiteSpace(varmanpower.jobworkname))
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[6];
                        sqlParameters[0] = new SqlParameter("@JmnJobVou", 1);
                        sqlParameters[1] = new SqlParameter("@JmnSrNo", varmanpower.mno);
                        sqlParameters[2] = new SqlParameter("@JmnCatVou", varmanpower.category);
                        sqlParameters[3] = new SqlParameter("@JmnQty", varmanpower.qty1);
                        sqlParameters[4] = new SqlParameter("@JmnRem", varmanpower.mremarks2);
                        sqlParameters[5] = new SqlParameter("@FLG", "1");

                        DataTable DtCity = ObjDBConnection.CallStoreProcedure("Usp_JmnMst_ManPower_Insert", sqlParameters);
                        if (DtCity != null && DtCity.Rows.Count > 0)
                        {
                            int status = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                            if (status == -1)
                            {
                                SetErrorMessage("Duplicate Job Details");
                                ViewBag.FocusType = "-1";
                                //cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(JobModel);
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
                                return RedirectToAction("index", "JOB", new { id = 0 });
                            }
                        }
                        else
                        {
                            SetErrorMessage("Please Enter the Value");
                            ViewBag.FocusType = "-1";
                            //cityModel.StateList = objProductHelper.GetStateMasterDropdown(companyId, administrator); return View(cityModel);
                        }
                    }

                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            //return View(new JobModel());
            return Json(_manpower);
        }
        public IActionResult Delete(long id)
        {
            try
            {
                JobModel jobmodel = new JobModel();
                if (id > 0)
                {
                    using (TransactionScope transactionScope = new TransactionScope())
                    {
                        long userId = GetIntSession("UserId");
                        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                        SqlParameter[] sqlParameters = new SqlParameter[2];
                        sqlParameters[0] = new SqlParameter("@JobVou", id);
                        sqlParameters[1] = new SqlParameter("@FLG", "2");

                        DataTable DtCity = ObjDBConnection.CallStoreProcedure("Usp_JobMst_Main_Insert", sqlParameters);
                        if (DtCity != null && DtCity.Rows.Count > 0)
                        {
                            int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                            if (value == 0)
                            {
                                SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                            }
                            else
                            {
                                SqlParameter[] prsqlParameters = new SqlParameter[2];
                                prsqlParameters[0] = new SqlParameter("@JprJobVou", id);
                                prsqlParameters[1] = new SqlParameter("@FLG", "4");

                                DataTable prDtCity = ObjDBConnection.CallStoreProcedure("Usp_JobMst_PrdDtl_Insert", prsqlParameters);

                                SqlParameter[] mnsqlParameters = new SqlParameter[2];
                                mnsqlParameters[0] = new SqlParameter("@JmnJobVou", id);
                                mnsqlParameters[1] = new SqlParameter("@FLG", "4");

                                DataTable mnDtCity = ObjDBConnection.CallStoreProcedure("Usp_JmnMst_ManPower_Insert", mnsqlParameters);
                                if ((prDtCity != null && prDtCity.Rows.Count > 0) && (mnDtCity != null && mnDtCity.Rows.Count > 0))
                                    SetSuccessMessage("Job Deleted Successfully");
                            }
                        }
                        transactionScope.Complete();
                        transactionScope.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "JobMaster");
        }
        [HttpPost]
        public IActionResult Delete1(int jobvou, int sno, int type)
        {
            try
            {
                //JobModel jobmodel = new JobModel();
                if (type == 1)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] PrsqlParameters = new SqlParameter[3];
                    PrsqlParameters[0] = new SqlParameter("@JprJobVou", jobvou);
                    PrsqlParameters[1] = new SqlParameter("@JprSrNo", sno);
                    PrsqlParameters[2] = new SqlParameter("@FLG", "2");

                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("Usp_JobMst_PrdDtl_Insert", PrsqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        return Json(new { msg = "success" });
                        //int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        //if (value == 0)
                        //{
                        //    SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        //}
                        //else
                        //{
                        //    SetSuccessMessage("Job Deleted Successfully");
                        //}
                    }
                }

                if (type == 2)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] PrsqlParameters = new SqlParameter[3];
                    PrsqlParameters[0] = new SqlParameter("@JmnJobVou", jobvou);
                    PrsqlParameters[1] = new SqlParameter("@JmnSrNo", sno);
                    PrsqlParameters[2] = new SqlParameter("@FLG", "2");

                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("Usp_JmnMst_ManPower_Insert", PrsqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        return Json(new { msg = "success" });
                        //int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        //if (value == 0)
                        //{
                        //    SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        //}
                        //else
                        //{
                        //    SetSuccessMessage("Job Deleted Successfully");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "JobMaster");
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
                    string currentURL = "/JobMaster/Index";
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
                    getReportDataModel.ControllerName = "JobMaster";
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
                    var bytes = Excel(getReportDataModel, "Job Report", companyDetails.CmpName);
                    return File(
                          bytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          "City.xlsx");
                }
                else
                {
                    var bytes = PDF(getReportDataModel, "Job Report", companyDetails.CmpName);
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


        //public IActionResult AddState(string name, string code, int position)
        //{
        //    try
        //    {

        //        long userId = GetIntSession("UserId");
        //        int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
        //        int administrator = 0;
        //        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(code))
        //        {
        //            SqlParameter[] sqlParameters = new SqlParameter[9];
        //            sqlParameters[0] = new SqlParameter("@MscName", name);
        //            sqlParameters[1] = new SqlParameter("@MscCode", code);
        //            sqlParameters[2] = new SqlParameter("@MscType", "STA");
        //            sqlParameters[3] = new SqlParameter("@MscPos", position);
        //            sqlParameters[4] = new SqlParameter("@MscActYN", "0");
        //            sqlParameters[5] = new SqlParameter("@MscVou", "0");
        //            sqlParameters[6] = new SqlParameter("@CmpVou", companyId);
        //            sqlParameters[7] = new SqlParameter("@UsrVou", userId);
        //            sqlParameters[8] = new SqlParameter("@Flg", 1);

        //            DataTable DtState = ObjDBConnection.CallStoreProcedure("MscMst_Insert", sqlParameters);
        //            if (DtState != null && DtState.Rows.Count > 0)
        //            {
        //                int status = DbConnection.ParseInt32(DtState.Rows[0][0].ToString());
        //                if (status == -1)
        //                {
        //                    return Json(new { result = false, message = "Duplicate Code" });
        //                }
        //                else if (status == -2)
        //                {
        //                    return Json(new { result = false, message = "Duplicate Name" });
        //                }
        //                else
        //                {
        //                    var stateList = objProductHelper.GetStateMasterDropdown(companyId, administrator);
        //                    return Json(new { result = true, message = "Inserted successfully", data = stateList });
        //                }
        //            }
        //            else
        //            {
        //                return Json(new { result = false, message = "Please Enter the Value" });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { result = false, message = "Please Enter the Value" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

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

