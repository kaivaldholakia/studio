using iTextSharp.text;
using iTextSharp.text.pdf;
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
using System.IO;
using System.Linq;

namespace Studio_Mobile.Controllers
{
	public class AdminController : ControllerBase
    {
        DbConnections ObjDBConnection = new DbConnections();
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AdminController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        [Route("api/admin/viewschedulelist")]
        public IActionResult viewschedulelist(long Userid = 0)
        {
            List<Schedulelistord> details = new List<Schedulelistord>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@Userid", Userid);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_GetPhotographeallScheduleList", sqlParameters);
                if (dtlogin.Rows.Count > 0)
                {
                    for (int i = 0; i < dtlogin.Rows.Count; i++)
                    {
                        Schedulelistord model = new Schedulelistord();
                        model.fromtime = dtlogin.Rows[i]["FromTime"].ToString();
                        model.totime = dtlogin.Rows[i]["ToTime"].ToString();
                        model.fromdate = dtlogin.Rows[i]["FromDate"].ToString();
                        model.todate = dtlogin.Rows[i]["ToDate"].ToString();
                        model.Person = dtlogin.Rows[i]["Person"].ToString();
                        model.OrdTitle = dtlogin.Rows[i]["OrdTitle"].ToString();
                        model.events = dtlogin.Rows[i]["OrtEvnNm"].ToString();
                        model.CatNm = dtlogin.Rows[i]["CatNm"].ToString();
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
        [Route("api/admin/orderstatus")]
        public IActionResult orderstatus(long orderid)
        {
            List<OrderModel> details = new List<OrderModel>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@orderid", orderid);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_orderstatus", sqlParameters);
                if (dtlogin.Rows.Count > 0)
                {
                    for (int i = 0; i < dtlogin.Rows.Count; i++)
                    {
                        OrderModel model = new OrderModel();
                        model.OrderId = Convert.ToInt32(dtlogin.Rows[i]["OrdVou"].ToString());
                        model.sl = Convert.ToInt32(dtlogin.Rows[i]["sl"].ToString());
                        model.OrderDate = dtlogin.Rows[i]["OrdDt"].ToString();
                        model.ordAmount = dtlogin.Rows[i]["OrdAmt"].ToString();
                        model.refby = dtlogin.Rows[i]["OrdRefBy"].ToString();
                        model.Remarks = dtlogin.Rows[i]["OrdRem"].ToString();
                        model.OrderTittle = dtlogin.Rows[i]["OrdTitle"].ToString();
                        model.ordSubTitle = dtlogin.Rows[i]["ordSubTittle"].ToString();
                        model.DiscountAmount = Convert.ToDecimal(dtlogin.Rows[i]["DiscounAmount"].ToString());
                        model.NetAmount = Convert.ToDecimal(dtlogin.Rows[i]["NetAmount"].ToString());
                        model.Customer = dtlogin.Rows[i]["AccNm"].ToString();
                        model.MobileNo = dtlogin.Rows[i]["AccMob"].ToString();
                        model.Address = dtlogin.Rows[i]["Address"].ToString();
                        model.OrdStatus = dtlogin.Rows[i]["OrdStatus"].ToString();
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
        [Route("api/admin/person")]
        public IActionResult person(long personid)
        {
            List<Personmodelevent> details = new List<Personmodelevent>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@personid", personid);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_GetPersonevent", sqlParameters);
                if (dtlogin.Rows.Count > 0)
                {
                    for (int i = 0; i < dtlogin.Rows.Count; i++)
                    {
                        Personmodelevent model = new Personmodelevent();
                        model.person = dtlogin.Rows[i]["Person"].ToString();
                        model.mobile = dtlogin.Rows[i]["Mobile"].ToString();
                        model.add1Loc1 = dtlogin.Rows[i]["VenueOneAddOne"].ToString();
                        model.add1Loc2 = dtlogin.Rows[i]["VenueOneAddTo"].ToString();
                        model.add2Loc1 = dtlogin.Rows[i]["VenueToAddOne"].ToString();
                        model.add2Loc2 = dtlogin.Rows[i]["VenueToAddTwo"].ToString();
                        model.fromdate = dtlogin.Rows[i]["FromDate"].ToString();
                        model.todate = dtlogin.Rows[i]["ToDate"].ToString();
                        model.category = dtlogin.Rows[i]["CatNm"].ToString();
                        model.eventName = dtlogin.Rows[i]["OrtEvnNm"].ToString();
                        model.meetperson = dtlogin.Rows[i]["meetperson"].ToString();
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
        [Route("api/admin/quotation")]
        public IActionResult quotation(int id = 0, string Headertype = "")
        {

            Baseresponseclscsprop? baseresponse = null;
            Document doc = new Document();
            string filePath = "";
            ReportFileInfo fileInfo = new ReportFileInfo();


            using (MemoryStream memoryStream = new MemoryStream())
            {
                List<InqueryReportModel> Inquery = getReportcontent(id);
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, memoryStream);
                pdfWriter.PageEvent = new PageNumberEventHandler();
                doc.Open();
                PdfPTable tableLayout = new PdfPTable(5);
                tableLayout = Add_Content_To_PDF(tableLayout, Inquery, Headertype);
                tableLayout.HeaderRows = 7;
                doc.Add(tableLayout);
                doc.Close();
                string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports");
                string fileName = Inquery[0].FileName.ToString() + ".pdf";

                filePath = Path.Combine(folderPath, fileName);
                fileInfo.FileName = fileName;
                fileInfo.FilePath = filePath;
                fileInfo.Id = id;
                fileInfo.Title = Inquery[0].InqTitle;
                fileInfo.Sendto = Inquery[0].AccEmail;
                System.IO.File.WriteAllBytes(filePath, memoryStream.ToArray());
                baseresponse = Baseresponseclscs.CreateResponse(fileInfo, 200, "Faild", "Please try again", false);
            }

            return Ok(baseresponse);
        }

        [HttpGet]
        [Route("api/admin/notification")]
        public IActionResult notification(long userid=0,long flag=0)
        {
            List<notification> details = new List<notification>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {
               
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@Userid", userid);
                sqlParameters[1] = new SqlParameter("@flag", 1);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("usp_mob_getNotification", sqlParameters);
                    if (dtlogin.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtlogin.Rows.Count; i++)
                        {
                            notification model = new notification();
                            model.fromdate = dtlogin.Rows[i]["OrtOccDt"].ToString();
                            model.todate = dtlogin.Rows[i]["Todate"].ToString();
                            model.fromtime = dtlogin.Rows[i]["OrtFrTm"].ToString();
                            model.totime = dtlogin.Rows[i]["OrtToTm"].ToString();
                            model.events = dtlogin.Rows[i]["OrtEvnNm"].ToString();
                            model.person = dtlogin.Rows[i]["AccNm"].ToString();
                            model.id = Convert.ToInt32(dtlogin.Rows[i]["Id"].ToString());
                        model.IsViewByAdmin = Convert.ToInt32(dtlogin.Rows[i]["IsViewByAdmin"].ToString());
                        model.IsViewbyPerson = Convert.ToInt32(dtlogin.Rows[i]["IsViewbyPerson"].ToString());
                        
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
        [Route("api/admin/savenotification")]
        public IActionResult savenotification(long userid = 0, long flag = 0)
        {
            List<notification> details = new List<notification>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@Userid", userid);
                sqlParameters[1] = new SqlParameter("@flag", flag);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("usp_mob_getNotification", sqlParameters);
              
               
                    baseresponse = Baseresponseclscs.CreateResponse(null, 200, "saved", "saved", false);
              




            }
            catch (Exception ex)
            {
                baseresponse = Baseresponseclscs.CreateResponse(null, 200, "Faild", ex.Message, false);
            }

            return Ok(baseresponse);
        }

        [HttpGet]
        [Route("api/admin/attendentnotification")]
        public IActionResult attendentnotification()
        {
            List<OrderManpowerDetailsModel> details = new List<OrderManpowerDetailsModel>();
            Baseresponseclscsprop? baseresponse = null;
            try
            {

                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@Userid", 0);
                DataTable dtlogin = ObjDBConnection.CallStoreProcedure("ProcMob_Getnotifyoncomplete", sqlParameters);
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

        


       
        public List<InqueryReportModel> getReportcontent(long id)
        {
            List<InqueryReportModel> Inquery = new List<InqueryReportModel>();
            List<ExtraItemreport> extitemlst = new List<ExtraItemreport>();
            List<Exclusierepoer> exclusierepoers = new List<Exclusierepoer>();
            List<Inclusivereport> inclusivereports = new List<Inclusivereport>();
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
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    InqueryReportModel inq = new InqueryReportModel();
                    inq.exclusive = exclusierepoers;
                    inq.Inclusive = inclusivereports;
                    inq.ExtraItem = extitemlst;
                    inq.AccEmail = item["AccEmail"].ToString();
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
                    inq.Studio = item["Studio"].ToString();
                    inq.pinstate = item["pinstate"].ToString();
                    inq.mobile = item["mobile"].ToString();
                    inq.phone = item["phone"].ToString();
                    inq.OwnerName = item["OwnerName"].ToString();
                    inq.ownerEmail = item["ownerEmail"].ToString();
                    inq.FileName = item["FileName"].ToString();
                    inq.TotalAmount = Convert.ToDecimal(item["TotalAmountt"].ToString());
                    inq.DiscountAmont = Convert.ToDecimal(item["IntNetAmt"].ToString());
                    inq.InqFooter = item["InqFooter"].ToString();
                    inq.FooterMobile = item["FooterMobile"].ToString();
                    inq.AccMob = item["AccMob"].ToString();
                    Inquery.Add(inq);
                }
            }



            return Inquery;
        }

      
        private PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, List<InqueryReportModel> Inquery, string Headertype)
        {

            List<InqueryReportModel> Inquery1 = new List<InqueryReportModel>();

            var uniqu = Inquery.Select(p => p.IntEvnNm)
                                 .Distinct();
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
                            where d.IntEvnNm == item
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
                    netamount = netamount + item1.DiscountAmont;
                }


                AddCellToBodyBotoomborder(tableLayout, "");
                AddCellToBodyBotoomborder(tableLayout, "");
                AddCellToBodyBotoomborder(tableLayout, "");
                AddCellToBodyBotoomborder(tableLayout, "Subtotal :");
                AddCellToBodyBotoomborder(tableLayout, Convert.ToDecimal(netamount).ToString("N2"));


            }
            AddCellToBodyBotoomborder(tableLayout, "");
            AddCellToBodyBotoomborder(tableLayout, "");
            AddCellToBodyBotoomborder(tableLayout, "");
            AddCellToBodyBotoomborder(tableLayout, "Total :");
            AddCellToBodyBotoomborder(tableLayout, Convert.ToDecimal(Inquery[0].TotalAmount).ToString("N2"));


            tableLayout.AddCell(new PdfPCell(new Phrase("All Below Packages Inclusive of", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 3, PaddingTop = 3, HorizontalAlignment = Element.ALIGN_LEFT });

            foreach (var item in Inquery[0].Inclusive)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("* " + item.IncTncDesc, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT });


            }

            tableLayout.AddCell(new PdfPCell(new Phrase("All Below Packages Exclusive of", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 3, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });
            foreach (var item in Inquery[0].exclusive)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase("* " + item.IExTncDesc, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT });


            }

            tableLayout.AddCell(new PdfPCell(new Phrase("Extra Item", new Font(Font.FontFamily.HELVETICA, 14, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 3, PaddingBottom = 2, Top = 2, HorizontalAlignment = Element.ALIGN_LEFT });
            foreach (var item in Inquery[0].ExtraItem)
            {
                tableLayout.AddCell(new PdfPCell(new Phrase(item.IteEitNm, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

                tableLayout.AddCell(new PdfPCell(new Phrase("Rs : " + Convert.ToDecimal(item.IteEitAmt).ToString("N2"), new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

                tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 1, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });


            }


            if (Inquery[0].InqFooter != "")
            {

                tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].InqFooter, new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 5, HorizontalAlignment = Element.ALIGN_LEFT });
                tableLayout.AddCell(new PdfPCell(new Phrase("Mobile-", new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 1, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

                tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].FooterMobile, new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 4, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });


            }


            return tableLayout;
        }


        // Push Notification
       

        // Method to add single cell to the header
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(0, 51, 102) });
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

    }
}
