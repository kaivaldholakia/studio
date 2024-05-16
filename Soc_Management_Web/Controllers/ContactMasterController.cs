using Microsoft.AspNetCore.Mvc;
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

namespace Soc_Management_Web.Controllers
{
    public class ContactMasterController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();


        public IActionResult Index(long id, string type = null)
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
                long contactid = id;
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int administrator = 0;
                int clientId = 0;
                ContactModel contactmodel = new ContactModel();
                contactmodel.Code = getrenadomkey();
                if (!string.IsNullOrEmpty(type))
                {
                    contactmodel.IsContact = false;
                }
				else
				{
                    contactmodel.IsContact = true;
                }

                if (id > 0)
                {
                    //contactmodel.societyid = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[2];

                    sqlParameters[0] = new SqlParameter("@flag", 2);
                    sqlParameters[1] = new SqlParameter("@societyid", contactid);
                    //sqlParameters[2] = new SqlParameter("@CmpVou", companyId);
                    DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetSocietyDetails", sqlParameters);
                    if (DtEmp != null && DtEmp.Rows.Count > 0)
                    {
                        contactmodel.Code = Convert.ToInt32(DtEmp.Rows[0]["AccCd"]);
                        contactmodel.Date = DtEmp.Rows[0]["AccDate"].ToString();
                        contactmodel.Category = Convert.ToInt32(DtEmp.Rows[0]["AccCat"]);
                        contactmodel.Name = DtEmp.Rows[0]["AccNm"].ToString();
                        // contactmodel.RefBy = Convert.ToInt32(DtEmp.Rows[0]["AccRef"]);
                        contactmodel.RefBy = DtEmp.Rows[0]["AccountRef"].ToString();
                        contactmodel.Group = Convert.ToInt32(DtEmp.Rows[0]["AccGroup"]);

                        contactmodel.Person1 = DtEmp.Rows[0]["AccCont"].ToString();
                        contactmodel.Person2 = DtEmp.Rows[0]["AccCont2"].ToString();
                        contactmodel.Person3 = DtEmp.Rows[0]["AccCont3"].ToString();
                        contactmodel.Person4 = DtEmp.Rows[0]["AccCont4"].ToString();

                        contactmodel.Mobileno1 = DtEmp.Rows[0]["AccMob"].ToString();
                        contactmodel.Mobileno2 = DtEmp.Rows[0]["AccMob2"].ToString();
                        contactmodel.Mobileno3 = DtEmp.Rows[0]["AccMob3"].ToString();
                        contactmodel.Mobileno4 = DtEmp.Rows[0]["AccMob4"].ToString();

                        contactmodel.Birthdate1 = DtEmp.Rows[0]["AccBirthDt"].ToString();
                        contactmodel.Birthdate2 = DtEmp.Rows[0]["AccBirthDt2"].ToString();
                        contactmodel.Birthdate3 = DtEmp.Rows[0]["AccBirthDt3"].ToString();
                        contactmodel.Birthdate4 = DtEmp.Rows[0]["AccBirthDt4"].ToString();

                        contactmodel.Marrigedate1 = DtEmp.Rows[0]["AccMariageDt"].ToString();
                        contactmodel.Marrigedate2 = DtEmp.Rows[0]["AccMariageDt2"].ToString();
                        //contactmodel.Marrigedate3 = DtEmp.Rows[0]["AccBirthDt3"].ToString();
                        //contactmodel.Marrigedate4 = DtEmp.Rows[0]["AccBirthDt4"].ToString();

                        contactmodel.Phone_R = DtEmp.Rows[0]["AccPhoRes"].ToString();
                        contactmodel.Phone_O = DtEmp.Rows[0]["AccPhoOff"].ToString();

                        contactmodel.Mail_id1 = DtEmp.Rows[0]["AccEmail"].ToString();
                        contactmodel.Mail_id2 = DtEmp.Rows[0]["AccEmail2"].ToString();

                        contactmodel.Address_line1 = DtEmp.Rows[0]["AccAdd1"].ToString();
                        contactmodel.Address_line2 = DtEmp.Rows[0]["AccAdd2"].ToString();
                        contactmodel.Address_line3 = DtEmp.Rows[0]["AccAdd3"].ToString();
                        contactmodel.Area = DtEmp.Rows[0]["AccAreaNm"].ToString();
                        contactmodel.City = Convert.ToInt32(DtEmp.Rows[0]["AccCtyNm"]);
                        contactmodel.Pin_code = Convert.ToInt32(DtEmp.Rows[0]["AccPIN"]);
                        contactmodel.Pan_no = DtEmp.Rows[0]["AccPANNo"].ToString();
                        contactmodel.SocietyId = Convert.ToInt32(DtEmp.Rows[0]["AccVou"].ToString());
                        contactmodel.Code = Convert.ToInt32(DtEmp.Rows[0]["AccCd"].ToString());
                        //contactmodel.BlockCount = DtEmp.Rows[0]["BlockCount"].ToString();
                        //contactmodel.Units = DtEmp.Rows[0]["Units"].ToString();


                        //contactmodel.ClientId = Convert.ToInt32(DtEmp.Rows[0]["ClientId"].ToString());
                    }
                }
                else
                {
                    contactmodel.Code = objProductHelper.GetMaxCode() + 1;
                    contactmodel.Date = System.DateTime.Now.ToString("yyyy-MM-dd");
                }
                //else
                //{
                //    contactmodel.Code = getrenadomkey();
                //}
                contactmodel.lstcategory = objProductHelper.GetCategoryDropdown().OrderBy(x => x.Text).ToList();
                contactmodel.lstGroup = objProductHelper.GetGroupDropdown().OrderBy(x => x.Text).ToList();

