using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PIOAccount.Classes;
using System.Threading;

namespace PIOAccount.Controllers
{
    public class LoginController : BaseController
    {

        DbConnection ObjDBConnection = new DbConnection();
        public IActionResult Index()
        {
            // SendWhatAppMessage("918200137917","Hello Anand", "E:/04012023095916.pdf");
            return View();
        }

        [HttpPost]
        public IActionResult Index(string txtUsername, string txtPassword)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtUsername) && !string.IsNullOrWhiteSpace(txtPassword))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@UserID", txtUsername);
                    sqlParameters[1] = new SqlParameter("@UserPass", txtPassword);

                    DataSet dtLogin = ObjDBConnection.GetDataSet("LoginPage", sqlParameters);
                    if (dtLogin != null && dtLogin.Tables != null && dtLogin.Tables.Count > 0 && dtLogin.Tables[0].Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(dtLogin.Tables[0].Rows[0][0].ToString());
                        if (status == 0)
                        {
                            ViewBag.Msg = "Please Enter User Name & Password";
                            ViewBag.MsgType = "2";
                            ViewBag.FocusType = "1";
                        }
                        else if (status == 1)
                        {
                            ViewBag.Msg = "Please enter correct username";
                            ViewBag.MsgType = "2";
                            ViewBag.FocusType = "1";
                        }
                        else if (status == 2)
                        {
                            ViewBag.Username = txtUsername;
                            ViewBag.Msg = "Please enter correct password";
                            ViewBag.MsgType = "2";
                            ViewBag.FocusType = "2";
                        }
                        else if (status == 3)
                        {
                            Response.Cookies.Delete("UserId");
                            Response.Cookies.Delete("Username");
                            Response.Cookies.Delete("ClientId");
                            Response.Cookies.Delete("CompanyId");
                            Response.Cookies.Delete("YearId");
                            Response.Cookies.Delete("IsAdministrator");
                            Response.Cookies.Delete("CompanyName");
                            Response.Cookies.Delete("YearName");
                            Response.Cookies.Delete("LoginGUID");
                            Response.Cookies.Delete("Lang");
                            Response.Cookies.Delete("LangData");
                            HttpContext.Session.Clear();

                            Response.Cookies.Append("UserId", dtLogin.Tables[1].Rows[0]["Uservou"].ToString());
                            Response.Cookies.Append("Username", dtLogin.Tables[1].Rows[0]["UserId"].ToString());
                            Response.Cookies.Append("ClientId", dtLogin.Tables[1].Rows[0]["ClientId"].ToString());
                            Response.Cookies.Append("IsAdministrator", dtLogin.Tables[1].Rows[0]["IsAdministrator"].ToString());
                            Response.Cookies.Append("LoginGUID", Guid.NewGuid().ToString());

                            Response.Cookies.Delete("CompanyId");
                            Response.Cookies.Delete("YearId");
                            Response.Cookies.Delete("CompanyName");
                            Response.Cookies.Delete("YearName");

                            var companyDetails = DbConnection.GetCompanyYearList(0, 1);
                            if (companyDetails != null && companyDetails.Count > 0)
                            {
                                Response.Cookies.Append("CompanyId", companyDetails[0].Id.ToString());
                                Response.Cookies.Append("CompanyName", companyDetails[0].Name);

                                var yearList = DbConnection.GetYearList(Convert.ToInt32(companyDetails[0].Id.ToString()));

                                Response.Cookies.Append("YearName", yearList.Where(x => x.Value == yearList[0].Value.ToString()).Select(x => x.Text).FirstOrDefault());
                                Response.Cookies.Append("YearId", yearList[0].Value.ToString()); 

                                Response.Cookies.Append("Lang", companyDetails[0].Lang); 
                                Response.Cookies.Append("LangData", companyDetails[0].LangData);
                            }

                            DataTable dtGenSettings = ObjDBConnection.CallStoreProcedure("GetGenSettings", null);
                            if (dtGenSettings != null && dtGenSettings.Rows.Count > 0)
                            {
                                MailHelper.FromEmail = Convert.ToString(dtGenSettings.Rows[0]["GenEmail"]);
                                MailHelper.Host = Convert.ToString(dtGenSettings.Rows[0]["GenHost"]);
                                MailHelper.Password = Convert.ToString(dtGenSettings.Rows[0]["GenPass"]);
                                MailHelper.Port = Convert.ToInt32(dtGenSettings.Rows[0]["GenSMTP"]);

                                WhatAppHelper.SToken = Convert.ToString(dtGenSettings.Rows[0]["GenTokenId"]);
                                WhatAppHelper.SInstanceID = Convert.ToString(dtGenSettings.Rows[0]["GenInstId"]);
                                WhatAppHelper.SURL = Convert.ToString(dtGenSettings.Rows[0]["GenSURL"]);
                                WhatAppHelper.skrumessage = Convert.ToString(dtGenSettings.Rows[0]["GenSkruAPI"]);
                            }

                           return RedirectToAction("Company", "Login");
                            //return RedirectToAction("index", "dashboard");
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Please Enter Correct User Name & Password";
                        ViewBag.MsgType = "2";
                        ViewBag.FocusType = "1";
                    }
                }
                else
                {
                    ViewBag.Msg = "Please Enter User Name & Password";
                    ViewBag.MsgType = "2";
                    ViewBag.FocusType = "1";

                }
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public IActionResult ForgotPassword(string txtMobilnumber)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtMobilnumber))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("@MobNo", txtMobilnumber);

                    DataTable dtLogin = ObjDBConnection.CallStoreProcedure("ForgotMobile", sqlParameters);
                    if (dtLogin != null && dtLogin.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(dtLogin.Rows[0][0].ToString());
                        if (status == 0)
                        {
                            ViewBag.Msg = "Please Enter Correct Mobile NUmber";
                            ViewBag.MsgType = "2";
                            ViewBag.FocusType = "2";
                        }
                        else if (status == 1)
                        {
                            ViewBag.Msg = "Mobile Successfully";
                            ViewBag.MsgType = "1";
                            ViewBag.FocusType = "1";
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Please Enter Mobile NUmber";
                        ViewBag.MsgType = "2";
                        ViewBag.FocusType = "2";
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("ClientId");
            Response.Cookies.Delete("CompanyId");
            Response.Cookies.Delete("YearId");
            Response.Cookies.Delete("IsAdministrator");
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }

        public IActionResult Company(int companyId, bool isFromDashboard = false)
        {
            try
            {
                if (companyId > 0 )
                {
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-IN");

                    var yearList = DbConnection.GetYearList(companyId);
                    Response.Cookies.Delete("CompanyId");
                    Response.Cookies.Delete("YearId");
                    Response.Cookies.Delete("CompanyName");
                    Response.Cookies.Delete("YearName");
                    Response.Cookies.Append("CompanyId", companyId.ToString());
                    Response.Cookies.Append("YearId", "0");
                    string clientId = HttpContext.Request.Cookies["ClientId"].ToString();
                    int isadministrator = Convert.ToInt32(HttpContext.Request.Cookies["IsAdministrator"].ToString());
                    var companyList = DbConnection.GetCompanyYearList(Convert.ToInt32(clientId), isadministrator);
                    var stdt = Convert.ToDateTime(companyList.Where(x => x.Id == companyId.ToString()).Select(x => x.StartDate).FirstOrDefault()).ToString("yyyy") + " - " + Convert.ToDateTime(companyList.Where(x => x.Id == companyId.ToString()).Select(x => x.EndDate).FirstOrDefault()).ToString("yyyy");
                    Response.Cookies.Append("CompanyName", companyList.Where(x => x.Id == companyId.ToString()).Select(x => x.Name).FirstOrDefault());
                    Response.Cookies.Append("YearName", stdt);
                    //Response.Cookies.Append("ClientId", companyList.Where(x => x.Id == companyId.ToString()).Select(x => x.ClientId).FirstOrDefault());

                    Response.Cookies.Append("ClientId", companyList.Where(x => x.Id == companyId.ToString()).Select(x => x.ClientId).FirstOrDefault());

                    DataTable dtGenSettings = ObjDBConnection.CallStoreProcedure("GetGenSettings", null);
                    if (dtGenSettings != null && dtGenSettings.Rows.Count > 0)
                    {
                        MailHelper.FromEmail = Convert.ToString(dtGenSettings.Rows[0]["GenEmail"]);
                        MailHelper.Host = Convert.ToString(dtGenSettings.Rows[0]["GenHost"]);
                        MailHelper.Password = Convert.ToString(dtGenSettings.Rows[0]["GenPass"]);
                        MailHelper.Port = Convert.ToInt32(dtGenSettings.Rows[0]["GenSMTP"]);

                        WhatAppHelper.SToken = Convert.ToString(dtGenSettings.Rows[0]["GenTokenId"]);
                        WhatAppHelper.SInstanceID = Convert.ToString(dtGenSettings.Rows[0]["GenInstId"]);
                        WhatAppHelper.SURL = Convert.ToString(dtGenSettings.Rows[0]["GenSURL"]);
                        WhatAppHelper.skrumessage = Convert.ToString(dtGenSettings.Rows[0]["GenSkruAPI"]);
                    }

                    return RedirectToAction("index", "dashboard");

                }
                if (!string.IsNullOrWhiteSpace(HttpContext.Request.Cookies["ClientId"].ToString()))
                {
                    string clientId = HttpContext.Request.Cookies["ClientId"].ToString();
                    int isadministrator = Convert.ToInt32(HttpContext.Request.Cookies["IsAdministrator"].ToString());
                    if (isFromDashboard)
                    {
                        clientId = DbConnection.GetClientIdByCompanyId(companyId).ToString();
                        return View(DbConnection.GetCompanyYearList(Convert.ToInt32(clientId), 0));
                    }
                    else
                    {
                        return View(DbConnection.GetCompanyYearList(Convert.ToInt32(clientId), isadministrator));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(new List<SelectListItem>());
        }
    }
}
