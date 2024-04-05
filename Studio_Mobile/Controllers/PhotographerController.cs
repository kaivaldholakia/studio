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
using System.Linq;
using System.Threading.Tasks;

namespace Studio_Mobile.Controllers
{
    [ApiController]
    public class PhotographerController : ControllerBase
    {
        DbConnections ObjDBConnection = new DbConnections();


        [HttpGet]
        [Route("api/Photographer/pastandupcommingschedule")]
        public IActionResult pastandupcommingschedule(long Userid=0)
        {
            List<OrderManpowerDetailsModel> details = new List<OrderManpowerDetailsModel>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {
                if(Userid==0)
                {
                    baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Fild", "Please Provide userid", false);
                }
                else
                {
                    SqlParameter[] sqlParameters = new SqlParameter[1];
                    sqlParameters[0] = new SqlParameter("@Userid", Userid);
                    DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_GetPhotographeallSchedule", sqlParameters);
                    if (dtlogin.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtlogin.Rows.Count; i++)
                        {
                            OrderManpowerDetailsModel model = new OrderManpowerDetailsModel();
                            model.FromDate = dtlogin.Rows[i]["FromDate"].ToString();
                            model.ToDate = dtlogin.Rows[i]["ToDate"].ToString();
                            model.fromTime = dtlogin.Rows[i]["FromTime"].ToString();
                            model.ToTime = dtlogin.Rows[i]["ToTime"].ToString();
                            model.Venue = dtlogin.Rows[i]["Venue"].ToString();
                            model.Person = dtlogin.Rows[i]["Person"].ToString();
                            model.ordSubTittle = dtlogin.Rows[i]["ordSubTittle"].ToString();
                            model.OrdTitle = dtlogin.Rows[i]["OrdTitle"].ToString();
                            model.ordremarks = dtlogin.Rows[i]["OrdRem"].ToString();
                            model.cusmobile = dtlogin.Rows[i]["AccMob"].ToString();
                            model.CustomerName = dtlogin.Rows[i]["CustomerName"].ToString();
                            model.job = dtlogin.Rows[i]["JobNm"].ToString();
                            model.Id = Convert.ToInt32(dtlogin.Rows[i]["Id"].ToString());
                            model.Statusid = Convert.ToInt32(dtlogin.Rows[i]["StatusId"].ToString());
                            model.StatusName = dtlogin.Rows[i]["StatusName"].ToString();
                            details.Add(model);
                        }


                        baseresponse = Baseresponseclscs.CreateResponse(details, 200, "Success", "Data Loaded", false);


                    }
                    else
                    {
                        baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Sucess", "No data found", false);
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
        [Route("api/Photographer/updatestatus")]
        public async Task<IActionResult> updatestatusAsync(long Userid,long statusId, long Id)
        {
            
            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@UserId", Userid);
                sqlParameters[1] = new SqlParameter("@StusId", statusId);
                sqlParameters[2] = new SqlParameter("@Id", Id);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_updateeventStatus", sqlParameters);
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Sucess", "Updated", false);

                SqlParameter[] sqlParameters1 = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@Id", Id);
                sqlParameters[1] = new SqlParameter("@status", statusId);
                sqlParameters[2] = new SqlParameter("@flag", 3);
                DataTable dt = ObjDBConnection.CallStoreProcedure("prc_mob_getPushnoification", sqlParameters);
                if (dtlogin.Rows.Count > 0)
                {
                    await NotificationService.NotifyAsync(dtlogin.Rows[0]["DeviceId"].ToString(), dtlogin.Rows[0]["OrtEvnNm"].ToString(), dtlogin.Rows[0]["OrtVenNm"].ToString());
                }
                    


            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }

        [HttpGet]
        [Route("api/Photographer/savefeedback")]
        public IActionResult savefeedback(long Userid, string remarks, long Id)
        {

            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@UserId", Userid);
                sqlParameters[1] = new SqlParameter("@remarks", remarks);
                sqlParameters[2] = new SqlParameter("@Id", Id);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_savefeedback", sqlParameters);
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Sucess", "Updated", false);


            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }

        [HttpPost]
        [Route("api/Photographer/updateprofile")]
        public IActionResult updateprofile(MobileUsermodel cm)
        {

            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[7];
                sqlParameters[0] = new SqlParameter("@userid", cm.userid);
                sqlParameters[1] = new SqlParameter("@addby", cm.userid);
                sqlParameters[2] = new SqlParameter("@mob", cm.mob);
                sqlParameters[3] = new SqlParameter("@email", cm.email);
                sqlParameters[4] = new SqlParameter("@name", cm.name);
                sqlParameters[5] = new SqlParameter("@userType", cm.userType);
                sqlParameters[6] = new SqlParameter("@FLG", "1");
                DataTable DtCity = ObjDBConnection.CallStoreProcedure("MobileuserMst_Insert", sqlParameters);
                if (DtCity != null && DtCity.Rows.Count > 0)
                {
                    baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Sucess", "Updated", false);
                }
              


            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }

        [HttpGet]
        [Route("api/Photographer/getprofile")]
        public IActionResult getprofile(long userid)
        {
            MobileUsermodel mu = new MobileUsermodel();
        Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[7];
                sqlParameters[0] = new SqlParameter("@userid", userid);
                sqlParameters[1] = new SqlParameter("@addby", userid);
                sqlParameters[2] = new SqlParameter("@mob","");
                sqlParameters[3] = new SqlParameter("@email", "");
                sqlParameters[4] = new SqlParameter("@name","");
                sqlParameters[5] = new SqlParameter("@userType", 0);
                sqlParameters[6] = new SqlParameter("@FLG", "2");
                DataTable DtCity = ObjDBConnection.CallStoreProcedure("MobileuserMst_Insert", sqlParameters);
                if (DtCity != null && DtCity.Rows.Count > 0)
                {
                    
                    mu.email = DtCity.Rows[0]["Email"].ToString();
                    mu.mob = DtCity.Rows[0]["Mobile"].ToString();
                    mu.name = DtCity.Rows[0]["Names"].ToString();
                    mu.deviceId = DtCity.Rows[0]["DeviceId"].ToString();
                    mu.userid = userid;
                    mu.userType = Convert.ToInt32(DtCity.Rows[0]["UserType"].ToString());

                    baseresponse = Baseresponseclscs.CreateResponse(mu, 200, "Sucess", "Loaded", false);
                }
				else
				{
                    baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Sucess", "mo data found", false);
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
