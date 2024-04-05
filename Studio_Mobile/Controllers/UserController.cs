using Microsoft.AspNetCore.Mvc;
using Soc_Management_Web.Models;
using Studio_Mobile.Classes;
using Studio_Mobile.Common;
using Studio_Mobile.Helper;
using Studio_Mobile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;

namespace Studio_Mobile.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        DbConnections ObjDBConnection = new DbConnections();


        [HttpGet]
        [Route("api/User/Login")]
        public IActionResult login(string mobile,string password,string email,string type,string deviceid,bool isAndroid)
        {

            UserMasterModel user = new UserMasterModel();
            string mobile1 = mobile + "" + email;
            Baseresponseclscsprop? baseresponse = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(mobile1) && !string.IsNullOrWhiteSpace(password))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[6];
                    sqlParameters[0] = new SqlParameter("@mobile", mobile);
                    sqlParameters[1] = new SqlParameter("@UserPass", password);
                    sqlParameters[2] = new SqlParameter("@email", email);
                    sqlParameters[3] = new SqlParameter("@type", type);
                    sqlParameters[4] = new SqlParameter("@deviceid", deviceid);
                    sqlParameters[5] = new SqlParameter("@isAndroid", isAndroid);
                    DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_GetUser", sqlParameters);
                    if (dtlogin.Rows.Count > 0)
                    {
                       

                        user.UserId= dtlogin.Rows[0]["UserId"].ToString();
                        user.Email = dtlogin.Rows[0]["Email"].ToString();
                        user.Password = dtlogin.Rows[0]["Password"].ToString();
                        user.Role = Convert.ToInt32(dtlogin.Rows[0]["UserType"].ToString());
                        user.UserMobNo = dtlogin.Rows[0]["Mobile"].ToString();
                        user.Active = dtlogin.Rows[0]["IsActive"].ToString();
                        user.EmpName = dtlogin.Rows[0]["AccNm"].ToString();
                        user.LastLogin = dtlogin.Rows[0]["LastLogin"].ToString();
                        user.IsPasswordVerified = Convert.ToBoolean(dtlogin.Rows[0]["IsPasswordVerified"].ToString());
                        user.IsLogin = Convert.ToBoolean(dtlogin.Rows[0]["IsLogin"].ToString());
                        baseresponse = Baseresponseclscs.CreateResponse(user, 200, "Success", "Data Loaded", false);

                    
                    }
                    else
                    {      if(email!="")
						{
                            baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", " Please enter registered Email ID", false);
                        }
						else
                        {
                           
                     baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", "Please enter registered mobile number", false);
                        }
                    }
                }
                else
                {
                    if (email != "")
                    {
                        baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", " Please enter registered Email ID", false);
                    }
                    else
                    {

                        baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", "Please enter registered mobile number", false);
                    }

                }
            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }

        [HttpGet]
        [Route("api/User/validatemobileEmail")]
        public IActionResult validatemobileEmail(string mobile, string email,string types,string deviceid,bool isAndroid)
        {
            UserMasterModel user = new UserMasterModel();
            string mobile1 = mobile + "" + email;
            Baseresponseclscsprop? baseresponse = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(mobile1))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@mobile", mobile);
                    sqlParameters[1] = new SqlParameter("@email", email);
                    sqlParameters[2] = new SqlParameter("@Password", email);
                    sqlParameters[3] = new SqlParameter("@types", types);
                    sqlParameters[4] = new SqlParameter("@userId", 0);
                    sqlParameters[5] = new SqlParameter("@deviceid", deviceid);
                    sqlParameters[6] = new SqlParameter("@isAndroid", isAndroid);
                    DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_resetpassword", sqlParameters);
                    if (dtlogin.Rows.Count > 0)
                    {
                        string oTp = "";
                        if(!string.IsNullOrEmpty(mobile))
                        {
                            oTp = SendOtpOnMobile(mobile);
                        }
                        else
                        {
                            oTp = SentOtpOnEmail(email);
                        }
                            user.UserId = dtlogin.Rows[0]["UserId"].ToString();
                        user.otp = Convert.ToInt32(oTp);
                        user.Password= dtlogin.Rows[0]["Password"].ToString();
                        baseresponse = Baseresponseclscs.CreateResponse(user, 200, "Success", "User Found", false);
                        
                       


                    }
                    else
                    {
                        if (email != "")
                        {
                            baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", " Please enter registered Email ID", false);
                        }
                        else
                        {

                            baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", "Please enter registered mobile number", false);
                        }
                    }
                }
                else
                {
                    if (email != "")
                    {
                        baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", " Please enter registered Email ID", false);
                    }
                    else
                    {

                        baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", "Please enter registered mobile number", false);
                    }

                }
            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }

        private string SentOtpOnEmail(string email)
        {
            Random generator = new Random();
            String r = generator.Next(1000, 10000).ToString();
            NotificationService.SendMail("Dear User,Your Forgot Pin Verification code Is : " + r, email);
            return r;
        }

        private string SendOtpOnMobile(string mobile)
        {
            Random generator = new Random();
            String r = generator.Next(1000, 10000).ToString();
            //http://piosys.co.in/SendSMS.aspx?UserName=tpiodemo&Password=tpiodemo&SenderId=COMMSG&Message=Dear Customer, Verification code : 931941 For Studio Registration PS&MobileNo=8866156938&MsgTyp=T&EntityId=1701158046859603415&TemplateId=1707162080497255983

            NotificationService.SendCode(r, "tpiodemo", "191508154589", "Dear User, Verification code : " + r + " For Studio Registration ", "" + mobile + "", "T", "1707162080497255983", "1707162349864516298");
            return r;
        }

        [HttpGet]
        [Route("api/User/resetpassword")]
        public IActionResult forgetPassword(string types, long userId,string password)
        {
            UserMasterModel user = new UserMasterModel();
            Baseresponseclscsprop? baseresponse = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(password))
                {
                    SqlParameter[] sqlParameters = new SqlParameter[7];
                    sqlParameters[0] = new SqlParameter("@mobile", "");
                    sqlParameters[1] = new SqlParameter("@email", "");
                    sqlParameters[2] = new SqlParameter("@Password", password);
                    sqlParameters[3] = new SqlParameter("@types", types);
                    sqlParameters[4] = new SqlParameter("@userId", userId);
                    sqlParameters[5] = new SqlParameter("@deviceid", "");
                    sqlParameters[6] = new SqlParameter("@isAndroid", false);
                    DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_resetpassword", sqlParameters);
                    if (dtlogin.Rows.Count > 0)
                    {
                       
                            user.UserId = dtlogin.Rows[0]["UserId"].ToString();
                            baseresponse = Baseresponseclscs.CreateResponse(user, 200, "Success", "Password eset Sucessfully", false);
                     


                    }
                    else
                    {
                        baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", "Please Enter password", false);
                    }
                }
                else
                {
                    baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", "Please Enter password", false);

                }
            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }

        [HttpGet]
        [Route("api/User/Getnotification")]
        public IActionResult Getnotification(long userId)
        {

            List<Notificationuser> notificationusers = new List<Notificationuser>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {
               
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("@userId", userId);
                    DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_Getnotification", sqlParameters);
                    if (dtlogin.Rows.Count > 0)
                    {
                    for (int i = 0; i < dtlogin.Rows.Count; i++)
                    {
                        Notificationuser nof = new Notificationuser();
                        nof.jobid = Convert.ToInt32(dtlogin.Rows[i]["JobId"].ToString());
                        nof.Id = Convert.ToInt32(dtlogin.Rows[i]["UserId"].ToString());
                        nof.jobName = dtlogin.Rows[i]["JobNm"].ToString();
                        nof.category = dtlogin.Rows[i]["CatNm"].ToString();
                        nof.location = dtlogin.Rows[i]["Locations"].ToString();
                        nof.FromTime = dtlogin.Rows[i]["FromTime"].ToString();
                        nof.ToDate = dtlogin.Rows[i]["ToDate"].ToString();
                        nof.FromDate = dtlogin.Rows[i]["FromDate"].ToString();
                        nof.ToTime = dtlogin.Rows[i]["ToTime"].ToString();
                        nof.Status= dtlogin.Rows[i]["Status"].ToString();
                        notificationusers.Add(nof);
                    }
                   
                    baseresponse = Baseresponseclscs.CreateResponse(notificationusers, 200, "Success", "Data Loaded", false);



                    }
                    else
                    {
                        baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", "Faild", false);
                    }
               
            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }


       
    }
}
