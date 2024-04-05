using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using Soc_Management_Web.Classes;
using Soc_Management_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;


namespace Soc_Management_Web.Controllers
{
    public class OrderController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly object dtlogin;
        MasterDropdownHelper master = new MasterDropdownHelper();
        public OrderController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(long id = 0, int inqid = 0)
        {
            OrderModel model = new OrderModel();
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
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@ordVou", id);
                sqlParameters[1] = new SqlParameter("@Flg", 5);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_OrderMst_Insert", sqlParameters);
                if (DtEmp != null && DtEmp.Rows.Count > 0)
                {
                    model.OrderId = Convert.ToInt32(id);
                    model.OdrNo = id.ToString();
                    model.InquiryNo = Convert.ToInt32(DtEmp.Rows[0]["OrdInqNo"].ToString());
                    model.OrderDate = DtEmp.Rows[0]["OrdDt"].ToString();
                    model.Customerid = Convert.ToInt32(DtEmp.Rows[0]["OrdAccVou"].ToString());
                    model.ordAmount = DtEmp.Rows[0]["OrdAmt"].ToString();
                    model.Remarks = DtEmp.Rows[0]["OrdRem"].ToString();
                    model.OrderTittle = DtEmp.Rows[0]["OrdTitle"].ToString();
                    model.MobileNo = DtEmp.Rows[0]["OrdMobVou"].ToString();
                    model.Address = DtEmp.Rows[0]["add1"].ToString();
                    model.RefId = DtEmp.Rows[0]["OrdRefBy"].ToString();
                    model.StatusId = Convert.ToInt32(DtEmp.Rows[0]["OrdSalFlg"].ToString());
                    model.ordSubTitle = DtEmp.Rows[0]["ordSubTittle"].ToString();
                    model.DiscountAmount = Convert.ToDecimal(DtEmp.Rows[0]["DiscounAmount"].ToString());
                    model.NetAmount = Convert.ToDecimal(DtEmp.Rows[0]["NetAmount"].ToString());
                    model.manual = Convert.ToBoolean(DtEmp.Rows[0]["OrdManualYN"].ToString());
                }
            }
            else if (inqid > 0)
            {
                SqlParameter[] sqlParameters1 = new SqlParameter[2];
                sqlParameters1[0] = new SqlParameter("@flag", 1);
                sqlParameters1[1] = new SqlParameter("@Id", inqid);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_GetOrderMs", sqlParameters1);
                if (DtEmp != null && DtEmp.Rows.Count > 0)
                {
                    model.OrderId = Convert.ToInt32(DtEmp.Rows[0]["ordvou"].ToString());
                    model.OdrNo = DtEmp.Rows[0]["OrdNo"].ToString();
                    model.InquiryNo = Convert.ToInt32(DtEmp.Rows[0]["OrdInqNo"].ToString());
                    model.OrderDate = DtEmp.Rows[0]["OrdDt"].ToString();
                    model.Customerid = Convert.ToInt32(DtEmp.Rows[0]["OrdAccVou"].ToString());
                    model.ordAmount = DtEmp.Rows[0]["OrdAmt"].ToString();
                    model.Remarks = DtEmp.Rows[0]["OrdRem"].ToString();
                    model.OrderTittle = DtEmp.Rows[0]["OrdTitle"].ToString();
                    model.MobileNo = DtEmp.Rows[0]["OrdMobVou"].ToString();
                    model.Address = DtEmp.Rows[0]["add1"].ToString();
                    model.RefId = DtEmp.Rows[0]["OrdRefBy"].ToString();
                    model.StatusId = Convert.ToInt32(DtEmp.Rows[0]["OrdSalFlg"].ToString());
                    model.ordSubTitle = DtEmp.Rows[0]["ordSubTittle"].ToString();
                    model.DiscountAmount = Convert.ToDecimal(DtEmp.Rows[0]["DiscounAmount"].ToString());
                    model.NetAmount = Convert.ToDecimal(DtEmp.Rows[0]["NetAmount"].ToString());
                    model.manual = Convert.ToBoolean(DtEmp.Rows[0]["OrdManualYN"].ToString());
                    model.Customer = DtEmp.Rows[0]["AccNm"].ToString();


                    SqlParameter[] PrsqlParameters1 = new SqlParameter[4];
                    PrsqlParameters1[0] = new SqlParameter("@OrderId", model.OrderId);
                    PrsqlParameters1[1] = new SqlParameter("@JobId", 0);
                    PrsqlParameters1[2] = new SqlParameter("@Type", "Order");
                    PrsqlParameters1[3] = new SqlParameter("@flag", 1);
                    DataTable PrDtEmp = ObjDBConnection.CallStoreProcedure("Usp_GetManpowerAndProductdetails", PrsqlParameters1);

                }




            }
            else
            {
                SqlParameter[] sqlParameters1 = new SqlParameter[1];
                sqlParameters1[0] = new SqlParameter("@Flag", 2);
                DataTable DtEmp1 = ObjDBConnection.CallStoreProcedure("DeleteDuplicaterecord", sqlParameters1);
                model.OdrNo = (objProductHelper.GetSlNo(3) + 1).ToString();
                model.OrderDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

            model.lstCustomer = objProductHelper.GetCustomerListOnly();
            model.lstInqStatus = master.GetDropgen("Status");
            model.lstExtraitem = objProductHelper.GetlstExtraItemsDropdown();
            model.lstReference = objProductHelper.GetlstRefByDropdown();
            model.lstTermsAndCondition = objProductHelper.GetlstTearmAndConditionDropdown();

            model.lstJobmaster = objProductHelper.GetJobMasterLst();
            model.EventLst = objProductHelper.GetEventLst();
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
        public ActionResult Index(long id, OrderModel obj)
        {
            string obj1 = "";
            try
            {
                bool isreturn = false;
                INIT(ref isreturn);

                long userId = GetIntSession("UserId");
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                if (!string.IsNullOrWhiteSpace(Convert.ToString(obj.InquiryNo)))
                {
                    if (id == 0)
                    {
                        id = obj.OrderId;

                    }
                    SqlParameter[] sqlParameters = new SqlParameter[16];
                    sqlParameters[0] = new SqlParameter("@ordVou", id);
                    sqlParameters[1] = new SqlParameter("@InqNo", obj.InquiryNo);
                    sqlParameters[2] = new SqlParameter("@ordDate", obj.OrderDate);
                    sqlParameters[3] = new SqlParameter("@OdrNo", obj.OdrNo);
                    sqlParameters[4] = new SqlParameter("@Custmrid", obj.Customerid);
                    sqlParameters[5] = new SqlParameter("@RefId", obj.RefId);
                    sqlParameters[6] = new SqlParameter("@Title", obj.OrderTittle);
                    sqlParameters[7] = new SqlParameter("@Remarks", obj.Remarks);
                    sqlParameters[8] = new SqlParameter("@statuss", obj.StatusId);
                    sqlParameters[9] = new SqlParameter("@Amount", obj.ordAmount);
                    sqlParameters[10] = new SqlParameter("@DiscountAmount", obj.DiscountAmount);
                    sqlParameters[11] = new SqlParameter("@NetAmount", obj.NetAmount);
                    sqlParameters[12] = new SqlParameter("@subtittle", obj.ordSubTitle);
                    sqlParameters[13] = new SqlParameter("@FLG", "1");
                    sqlParameters[14] = new SqlParameter("@ManualFlag", obj.manual == true ? 1 : 0);
                    sqlParameters[15] = new SqlParameter("@Mobile", obj.MobileNo);
                    DataTable DtCat = ObjDBConnection.CallStoreProcedure("usp_OrderMst_Insert", sqlParameters);
                    if (DtCat != null && DtCat.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtCat.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            id = status;
                            obj1 = "Dulplicate Category Details";
                            return RedirectToAction("index", "Order", new { id = 0 });
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
                                id = Convert.ToInt32(obj.OdrNo);
                                obj1 = "Inserted Sucessfully";
                                SetSuccessMessage("Inserted Sucessfully");

                            }

                            return RedirectToAction("index", "Order", new { id = id });
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
                    string currentURL = "/Order/Index";
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
                    getReportDataModel.ControllerName = "Order";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_reportView", getReportDataModel);
        }

        [HttpPost]
        public IActionResult GetExtraItem()
        {
            try
            {
                List<Extraitem> etrItmObj = new List<Extraitem>();
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@Flg", 5);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Usp_ExtraItems_Insert", sqlParameters);
                for (var item = 0; item < DtEmp.Rows.Count; item++)
                {
                    Extraitem obj = new Extraitem();
                    obj.Sl = Convert.ToInt32(DtEmp.Rows[item]["Sl"].ToString());
                    obj.EtrVou = Convert.ToInt32(DtEmp.Rows[item]["EitVou"].ToString());
                    obj.extraitem = DtEmp.Rows[item]["EitNm"].ToString();
                    obj.amount = Convert.ToInt32(DtEmp.Rows[item]["EitAmt"].ToString());
                    etrItmObj.Add(obj);
                }
                return Json(new { str = etrItmObj });
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        [HttpPost]
        public IActionResult GetNewInquiryNumber()
        {
            try
            {
                List<Extraitem> etrItmObj = new List<Extraitem>();
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@Flg", 5);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Usp_ExtraItems_Insert", sqlParameters);
                for (var item = 0; item < DtEmp.Rows.Count; item++)
                {
                    Extraitem obj = new Extraitem();
                    obj.EtrVou = Convert.ToInt32(DtEmp.Rows[item]["EitVou"].ToString());
                    obj.extraitem = DtEmp.Rows[item]["EitNm"].ToString();
                    obj.amount = Convert.ToInt32(DtEmp.Rows[item]["EitAmt"].ToString());
                    etrItmObj.Add(obj);
                }
                return Json(new { str = etrItmObj });
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        [HttpPost]
        public IActionResult GetMobAndAdrs(int id)
        {
            SqlParameter[] sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("@InqVou", "0");
            sqlParameters[1] = new SqlParameter("@Custmrid", id);
            sqlParameters[2] = new SqlParameter("@FLG", "3");
            DataTable DtCat = ObjDBConnection.CallStoreProcedure("usp_InquiryMst_Insert", sqlParameters);
            object obj = null;
            if (DtCat.Rows.Count > 0)
            {
                obj = new
                {
                    Address = DtCat.Rows[0]["Address"].ToString(),
                    Mobile = DtCat.Rows[0]["MobNo"].ToString(),
                    refrence = DtCat.Rows[0]["AcrNm"].ToString()
                };
            }
            return Json(new { obj });
        }

        [HttpPost]
        public IActionResult GetRenderPartialView(string idNm = "")
        {
            InquerySubMasterModel objModel = new InquerySubMasterModel();
            if (idNm == "home-tab")
            {
                return PartialView("_InqJobView", objModel);
            }
            else
            {
                return null;
            }

        }

        // Tab Show Area
        public PartialViewResult listdata()
        {
            return PartialView("_ListEnquiry");
        }
        public PartialViewResult Adddata(long id = 0)
        {
            ViewBag.InqueryId = id;
            return PartialView("_AddEnquiry");
        }
        public PartialViewResult loadOrderDetails(long id = 0)
        {
            ViewBag.InqueryId = id;
            InquerySubMasterModel objModel = new InquerySubMasterModel();

            return PartialView("_OrderDetailsView", objModel);

            // return PartialView("_JobDetails");
        }

        public PartialViewResult DetailstabLoad(long id = 0)
        {
            ViewBag.InqueryId = id;
            InquerySubMasterModel objModel = new InquerySubMasterModel();

            return PartialView("_ordJobView", objModel);

            // return PartialView("_JobDetails");
        }
        public PartialViewResult ExraitemsLoad(long id = 0)
        {
            try
            {
                List<Extraitem> extraItems = new List<Extraitem>();

                var sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@ExtraItem", "");
                sqlParameters[1] = new SqlParameter("@Amount", 0);
                sqlParameters[2] = new SqlParameter("@InQId", id);
                sqlParameters[3] = new SqlParameter("@EIId", 0);
                sqlParameters[4] = new SqlParameter("@FLG", 3);

                using (DataTable dtEmp = ObjDBConnection.CallStoreProcedure("Add_ord_ExtraItem_Temp", sqlParameters))
                {
                    // Assuming there is a method to map DataTable to a List<Extraitem>
                    extraItems = MapDataTableToExtraItemList(dtEmp);
                }

                ViewBag.InqueryId = id;
                return PartialView("_ExraitemsOrd", extraItems);
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error view
                // Example: return View("Error", new ErrorViewModel { Message = ex.Message });
                throw;
            }
        }

        private List<Extraitem> MapDataTableToExtraItemList(DataTable dataTable)
        {
            List<Extraitem> extraItems = new List<Extraitem>();

            foreach (DataRow row in dataTable.Rows)
            {
                Extraitem extraItem = new Extraitem
                {
                    IteValue = Convert.ToInt32(row["IteVou"]),
                    extraitem = row["IteEitNm"].ToString(),
                    Sl = Convert.ToInt32(row["IteSrNo"]),
                    amount = Convert.ToInt32(row["IteEitAmt"]),
                    // Map other properties accordingly
                };

                extraItems.Add(extraItem);
            }

            return extraItems;
        }

        //private List<Inclusive> MapDataTableToInclusiveToInclusive(DataTable dataTable)
        //{
        //    List<Inclusive> INEx = new List<Inclusive>();
        //        foreach (DataRow row in dataTable.Rows)
        //        {
        //            Inclusive INExa = new Inclusive
        //            {
        //                Id = Convert.ToInt32(row["IncVou"]),
        //                Tranid = Convert.ToInt32(row["IncInqVou"]),
        //                sl = Convert.ToInt32(row["incSrNo"]),
        //                Item = row["IncTncNm"].ToString(),
        //                description = row["IncTncType"].ToString()
        //            };

        //            INEx.Add(INExa);
        //        }


        //    return INEx;
        //}

        //private List<Enclusive> MapDataTableToInclusiveToEsclusive(DataTable dataTable)
        //{
        //    List<Enclusive> INEx = new List<Enclusive>();
        //       foreach (DataRow row in dataTable.Rows)
        //        {
        //        Enclusive INExa = new Enclusive
        //            {
        //                Id = Convert.ToInt32(row["IteVou"]),
        //                Tranid = Convert.ToInt32(row["IExInqVou"]),
        //                sl = Convert.ToInt32(row["IExSRNo"]),
        //                Item = row["IExTncNm"].ToString(),
        //                description = row["IExTncDesc"].ToString()
        //            };

        //            INEx.Add(INExa);
        //        }

        //    return INEx;
        //}
        [HttpPost]
        public JsonResult DeleteExtraItem(int extraItemId)
        {
            try
            {

                List<Extraitem> extraItems = new List<Extraitem>();

                var sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@ExtraItem", "");
                sqlParameters[1] = new SqlParameter("@Amount", 0);
                sqlParameters[2] = new SqlParameter("@InQId", 2);
                sqlParameters[3] = new SqlParameter("@EIId", extraItemId);
                sqlParameters[4] = new SqlParameter("@FLG", 2);

                using (DataTable dtEmp = ObjDBConnection.CallStoreProcedure("Add_ord_ExtraItem_Temp", sqlParameters))
                {
                    return Json(new { success = true, message = "Record deleted successfully" });
                }


            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error view
                // Example: return View("Error", new ErrorViewModel { Message = ex.Message });
                throw;
            }

            return Json(new { success = true, message = "Record deleted successfully" });

        }

        [HttpPost]
        public JsonResult DeleteTermsAndCondition(int extraItemId)
        {
            try
            {

                List<Extraitem> extraItems = new List<Extraitem>();

                var sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@InTcVou", extraItemId);

                sqlParameters[1] = new SqlParameter("@FLG", 2);

                using (DataTable dtEmp = ObjDBConnection.CallStoreProcedure("Add_Ord_Inclusive", sqlParameters))
                {
                    return Json(new { success = true, message = "Record deleted successfully" });
                }


            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error view
                // Example: return View("Error", new ErrorViewModel { Message = ex.Message });
                throw;
            }

            return Json(new { success = true, message = "Record deleted successfully" });

        }


        public PartialViewResult InclusiveexclusivetabLoad(long id = 0)
        {
            List<InclusiveExclusiveModel> inlst = new List<InclusiveExclusiveModel>();
            try
            {

                var sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@Tranid", id);
                sqlParameters[1] = new SqlParameter("@Id", 0);
                sqlParameters[2] = new SqlParameter("@Item", 2);
                sqlParameters[3] = new SqlParameter("@Description", "");
                sqlParameters[4] = new SqlParameter("@Type", 0);
                sqlParameters[5] = new SqlParameter("@FLG", 3);


                using (DataTable dt = ObjDBConnection.CallStoreProcedure("Add_Ord_InclusiveExclusive", sqlParameters))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        InclusiveExclusiveModel INExa = new InclusiveExclusiveModel
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Tranid = Convert.ToInt32(row["TranId"]),
                            sl = Convert.ToInt32(row["Sl"]),
                            Item = row["Item"].ToString(),
                            description = row["Description"].ToString(),
                            Type = row["Type"].ToString(),
                        };
                        inlst.Add(INExa);
                    }
                }

                ViewBag.InqueryId = id;
                return PartialView("_InclusiveExclusiveOrd", inlst);
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error view
                // Example: return View("Error", new ErrorViewModel { Message = ex.Message });
                throw;
            }
            ViewBag.InqueryId = id;
            return PartialView("_InclusiveExclusiveOrd", inlst);
        }
        public PartialViewResult LocationVideoAndPhotos(long id = 0)
        {
            LocationOrderModel model = new LocationOrderModel();
            MasterDropdownHelper masterDropdownHelper = new MasterDropdownHelper();
            ViewBag.InqueryId = id;
            model.TypeList = masterDropdownHelper.GetRecordingType();
            SqlParameter[] sqlParameters = new SqlParameter[12];
            sqlParameters[0] = new SqlParameter("@Id", id);
            sqlParameters[1] = new SqlParameter("@orderId", id);
            sqlParameters[2] = new SqlParameter("@RecorType", id);
            sqlParameters[3] = new SqlParameter("@AllPhotos", 0);
            sqlParameters[4] = new SqlParameter("@Allvieos", 0);
            sqlParameters[5] = new SqlParameter("@VideoLocaton", "");
            sqlParameters[6] = new SqlParameter("@PhotosLocation", "");
            sqlParameters[7] = new SqlParameter("@VideoQty", 0);
            sqlParameters[8] = new SqlParameter("@PhotosQty", 0);
            sqlParameters[9] = new SqlParameter("@Remarks", "");
            sqlParameters[10] = new SqlParameter("@Fileformatedetails", "");
            sqlParameters[11] = new SqlParameter("@flag", 2);
            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_locationdetails", sqlParameters);
            if (DtEmp != null && DtEmp.Rows.Count > 0)
            {
                model.LocationId = Convert.ToInt32(DtEmp.Rows[0]["Id"].ToString());
                model.OrderId = Convert.ToInt32(id);
                model.RecType = DtEmp.Rows[0]["TypeName"].ToString();
                model.AllPhotos = Convert.ToBoolean(DtEmp.Rows[0]["AllPhotos"].ToString());
                model.Allvieos = Convert.ToBoolean(DtEmp.Rows[0]["AllVideos"].ToString());
                model.VideoLocaton = DtEmp.Rows[0]["VideoLocation"].ToString();
                model.PhotosLocation = DtEmp.Rows[0]["PhotosLocation"].ToString();
                model.VideoQty = Convert.ToInt32(DtEmp.Rows[0]["VideosQty"].ToString());
                model.PhotosQty = Convert.ToInt32(DtEmp.Rows[0]["PhotosQty"].ToString());
                model.Fileformatedetails = DtEmp.Rows[0]["FileFormateSize"].ToString();
                model.Remarks = DtEmp.Rows[0]["Remarks"].ToString();
            }

            return PartialView("_VideoLocationOrd", model);
        }

        [HttpPost]
        public IActionResult SaveInqSubMobFooter(InqHeaderAndFooterModel data)
        {


            List<Extraitem> etrItmObj = new List<Extraitem>();
            SqlParameter[] sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("@inqid", data.InqId);
            sqlParameters[1] = new SqlParameter("@sub", data.Subject);
            sqlParameters[2] = new SqlParameter("@header", data.Hedaer);
            sqlParameters[3] = new SqlParameter("@footer", data.Footer);
            sqlParameters[4] = new SqlParameter("@mob", data.Mobile);
            sqlParameters[5] = new SqlParameter("@flag", 1);

            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Update_Inq_sub_Mob_Foot", sqlParameters);
            if (DtEmp != null && DtEmp.Rows.Count > 0)
            {
                int status = DbConnection.ParseInt32(DtEmp.Rows[0][0].ToString());
            }
            return Json(new { data = "1" });
        }

        #region Terms & COndition
        public PartialViewResult TearmAndConditionLoad(long id = 0)
        {
            List<InqTermandCondition> obj = new List<InqTermandCondition>();
            ViewBag.InqueryId = id;

            List<InqTermandCondition> extraItems = new List<InqTermandCondition>();

            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@InTcInqVou", id);
            sqlParameters[1] = new SqlParameter("@FLG", 3);

            using (DataTable dtEmp = ObjDBConnection.CallStoreProcedure("Add_Ord_Inclusive", sqlParameters))
            {
                // Assuming there is a method to map DataTable to a List<Extraitem>
                extraItems = MapDataTableToTermsConditionList(dtEmp);
            }
            return PartialView("_TermandconditionviewOrd", extraItems);
        }
        private List<InqTermandCondition> MapDataTableToTermsConditionList(DataTable dataTable)
        {
            List<InqTermandCondition> extraItems = new List<InqTermandCondition>();

            foreach (DataRow row in dataTable.Rows)
            {
                InqTermandCondition extraItem = new InqTermandCondition
                {
                    InqId = Convert.ToInt32(row["InTcVou"]),
                    SlNo = Convert.ToInt32(row["InTcSrNo"]),
                    Terms = row["IntcTerm"].ToString(),
                    Description = row["InTcTncDesc"].ToString(),
                };

                extraItems.Add(extraItem);
            }

            return extraItems;
        }

        #endregion
        // Save Term and condition data
        [HttpPost]
        public IActionResult SaveTermdata(InqTermandCondition data)
        {


            List<Extraitem> etrItmObj = new List<Extraitem>();
            SqlParameter[] sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("@InTcVou", data.TermInqId);
            sqlParameters[1] = new SqlParameter("@InTcInqVou", data.InqId);
            sqlParameters[2] = new SqlParameter("@IntcTerm", data.Terms);
            sqlParameters[3] = new SqlParameter("@InTcTncDesc", data.Description);
            sqlParameters[4] = new SqlParameter("@InTcTncType", "Terms & Condition");
            sqlParameters[5] = new SqlParameter("@FLG", 1);

            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Add_Ord_Inclusive", sqlParameters);
            if (DtEmp != null && DtEmp.Rows.Count > 0)
            {
                int status = DbConnection.ParseInt32(DtEmp.Rows[0][0].ToString());
            }
            return Json(new { data = "1" });
        }

        [HttpPost]
        public JsonResult ExraItemSave(Extraitem data)
        {

            List<Extraitem> etrItmObj = new List<Extraitem>();
            SqlParameter[] sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("@ExtraItem", data.extraitem == null ? "" : data.extraitem);
            sqlParameters[1] = new SqlParameter("@Amount", data.amount);
            sqlParameters[2] = new SqlParameter("@InQId", data.InqId);
            sqlParameters[3] = new SqlParameter("@EIId", data.EtrVou);

            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Add_ord_ExtraItem", sqlParameters);
            string obj = DtEmp.Rows[0]["Result"].ToString();
            List<Extraitem> obj1 = new List<Extraitem>();
            return Json(new { obj });
        }

        [HttpPost]
        public JsonResult SaveJonDetails(PostJobDetails data)
        {
            CultureInfo culture = new CultureInfo("en-US");
            List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
            SqlParameter[] sqlParameters = new SqlParameter[29];
            sqlParameters[0] = new SqlParameter("@pjobentrychk ", data.pjobentrychk);
            sqlParameters[1] = new SqlParameter("@addoneday ", data.addoneday);
            sqlParameters[2] = new SqlParameter("@fullday ", data.fullday);
            sqlParameters[3] = new SqlParameter("@Job", data.Job);
            sqlParameters[4] = new SqlParameter("@Eventsid", data.Eventsid);
            sqlParameters[5] = new SqlParameter("@venueid", data.venueid);
            sqlParameters[6] = new SqlParameter("@ocasiondate", DateTime.ParseExact(data.ocasiondate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
            sqlParameters[7] = new SqlParameter("@fromtime", data.fromtime);
            sqlParameters[8] = new SqlParameter("@totime", data.totime);
            sqlParameters[9] = new SqlParameter("@qty", data.qty);
            sqlParameters[10] = new SqlParameter("@rate", data.rate);
            sqlParameters[11] = new SqlParameter("@amount", data.amount);
            sqlParameters[12] = new SqlParameter("@discountpercentage", data.discountpercentage);
            sqlParameters[13] = new SqlParameter("@discountamt ", data.discountamt);
            sqlParameters[14] = new SqlParameter("@totalnet ", data.totalnet);
            sqlParameters[15] = new SqlParameter("@remarks", data.remarks);
            sqlParameters[16] = new SqlParameter("@spnoes", data.spnoes);
            sqlParameters[17] = new SqlParameter("@status", data.status);
            sqlParameters[18] = new SqlParameter("@eventhrs", data.eventhrs);
            sqlParameters[19] = new SqlParameter("@IndId", data.IndId);
            sqlParameters[20] = new SqlParameter("@JobId", data.JobId);
            sqlParameters[21] = new SqlParameter("@Todate", DateTime.ParseExact(data.Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
            sqlParameters[22] = new SqlParameter("@venuelink", data.venuelink);
            sqlParameters[23] = new SqlParameter("@sl", data.sl);
            sqlParameters[24] = new SqlParameter("@VenueOneAddTo", data.VenueOneAddTo);
            sqlParameters[25] = new SqlParameter("@VenueToAddOne", data.VenueToAddOne);
            sqlParameters[26] = new SqlParameter("@VenueToAddTwo", data.VenueToAddTwo);
            sqlParameters[27] = new SqlParameter("@VenueToUrl", data.VenueToUrl);
            sqlParameters[28] = new SqlParameter("@opionwending", data.Wedingceremonyoptions);

            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Add_Ord_Jobdetails", sqlParameters);

            string obj = DtEmp.Rows[0]["Result"].ToString();
            return Json(new { obj });
        }

        [HttpGet]
        public JsonResult GetJobDetails(OrderJob data)
        {
            List<OrderProductandManpowerModel> prLst = new List<OrderProductandManpowerModel>();

            SqlParameter[] PrsqlParameters = new SqlParameter[4];
            PrsqlParameters[0] = new SqlParameter("@OrderId", data.OrderId);
            PrsqlParameters[1] = new SqlParameter("@JobId", data.JobId);
            PrsqlParameters[2] = new SqlParameter("@Type", data.Type);
            PrsqlParameters[3] = new SqlParameter("@flag", data.flag);
            DataTable PrDtEmp = ObjDBConnection.CallStoreProcedure("Usp_GetManpowerAndProductdetails", PrsqlParameters);
            if (PrDtEmp != null && PrDtEmp.Rows.Count > 0)
            {
                for (var i = 0; i < PrDtEmp.Rows.Count; i++)
                {
                    OrderProductandManpowerModel obj = new OrderProductandManpowerModel();
                    obj.SlNo = Convert.ToInt32(PrDtEmp.Rows[i]["SlNo"].ToString());
                    obj.Id = Convert.ToInt32(PrDtEmp.Rows[i]["Id"].ToString());
                    obj.OrderId = Convert.ToInt32(PrDtEmp.Rows[i]["OrderId"].ToString());
                    obj.InqId = Convert.ToInt32(PrDtEmp.Rows[i]["InqId"].ToString());
                    obj.Qty = Convert.ToInt32(PrDtEmp.Rows[i]["Qty"].ToString());
                    obj.ProductId = Convert.ToInt32(PrDtEmp.Rows[i]["ProductId"].ToString());
                    obj.JobId = Convert.ToInt32(PrDtEmp.Rows[i]["JobId"].ToString());
                    obj.ProdName = PrDtEmp.Rows[i]["ProdName"].ToString();
                    obj.Venue = PrDtEmp.Rows[i]["Venue"].ToString();
                    obj.FromDate = PrDtEmp.Rows[i]["FromDate"].ToString();
                    obj.ToDate = PrDtEmp.Rows[i]["ToDate"].ToString();
                    obj.FromTime = PrDtEmp.Rows[i]["FromTime"].ToString();
                    obj.ToTime = PrDtEmp.Rows[i]["ToTime"].ToString();
                    obj.FromTime = PrDtEmp.Rows[i]["FromTime"].ToString();
                    obj.Remarks = PrDtEmp.Rows[i]["Remarks"].ToString();

                    prLst.Add(obj);
                }
            }

            return Json(new { prLst });
        }

        [HttpGet]
        public JsonResult GetManposerDetails(OrderJob data)
        {
            List<OrderProductandManpowerModel> prLst = new List<OrderProductandManpowerModel>();

            SqlParameter[] PrsqlParameters = new SqlParameter[4];
            PrsqlParameters[0] = new SqlParameter("@OrderId", data.OrderId);
            PrsqlParameters[1] = new SqlParameter("@JobId", data.JobId);
            PrsqlParameters[2] = new SqlParameter("@Type", data.Type);
            PrsqlParameters[3] = new SqlParameter("@flag", data.flag);
            DataTable PrDtEmp = ObjDBConnection.CallStoreProcedure("Usp_GetManpowerAndProductdetails", PrsqlParameters);
            if (PrDtEmp != null && PrDtEmp.Rows.Count > 0)
            {
                for (var i = 0; i < PrDtEmp.Rows.Count; i++)
                {
                    OrderProductandManpowerModel obj = new OrderProductandManpowerModel();
                    obj.SlNo = Convert.ToInt32(PrDtEmp.Rows[i]["SlNo"].ToString());
                    obj.Id = Convert.ToInt32(PrDtEmp.Rows[i]["Id"].ToString());
                    obj.OrderId = Convert.ToInt32(PrDtEmp.Rows[i]["OrderId"].ToString());
                    obj.InqId = Convert.ToInt32(PrDtEmp.Rows[i]["InqId"].ToString());
                    obj.Qty = Convert.ToInt32(PrDtEmp.Rows[i]["Qty"].ToString());
                    obj.ManpowerId = Convert.ToInt32(PrDtEmp.Rows[i]["ManpowerId"].ToString());
                    obj.JobId = Convert.ToInt32(PrDtEmp.Rows[i]["JobId"].ToString());
                    obj.Person = PrDtEmp.Rows[i]["Person"].ToString();
                    obj.datayn = Convert.ToInt32(PrDtEmp.Rows[i]["datayn"].ToString());
                    obj.CatNm = PrDtEmp.Rows[i]["CatNm"].ToString();
                    obj.Venue = PrDtEmp.Rows[i]["Venue"].ToString();
                    obj.FromDate = PrDtEmp.Rows[i]["FromDate"].ToString();
                    obj.ToDate = PrDtEmp.Rows[i]["ToDate"].ToString();
                    obj.FromTime = PrDtEmp.Rows[i]["FromTime"].ToString();
                    obj.ToTime = PrDtEmp.Rows[i]["ToTime"].ToString();
                    obj.FromTime = PrDtEmp.Rows[i]["FromTime"].ToString();
                    obj.Remarks = PrDtEmp.Rows[i]["Remarks"].ToString();
                    prLst.Add(obj);
                }
            }

            return Json(new { prLst });
        }
        [HttpGet]
        public JsonResult GetOrderDetails(GetAlldetails data)
        {

            List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
            SqlParameter[] sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("@IndId", data.TranId);
            sqlParameters[1] = new SqlParameter("@Id", data.Id);
            sqlParameters[2] = new SqlParameter("@JobPrev", data.Type);
            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SpGet_Orderetails", sqlParameters);
            for (int i = 0; i < DtEmp.Rows.Count; i++)
            {
                PostJobDetails dt = new PostJobDetails();
                dt.JobId = Convert.ToInt32(DtEmp.Rows[i]["OrtVou"].ToString());
                dt.sl = Convert.ToInt32(DtEmp.Rows[i]["SlNo"].ToString());
                dt.IndId = Convert.ToInt32(DtEmp.Rows[i]["OrtOrdVou"].ToString());
                dt.Job = Convert.ToInt32(DtEmp.Rows[i]["OrtJobVou"].ToString());
                dt.JobName = DtEmp.Rows[i]["JobName"].ToString();
                dt.Eventsid = DtEmp.Rows[i]["OrtEvnNm"].ToString();
                dt.venueid = DtEmp.Rows[i]["OrtVenNm"].ToString();
                dt.ocasiondate = DtEmp.Rows[i]["OrtOccDt"].ToString();
                dt.date = DtEmp.Rows[i]["OrtOccDt"].ToString();
                dt.fromtime = DtEmp.Rows[i]["OrtFrTm"].ToString();
                dt.totime = DtEmp.Rows[i]["OrtToTm"].ToString();
                dt.qty = Convert.ToDecimal(DtEmp.Rows[i]["OrtQty"].ToString());
                dt.rate = Convert.ToDecimal(DtEmp.Rows[i]["OrtRt"].ToString());
                dt.amount = Convert.ToDecimal(DtEmp.Rows[i]["OrtAmt"].ToString());
                dt.discountpercentage = Convert.ToDecimal(DtEmp.Rows[i]["OrtDisPer"].ToString());
                dt.discountamt = Convert.ToDecimal(DtEmp.Rows[i]["OrtDisAmt"].ToString());
                dt.totalnet = Convert.ToDecimal(DtEmp.Rows[i]["OrdNetAmt"].ToString());
                dt.remarks = DtEmp.Rows[i]["OrtRem"].ToString();
                dt.spnoes = DtEmp.Rows[i]["OrtSpRem"].ToString();
                dt.status = DtEmp.Rows[i]["OrdEveStat"].ToString();
                dt.fullday = Convert.ToInt32(DtEmp.Rows[i]["OrdFullDayYN"].ToString());
                dt.eventhrs = DtEmp.Rows[i]["OrtEveHrs"].ToString();
                dt.pjobentrychk = Convert.ToInt32(DtEmp.Rows[i]["PJobEntryCheck"].ToString());
                dt.addoneday = Convert.ToInt32(DtEmp.Rows[i]["AddOneDay"].ToString());
                dt.Todate = DtEmp.Rows[i]["ToDate"].ToString();

                dt.InqAmount = Convert.ToDecimal(DtEmp.Rows[i]["OrderAmount"].ToString());
                dt.venuelink = DtEmp.Rows[i]["VenueLink"].ToString();
                dt.sl = Convert.ToInt32(DtEmp.Rows[i]["SlNo"].ToString());
                dt.VenueOneAddTo = DtEmp.Rows[i]["VenueOneAddTo"].ToString();
                dt.VenueToAddTwo = DtEmp.Rows[i]["VenueToAddTwo"].ToString();
                dt.VenueToAddOne = DtEmp.Rows[i]["VenueToAddOne"].ToString();
                dt.VenueToUrl = DtEmp.Rows[i]["VenueToUrl"].ToString();
                dt.Wedingceremonyoptions = DtEmp.Rows[i]["opionwending"].ToString();
                etrItmObj.Add(dt);
            }
            return Json(new { etrItmObj });
        }

        [HttpPost]
        public JsonResult deletejobdeails(GetAlldetails data)
        {

            List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
            SqlParameter[] sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("@IndId", data.TranId);
            sqlParameters[1] = new SqlParameter("@Id", data.Id);
            sqlParameters[2] = new SqlParameter("@Type", data.Type);
            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Delete_Inq_Jobdetails", sqlParameters);

            string obj = DtEmp.Rows[0]["Result"].ToString();
            return Json(new { obj });
        }

        [HttpPost]
        public JsonResult deleteOrderdeails(GetAlldetails data)
        {

            List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
            SqlParameter[] sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("@IndId", data.TranId);
            sqlParameters[1] = new SqlParameter("@Id", data.Id);
            sqlParameters[2] = new SqlParameter("@Type", data.Type);
            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Delete_Inq_Jobdetails", sqlParameters);

            string obj = DtEmp.Rows[0]["Result"].ToString();
            return Json(new { obj });
        }

        [HttpGet]
        public JsonResult GetEnqueryDetails(GetAlldetails data)
        {

            List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
            SqlParameter[] sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("@IndId", data.TranId);
            sqlParameters[1] = new SqlParameter("@Id", data.Id);
            sqlParameters[2] = new SqlParameter("@Type", data.Type);
            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SpGet_Inq_details", sqlParameters);
            for (int i = 0; i < DtEmp.Rows.Count; i++)
            {
                PostJobDetails dt = new PostJobDetails();
                dt.JobId = Convert.ToInt32(DtEmp.Rows[i]["IntVou"].ToString());
                dt.sl = Convert.ToInt32(DtEmp.Rows[i]["SlNo"].ToString());
                dt.IndId = Convert.ToInt32(DtEmp.Rows[i]["IntInqVou"].ToString());
                dt.Job = Convert.ToInt32(DtEmp.Rows[i]["IntJobVou"].ToString());
                dt.JobName = DtEmp.Rows[i]["JobName"].ToString();
                dt.Eventsid = DtEmp.Rows[i]["IntEvnNm"].ToString();
                dt.venueid = DtEmp.Rows[i]["IntVenNm"].ToString();
                dt.ocasiondate = DtEmp.Rows[i]["IntOccDt"].ToString();
                dt.date = DtEmp.Rows[i]["IntOccDt"].ToString();
                dt.fromtime = DtEmp.Rows[i]["IntFrTm"].ToString();
                dt.totime = DtEmp.Rows[i]["IntToTm"].ToString();
                dt.qty = Convert.ToDecimal(DtEmp.Rows[i]["IntQty"].ToString());
                dt.rate = Convert.ToDecimal(DtEmp.Rows[i]["IntRt"].ToString());
                dt.amount = Convert.ToDecimal(DtEmp.Rows[i]["IntAmt"].ToString());
                dt.discountpercentage = Convert.ToDecimal(DtEmp.Rows[i]["IntDisPer"].ToString());
                dt.discountamt = Convert.ToDecimal(DtEmp.Rows[i]["IntDisAmt"].ToString());
                dt.totalnet = Convert.ToDecimal(DtEmp.Rows[i]["IntNetAmt"].ToString());
                dt.remarks = DtEmp.Rows[i]["IntRem"].ToString();
                dt.spnoes = DtEmp.Rows[i]["IntSpRem"].ToString();
                dt.status = DtEmp.Rows[i]["IntEveStat"].ToString();
                dt.fullday = Convert.ToInt32(DtEmp.Rows[i]["IntFullDayYN"].ToString());
                dt.eventhrs = DtEmp.Rows[i]["IntEveHrs"].ToString();
                dt.pjobentrychk = Convert.ToInt32(DtEmp.Rows[i]["PJobEntryCheck"].ToString());
                dt.addoneday = Convert.ToInt32(DtEmp.Rows[i]["AddOneDay"].ToString());
                dt.InqAmount = Convert.ToInt32(DtEmp.Rows[i]["EnquiryAmt"].ToString());
                etrItmObj.Add(dt);
            }
            return Json(new { etrItmObj });
        }

        public IActionResult Delete(long id)
        {
            try
            {
                CategoryModel catModel = new CategoryModel();
                if (id > 0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@ordVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_OrderMst_Insert", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtEmp.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Order Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "Order");
        }

        [HttpPost]
        public JsonResult SaveInclusiveExclusive(InclusiveExclusiveModel data)
        {

            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("@Tranid", data.Tranid);
            sqlParameters[1] = new SqlParameter("@Id", data.Id);
            sqlParameters[2] = new SqlParameter("@Item", data.Item);
            sqlParameters[3] = new SqlParameter("@Description", data.Item);
            sqlParameters[4] = new SqlParameter("@Type", data.Type);
            sqlParameters[5] = new SqlParameter("@FLG", data.flag);

            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Add_Ord_InclusiveExclusive", sqlParameters);

            string obj = DtEmp.Rows[0]["Result"].ToString();
            return Json(new { obj });
        }

        [HttpPost]
        public JsonResult GetSerialNo(string Types, int InqId)
        {
            string SlNo = "";
            try
            {

                List<Extraitem> extraItems = new List<Extraitem>();

                var sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@Types", Types);
                sqlParameters[1] = new SqlParameter("@InqId", InqId);


                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SPGet_SerialNo", sqlParameters);
                if (DtEmp != null && DtEmp.Rows.Count > 0)
                {
                    SlNo = DtEmp.Rows[0]["Sl"].ToString();
                }


            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error view
                // Example: return View("Error", new ErrorViewModel { Message = ex.Message });
                throw;
            }

            return Json(new { success = true, result = SlNo });

        }
        [HttpGet]
        public JsonResult GetForAllSelection(string Types)
        {
            List<ForSelectAllModel> SelectAll = new List<ForSelectAllModel>();
            try
            {



                var sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@Types", Types);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SpGet_ForSelectAll", sqlParameters);
                if (DtEmp != null && DtEmp.Rows.Count > 0)
                {
                    foreach (DataRow dr in DtEmp.Rows)
                    {
                        ForSelectAllModel select = new ForSelectAllModel();
                        select.Amount = Convert.ToDecimal(dr["Amount"].ToString());
                        select.Id = Convert.ToInt32(dr["Id"].ToString());
                        select.Sl = Convert.ToInt32(dr["SL"].ToString());
                        select.Item = dr["Item"].ToString();
                        select.Description = dr["Description"].ToString();
                        SelectAll.Add(select);
                    }
                }


            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error view
                // Example: return View("Error", new ErrorViewModel { Message = ex.Message });
                throw;
            }

            return Json(new { success = true, result = SelectAll });

        }

        // Save Select All

        [HttpGet]
        public JsonResult SaveSelectAll(string Ids, string Type, int TranId)
        {
            try
            {



                var sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@Ids", Ids);
                sqlParameters[1] = new SqlParameter("@Type", Type);
                sqlParameters[2] = new SqlParameter("@TranId", TranId);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SpSave_ForSelectAll", sqlParameters);

            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error view
                // Example: return View("Error", new ErrorViewModel { Message = ex.Message });
                throw;
            }

            return Json(new { success = true, result = "" });

        }

        //public ActionResult GeneratePDF()
        //{
        //    // Output file path
        //    string outputPath = "output.pdf";

        //    // Create a PdfWriter object
        //    using (var writer = new PdfWriter(outputPath))
        //    {
        //        // Create a PdfDocument object
        //        using (var pdf = new PdfDocument(writer))
        //        {
        //            // Create a Document object
        //            var document = new Document(pdf);

        //            // Add content to the document
        //            document.Add(new Paragraph("Hello, iTextSharp!"));
        //            document.Add(new Paragraph("This is a simple PDF generated using iTextSharp."));

        //            // Close the document
        //            document.Close();
        //        }
        //    }
        //}

        public PartialViewResult InqueryReport(int id = 0, string Headertype = "")
        {

            Document doc = new Document();
            string filePath = "";
            ReportFileInfo fileInfo = new ReportFileInfo();


            using (MemoryStream memoryStream = new MemoryStream())
            {
                List<InqueryReportModel> Inquery = getReportcontent(id);


                // Associate the Document with the MemoryStream
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, memoryStream);

                // Add an event handler to add page numbers
                pdfWriter.PageEvent = new PageNumberEventHandler();

                doc.Open();

                PdfPTable tableLayout = new PdfPTable(5);

                tableLayout = Add_Content_To_PDF(tableLayout, Inquery, Headertype);
                tableLayout.HeaderRows = 7;
                doc.Add(tableLayout);

                // Close the Document to finish the PDF creation
                doc.Close();

                // Write the content of the MemoryStream to a file
                string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports");
                string fileName = Inquery[0].FileName.ToString() + ".pdf";

                filePath = Path.Combine(folderPath, fileName);
                fileInfo.FileName = fileName;
                fileInfo.FilePath = filePath;
                fileInfo.Id = id;
                fileInfo.Title = Inquery[0].InqTitle;
                fileInfo.Sendto = Inquery[0].AccEmail;
                System.IO.File.WriteAllBytes(filePath, memoryStream.ToArray());
            }



            return PartialView("_InqueryReport", fileInfo);
        }

        public List<InqueryReportModel> getReportcontent(long id)
        {
            List<InqueryReportModel> Inquery = new List<InqueryReportModel>();
            List<ExtraItemreport> extitemlst = new List<ExtraItemreport>();
            List<Exclusierepoer> exclusierepoers = new List<Exclusierepoer>();
            List<Inclusivereport> inclusivereports = new List<Inclusivereport>();
            List<tearmandcondition> tcl = new List<tearmandcondition>();
            ViewBag.InqueryId = id;
            var sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@Id", id);
            DataSet ds = ObjDBConnection.CallStoreProcedureDS("GetQuatationOrder", sqlParameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[1].Rows)
                    {
                        ExtraItemreport extite = new ExtraItemreport();
                        extite.IteEitNm = item["IteEitNm"].ToString();
                        extite.IteEitAmt = Convert.ToDecimal(item["IteEitAmt"].ToString());

                        extitemlst.Add(extite);
                    }
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[2].Rows)
                    {
                        Exclusierepoer exclu = new Exclusierepoer();
                        exclu.IExTncNm = item["IExTncNm"].ToString();
                        exclu.IExTncDesc = item["IExTncDesc"].ToString();


                        exclusierepoers.Add(exclu);
                    }
                }
                if (ds.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[3].Rows)
                    {
                        Inclusivereport exclu = new Inclusivereport();
                        exclu.IncTncNm = item["IncTncNm"].ToString();
                        exclu.IncTncDesc = item["IncTncDesc"].ToString();

                        inclusivereports.Add(exclu);
                    }
                }
                if (ds.Tables[4].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[4].Rows)
                    {
                        tearmandcondition exclu = new tearmandcondition();
                        exclu.item = item["IntcTerm"].ToString();
                        exclu.desc = item["InTcTncDesc"].ToString();

                        tcl.Add(exclu);
                    }
                }
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    InqueryReportModel inq = new InqueryReportModel();
                    inq.exclusive = exclusierepoers;
                    inq.Inclusive = inclusivereports;
                    inq.ExtraItem = extitemlst;
                    inq.tandc = tcl;
                    inq.AccEmail = item["AccEmail"].ToString();
                    inq.customerremarks = item["customerremarks"].ToString();
                    inq.AccNm = item["AccNm"].ToString();
                    inq.Add1 = item["Add1"].ToString();
                    inq.Add2 = item["Add2"].ToString();
                    inq.Heading = item["Heading"].ToString();
                    inq.InqDt = item["InqDt"].ToString();
                    inq.InqMobile = item["OrdMobVou"].ToString();
                    inq.InqTitle = item["OrdTitle"].ToString();
                    inq.IntOccDt = item["IntOccDt"].ToString();
                    inq.Todate = item["Todate"].ToString();
                    inq.IntFrTm = item["OrtFrTm"].ToString();
                    inq.IntToTm = item["OrtToTm"].ToString();
                    inq.AccAdd1 = item["AccAdd1"].ToString();
                    inq.IntEvnNm = item["OrtEvnNm"].ToString();
                    inq.JobNm = item["JobNm"].ToString();
                    inq.IntVenNm = item["OrtVenNm"].ToString();
                    inq.IntQty = Convert.ToDecimal(item["OrtQty"].ToString());
                    inq.IntRt = Convert.ToDecimal(item["OrtRt"].ToString());
                    inq.IntAmt = Convert.ToDecimal(item["OrtAmt"].ToString());
                    inq.Avobedis = Convert.ToDecimal(item["Avobedis"].ToString());
                    
                    inq.Studio = item["Studio"].ToString();
                    inq.pinstate = item["pinstate"].ToString();
                    inq.mobile = item["mobile"].ToString();
                    inq.phone = item["phone"].ToString();
                    inq.OwnerName = item["OwnerName"].ToString();
                    inq.ownerEmail = item["ownerEmail"].ToString();
                    inq.FileName = item["FileName"].ToString();
                    inq.TotalAmount = Convert.ToDecimal(item["TotalAmountt"].ToString());
                    inq.DiscountAmont = Convert.ToDecimal(item["IntNetAmt"].ToString());
                    inq.disamt = Convert.ToDecimal(item["OrtDisAmt"].ToString());
                    inq.disper = Convert.ToDecimal(item["OrtDisPer"].ToString());
                    inq.groupbyid =item["GroupId"].ToString();
                    inq.InqFooter = item["InqFooter"].ToString();
                    inq.FooterMobile = item["FooterMobile"].ToString();
                    inq.AccMob = item["AccMob"].ToString();
                    Inquery.Add(inq);
                }
            }



            return Inquery;
        }

