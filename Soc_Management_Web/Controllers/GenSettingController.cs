using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PIOAccount.Classes;
using PIOAccount.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PIOAccount.Controllers
{
    public class GenSettingController : BaseController
    {

        DbConnection ObjDBConnection = new DbConnection();
        TaxMasterHelpers ObjTaxMasterHelpers = new TaxMasterHelpers();
        public IActionResult Index(long id)
        {
            try
            {
                bool isreturn = false;
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                long userId = GetIntSession("UserId");
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }

                GenSettingMasterModel GenSettingModel = new GenSettingMasterModel();

                //int _icompanyID = Convert.ToInt32(companyId.ToString());
                GenSettingModel.SalesList = ObjTaxMasterHelpers.GetSalesAccountDropdown(companyId, 0);
                GenSettingModel.PurchaseList = ObjTaxMasterHelpers.GetSalesAccountDropdown(companyId, 0);
                id = DisplaySetting();

                ////Fetching Data based on Company ID
                if (id > 0)
                {
                    GenSettingModel.GENVou = Convert.ToInt32(id);
                    SqlParameter[] sqlParameters = new SqlParameter[3];
                    sqlParameters[0] = new SqlParameter("@GENVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 1);
                    sqlParameters[2] = new SqlParameter("@GenCmpVou", companyId);
                    DataTable dtAreaDetailCarry = ObjDBConnection.CallStoreProcedure("GetGenSettingDetails", sqlParameters);
                    if (dtAreaDetailCarry != null && dtAreaDetailCarry.Rows.Count > 0)
                    {
                        GenSettingModel.SalAccVou = int.Parse(dtAreaDetailCarry.Rows[0]["GENSalAccVou"].ToString());
                        GenSettingModel.PurAccVou = int.Parse(dtAreaDetailCarry.Rows[0]["GENPurAccVou"].ToString());
                    }
                }

                return View(GenSettingModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult Index(long id, GenSettingMasterModel GenModel)
        {
            try
            {
                bool isreturn = false;
                long companyId = GetIntSession("CompanyId");
                long userId = GetIntSession("UserId");
                INIT(ref isreturn);
                if (isreturn)
                {
                    return RedirectToAction("index", "dashboard");
                }
                id = DisplaySetting();
                SqlParameter[] sqlParameters = new SqlParameter[5];
                //sqlParameters[0] = new SqlParameter("@GENVou", 1); // commented by chirag on 27-09-2022
                sqlParameters[0] = new SqlParameter("@GENVou", id);
                sqlParameters[1] = new SqlParameter("@GENCmpVou", companyId);
                sqlParameters[2] = new SqlParameter("@SalAccVou", GenModel.SalAccVou);
                sqlParameters[3] = new SqlParameter("@PurAccVou", GenModel.PurAccVou);
                sqlParameters[4] = new SqlParameter("@FLG", 1);

                DataTable dtAreaDetail = ObjDBConnection.CallStoreProcedure("GenSetting_Insert", sqlParameters);
                if (dtAreaDetail != null && dtAreaDetail.Rows.Count > 0)
                {
                    int Status = DbConnection.ParseInt32(dtAreaDetail.Rows[0][0].ToString());

                    if (Status.Equals(2))
                    {
                        SetSuccessMessage("Update Successfully");
                    }
                    else
                    {
                        SetSuccessMessage("Inserted Successfully");
                    }

                    return RedirectToAction("index", "GenSetting", new { id = 0 });
                }
                else
                {
                    SetErrorMessage("Please Enter the Value");
                    ViewBag.FocusType = "1";
                    //GenModel.AreMstCtyList = ObjaccountGroupHelpers.GetCityDropdown(companyId);
                    return View(GenModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public long DisplaySetting()
        {
            GenSettingMasterModel GenSettingModel = new GenSettingMasterModel();
            long returnValue =0;
            try
            {
                long companyId = GetIntSession("CompanyId");
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@Flg", 1);
                sqlParameters[1] = new SqlParameter("@GenCmpVou", companyId);
                DataTable dtAreaDetailCarry = ObjDBConnection.CallStoreProcedure("GetGenSettingDetails", sqlParameters);
                if (dtAreaDetailCarry != null && dtAreaDetailCarry.Rows.Count > 0)
                {
                    returnValue = int.Parse(dtAreaDetailCarry.Rows[0]["GENVou"].ToString());
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return returnValue;
        }

        private void INIT(ref bool isReturn)
        {
            #region User Rights
            long userId = GetIntSession("UserId");
            UserFormRightModel userFormRights = new UserFormRightModel();
            string currentURL = GetCurrentURL();
            userFormRights = GetUserRights(userId, currentURL);
            userFormRights.IsList = false;
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

        [Route("/GenSetting/Account-list")]
        public IActionResult AccountList(string q)
        {
            int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
            var accountList = new List<CustomDropDown>();
            accountList.Add(CommonHelpers.GetDefaultValue());
            accountList.AddRange(ObjTaxMasterHelpers.GetSalesAccountDropdown(companyId, 0));

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                accountList = accountList.Where(x => x.Text.ToLower().StartsWith(q.ToLower())).ToList();
            }

            return Json(new { items = CommonHelpers.BindSelect2Model(accountList) });
        }
    }
}
