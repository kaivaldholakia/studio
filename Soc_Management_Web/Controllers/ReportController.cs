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
using System.IO;
using System.Linq;

namespace Soc_Management_Web.Controllers
{
    public class ReportController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers objProductHelper = new ProductHelpers();
        private readonly IWebHostEnvironment _hostingEnvironment;
        MasterDropdownHelper master = new MasterDropdownHelper();
        public ReportController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(long id)
        {
            ReportmodelParameter model = new ReportmodelParameter();
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


            model.lstperson = objProductHelper.GetCustomerList();
            model.lstcusomer = objProductHelper.GetCustomerListOnly();
            model.lslCategory = objProductHelper.GetCategorylist();
            model.lstpendingsatus = objProductHelper.Pending();
            model.lstsortorder = objProductHelper.OrderBySchedul();
            model.lsteventstittle = objProductHelper.GetEventLst();
            model.inqstatuslst = objProductHelper.GetInqStatusList();
            model.lstPending = objProductHelper.Pending();
            model.lstjob = master.GetDropgen("Job");
            model.lststatus = master.GetDropgen("Status");
            model.lstoccassiontittle = master.GetDropgen("OrdTiitle");
            model.lstjob = master.GetDropgen("Job");
            model.lstlayout = master.GetReportType("Schedulle");
           

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

        public PartialViewResult ScheduleEventReport(ReportmodelParameterpost reportmodel)
        {

            Document doc = new Document();
            string filePath = "";
            ReportFileInfo fileInfo = new ReportFileInfo();
            if (reportmodel.layout == "" || reportmodel.layout == null || reportmodel.layout== "Select")
            {
                reportmodel.layout = "Header";
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                List<scheduleReportModel> Inquery = getReportcontent(reportmodel);


                // Associate the Document with the MemoryStream
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, memoryStream);

                // Add an event handler to add page numbers
                pdfWriter.PageEvent = new PageNumberEventHandler();
                if (Inquery.Count > 0)
                {
                    doc.Open();

                    PdfPTable tableLayout = new PdfPTable(8);

                    tableLayout = Add_Content_To_PDF(tableLayout, Inquery, reportmodel.layout, reportmodel);
                    tableLayout.HeaderRows = 7;
                    doc.Add(tableLayout);

                    // Close the Document to finish the PDF creation
                    doc.Close();

                    // Write the content of the MemoryStream to a file
                    string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports");
                    string fileName = Inquery[0].Reportitle.ToString() + ".pdf";

                    filePath = Path.Combine(folderPath, fileName);
                    fileInfo.FileName = fileName;
                    fileInfo.FilePath = filePath;
                    fileInfo.Id = 0;
                    fileInfo.Title = "Scheduler Register";
                    System.IO.File.WriteAllBytes(filePath, memoryStream.ToArray());
                }
				else
				{
                    fileInfo.Title = "Data not found";

                }
            }

            TempData["fileInfo"] = fileInfo.FileName;

            return PartialView("_report", fileInfo);
        }
        public JsonResult ScheduleReportxcel(ReportmodelParameterpost rm)
        {
            List<scheduleReportModel> Inquery = new List<scheduleReportModel>();

            var sqlParameters = new SqlParameter[12];
            sqlParameters[0] = new SqlParameter("@OccfromDate", rm.occasionfromDate);
            sqlParameters[1] = new SqlParameter("@OccToDate", rm.occasiontoDate);
            sqlParameters[2] = new SqlParameter("@OrdfromNo", rm.orderFromNo);
            sqlParameters[3] = new SqlParameter("@OrdToNo", rm.orderToNo);
            sqlParameters[4] = new SqlParameter("@Customer", rm.customer);
            sqlParameters[5] = new SqlParameter("@EventTitle", rm.eventTittle);
            sqlParameters[6] = new SqlParameter("@OCCTittle", rm.occasionTitle);
            sqlParameters[7] = new SqlParameter("@Job", rm.job);
            sqlParameters[8] = new SqlParameter("@Category", rm.category);
            sqlParameters[9] = new SqlParameter("@Pending", rm.peningstatus);
            sqlParameters[10] = new SqlParameter("@person", rm.person);
            sqlParameters[11] = new SqlParameter("@orderby", rm.sortorder);
            DataTable dt = ObjDBConnection.CallStoreProcedure("GetSchedulerRegisterReport", sqlParameters);
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow item in dt.Rows)
                {
                    scheduleReportModel inq = new scheduleReportModel();

                    inq.Sl = Convert.ToInt32(item["Sl"].ToString());
                    inq.Id = Convert.ToInt32(item["OrtVou"].ToString());
                    inq.Date = item["EventDate"].ToString();
                    inq.Customer = item["AccNm"].ToString();
                    inq.events = item["OrtEvnNm"].ToString();
                    inq.CompanyTitle = item["companyName"].ToString();
                    inq.companyAddres = item["Addrescompany"].ToString();
                    inq.Reportitle = item["REPORTTittle"].ToString();
                    inq.person = item["Person"].ToString();
                    inq.category = item["CatNm"].ToString();
                    inq.fromtime = item["FromTime"].ToString();
                    inq.totime = item["ToTime"].ToString();
                    inq.address = item["Address"].ToString();
                    Inquery.Add(inq);
                }
            }

            return Json(new { success = true, result = Inquery });
        }

        private List<scheduleReportModel> getReportcontent(ReportmodelParameterpost rm)
        {
            List<scheduleReportModel> Inquery = new List<scheduleReportModel>();

            var sqlParameters = new SqlParameter[12];
            sqlParameters[0] = new SqlParameter("@OccfromDate", rm.occasionfromDate);
            sqlParameters[1] = new SqlParameter("@OccToDate", rm.occasiontoDate);
            sqlParameters[2] = new SqlParameter("@OrdfromNo", rm.orderFromNo);
            sqlParameters[3] = new SqlParameter("@OrdToNo", rm.orderToNo);
            sqlParameters[4] = new SqlParameter("@Customer", rm.customer);
            sqlParameters[5] = new SqlParameter("@EventTitle", rm.eventTittle);
            sqlParameters[6] = new SqlParameter("@OCCTittle", rm.occasionTitle);
            sqlParameters[7] = new SqlParameter("@Job", rm.job);
            sqlParameters[8] = new SqlParameter("@Category", rm.category);
            sqlParameters[9] = new SqlParameter("@Pending", rm.peningstatus);
            sqlParameters[10] = new SqlParameter("@person", rm.person);
            sqlParameters[11] = new SqlParameter("@orderby", rm.sortorder);
            DataTable dt = ObjDBConnection.CallStoreProcedure("GetSchedulerRegisterReport", sqlParameters);
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow item in dt.Rows)
                {
                    scheduleReportModel inq = new scheduleReportModel();

                    inq.Sl = Convert.ToInt32(item["Sl"].ToString());
                    inq.Id = Convert.ToInt32(item["OrtVou"].ToString());
                    inq.Date = item["EventDate"].ToString();
                    inq.Customer = item["AccNm"].ToString();
                    inq.events = item["OrtEvnNm"].ToString();
                    inq.CompanyTitle = item["companyName"].ToString();
                    inq.companyAddres = item["Addrescompany"].ToString();
                    inq.Reportitle = item["REPORTTittle"].ToString();
                    inq.person = item["Person"].ToString();
                    inq.category = item["CatNm"].ToString();
                    inq.fromtime = item["FromTime"].ToString();
                    inq.totime = item["ToTime"].ToString();
                    inq.address = item["Address"].ToString();
                    Inquery.Add(inq);
                }
            }


            return Inquery;
        }

        private PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, List<scheduleReportModel> Inquery, string layout, ReportmodelParameterpost reportmodel)
        {

            List<scheduleReportModel> Inquery1 = new List<scheduleReportModel>();


            float[] headers = { 6, 10, 15, 18, 17, 8, 12, 10 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 100;       //Set the PDF File witdh percentage
            tableLayout.AddCell(new PdfPCell(new Phrase(" Date : " + System.DateTime.Now.ToString("dd/MM/yyy"), new Font(Font.FontFamily.HELVETICA, 7, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 8, Border = 0, PaddingTop = -20, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_LEFT });

            if (layout.ToString() != "Header")
            {

                tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 8, Border = 0, PaddingBottom = 35, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 8, Border = 0, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 8, Border = 0, PaddingBottom = 0, HorizontalAlignment = Element.ALIGN_CENTER });

            }
            //Add Title to the PDF file at the top
            if (layout.ToString() == "Header")
            {
                tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].CompanyTitle, new Font(Font.FontFamily.HELVETICA, 17, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 8, Border = 0, PaddingTop = -4, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].companyAddres, new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 8, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });


                tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 22, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 8, Border = 0, BorderWidthBottom = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 22, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Maroon)))) { Colspan = 8, Border = 0, BorderWidthBottom = 0, PaddingBottom = 3, HorizontalAlignment = Element.ALIGN_CENTER });
            }
            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 4, PaddingTop = 2, HorizontalAlignment = Element.ALIGN_CENTER });
            tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].Reportitle, new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 4, Border = 0, BorderWidthBottom = 1, PaddingTop = 2, PaddingBottom = 4, HorizontalAlignment = Element.ALIGN_CENTER });
            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 4, PaddingTop = 2, HorizontalAlignment = Element.ALIGN_CENTER });

            // Customer details
            tableLayout.AddCell(new PdfPCell(new Phrase("Date : " + reportmodel.occasionfromDate + " To " + reportmodel.occasiontoDate, new Font(Font.FontFamily.HELVETICA, 9, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 8, Border = 0, PaddingTop = 3, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_LEFT });

            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");


            AddCellToBodyBold(tableLayout, "Sr No");
            AddCellToBodyBold(tableLayout, "Date");
            AddCellToBodyBold(tableLayout, "Party Name");
            AddCellToBodyBold(tableLayout, "Funcion & Address");
            AddCellToBodyBold(tableLayout, "Event Tittle");
            AddCellToBodyBold(tableLayout, "Time");
            AddCellToBodyBold(tableLayout, "Category");
            AddCellToBodyBold(tableLayout, "Person");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            AddCellToBody2(tableLayout, "");
            IList<long> arr = Inquery.Select(x => x.Id).Distinct().ToList();
            int slno = 1;
            foreach (var item in arr)
            {
               
                Inquery1 = Inquery.Where(x => x.Id == item).ToList();
                int i = 1;
                foreach (var item1 in Inquery1)
                {
                    if (i == 1)
                    {
                        AddCellToBodyleft(tableLayout, slno.ToString());
                        AddCellToBodyleft(tableLayout, item1.Date);
                        AddCellToBodyleftbold(tableLayout, item1.Customer);
                        AddCellToBodyleft(tableLayout, item1.events + "\n" + item1.address);
                        AddCellToBodyleftbold(tableLayout, item1.events);
                        AddCellToBodyleft(tableLayout, item1.fromtime + "\n" + item1.totime);
                        AddCellToBodyleft(tableLayout, item1.category);
                        AddCellToBodyleft1(tableLayout, item1.person);
                        slno = slno + 1;
                    }
                    else
                    {
                        AddCellToBodyleft(tableLayout, "");
                        AddCellToBodyleft(tableLayout, "");
                        AddCellToBodyleft(tableLayout, "");
                        AddCellToBodyleft(tableLayout, "");
                        AddCellToBodyleft(tableLayout, "");
                        AddCellToBodyleft(tableLayout, "");
                        AddCellToBodyleft(tableLayout, item1.category);
                        AddCellToBodyleft1(tableLayout, item1.person);
                    }

                    i = 1 + 1;
                }
            }

            //var events =Inquery.




            return tableLayout;
        }

        // Method to add single cell to the header
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(0, 51, 102) });
        }


        private static void AddCellToBodyBold(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 0, PaddingBottom = 1, Padding = 4, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        }
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 0, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 1, PaddingBottom = 1, Padding = 3, BackgroundColor = iTextSharp.text.BaseColor.WHITE }); ;
        }
        private static void AddCellToBodyleft(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 0, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 0, BorderWidthRight = 1, PaddingBottom = 1, Padding = 3, BackgroundColor = iTextSharp.text.BaseColor.WHITE }); ;
        }
        private static void AddCellToBodyleft1(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 0, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 0, BorderWidthRight = 0, PaddingBottom = 1, Padding = 3, BackgroundColor = iTextSharp.text.BaseColor.WHITE }); ;
        }
        private static void AddCellToBodyleftbold(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 0, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 0, BorderWidthRight = 1, PaddingBottom = 1, Padding = 3, BackgroundColor = iTextSharp.text.BaseColor.WHITE }); ;
        }
        private static void AddCellToBody2(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Border = 0,BorderWidthBottom=1,BorderWidthTop=0, Colspan = 8, PaddingBottom = 0, Padding = 2, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
        }



        public PartialViewResult ScheduleEventReporttest()
        {

            Document doc = new Document();
            string filePath = "";
            ReportFileInfo fileInfo = new ReportFileInfo();
           
            using (MemoryStream memoryStream = new MemoryStream())
            {


                // Associate the Document with the MemoryStream
                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, memoryStream);

                
                    doc.Open();

                    PdfPTable tableLayout = new PdfPTable(1);

                    tableLayout = Add_Content_To_PDF(tableLayout);
                    tableLayout.HeaderRows = 5;
                    doc.Add(tableLayout);

                    // Close the Document to finish the PDF creation
                    doc.Close();

                    // Write the content of the MemoryStream to a file
                    string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports");
                    string fileName = "test.pdf";

                    filePath = Path.Combine(folderPath, fileName);
                    fileInfo.FileName = fileName;
                    fileInfo.FilePath = filePath;
                    fileInfo.Id = 0;
                    fileInfo.Title = "test";
                    System.IO.File.WriteAllBytes(filePath, memoryStream.ToArray());
                }
            


            return PartialView("_report", fileInfo);
        }
        private PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {

            BaseFont font = BaseFont.CreateFont(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            // Create a Font object with the specified font and size
            Font gujaratiFont = new Font(font, 15, Font.NORMAL, BaseColor.BLACK);

            // Add the text using the Gujarati font
           

            float[] headers = { 100 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 100;       //Set the PDF File witdh percentage
            tableLayout.AddCell(new PdfPCell(new Phrase("લક્ષ્મીચંદ ,સુભાષચંદ્ર ,અરવિંદ ,ઈન્દુબેન ,ઊર્મિલાબેન  ", gujaratiFont)) { Colspan = 1, Border = 0, PaddingTop = 2, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("ઘનશ્યામભાઈ ,દર્શનભાઈ,રાજેન્દ્રભાઈ,પ્રકાશભાઈ ", gujaratiFont)) { Colspan = 1, Border = 0, PaddingTop = 2, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("નિબંધના ‘શીર્ષક' વિશે સૌપ્રથમ વિચાર થવો જોઈએ. શીર્ષકના આધારે નિબંધલેખનમાં કહ્યા મુદાઓ સમાવવા તેનો ખ્યાલ આવે છે", gujaratiFont)) { Colspan = 1, Border = 0, PaddingTop = 2, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("નિબંધના બધા મુદ્દાઓનું અનુસંધાન તેનું શીર્ષક બની રહેવું જોઈએ. વિષયની બહાર જઈ મુદાઓની ચર્ચા કરવી-એમાં વિષયનું તાદૃશ્ય જળવાઈ શકતું નથી", gujaratiFont)) { Colspan = 1, Border = 0, PaddingTop = 2, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("નિબંધ' શબ્દ સંસ્કૃત ભાષામાં પણ વપરાયેલો છે. અંગ્રેજીમાં નિબંધ માટે 'Essay' શબ્દનો પ્રયોગ થાય છે. 'નિઃ+બંધ' એમ જોડાણ થઈને નિબંધ શબ્દ બન્યો છે. 'નિઃ' એટલે પ્રેપૂરું અને 'બંધ' એટલે બંધાયેલું, ગંઠાયેલું અને રચાયેલું. કોઈ એક વિષય પર મુદાસર અને ક્રમબદ્ધ સૂચવેલી માહિતી આપવી એનું નામ નિબંધ.", gujaratiFont)) { Colspan = 1, Border = 0, PaddingTop = 2, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_LEFT });

            tableLayout.AddCell(new PdfPCell(new Phrase("મુદ્દાને અનુરૂપ અને વિષયને સંગત હોય તેવા અવતરણો, કહેવતો, રૂઢિપ્રયોગો, ગુજરાતી કે અન્ય જાણીતી ભાષાની પંક્તિઓ, સુભાષિતો, વગેરેનો ઉપયોગ નિબંધમાં કરવા જોઈએ, નિબંધના મુદાઓમાં અલગ-અલગ સ્થાને તે મુકાય; એકસાથે બધી જ પંક્તિઓ એક જ મુદામાં ન લખાય તેની સાવધાની રાખવી જોઈએ.", gujaratiFont)) { Colspan = 1, Border = 0, PaddingTop = 2, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_LEFT });

            AddCellToBody(tableLayout, "લક્ષ્મીચંદ ,સુભાષચંદ્ર ,અરવિંદ ,ઈન્દુબેન ,ઊર્મિલાબેન");



            return tableLayout;
        }
    }
}
