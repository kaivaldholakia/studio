using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Newtonsoft.Json;
using PIOAccount.Classes;
using PIOAccount.Common;
using PIOAccount.Models;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using PageSize = iTextSharp.text.PageSize;
using Paragraph = iTextSharp.text.Paragraph;
using Rectangle = iTextSharp.text.Rectangle;

namespace PIOAccount.Controllers 
{
    [CookieHelper]
    public class BaseController : Controller
    {
        DbConnection ObjDBConnection = new DbConnection();
        private readonly IWebHostEnvironment _iwebhostenviroment;
        public void SetSuccessMessage(string message)
        {
            TempData["TempMessage"] = message;
            TempData["TempMessageType"] = "success";
        }

        public void SetErrorMessage(string message)
        {
            TempData["TempMessage"] = message;
            TempData["TempMessageType"] = "error";
        }

        public long GetIntSession(string key)
        {
            return HttpContext.Session.GetInt32(key).HasValue ? Convert.ToInt64(HttpContext.Session.GetInt32(key).Value) : 0;
        }

        public string GetStringSession(string key)
        {
            return HttpContext.Session.GetString(key) != null ? Convert.ToString(HttpContext.Session.GetString(key)) : null;
        }

        public JsonResult GetLatestNumberByTableName(string TableName, string Vou)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Vou) && !string.IsNullOrWhiteSpace(TableName))
                {
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    int yearId = Convert.ToInt32(GetIntSession("YearId"));
                    DbConnection dbConnection = new DbConnection();
                    string value = dbConnection.GetLatestVoucherNumber(TableName, Convert.ToInt32(Vou), companyId, yearId);
                    return Json(new { response = true, message = value });
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Json(new { response = true, message = string.Empty });
        }

        public string GetCurrentURL()
        {
            string url = HttpContext.Request.Path;
            try
            {
                string[] splitted = url.Split('/');
                if (splitted != null && splitted.Length > 0 && !string.IsNullOrWhiteSpace(splitted[1]))
                {
                    if (splitted.Length >= 3)
                    {
                        url = string.IsNullOrWhiteSpace(splitted[2]) ? "/" + splitted[1] + "/Index" : "/" + splitted[1] + "/" + splitted[2];
                        return url;
                    }
                    else
                    {
                        url = "/" + splitted[1] + "/Index";
                        return url;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public UserFormRightModel GetUserRights(long userId, string url)
        {

            try
            {
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@UserFK", userId);
                DataSet dsDynamicSidebar = ObjDBConnection.GetDataSet("GetDynamicSidebar", parameters);
                if (dsDynamicSidebar != null && dsDynamicSidebar.Tables != null && dsDynamicSidebar.Tables.Count > 0)
                {
                    DataTable dtDetail = dsDynamicSidebar.Tables[0];

                    if (dtDetail != null && dtDetail.Rows.Count > 0)
                    {
                        DataRow[] drFind = dtDetail.Select("Link='" + url + "'");
                        if (drFind != null && drFind.Length > 0)
                        {
                            UserFormRightModel userFormRights = new UserFormRightModel();
                            userFormRights.ModuleId = drFind[0]["ModuleId"] != null ? Convert.ToInt32(drFind[0]["ModuleId"].ToString()) : 0;
                            userFormRights.ModuleNm = drFind[0]["Name"].ToString();

                            userFormRights.IsAdd = drFind[0]["IsAdd"] != null ? Convert.ToBoolean(drFind[0]["IsAdd"].ToString()) : false;

                            userFormRights.IsDelete = drFind[0]["IsDelete"] != null ? Convert.ToBoolean(drFind[0]["IsDelete"].ToString()) : false;

                            userFormRights.IsList = drFind[0]["IsList"] != null ? Convert.ToBoolean(drFind[0]["IsList"].ToString()) : false;

                            userFormRights.IsEdit = drFind[0]["IsEdit"] != null ? Convert.ToBoolean(drFind[0]["IsEdit"].ToString()) : false;
                            return userFormRights;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        public List<SelectListItem> GetGridLayoutDropDown(int gridType, int moduleId)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@ModuleId", moduleId);
                parameters[1] = new SqlParameter("@TypeId", gridType);
                DataTable dtData = ObjDBConnection.CallStoreProcedure("GetGridLayoutList", parameters);
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    List<SelectListItem> data = new List<SelectListItem>();
                    foreach (DataRow item in dtData.Rows)
                    {
                        data.Add(new SelectListItem
                        {
                            Text = item["GrdName"].ToString(),
                            Value = item["GrdVou"].ToString(),
                            Selected = item["GrdDftYNo"].ToString() == "0" ? false : true
                        });
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public GetReportDataModel GetReportData(int gridMasterId, int startRecord, int pageSize, string columnName, string sortby, string searchText, int companyId = 0, int clientId = 0, int yearId = 0, string pageName = "", int isAdministrator = 0, int isExcelPDF = 0, string whereCondition = "")
        {
            GetReportDataModel returnModel = new GetReportDataModel();
            try
            {
                if (gridMasterId > 0)
                {
                    SqlParameter[] parameters = new SqlParameter[1];
                    parameters[0] = new SqlParameter("@GridVou", gridMasterId);
                    DataSet dsData = ObjDBConnection.GetDataSet("GetGridReport", parameters);
                    if (dsData != null && dsData.Tables != null && dsData.Tables.Count > 0)
                    {
                        DataTable dtGridMst = dsData.Tables[0];
                        DataTable dtGridTrn = dsData.Tables[1];
                        DataTable dtGridShortTrn = dsData.Tables[2];

                        string Query = string.Empty;
                        string WQuery = string.Empty;

                        if (dtGridMst != null && dtGridMst.Rows.Count > 0)
                        {
                            returnModel.ReportType = Convert.ToInt32(dtGridMst.Rows[0]["GrdType"].ToString());
                            returnModel.multiYN = Convert.ToInt32(dtGridMst.Rows[0]["GrdMultiSelYN"].ToString());
                            returnModel.GrdTitle = dtGridMst.Rows[0]["GrdTitle"].ToString();
                            returnModel.DocumentPageSize = DbConnection.ParseInt32(dtGridMst.Rows[0]["PageSize"].ToString());
                            Query = "SELECT * FROM (" + dtGridMst.Rows[0]["GrdQryFields"].ToString() + " " + dtGridMst.Rows[0]["GrdQryJoin"].ToString();
                        }

                        if (!string.IsNullOrWhiteSpace(whereCondition))
                        {
                            string[] groupbySplitter = Query.ToUpper().Split("GROUP BY");
                            if (groupbySplitter != null && groupbySplitter.Length == 2)
                            {
                                WQuery = groupbySplitter[0] + whereCondition + " GROUP BY " + groupbySplitter[1];
                                Query = WQuery + ") MyTable ";
                            }
                            else
                            {
                                WQuery = whereCondition;
                                Query = Query + WQuery + ") MyTable ";
                            }

                        }
                        else
                        {
                            Query = Query + ") MyTable ";
                        }


                        if (!string.IsNullOrWhiteSpace(pageName) && pageName == "Client")
                        {
                            Query = Query.Replace("WHERE CLIVOU = @#CLIVOU#@", " ");
                        }
                        //if (!string.IsNullOrWhiteSpace(pageName) && pageName == "Company")
                        //{
                        //    Query = Query.Replace("WHERE CLIVOU = @#CLIVOU#@", " ");
                        //    //Query = Query.Replace("WHERE CLIENTID = @#CLIVOU#@", " ");
                        //}
                        if (isAdministrator == 1 && !string.IsNullOrWhiteSpace(pageName) && pageName == "User")
                        {
                            Query = Query.Replace("WHERE CLIENTMASTER.CLIVOU = @#CLIVOU#@", " ");
                        }
                        //if (!string.IsNullOrWhiteSpace(pageName) && pageName == "Department")
                        //{
                        //    Query = Query.Replace(" WHERE DEPCMPCDN = @#CMPVOU#@", " ");
                        //}

                        if (!string.IsNullOrWhiteSpace(searchText))
                        {
                            Query += "WHERE (";
                            if (dtGridTrn != null && dtGridTrn.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtGridTrn.Rows.Count; i++)
                                {
                                    Query += dtGridTrn.Rows[i]["GrdADbFld"].ToString() + " like N'%" + searchText + "%' ";
                                    if (i != dtGridTrn.Rows.Count - 1)
                                    {
                                        Query += " OR ";
                                    }
                                }
                            }
                            Query += " )";
                        }

                        if (dtGridShortTrn != null && dtGridShortTrn.Rows.Count > 0)
                        {
                            Query += " Order by ";
                            for (int i = 0; i < dtGridShortTrn.Rows.Count; i++)
                            {
                                Query += dtGridShortTrn.Rows[i]["GrdBDbFld"].ToString() + " asc, ";
                            }
                            Query = Query.Trim().TrimEnd().TrimEnd(',');

                        }
                        else
                        {
                            Query += " " + dtGridMst.Rows[0]["GrdQryOrderBy"].ToString();
                        }
                        parameters = new SqlParameter[1];
                        Query = Query.Replace("@#CMPVOU#@", companyId.ToString());
                        Query = Query.Replace("@#CLIVOU#@", companyId.ToString());
                        Query = Query.Replace("@#YEARVOU#@", yearId.ToString());
                        returnModel.Query = Query;
                        parameters[0] = new SqlParameter("@dynamicQuery", Query);

                        // these lines added by chirag on 10-8-23, bcz gujarati (unicode) text search thati nathi START
                        SqlConnection conn = ObjDBConnection.connection();
                        DataTable dtWholeData = new DataTable();
                        SqlCommand command = new SqlCommand(Query, conn);
                        command.CommandType = CommandType.Text;
                        conn.Open();
                        SqlDataAdapter daMst = new SqlDataAdapter();
                        daMst.SelectCommand = command;
                        daMst.Fill(dtWholeData);
                        conn.Close();
                        conn.Dispose();
                        // these lines added by chirag on 10-8-23, bcz gujarati (unicode) text search thati nathi END

                        //DataTable dtWholeData = ObjDBConnection.CallStoreProcedure("ExecuteDynamicQuery", parameters);
                        if (dtWholeData != null && dtWholeData.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(columnName) && !string.IsNullOrEmpty(sortby))
                            {
                                if (sortby == "1")
                                {
                                    DataView dv = dtWholeData.DefaultView;
                                    dv.Sort = columnName + " DESC";
                                    dtWholeData = dv.ToTable();
                                }
                                if (sortby == "2")
                                {
                                    DataView dv = dtWholeData.DefaultView;
                                    dv.Sort = columnName + " ASC";
                                    dtWholeData = dv.ToTable();
                                }
                            }

                            DataTable dtReport = new DataTable();
                            if (dtGridTrn != null && dtGridTrn.Rows.Count > 0)
                            {
                                if (returnModel.ReportType == DbConnection.GridTypeView)
                                    dtReport.Columns.Add("Id");
                                foreach (DataRow item in dtGridTrn.Rows)
                                {
                                    dtReport.Columns.Add(item["GrdADbFld"].ToString());
                                }

                                for (int i = 0; i < dtWholeData.Rows.Count; i++)
                                {
                                    DataRow drAdd = dtReport.NewRow();
                                    for (int j = 0; j < dtWholeData.Columns.Count; j++)
                                    {
                                        if (drAdd.Table.Columns.Contains(dtWholeData.Columns[j].ColumnName))
                                            drAdd[dtWholeData.Columns[j].ColumnName] = dtWholeData.Rows[i][j].ToString();
                                    }
                                    dtReport.Rows.Add(drAdd);
                                }
                            }
                            if (dtReport != null && dtReport.Rows.Count > 0)
                            {
                                List<ReportColumnConfiguration> columnList = new List<ReportColumnConfiguration>();
                                List<object> rowsList = new List<object>();
                                for (int i = 0; i < dtReport.Columns.Count; i++)
                                {
                                    DataRow[] drCheck = dtGridTrn.Select("GrdADbFld = '" + dtReport.Columns[i].ColumnName + "'");
                                    if (drCheck != null && drCheck.Length > 0)
                                    {
                                        string columnSortBy = "1";
                                        if (!string.IsNullOrWhiteSpace(columnName) && drCheck[0]["GrdADbFld"].ToString() == columnName)
                                        {
                                            if (sortby == "1")
                                            {
                                                columnSortBy = "2";
                                            }
                                            else
                                            {
                                                columnSortBy = "1";
                                            }
                                        }

                                        columnList.Add(new ReportColumnConfiguration
                                        {
                                            DbFldName = drCheck[0]["GrdADbFld"].ToString(),
                                            GrdAAlign = drCheck[0]["GrdAAlign"].ToString(),
                                            GrdADataType = drCheck[0]["GrdADataType"].ToString(),
                                            GrdALinkYN = drCheck[0]["GrdALinkYN"].ToString(),
                                            GrdANewColNm = drCheck[0]["GrdANewColNm"].ToString(),
                                            GrdAPosition = drCheck[0]["GrdAPosition"].ToString(),
                                            GrdASuppressIfVal = drCheck[0]["GrdASuppressIfVal"].ToString(),
                                            GrdATotYN = drCheck[0]["GrdATotYN"].ToString(),
                                            GrdCanGrow = drCheck[0]["CanGrow"].ToString(),
                                            GrdAWidth = drCheck[0]["GrdAWidth"].ToString(),
                                            GrdADecUpTo = drCheck[0]["GrdADecUpTo"].ToString(),
                                            GrdAHideYN = drCheck[0]["GrdAHideYN"].ToString(),
                                            OrderBy = columnSortBy
                                        });
                                    }
                                }
                                int start, conditionStart = 0;
                                int end, conditionEnd = 0;

                                if (isExcelPDF == 0)
                                {
                                    if (startRecord == 1) { startRecord = 0; }
                                    end = startRecord > 0 ? startRecord * pageSize : pageSize;
                                    start = startRecord > 0 ? (startRecord * pageSize) - pageSize : startRecord;
                                    conditionStart = start;
                                    conditionEnd = end;

                                    bool isNewLastPage = false;
                                    if (startRecord > (dtReport.Rows.Count / pageSize))
                                    {
                                        isNewLastPage = true;
                                    }
                                    if (isNewLastPage)
                                    {
                                        start = 0;
                                        end = dtReport.Rows.Count;
                                    }
                                }
                                else
                                {
                                    start = 0;
                                    end = dtReport.Rows.Count;
                                    conditionStart = start;
                                    conditionEnd = end;
                                }
                                int isLast = 0;
                                decimal lastPageIndex = 0;
                                if (isExcelPDF == 0)
                                {
                                    lastPageIndex = Convert.ToDecimal(dtReport.Rows.Count) / Convert.ToDecimal(pageSize);
                                    if (lastPageIndex.ToString().Split('.').Length > 1)
                                        lastPageIndex = Convert.ToDecimal(lastPageIndex.ToString().Split('.')[0]) + 1;
                                    int newStartRecord = startRecord;
                                    if (newStartRecord == 0)
                                        newStartRecord++;
                                    if (newStartRecord == lastPageIndex)
                                        isLast = 1;
                                }
                                else
                                {
                                    isLast = 1;
                                }

                                decimal[] total = new decimal[columnList.Count];
                                for (int i = start; i < end; i++)
                                {
                                    if (i >= dtReport.Rows.Count)
                                    {
                                        break;
                                    }
                                    List<string> row = new List<string>();
                                    for (int j = 0; j < dtReport.Columns.Count; j++)
                                    {
                                        if (returnModel.ReportType == DbConnection.GridTypeView && j > 0 && columnList[j - 1].GrdADataType == "2" && columnList[j - 1].GrdATotYN == "1" && isLast == 1)
                                        {
                                            total[j - 1] = Convert.ToDecimal(total[j - 1]) + (!string.IsNullOrWhiteSpace(dtReport.Rows[i][j].ToString()) ? Convert.ToDecimal(dtReport.Rows[i][j].ToString()) : 0);
                                        }

                                        if (returnModel.ReportType == DbConnection.GridTypeReport && columnList[j].GrdADataType == "2" && columnList[j].GrdATotYN == "1" && isLast == 1)
                                        {
                                            total[j] = Convert.ToDecimal(total[j]) + (!string.IsNullOrWhiteSpace(dtReport.Rows[i][j].ToString()) ? Convert.ToDecimal(dtReport.Rows[i][j].ToString()) : 0);
                                        }

                                        if (returnModel.ReportType == DbConnection.GridTypeView && j > 0 && columnList[j - 1].GrdADataType == "3")
                                        {
                                            row.Add(!string.IsNullOrWhiteSpace(dtReport.Rows[i][j].ToString()) ? DbConnection.GetFormatDateTime(Convert.ToDateTime(dtReport.Rows[i][j].ToString())) : string.Empty);
                                        }
                                        else if (returnModel.ReportType == DbConnection.GridTypeReport && columnList[j].GrdADataType == "3")
                                        {
                                            row.Add(!string.IsNullOrWhiteSpace(dtReport.Rows[i][j].ToString()) ? DbConnection.GetFormatDateTime(Convert.ToDateTime(dtReport.Rows[i][j].ToString())) : string.Empty);
                                        }
                                        else if (returnModel.ReportType == DbConnection.GridTypeView && j > 0 && columnList[j - 1].GrdADataType == "2")
                                        {
                                            row.Add(!string.IsNullOrWhiteSpace(dtReport.Rows[i][j].ToString()) && (dtReport.Rows[i][j].ToString().Equals("0") || dtReport.Rows[i][j].ToString().Equals("0.00")) ? string.Empty : dtReport.Rows[i][j].ToString());
                                        }
                                        else if (returnModel.ReportType == DbConnection.GridTypeReport && columnList[j].GrdADataType == "2")
                                        {
                                            row.Add(!string.IsNullOrWhiteSpace(dtReport.Rows[i][j].ToString()) && (dtReport.Rows[i][j].ToString().Equals("0") || dtReport.Rows[i][j].ToString().Equals("0.00")) ? string.Empty : dtReport.Rows[i][j].ToString());
                                        }
                                        else
                                        {
                                            row.Add(dtReport.Rows[i][j].ToString());
                                        }
                                    }

                                    if (i >= conditionStart && i < conditionEnd)
                                    {
                                        rowsList.Add(row);
                                    }
                                }

                                if (isLast == 1)
                                {
                                    if (columnList.Where(x => x.GrdATotYN == "1").Count() > 0)
                                    {
                                        List<string> row = new List<string>();
                                        bool isTotalAdded = false;
                                        for (int j = 0; j < columnList.Count; j++)
                                        {
                                            if (columnList[j].GrdATotYN == "1")
                                            {
                                                if (!isTotalAdded)
                                                {
                                                    if (returnModel.ReportType == DbConnection.GridTypeView)
                                                        row.Add("Total : ");
                                                    else
                                                        row[j - 1] = "Total : ";
                                                }
                                                if (total[j] > 0)
                                                    row.Add(DbConnection.DynamicDecimalPoints(total[j].ToString(), Convert.ToInt32(columnList[j].GrdADecUpTo)));
                                                else
                                                    row.Add(string.Empty);
                                                isTotalAdded = true;
                                            }
                                            else
                                            {
                                                row.Add(string.Empty);
                                            }
                                        }
                                        rowsList.Add(row);
                                    }
                                }
                                if (columnList.Where(x => x.GrdATotYN == "1").Count() > 0)
                                {
                                    for (int j = 0; j < columnList.Count; j++)
                                    {
                                        if (columnList[j].GrdATotYN == "1")
                                        {
                                            total[j] = dtReport.AsEnumerable().Sum(x => !string.IsNullOrWhiteSpace(x[columnList[j].DbFldName].ToString()) ? Convert.ToDecimal(x[columnList[j].DbFldName]) : 0);
                                            if (total[j] > 0)
                                                if (isExcelPDF == 0)
                                                    columnList[j].GrdANewColNm += "@@@ (" + DbConnection.DynamicDecimalPoints(total[j].ToString(), Convert.ToInt32(columnList[j].GrdADecUpTo)).ToString() + ")";
                                                else
                                                    columnList[j].GrdANewColNm += "(" + DbConnection.DynamicDecimalPoints(total[j].ToString(), Convert.ToInt32(columnList[j].GrdADecUpTo)).ToString() + ")";
                                        }
                                    }
                                }

                                returnModel.totalRecord = dtReport.Rows.Count;
                                returnModel.ColumnsData = columnList;
                                returnModel.RowsData = rowsList;
                                returnModel.pageSize = pageSize;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                returnModel.IsError = true;
                return returnModel;
            }
            return returnModel;
        }

        public byte[] Excel(GetReportDataModel getReportDataModel, string header, string companyName, string cmpaddress = "")
        {
            try
            {
                string[] columnNames = new string[getReportDataModel.ColumnsData.Count];
                DataTable dataTableData = new DataTable();
                for (int i = 0; i < getReportDataModel.ColumnsData.Count; i++)
                {
                    columnNames[i] = getReportDataModel.ColumnsData[i].GrdANewColNm;
                    dataTableData.Columns.Add(getReportDataModel.ColumnsData[i].GrdANewColNm);
                }
                DataTable dataTableTotal = new DataTable();
                if (getReportDataModel != null && getReportDataModel.RowsData != null)
                {
                    for (int x = 0; x < getReportDataModel.RowsData.Count; x++)
                    {
                        DataRow drAdd = dataTableData.NewRow();
                        var item = getReportDataModel.RowsData[x] as List<string>;
                        int start = 0;
                        if (getReportDataModel.ReportType == 1)
                            start = 1;
                        for (int y = start; y < item.Count; y++)
                        {
                            if (getReportDataModel.ReportType == 1)
                                drAdd[y - 1] = item[y];
                            else
                                drAdd[y] = item[y];
                        }
                        dataTableData.Rows.Add(drAdd);
                    }
                }

                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(header);
                    var currentRow = 1;

                    #region Comment Code
                    worksheet.Range("A" + currentRow + ":" + alpha[columnNames.Length - 1].ToString() + "1")
                                .Row(1)
                                .SetValue(companyName).Merge()
                                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Font.SetBold().Font.SetFontSize(16);

                    currentRow++;

                    worksheet.Range("A" + currentRow + ":" + alpha[columnNames.Length - 1].ToString() + "1")
                                .Row(1)
                                .SetValue(cmpaddress).Merge()
                                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Font.SetFontSize(11);

                    currentRow++;

                    worksheet.Range("A" + currentRow + ":" + alpha[columnNames.Length - 1].ToString() + "1")
                             .Row(1)
                             .SetValue(header).Merge()
                             .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                             .Font.SetBold().Font.SetFontSize(12);

                    #endregion

                    worksheet.Rows().AdjustToContents();
                    worksheet.Columns().AdjustToContents();
                    if (dataTableData != null && dataTableData.Columns.Count > 0)
                    {
                        currentRow++;

                        int columnIndex = 0;
                        int columnIndexJustigy = 1;
                        foreach (var columnName in getReportDataModel.ColumnsData)
                        {
                            if (columnName.GrdAHideYN == "0")
                            {
                                columnIndex++;
                                worksheet.Cell(currentRow, columnIndex).SetValue(columnName.GrdANewColNm).Style.Font.SetBold().Font.SetFontSize(11);
                                if (columnName.GrdAAlign == "1")
                                {
                                    worksheet.Cell(currentRow, columnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                }
                                else if (columnName.GrdAAlign == "2")
                                {
                                    worksheet.Cell(currentRow, columnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                }
                                else if (columnName.GrdAAlign == "3")
                                {
                                    worksheet.Cell(currentRow, columnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                }
                                //worksheet.Column(columnIndex).Width = Convert.ToDouble(columnName.GrdAWidth);
                            }
                            columnIndexJustigy++;
                        }

                        worksheet.Rows().AdjustToContents();
                        worksheet.Columns().AdjustToContents();
                        if (dataTableData != null && dataTableData.Rows.Count > 0)
                        {
                            foreach (DataRow dataRow in dataTableData.Rows)
                            {
                                if (dataRow != null)
                                {
                                    currentRow++;
                                    int columnRowIndex = 0, columnColumnIndex = 0;
                                    int totalColumn = dataTableData.Columns.Count;

                                    for (int i = 0; i < totalColumn; i++)
                                    {
                                        if (getReportDataModel.ColumnsData[i].GrdAHideYN == "0")
                                        {

                                            columnRowIndex++;
                                            columnColumnIndex++;
                                            worksheet.Row(currentRow).Style.Alignment.SetWrapText();
                                            if (getReportDataModel.ColumnsData[i].GrdADataType == "2")
                                            {
                                                worksheet.Cell(currentRow, columnColumnIndex).SetValue(DbConnection.DynamicDecimalPoints(dataRow[i].ToString(), Convert.ToInt32(getReportDataModel.ColumnsData[i].GrdADecUpTo)));
                                            }
                                            else
                                            {
                                                worksheet.Cell(currentRow, columnColumnIndex).SetValue(dataRow[i].ToString());
                                            }
                                            if (getReportDataModel.ColumnsData[i].GrdAAlign == "1")
                                            {
                                                worksheet.Cell(currentRow, columnColumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                            }
                                            else if (getReportDataModel.ColumnsData[i].GrdAAlign == "2")
                                            {
                                                worksheet.Cell(currentRow, columnColumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                            }
                                            else if (getReportDataModel.ColumnsData[i].GrdAAlign == "3")
                                            {
                                                worksheet.Cell(currentRow, columnColumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }

                    worksheet.Range("A1:" + alpha[columnNames.Length - 1].ToString() + "1").Row(1).Merge();
                    worksheet.Columns().AdjustToContents();
                    worksheet.Rows().AdjustToContents();
                    worksheet.Style.Font.SetFontName("calibri");
                    worksheet.Row(1).Height = 25;

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);

                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //public byte[] PDF(GetReportDataModel getReportDataModel, string header, string companyName, string companyID = "", List<string> headerValues = null)
        //{
        public byte[] PDF(GetReportDataModel getReportDataModel, string header, string companyName, string companyID = "", string deparmentId = "", List<string> headerValues = null)
        {
            MemoryStream _stream = new MemoryStream();
            int columnCount = 0;
            DepartmentMasterModel departmentMaster = new DepartmentMasterModel();
            try
            {
                int companyId = string.IsNullOrEmpty(Convert.ToInt32(companyID).ToString()) ? 0 : Convert.ToInt32(companyID);
                departmentMaster = DbConnection.GetDepartmentMasterByCompanyId(companyId,Convert.ToInt32(deparmentId));
                if (getReportDataModel.ColumnsData != null)
                {
                    columnCount = getReportDataModel.ColumnsData.Where(x => x.GrdAHideYN == "0").Count();
                    string[] columnNames = new string[getReportDataModel.ColumnsData.Count];
                    DataTable dataTableData = new DataTable();
                    for (int i = 0; i < getReportDataModel.ColumnsData.Count; i++)
                    {
                        columnNames[i] = getReportDataModel.ColumnsData[i].GrdANewColNm;
                        dataTableData.Columns.Add(getReportDataModel.ColumnsData[i].GrdANewColNm);
                    }
                    DataTable dataTableTotal = new DataTable();
                    if (getReportDataModel != null && getReportDataModel.RowsData != null)
                    {
                        for (int x = 0; x < getReportDataModel.RowsData.Count; x++)
                        {
                            DataRow drAdd = dataTableData.NewRow();
                            var item = getReportDataModel.RowsData[x] as List<string>;
                            int start = 0;
                            if (getReportDataModel.ReportType == 1)
                                start = 1;
                            for (int y = start; y < item.Count; y++)
                            {
                                if (getReportDataModel.ReportType == 1)
                                    drAdd[y - 1] = item[y];
                                else
                                    drAdd[y] = item[y];
                            }
                            dataTableData.Rows.Add(drAdd);
                        }
                    }
                    int[] RequisitionReportRightJustify = { 8, 9 };

                    string ARIALUNI_TFF = @"C:\FONTS\ARIALUNI.TTF";
                    Font _blankSpaceLine = FontFactory.GetFont(FontFactory.HELVETICA, 2f);

                    //string ARIALUNI_TFF1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
                    BaseFont bf = (dynamic)null;
                    //try
                    //{
                         bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    //}
                    //catch (Exception ex)
                    //{
                    //    HttpResponseWritingExtensions.WriteAsync(this.Response, ex.Message);
                    //    throw;
                    //}
                   

                    Font _companyAddressFontStyle = new Font(bf, 10f, Font.NORMAL);
                    Font _filterFontStyle = new Font(bf, 10f, Font.NORMAL);
                    Font _tableHeaderFontStyle = new Font(bf, 10f, Font.NORMAL);
                    Font _totalAndProductFontStyle = new Font(bf, 9f, Font.NORMAL);
                    Font _tableDataFontStyle = new Font(bf, 8f, Font.NORMAL);

                    //Font _companynmFontStyle = new Font(bf, 10f, Font.NORMAL);
                    //Font _headerFontStyle = new Font(bf, 10f, Font.NORMAL);

                    //Font _companyAddressFontStyle = FontFactory.GetFont(FontFactory.HELVETICA, 10f);

                    //Font _filterFontStyle = FontFactory.GetFont(FontFactory.HELVETICA, 10f, BaseColor.BLACK);
                    //Font _tableHeaderFontStyle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f, BaseColor.BLACK);
                    //Font _totalAndProductFontStyle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9f, BaseColor.BLACK);
                    //Font _tableDataFontStyle = FontFactory.GetFont("calibri", 8f, BaseColor.BLACK);
                    //Font _tableDataFontStyle = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 0);
                    //iTextSharp.text.pdf.BaseFont bfR;
                    //bfR = iTextSharp.text.pdf.BaseFont.CreateFont(myFont, iTextSharp.text.pdf.BaseFont.IDENTITY_H, iTextSharp.text.pdf.BaseFont.EMBEDDED);
                    //iTextSharp.text.BaseColor clrBlack = new iTextSharp.text.BaseColor(0, 0, 0);
                    //iTextSharp.text.Font fntHead = new iTextSharp.text.Font(bfR, 8f, iTextSharp.text.Font.NORMAL, clrBlack);

                    


                    Document document = new Document();
                    if (getReportDataModel.DocumentPageSize == (int)EnumPageSize.A4_LANDSCAPE)
                    {
                        document = new Document(PageSize.A4_LANDSCAPE.Rotate().Rotate().Rotate(), 50f, 50f, 50f, 50f);

                    }
                    else if (getReportDataModel.DocumentPageSize == (int)EnumPageSize.LEGAL_LANDSCAPE)
                    {
                        document = new Document(PageSize.LEGAL_LANDSCAPE.Rotate().Rotate().Rotate(), 50f, 50f, 50f, 50f);

                    }
                    else
                    {
                        document = new Document(PageSize.A4, 50f, 50f, 50f, 50f);
                    }
                    PdfWriter.GetInstance(document, _stream);
                    document.Open();


                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    line.SpacingBefore = 0.0f;
                    document.Add(line);

                    PdfPTable _pdfTable = new PdfPTable(columnCount);

                    _pdfTable.SpacingBefore = 5f;
                    _pdfTable.WidthPercentage = 100;
                    _pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;

                    float[] width = new float[columnCount];
                    for (int i = 0, count = 0; i < getReportDataModel.ColumnsData.Count; i++)
                    {
                        if (getReportDataModel.ColumnsData[i].GrdAHideYN == "0")
                        {
                            width[count] = float.Parse(getReportDataModel.ColumnsData[i].GrdAWidth);
                            count++;
                        }
                    }

                    _pdfTable.SetWidths(width);


                    PdfPCell _pdfCell = new PdfPCell();
                    if (getReportDataModel != null && getReportDataModel.RowsData != null)
                    {
                        #region Table Header
                        int columnIndexJustigy = 1;

                        for (int i = 0; i < getReportDataModel.ColumnsData.Count; i++)
                        {
                            if (getReportDataModel.ColumnsData[i].GrdAHideYN == "0")
                            {
                                _pdfCell = new PdfPCell(new Phrase(getReportDataModel.ColumnsData[i].GrdANewColNm.ToString(), _tableHeaderFontStyle));
                                _pdfCell.PaddingBottom = 5f;
                                _pdfCell.BackgroundColor = BaseColor.WHITE;

                                if (getReportDataModel.ColumnsData[i].GrdAAlign == "1")
                                {
                                    _pdfCell.VerticalAlignment = Element.ALIGN_LEFT;
                                    _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                                }
                                else if (getReportDataModel.ColumnsData[i].GrdAAlign == "2")
                                {
                                    _pdfCell.VerticalAlignment = Element.ALIGN_RIGHT;
                                    _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                }
                                else if (getReportDataModel.ColumnsData[i].GrdAAlign == "3")
                                {
                                    _pdfCell.VerticalAlignment = Element.ALIGN_CENTER;
                                    _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                }

                                _pdfTable.AddCell(_pdfCell);

                                columnIndexJustigy++;
                            }
                        }

                        _pdfTable.CompleteRow();
                        #endregion


                        #region Table Body

                        for (int i = 0; i < dataTableData.Rows.Count; i++)
                        {
                            int columnDataIndexJustify = 1;

                            for (int j = 0; j < dataTableData.Columns.Count; j++)
                            {
                                if (getReportDataModel.ColumnsData[j].GrdAHideYN == "0")
                                {
                                    string value = string.Empty;
                                    if (getReportDataModel.ColumnsData[j].GrdADataType == "2")
                                    {
                                        value = DbConnection.DynamicDecimalPoints(dataTableData.Rows[i][j].ToString(), Convert.ToInt32(getReportDataModel.ColumnsData[j].GrdADecUpTo));
                                    }
                                    else
                                    {
                                        value = dataTableData.Rows[i][j].ToString();
                                    }
                                    _pdfCell = new PdfPCell(new Phrase(12,value, _tableDataFontStyle));

                                    _pdfCell.PaddingBottom = 3f;
                                    _pdfCell.BackgroundColor = BaseColor.WHITE;

                                    if (getReportDataModel.ColumnsData[j].GrdAAlign == "1")
                                    {
                                        _pdfCell.VerticalAlignment = Element.ALIGN_LEFT;
                                        _pdfCell.HorizontalAlignment = Element.ALIGN_LEFT;
                                    }
                                    else if (getReportDataModel.ColumnsData[j].GrdAAlign == "2")
                                    {
                                        _pdfCell.VerticalAlignment = Element.ALIGN_RIGHT;
                                        _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    }
                                    else if (getReportDataModel.ColumnsData[j].GrdAAlign == "3")
                                    {
                                        _pdfCell.VerticalAlignment = Element.ALIGN_CENTER;
                                        _pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    }


                                    _pdfTable.AddCell(_pdfCell);

                                    columnDataIndexJustify++;
                                }
                            }

                            _pdfTable.CompleteRow();
                        }

                        _pdfTable.CompleteRow();
                        #endregion
                    }
                    document.Add(_pdfTable);
                    document.Close();
                    //HttpResponseWritingExtensions.WriteAsync(this.Response, "Pdf Done");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return AddPageNumbers(_stream.ToArray(), columnCount, departmentMaster, getReportDataModel.GrdTitle, header, getReportDataModel.DocumentPageSize, headerValues);
            //return AddPageNumbers(_stream.ToArray(), columnCount, departmentMaster, getReportDataModel.GrdTitle, header, getReportDataModel.DocumentPageSize);
        }

        public List<SelectListItem> GetPageNo()
        {
            List<SelectListItem> returnList = new List<SelectListItem>();
            int[] pageNo = new int[] { 10, 20, 50, 100, 500, 1000, 5000 };
            for (int i = 0; i < pageNo.Length; i++)
            {
                returnList.Add(new SelectListItem
                {
                    Text = pageNo[i].ToString(),
                    Value = pageNo[i].ToString()
                });
            }
            return returnList;
        }

        public List<SelectListItem> GetUserRightsList(long userId, int ClientId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter("@UserFK", userId);
                parameters[1] = new SqlParameter("@CliVou", ClientId);
                parameters[2] = new SqlParameter("@Flg", 1);
                DataSet dsDynamicSidebar = ObjDBConnection.GetDataSet("GetDynamicSidebar", parameters);
                if (dsDynamicSidebar != null && dsDynamicSidebar.Tables != null && dsDynamicSidebar.Tables.Count > 0)
                {
                    DataTable dtDetail = dsDynamicSidebar.Tables[0];
                    if (dtDetail != null && dtDetail.Rows.Count > 0)
                    {
                        foreach (DataRow item in dtDetail.Rows)
                        {
                            list.Add(new SelectListItem
                            {
                                Text = item["Name"].ToString(),
                                Value = item["Link"].ToString()
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return list;
        }


        private static byte[] AddPageNumbers(byte[] pdf, int columnCount, DepartmentMasterModel departmentMaster, string grdTitle, string header, int documentPageSize, List<string> headerValues = null)
        {
            MemoryStream ms = new MemoryStream();
            // we create a reader for a certain document
            PdfReader reader = new PdfReader(pdf);
            // we retrieve the total number of pages
            int n = reader.NumberOfPages;
            // we retrieve the size of the first page
            Rectangle psize = reader.GetPageSize(1);
            // step 1: creation of a document-object
            //Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            Document document = new Document();
            if (documentPageSize == (int)EnumPageSize.A4_LANDSCAPE)
            {
                document = new Document(PageSize.A4_LANDSCAPE.Rotate(), 50f, 50f, 50f, 50f);

            }
            else if (documentPageSize == (int)EnumPageSize.LEGAL_LANDSCAPE)
            {
                document = new Document(PageSize.LEGAL_LANDSCAPE.Rotate().Rotate().Rotate(), 50f, 50f, 50f, 50f);

            }
            else
            {
                document = new Document(PageSize.A4, 50f, 50f, 50f, 50f);
            }
            //Document document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);

            //document.SetPageSize(PageSize.A4);
            //document.SetMargins(50f, 50f, 50f, 50f);
            // step 2: we create a writer that listens to the document
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            // step 3: we open the document

            document.Open();
            // step 4: we add content
            //string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            string ARIALUNI_TFF = @"C:\FONTS\ARIALUNI.TTF";
            //wwwroot = _iwebhostenviroment.WebRootPath + "/PDF/" + filenm;
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font _companyNameFontStyle = new Font(bf, 14f, Font.NORMAL);
            Font _headerFontStyle = new Font(bf, 11f, Font.NORMAL);
            Font ffont = new Font(bf, 10f, Font.NORMAL);

            //Font ffont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);
            PdfContentByte cb = writer.DirectContent;
            //Font _companyNameFontStyle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15f, BaseColor.BLACK);
            //Font _headerFontStyle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f, BaseColor.BLACK);
            PdfPTable table = new PdfPTable(1);
            PdfPCell cell = new PdfPCell();

            int p = 0, temp = -25;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                document.NewPage();
                p++;

                PdfImportedPage importedPage = writer.GetImportedPage(reader, page);
              //  cb.AddTemplate(importedPage, 0, -43);

                Phrase header1 = new Phrase("Report Date: " + DateTime.Now.ToString("dd-MM-yyyy"), ffont);
                Phrase header2 = new Phrase("Page No.: " + p, ffont);
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, header1, 50, document.Top + 30, 0);
                if (documentPageSize == (int)EnumPageSize.A4_LANDSCAPE)
                {
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_RIGHT, header2, 790, document.Top + 30, 0);
                }
                else if (documentPageSize == (int)EnumPageSize.LEGAL_LANDSCAPE)
                {
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_RIGHT, header2, 950, document.Top + 30, 0);

                }
                else
                {
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_RIGHT, header2, 570, document.Top + 30, 0);
                }
                int top = 10;
                Phrase header4 = new Phrase(departmentMaster.DepName, _companyNameFontStyle);
                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, header4, 50, document.Top + top, 0);
                if (departmentMaster != null && !string.IsNullOrWhiteSpace(departmentMaster.DepAdd))
                {
                    top = top - 12;
                    Phrase header5 = new Phrase(departmentMaster.DepAdd, _headerFontStyle);
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, header5, 50, document.Top + top, 0);
                }
                if (departmentMaster != null && !string.IsNullOrWhiteSpace(departmentMaster.DepGST))
                {
                    top = top - 12;
                    Phrase header5 = new Phrase(departmentMaster.DepGST, _headerFontStyle);
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, header5, 50, document.Top + top, 0);
                }
                if (departmentMaster != null && !string.IsNullOrWhiteSpace(grdTitle))
                {
                    top = top - 12;
                    Phrase header5 = new Phrase(grdTitle, _headerFontStyle);
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, header5, 50, document.Top + top, 0);
                }
                if (departmentMaster != null && !string.IsNullOrWhiteSpace(header))
                {
                    top = top - 12;
                    Phrase header5 = new Phrase(header, _headerFontStyle);
                    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, header5, 50, document.Top + top, 0);
                }
                if (departmentMaster != null && headerValues != null && headerValues.Count > 0)
                {
                    foreach (var item in headerValues)
                    {
                        top = top - 12;
                        Phrase header5 = new Phrase(item, _headerFontStyle);
                        ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, header5, 50, document.Top + top, 0); 
                    }
                }
                temp = temp - 2;
            }
            // step 5: we close the document
            document.Close();
            return ms.ToArray();
        }
        public bool SendEmail(string to, string subject, string body, string attachmentFile = null)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                foreach (var item in to.Split(','))
                {
                    mail.To.Add(item);
                }
                mail.From = new MailAddress(MailHelper.FromEmail, MailHelper.FromEmail, System.Text.Encoding.UTF8);
                mail.Subject = subject;//"This mail is send from asp.net application";
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = body;// "This is Email Body Text";
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                //mail.Priority = MailPriority.High;
                var smtpClient = new SmtpClient(MailHelper.Host)
                {
                    Port = MailHelper.Port,
                    Credentials = new NetworkCredential(MailHelper.FromEmail, MailHelper.Password),
                    EnableSsl = true,
                };
                if (!string.IsNullOrEmpty(attachmentFile))
                {
                    System.Net.Mail.Attachment attachment;
                    foreach (var item in attachmentFile.Split(';'))
                    {
                        attachment = new System.Net.Mail.Attachment(item);
                        mail.Attachments.Add(attachment);
                    }

                }
                try
                {
                    smtpClient.Send(mail);
                    return true;

                }
                catch (Exception)
                {
                    return false;
                }
                //using (SmtpClient client = new SmtpClient())
                //{
                //    client.Credentials = new System.Net.NetworkCredential(MailHelper.FromEmail, MailHelper.Password);
                //    client.Port = MailHelper.Port;
                //    client.Host = MailHelper.Host;
                //    client.EnableSsl = true;
                //    try
                //    {
                //        client.Send(mail);
                //        return true;

                //    }
                //    catch (Exception ex)
                //    {
                //        return false;
                //    }
                //};
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

    }
}