        // for pdf report





        private PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, List<InqueryReportModel> Inquery, string Headertype)
        {

            List<InqueryReportModel> Inquery1 = new List<InqueryReportModel>();

            var uniqu = Inquery.Select(p => p.groupbyid).Distinct();
            float[] headers = { 15, 40, 10, 20, 15 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 100;       //Set the PDF File witdh percentage
            if (Headertype != "Header")
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 17, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = -4, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 35, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 0, HorizontalAlignment = Element.ALIGN_CENTER });

            }
            //Add Title to the PDF file at the top
            if (Headertype == "Header")
            {
                tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].Studio, new Font(Font.FontFamily.HELVETICA, 17, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = -4, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].Add1, new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].Add2, new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].pinstate, new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase("Phone :" + Inquery[0].phone + " Mobile " + Inquery[0].mobile, new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase("Email :" + Inquery[0].ownerEmail, new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 7, HorizontalAlignment = Element.ALIGN_CENTER });

                tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 22, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, BorderWidthBottom = 2, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 22, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Maroon)))) { Colspan = 5, Border = 0, BorderWidthBottom = 3, PaddingBottom = 3, HorizontalAlignment = Element.ALIGN_CENTER });
            }

            tableLayout.AddCell(new PdfPCell(new Phrase("Order Confirmation", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.DarkBlue)))) { Colspan = 5, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });
            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 13, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 2, BorderWidthBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });

            // Customer details
            tableLayout.AddCell(new PdfPCell(new Phrase("Date : " + Inquery[0].InqDt, new Font(Font.FontFamily.HELVETICA, 9, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 6, HorizontalAlignment = Element.ALIGN_LEFT });
            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 9, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 3, Border = 0, PaddingBottom = 6, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("Respected ,", new Font(Font.FontFamily.HELVETICA, 9, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 3, HorizontalAlignment = Element.ALIGN_LEFT });
            tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].AccNm, new Font(Font.FontFamily.HELVETICA, 9, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_LEFT });
            tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].AccAdd1, new Font(Font.FontFamily.HELVETICA, 9, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("Mobile : ", new Font(Font.FontFamily.HELVETICA, 9, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 1, Border = 0, PaddingBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT });
            tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].AccMob, new Font(Font.FontFamily.HELVETICA, 9, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 4, Border = 0, PaddingBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("Email : ", new Font(Font.FontFamily.HELVETICA, 9, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 1, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_LEFT });
            tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].AccEmail, new Font(Font.FontFamily.HELVETICA, 9, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 4, Border = 0, PaddingBottom = 9, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("Function : ", new Font(Font.FontFamily.HELVETICA, 9, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 1, Border = 0, PaddingBottom = 9, HorizontalAlignment = Element.ALIGN_LEFT });
            tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].InqTitle, new Font(Font.FontFamily.HELVETICA, 9, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 4, Border = 0, PaddingBottom = 9, HorizontalAlignment = Element.ALIGN_LEFT });



            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 11, 1, BaseColor.BLACK))) { Colspan = 1, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
            tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].Heading, new Font(Font.FontFamily.HELVETICA, 10, 1, BaseColor.BLACK))) { Colspan = 3, Border = 0, BorderWidthBottom = 1, Padding = 1, BorderColor = BaseColor.BLACK, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 11, 1, BaseColor.BLACK))) { Colspan = 1, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });




            tableLayout.AddCell(new PdfPCell(new Phrase("          ", new Font(Font.FontFamily.HELVETICA, 13, 1, BaseColor.BLACK))) { Colspan = 5, Border = 0, Top = 10, HorizontalAlignment = Element.ALIGN_CENTER });



            //var events =Inquery.
            foreach (var item in uniqu.ToList())
            {
                Inquery1 = (from d in Inquery
                            where d.groupbyid == item
                            select d).ToList();

                AddCellToBodyNobold(tableLayout, "Function :");
                AddCellToBody(tableLayout, Inquery1[0].IntEvnNm);
                AddCellToBody(tableLayout, "");
                AddCellToBody(tableLayout, "");
                AddCellToBody(tableLayout, "");


                AddCellToBodyNobold(tableLayout, "Date :");
                AddCellToBody(tableLayout, Inquery1[0].IntOccDt + "  " + Inquery1[0].Todate);
                AddCellToBody(tableLayout, "");
                AddCellToBody(tableLayout, "");
                AddCellToBody(tableLayout, "");

                AddCellToBodyNobold(tableLayout, "Time :");
                AddCellToBody(tableLayout, Inquery1[0].IntFrTm + "  " + Inquery1[0].IntToTm);
                AddCellToBody(tableLayout, "");
                AddCellToBody(tableLayout, "");
                AddCellToBody(tableLayout, "");

                AddCellToBodyNobold(tableLayout, "Venue :");
                AddCellToBody(tableLayout, Inquery1[0].IntVenNm);
                AddCellToBody(tableLayout, "");
                AddCellToBody(tableLayout, "");
                AddCellToBody(tableLayout, "");

                AddCellToBodyNobold(tableLayout, "Customer Remarks :");
                AddCellToBody4(tableLayout, Inquery1[0].customerremarks);

                decimal netamount = 0;
                int a = Inquery1.Count;
                int b = 0;
                foreach (var item1 in Inquery1)
                {
                    b = b + 1;

                    AddCellToBody2(tableLayout, item1.JobNm);
                    if (a == b)
                    {
                        AddCellToBodyBotoomborder(tableLayout, "Qty :" + item1.IntQty);
                        AddCellToBodyBotoomborder(tableLayout, "Rate :" + Convert.ToDecimal(item1.IntRt).ToString("N2"));
                        AddCellToBodyBotoomborder(tableLayout, Convert.ToDecimal(item1.DiscountAmont).ToString("N2"));
                    }
                    else
                    {
                        AddCellToBody(tableLayout, "Qty :" + item1.IntQty);
                        AddCellToBody(tableLayout, "Rate :" + item1.IntRt);
                        AddCellToBody(tableLayout, Convert.ToDecimal(item1.DiscountAmont).ToString("N2"));
                    }
                    AddCellToBody(tableLayout, "");
                    AddCellToBody(tableLayout, "");
                    AddCellToBody(tableLayout, "");
                    if (item1.disamt > 0)
                    {
                        AddCellToBody3(tableLayout, "Discount ( " + item1.disper.ToString() + " %)");
                        AddCellToBody3(tableLayout, Convert.ToDecimal(item1.disamt).ToString("N2"));
                    }
                    else
                    {
                        AddCellToBody3(tableLayout, "");
                        AddCellToBody3(tableLayout, "");
                    }

                    netamount = netamount + item1.DiscountAmont;
                }


                AddCellToBodyBotoomborder(tableLayout, "");
                AddCellToBodyBotoomborder(tableLayout, "");
                AddCellToBodyBotoomborder(tableLayout, "");
                AddCellToBodyBotoomborder(tableLayout, "Subtotal :");
                AddCellToBodyBotoomborder(tableLayout, Convert.ToDecimal(netamount).ToString("N2"));


            }
            if (Inquery[0].Avobedis > 0)
            {
                AddCellToBodyBotoomborder(tableLayout, "");
                AddCellToBodyBotoomborder(tableLayout, "");
                AddCellToBodyBotoomborder(tableLayout, "");
                AddCellToBodyBotoomborder(tableLayout, "Discount :");
                AddCellToBodyBotoomborder(tableLayout, Convert.ToDecimal(Inquery[0].Avobedis).ToString("N2"));

            }
            AddCellToBodyBotoomborder(tableLayout, "");
            AddCellToBodyBotoomborder(tableLayout, "");
            AddCellToBodyBotoomborder(tableLayout, "");
            AddCellToBodyBotoomborder(tableLayout, "Total :");
            AddCellToBodyBotoomborder(tableLayout, (Convert.ToDecimal(Inquery[0].TotalAmount)- Convert.ToDecimal(Inquery[0].Avobedis)).ToString("N2"));
            if (Inquery[0].Inclusive.Count > 0)
            {

                tableLayout.AddCell(new PdfPCell(new Phrase("All Below Packages Inclusive of", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 3, PaddingTop = 3, HorizontalAlignment = Element.ALIGN_LEFT });
            }

            foreach (var item in Inquery[0].Inclusive)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("* " + item.IncTncDesc, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT });


            }
            if (Inquery[0].exclusive.Count > 0)
            {

                tableLayout.AddCell(new PdfPCell(new Phrase("All Below Packages Exclusive of", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 3, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });
            }
            foreach (var item in Inquery[0].exclusive)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("* " + item.IExTncDesc, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT });


            }
            if (Inquery[0].ExtraItem.Count > 0)
            {

                tableLayout.AddCell(new PdfPCell(new Phrase("Extra Item", new Font(Font.FontFamily.HELVETICA, 14, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 3, PaddingBottom = 2, Top = 2, HorizontalAlignment = Element.ALIGN_LEFT });
            }
            foreach (var item in Inquery[0].ExtraItem)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase(item.IteEitNm, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

                tableLayout.AddCell(new PdfPCell(new Phrase("Rs : " + Convert.ToDecimal(item.IteEitAmt).ToString("N2"), new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

                tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 1, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });


            }
            if (Inquery[0].tandc.Count > 0)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("Notes ", new Font(Font.FontFamily.HELVETICA, 14, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 3, PaddingBottom = 2, Top = 2, HorizontalAlignment = Element.ALIGN_LEFT });
            }

            foreach (var item in Inquery[0].tandc)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase(item.item, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

                tableLayout.AddCell(new PdfPCell(new Phrase(item.desc, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 3, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });




            }
            //tableLayout.AddCell(new PdfPCell(new Phrase("Customer Remarks :", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 10, Border = 0, PaddingTop = 15, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_LEFT });
            //tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].customerremarks, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 10, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });



            tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].InqFooter, new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 10, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_LEFT });
            tableLayout.AddCell(new PdfPCell(new Phrase("Mobile-" + Inquery[0].FooterMobile, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 3, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });



            return tableLayout;
        }

        // Method to add single cell to the header
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(0, 51, 102) });
        }
        private static void AddCellToBody3(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0, PaddingBottom = 0, Padding = 2, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        }
        // Method to add single cell to the body
        private static void AddCellToBodyblanck(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 0, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 0, Padding = 2, PaddingBottom = 0, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        }
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 0, PaddingBottom = 0, Padding = 2, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        }

        private static void AddCellToBody2(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 0, Colspan = 2, PaddingBottom = 0, Padding = 2, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        }
        private static void AddCellToBodyNobold(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 0, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 0, PaddingBottom = 0, Padding = 2, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        }
        private static void AddCellToBodyBotoomborder(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = 0,
                BorderWidthBottom = 1,
                PaddingBottom = 1,
                Padding = 2,
                BackgroundColor = iTextSharp.text.BaseColor.WHITE
            });
        }
        private static void AddCellToBody4(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 0, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Colspan = 4, Border = 0, PaddingBottom = 0, Padding = 2, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
            //BaseFont baseFont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            //Font textFont = new Font(baseFont, 9, Font.NORMAL, BaseColor.BLACK);

            //tableLayout.AddCell(new PdfPCell(new Phrase(cellText, textFont))
            //{
            //    Colspan = 10,
            //    Border = 0,
            //    PaddingTop = 15,
            //    PaddingBottom = 5,
            //    HorizontalAlignment = Element.ALIGN_LEFT
            //});
        }
        private static void AddCellToBodyDottedBottomBorder(PdfPTable tableLayout, string cellText)
        {
            PdfPCell cell = new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = Rectangle.BOTTOM_BORDER,
                BorderWidthBottom = -1f, // Set a negative value to create a dotted line
                PaddingBottom = 1,
                Padding = 2,
                BackgroundColor = iTextSharp.text.BaseColor.WHITE
            };

            tableLayout.AddCell(cell);
        }

        [HttpPost]
        public JsonResult SendMail(ReportFileInfo dt)
        {
            // Sender's Gmail credentials
            string senderEmail = "email2pioneer@gmail.com";
            string senderPassword = "bswslprzydbebgsq";

            // Recipient's email address
            string recipientEmail = dt.Sendto;
            // Creating the MailMessage object
            MailMessage mail = new MailMessage(senderEmail, recipientEmail);
            mail.Subject = dt.Title;
            mail.Body = "Please find attachment inquiry/quotation details bellow";
            mail.IsBodyHtml = true;
            string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports");
            string fullpath = Path.Combine(folderPath, dt.FileName);
            Attachment at = new Attachment(fullpath);
            mail.Attachments.Add(at);
            // Creating the SmtpClient object
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtpClient.EnableSsl = true;

            try
            {
                // Sending the email
                smtpClient.Send(mail);

            }
            catch (Exception ex)
            {

                return Json(new { success = true, result = "Fail" });
            }
            return Json(new { success = true, result = "Send" });

        }

        [HttpGet]
        public JsonResult GetOrderbyIndNo(int Id)
        {


            List<OrderModel> models = new List<OrderModel>();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@flag", 2);
            sqlParameters[1] = new SqlParameter("@Id", 0);
            DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_GetOrderMs", sqlParameters);
            if (DtEmp != null && DtEmp.Rows.Count > 0)
            {
                for (int i = 0; i < DtEmp.Rows.Count; i++)
                {
                    OrderModel model = new OrderModel();
                    model.InquiryNo = Convert.ToInt32(DtEmp.Rows[i]["InqVou"].ToString());
                    model.OrderDate = DtEmp.Rows[i]["InqDt"].ToString();
                    model.Remarks = DtEmp.Rows[i]["Status"].ToString();
                    model.NetAmount = Convert.ToDecimal(DtEmp.Rows[i]["NetAmount"].ToString());
                    model.Customer = DtEmp.Rows[i]["AccNm"].ToString();
                    model.OrderTittle = DtEmp.Rows[i]["InqTitle"].ToString();
                    models.Add(model);
                }

            }

            return Json(new { success = true, result = models });

        }
        [HttpPost]
        public JsonResult Savelocation(LocationOrderModel locationOrder)
        {
            string result = "";
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[12];
                sqlParameters[0] = new SqlParameter("@Id", locationOrder.LocationId);
                sqlParameters[1] = new SqlParameter("@orderId", locationOrder.OrderId);
                sqlParameters[2] = new SqlParameter("@RecorType", locationOrder.RecType);
                sqlParameters[3] = new SqlParameter("@AllPhotos", locationOrder.AllPhotos);
                sqlParameters[4] = new SqlParameter("@Allvieos", locationOrder.Allvieos);
                sqlParameters[5] = new SqlParameter("@VideoLocaton", locationOrder.VideoLocaton);
                sqlParameters[6] = new SqlParameter("@PhotosLocation", locationOrder.PhotosLocation);
                sqlParameters[7] = new SqlParameter("@VideoQty", locationOrder.VideoQty);
                sqlParameters[8] = new SqlParameter("@PhotosQty", locationOrder.PhotosQty);
                sqlParameters[9] = new SqlParameter("@Remarks", locationOrder.Remarks);
                sqlParameters[10] = new SqlParameter("@Fileformatedetails", locationOrder.Fileformatedetails);
                sqlParameters[11] = new SqlParameter("@flag", 1);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_locationdetails", sqlParameters);
                result = "Saved";
            }
            catch (Exception)
            {
                result = "Faild";
            }
            return Json(new { success = true, result = result });


        }
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> SavePersondataAsync(ManpowerEntry obj)
        {
            string result = "";
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@Id", obj.id);
                sqlParameters[1] = new SqlParameter("@orderId", obj.orderId);
                sqlParameters[2] = new SqlParameter("@Person", obj.person);
                sqlParameters[3] = new SqlParameter("@remarks", obj.remarks);
                sqlParameters[4] = new SqlParameter("@flag", obj.flag);

                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("prc_usp_updatemanpower", sqlParameters);

                //SqlParameter[] sqlParameters1 = new SqlParameter[3];
                //sqlParameters[0] = new SqlParameter("@Id", obj.id);
                //sqlParameters[1] = new SqlParameter("@status", "");
                //sqlParameters[2] = new SqlParameter("@flag", 4);
                //DataTable dtlogin = ObjDBConnection.CallStoreProcedure("prc_mob_getPushnoification", sqlParameters);
                //if (dtlogin.Rows.Count > 0)
                //{
                //    await NotificationService.NotifyAsync(dtlogin.Rows[0]["DeviceId"].ToString(), dtlogin.Rows[0]["OrtEvnNm"].ToString(), dtlogin.Rows[0]["OrtVenNm"].ToString());
                //}
                result = "Saved";
            }
            catch (Exception)
            {
                result = "Faild";
            }
            return Json(new { success = true, result = result });

        }
        [HttpGet]
        public JsonResult deletedeletedata(long id, string types)
        {

            SqlParameter[] PrsqlParameters = new SqlParameter[2];
            PrsqlParameters[0] = new SqlParameter("@id", id);
            PrsqlParameters[1] = new SqlParameter("@types", types);
            DataTable PrDtEmp = ObjDBConnection.CallStoreProcedure("Usp_deletedeletedata", PrsqlParameters);

            return Json("Record Deleted");
        }
    }
}