                contactmodel.GetCityMasterDropdown1 = objProductHelper.GetCityMasterDropdown1(companyId, administrator)
     .GroupBy(x => x.Text, StringComparer.OrdinalIgnoreCase)  // Use a case-insensitive comparer if needed
     .Select(g => g.First())
     .OrderBy(x => x.Text)
     .ToList();



                var areaList = objProductHelper.GetAreaList()
    .Where(x => !string.IsNullOrWhiteSpace(x.Text))
    .OrderBy(x => x.Text)
     .GroupBy(x => x.Text, StringComparer.OrdinalIgnoreCase)  // Use a case-insensitive comparer if needed
     .Select(g => g.First())
     .OrderBy(x => x.Text)
    .ToList();

                contactmodel.GetStateMasterDropdown = areaList;// objProductHelper.GetAreaList().OrderBy(x => x.Text).ToList();

                var areaList1 = objProductHelper.GetlstRefByDropdown()
.Where(x => !string.IsNullOrWhiteSpace(x.Text))
.OrderBy(x => x.Text)
.ToList();


                contactmodel.lstRefBy = areaList1;

                return View(contactmodel);
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
        public IActionResult Index(long id, ContactModel contactmodel)
        {
            try
            {
                int companyId = 0;
                long userId = GetIntSession("UserId");



                bool isreturn = false;
                INIT(ref isreturn);

                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                int administrator = 0;
                if (!string.IsNullOrWhiteSpace(contactmodel.Name))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[38];
                    sqlParameters[0] = new SqlParameter("@AccVou", contactmodel.SocietyId);
                    sqlParameters[1] = new SqlParameter("@AccCd", contactmodel.Code);                     //getrenadomkey()
                    sqlParameters[2] = new SqlParameter("@AccDate", contactmodel.Date);
                    sqlParameters[3] = new SqlParameter("@AccCatVou", contactmodel.Category);
                    sqlParameters[4] = new SqlParameter("@AcName", contactmodel.Name);

                    sqlParameters[5] = new SqlParameter("@AccRef", 0);
                    sqlParameters[6] = new SqlParameter("@AccGroup", contactmodel.Group);
                    sqlParameters[7] = new SqlParameter("@AccCont", contactmodel.Person1);
                    sqlParameters[8] = new SqlParameter("@AccCont2", contactmodel.Person2);
                    sqlParameters[9] = new SqlParameter("@AccCont3", contactmodel.Person3);
                    sqlParameters[10] = new SqlParameter("@AccCont4", contactmodel.Person4);

                    sqlParameters[11] = new SqlParameter("@AccMob", contactmodel.Mobileno1);
                    sqlParameters[12] = new SqlParameter("@AccMob2", contactmodel.Mobileno2);
                    sqlParameters[13] = new SqlParameter("@AccMob3", contactmodel.Mobileno3);
                    sqlParameters[14] = new SqlParameter("@AccMob4", contactmodel.Mobileno4);

                    sqlParameters[15] = new SqlParameter("@AccBirthDt", contactmodel.Birthdate1);
                    sqlParameters[16] = new SqlParameter("@AccBirthDt2", contactmodel.Birthdate2);
                    sqlParameters[17] = new SqlParameter("@AccBirthDt3", contactmodel.Birthdate3);
                    sqlParameters[18] = new SqlParameter("@AccBirthDt4", contactmodel.Birthdate4);

                    sqlParameters[19] = new SqlParameter("@AccMariageDt", contactmodel.Marrigedate1);
                    sqlParameters[20] = new SqlParameter("@AccMariageDt2", contactmodel.Marrigedate2);
                    sqlParameters[21] = new SqlParameter("@AccMariageDt3", contactmodel.Marrigedate3);
                    sqlParameters[22] = new SqlParameter("@AccMariageDt4", contactmodel.Marrigedate4);

                    sqlParameters[23] = new SqlParameter("@AccPhoRes", contactmodel.Phone_R);
                    sqlParameters[24] = new SqlParameter("@AccPhoOff", contactmodel.Phone_O);

                    sqlParameters[25] = new SqlParameter("@AccEmail", contactmodel.Mail_id1);
                    sqlParameters[26] = new SqlParameter("@AccEmail2", contactmodel.Mail_id2);

                    sqlParameters[27] = new SqlParameter("@AccAdd1", contactmodel.Address_line1);
                    sqlParameters[28] = new SqlParameter("@AccAdd2", contactmodel.Address_line2);
                    sqlParameters[29] = new SqlParameter("@AccAdd3", contactmodel.Address_line3);
                    sqlParameters[30] = new SqlParameter("@AccAreaNm", contactmodel.Area == null ? contactmodel.drpArea : contactmodel.Area);
                    sqlParameters[31] = new SqlParameter("@AccCtyNm", contactmodel.City);

                    sqlParameters[32] = new SqlParameter("@AccPIN", contactmodel.Pin_code);
                    sqlParameters[33] = new SqlParameter("@AccPANNo", contactmodel.Pan_no);
                    sqlParameters[34] = new SqlParameter("@FLG", "1");
                    sqlParameters[35] = new SqlParameter("@AccCmpCdN", companyId);
                    sqlParameters[36] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[37] = new SqlParameter("@AccountRef", contactmodel.RefBy);
                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("Usp_ContactMst_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (status == -1)
                        {
                            SetErrorMessage("Dulplicate Contact Details");
                            ViewBag.FocusType = "-1";
                            contactmodel.lstcategory = objProductHelper.GetCategoryDropdown().OrderBy(x => x.Text).ToList();
                            contactmodel.lstGroup = objProductHelper.GetGroupDropdown().OrderBy(x => x.Text).ToList();
                            contactmodel.GetCityMasterDropdown1 = objProductHelper.GetCityMasterDropdown1(companyId, administrator).OrderBy(x => x.Text).ToList();
                            contactmodel.lstRefBy = objProductHelper.GetlstRefByDropdown().OrderBy(x => x.Text).ToList();
                            //contactmodel.GetStateMasterDropdown = objProductHelper.GetStateMasterDropdown(companyId, administrator);
                            //contactmodel.GetClientlist = DbConnection.GetClientList(0, 1);
                            //contactmodel.GetClientlist = DbConnection.GetcontactList(0, 1);
                            //contactmodel.StateList = objProductHelper.GetStateMasterDropdown(0, administrator);
                            //contactmodel.CityList = objProductHelper.GetCityMasterDropdown1(0, administrator);
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
                            if (contactmodel.IsContact == false)
                            {
                                return RedirectToAction("index", "InquiryTransaction", new { type = "contact" });
                            }
                            return RedirectToAction("index", "ContactMaster", new { id = 0 });
                        }

                    }
                    else
                    {
                        SetErrorMessage("Please Enter the Value");
                        ViewBag.FocusType = "-1";
                        //contactmodel.GetClientlist = DbConnection.GetClientList(0, 1);
                        contactmodel.GetClientlist = DbConnection.GetcontactList(0, 1);
                        //contactmodel.StateList = objProductHelper.GetStateMasterDropdown(0, administrator);
                        //contactmodel.CityList = objProductHelper.GetCityMasterDropdown1(0, administrator);
                    }
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "-1";
                    //contactmodel.GetClientlist = DbConnection.GetClientList(0, 1);
                    contactmodel.GetClientlist = DbConnection.GetcontactList(0, 1);
                    //contactmodel.StateList = objProductHelper.GetStateMasterDropdown(0, administrator);
                    //contactmodel.CityList = objProductHelper.GetCityMasterDropdown1(0, administrator);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new ContactModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                ContactModel cityModel = new ContactModel();
                if (id > 0)
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    SqlParameter[] sqlParameters = new SqlParameter[37];
                    sqlParameters[0] = new SqlParameter("@AccVou", id);
                    sqlParameters[1] = new SqlParameter("@AccCd", 0);
                    sqlParameters[2] = new SqlParameter("@AccDate", "");
                    sqlParameters[3] = new SqlParameter("@AccCatVou", "");
                    sqlParameters[4] = new SqlParameter("@AcName", "");

                    sqlParameters[5] = new SqlParameter("@AccRef", "");
                    sqlParameters[6] = new SqlParameter("@AccGroup", "");
                    sqlParameters[7] = new SqlParameter("@AccCont", "");
                    sqlParameters[8] = new SqlParameter("@AccCont2", "");
                    sqlParameters[9] = new SqlParameter("@AccCont3", "");
                    sqlParameters[10] = new SqlParameter("@AccCont4", "");

                    sqlParameters[11] = new SqlParameter("@AccMob", "");
                    sqlParameters[12] = new SqlParameter("@AccMob2", "");
                    sqlParameters[13] = new SqlParameter("@AccMob3", "");
                    sqlParameters[14] = new SqlParameter("@AccMob4", "");

                    sqlParameters[15] = new SqlParameter("@AccBirthDt", "");
                    sqlParameters[16] = new SqlParameter("@AccBirthDt2", "");
                    sqlParameters[17] = new SqlParameter("@AccBirthDt3", "");
                    sqlParameters[18] = new SqlParameter("@AccBirthDt4", "");

                    sqlParameters[19] = new SqlParameter("@AccMariageDt", "");
                    sqlParameters[20] = new SqlParameter("@AccMariageDt2", "");
                    sqlParameters[21] = new SqlParameter("@AccMariageDt3", "");
                    sqlParameters[22] = new SqlParameter("@AccMariageDt4", "");

                    sqlParameters[23] = new SqlParameter("@AccPhoRes", "");
                    sqlParameters[24] = new SqlParameter("@AccPhoOff", "");

                    sqlParameters[25] = new SqlParameter("@AccEmail", "");
                    sqlParameters[26] = new SqlParameter("@AccEmail2", "");

                    sqlParameters[27] = new SqlParameter("@AccAdd1", "");
                    sqlParameters[28] = new SqlParameter("@AccAdd2", "");
                    sqlParameters[29] = new SqlParameter("@AccAdd3", "");
                    sqlParameters[30] = new SqlParameter("@AccAreaNm", "");
                    sqlParameters[31] = new SqlParameter("@AccCtyNm", "");

                    sqlParameters[32] = new SqlParameter("@AccPIN", 0);
                    sqlParameters[33] = new SqlParameter("@AccPANNo", "");
                    sqlParameters[34] = new SqlParameter("@FLG", "2");
                    sqlParameters[35] = new SqlParameter("@AccCmpCdN", companyId);
                    sqlParameters[36] = new SqlParameter("@UsrVou", userId);

                    DataTable DtCity = ObjDBConnection.CallStoreProcedure("Usp_ContactMst_Insert", sqlParameters);
                    if (DtCity != null && DtCity.Rows.Count > 0)
                    {
                        int @value = DbConnection.ParseInt32(DtCity.Rows[0][0].ToString());
                        if (value == 0)
                        {
                            SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
                        }
                        else
                        {
                            SetSuccessMessage("Contact Deleted Successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "ContactMaster");
        }
        //public IActionResult GetCityByState(long itemId)
        //{
        //    try
        //    {
        //        List<SelectListItem> productMastList = new List<SelectListItem>();

        //        if (itemId > 0)
        //        {
        //            productMastList = DbConnection.GetCityByState(itemId);
        //        }

        //        return Json(new { productMastList = productMastList });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
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
                    string currentURL = "/ContactMaster/Index";
                    userFormRights = GetUserRights(userId, currentURL);
                    if (userFormRights == null)
                    {
                        SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
                    }
                    ViewBag.userRight = userFormRights;
                    #endregion

                    int companyId = Convert.ToInt32(GetIntSession("ClientId"));


                    double startRecord = 0;
                    if (pageIndex > 0)
                    {
                        startRecord = (pageIndex - 1) * pageSize;
                    }
                    getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId: companyId);

                    if (getReportDataModel.IsError)
                    {
                        ViewBag.Query = getReportDataModel.Query;
                        return PartialView("_reportView");
                    }
                    getReportDataModel.pageIndex = pageIndex;
                    getReportDataModel.ControllerName = "ContactMaster";
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
                getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, 0, "", 0, 1);
                getReportDataModel = getReportDataModel = GetReportData(gridMstId, 0, 0, "", "", searchValue, companyId, 0, YearId, "", 0, 1);
                if (type == 1)
                {
                    var bytes = Excel(getReportDataModel, "Contact Report", companyDetails.CmpName);
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

        public static int getrenadomkey()
        {
            Random rnd = new Random();
            string randomNumber = (rnd.Next(100000, 999999)).ToString();
            return Convert.ToInt32(randomNumber);
        }
    }
}

