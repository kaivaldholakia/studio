using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Soc_Management_Web.Models;
using Studio_Mobile.Classes;
using Studio_Mobile.Common;
using Studio_Mobile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Studio_Mobile.Controllers
{
    [EnableCors("AllowAll")]
    public class WebsiteController : ControllerBase
    {
        DbConnections ObjDBConnection = new DbConnections();
        private readonly IWebHostEnvironment _hostingEnvironment;
        public WebsiteController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        [Route("api/Web/Category")]
        public IActionResult Category(long id = 0)
        {
            List<CategoryModel> details = new List<CategoryModel>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@id", id);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("sp_getcategoryforwebsite", sqlParameters);
                if (dtlogin.Rows.Count > 0)
                {
                    for (int i = 0; i < dtlogin.Rows.Count; i++)
                    {
                        CategoryModel model = new CategoryModel();
                        model.id =Convert.ToInt32(dtlogin.Rows[i]["Id"].ToString());
                        model.categoryName = dtlogin.Rows[i]["CatName"].ToString();
                        model.description = dtlogin.Rows[i]["CatDescription"].ToString();
                        model.addDate = dtlogin.Rows[i]["AddDate"].ToString();
                        model.fileNames = dtlogin.Rows[i]["FileNames"].ToString();
                        model.FilePath = dtlogin.Rows[i]["FilePath"].ToString();
                        details.Add(model);
                    }


                    baseresponse = Baseresponseclscs.CreateResponse(details, 200, "Success", "Data Loaded", false);


                }
                else
                {
                    baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", "Please try again", false);
                }

            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }

        [HttpGet]
        [Route("api/Web/Categorydetails")]
        public IActionResult Categorydetails(long id = 0)
        {
            List<CategorydetailsModel> details = new List<CategorydetailsModel>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@id", id);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("sp_getcategoryforwebsite", sqlParameters);
                if (dtlogin.Rows.Count > 0)
                {
                    for (int i = 0; i < dtlogin.Rows.Count; i++)
                    {
                        CategorydetailsModel model = new CategorydetailsModel();
                        model.id = Convert.ToInt32(dtlogin.Rows[i]["Id"].ToString());
                        model.categoryid = Convert.ToInt32(dtlogin.Rows[i]["CategoryId"].ToString());
                        model.categoryName = dtlogin.Rows[i]["CatName"].ToString();
                        model.tittle = dtlogin.Rows[i]["Tittle"].ToString();
                        model.categorydetails = dtlogin.Rows[i]["Categorydetails"].ToString();
                        model.categoryName = dtlogin.Rows[i]["CatDescription"].ToString();
                        model.addDate = dtlogin.Rows[i]["AddDate"].ToString();
                        model.fileNames = dtlogin.Rows[i]["FileNames"].ToString();
                        model.FilePath = dtlogin.Rows[i]["ImagePath"].ToString();
                        model.videoPath = dtlogin.Rows[i]["VideoPath"].ToString();
                        model.catbanner = dtlogin.Rows[i]["catbanner"].ToString();
                        model.catdescription = dtlogin.Rows[i]["catdescription"].ToString();
                        model.cattittle = dtlogin.Rows[i]["cattittle"].ToString();
                        details.Add(model);
                    }


                    baseresponse = Baseresponseclscs.CreateResponse(details, 200, "Success", "Data Loaded", false);


                }
                else
                {
                    baseresponse = Baseresponseclscs.CreateResponse(details, 200, "Faild", "Please try again", false);
                }

            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }

        [HttpGet]
        [Route("api/Web/comment")]
        public IActionResult comment(Comment cm)
        {
            List<Comment> details = new List<Comment>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@id", cm.id);
                sqlParameters[1] = new SqlParameter("@commect", cm.comment);
                sqlParameters[2] = new SqlParameter("@Type", cm.types);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("sp_Savecomment", sqlParameters);
                if (dtlogin.Rows.Count > 0)
                {
                    for (int i = 0; i < dtlogin.Rows.Count; i++)
                    {
                        Comment model = new Comment();
                        model.id = Convert.ToInt32(dtlogin.Rows[i]["Id"].ToString());
                        model.comment = dtlogin.Rows[i]["Comment"].ToString();
                        model.types = dtlogin.Rows[i]["Types"].ToString();
                        model.adddate = dtlogin.Rows[i]["Adddate"].ToString();
                        details.Add(model);
                    }


                    baseresponse = Baseresponseclscs.CreateResponse(details, 200, "Success", "Data Loaded", false);


                }
                else
                {
                    baseresponse = Baseresponseclscs.CreateResponse(details, 200, "Success", "Data Saved", false);
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
