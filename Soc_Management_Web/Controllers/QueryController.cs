using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PIOAccount.Classes;
using PIOAccount.Models;

namespace PIOAccount.Controllers
{
    public class QueryController : Controller
    {
        DbConnection ObjDBConnection = new DbConnection();

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult RunQuery(string query)
        {
            QueryMasterModel queryMasterModel = new QueryMasterModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(query))
                {
                    if (query.ToUpper().Contains("UPDATE") || query.ToUpper().Contains("DELETE"))
                    {
                        queryMasterModel.IsDisallow = "true";
                    }
                    else
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[2];
                        sqlParameters[0] = new SqlParameter("@Flg", 2);
                        sqlParameters[1] = new SqlParameter("@Query", query);
                        DataTable dtData = ObjDBConnection.CallStoreProcedure("QUERYMASTERSP", sqlParameters);
                        if (dtData != null && dtData.Rows.Count > 0)
                        {
                            if (dtData != null && dtData.Columns.Count > 0)
                            {
                                queryMasterModel.ColumnsList = new List<string>();
                                foreach (DataColumn item in dtData.Columns)
                                {
                                    queryMasterModel.ColumnsList.Add(item.ColumnName);
                                }
                            }

                            List<object> rowsList = new List<object>();
                            for (int i = 0; i < dtData.Rows.Count; i++)
                            {
                                List<string> rows = new List<string>();
                                for (int j = 0; j < dtData.Columns.Count; j++)
                                {
                                    rows.Add(dtData.Rows[i][j].ToString());
                                }
                                rowsList.Add(rows);
                            }
                            queryMasterModel.RowsList = new List<object>();
                            queryMasterModel.RowsList = rowsList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(queryMasterModel);
        }

        public JsonResult SplitQuery(string query, int querycd1)
        {
            SaveQuery saveQuery = new SaveQuery();
            try
            {
                if (!string.IsNullOrWhiteSpace(query))
                {
                    if (query.ToUpper().Contains("UPDATE") || query.ToUpper().Contains("DELETE"))
                        saveQuery.IsDisallow = "true";
                }
                bool isAllow = false;
                if (saveQuery.IsDisallow != "true")
                {
                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[1];
                        sqlParameters[0] = new SqlParameter("@Flg", 5);
                        DataTable dtData = ObjDBConnection.CallStoreProcedure("QUERYMASTERSP", sqlParameters);
                        if (dtData != null && dtData.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtData.Rows.Count; i++)
                            {
                                string fullQuery = dtData.Rows[i]["SQueryPrefix"].ToString() + dtData.Rows[i]["SQueryFields"].ToString() + dtData.Rows[i]["SQuerySufix"].ToString();
                                if (fullQuery.ToUpper().Equals(query.ToUpper()))
                                {
                                    isAllow = true;
                                }
                            }
                        }
                    }
                    if (!isAllow && saveQuery.IsDisallow != "true")
                    {
                        if (querycd1 > 0)
                        {
                            SqlParameter[] sqlParameters = new SqlParameter[2];
                            sqlParameters[0] = new SqlParameter("@Flg", 1);
                            sqlParameters[1] = new SqlParameter("@QUERYCODE", querycd1);
                            DataTable dtData = ObjDBConnection.CallStoreProcedure("QUERYMASTERSP", sqlParameters);
                            if (dtData != null && dtData.Rows.Count > 0)
                            {
                                if (!string.IsNullOrWhiteSpace(dtData.Rows[0][0].ToString()))
                                {
                                    for (int i = 0; i < dtData.Rows.Count; i++)
                                    {
                                        saveQuery.QueryId = dtData.Rows[i]["iQueryID"].ToString();
                                        saveQuery.QueryCode = dtData.Rows[i]["iQueryCode"].ToString();
                                        saveQuery.QueryDesc = dtData.Rows[i]["sQueryDesc"].ToString();
                                        saveQuery.QueryName = dtData.Rows[i]["sQueryName"].ToString();
                                    }

                                }
                                else
                                {
                                    saveQuery.QueryCode = "1";
                                }
                            }
                        }
                        else
                        {
                            SqlParameter[] sqlParameters = new SqlParameter[1];
                            sqlParameters[0] = new SqlParameter("@Flg", 1);
                            DataTable dtData = ObjDBConnection.CallStoreProcedure("QUERYMASTERSP", sqlParameters);
                            if (dtData != null && dtData.Rows.Count > 0)
                            {
                                if (!string.IsNullOrWhiteSpace(dtData.Rows[0][0].ToString()))
                                {
                                    saveQuery.QueryCode = (Convert.ToInt32(dtData.Rows[0][0].ToString()) + 1).ToString();
                                }
                                else
                                {
                                    saveQuery.QueryCode = "1";
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(query))
                        {
                            string[] splitter = { "SELECT", "FROM" };
                            string[] split = query.ToUpper().Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                            if (split != null && split.Length > 0)
                            {
                                saveQuery.QueryPrefix = "SELECT";
                                saveQuery.QueryFields = split[0];
                                saveQuery.QuerySufix = "FROM " + split[1];
                            }
                        }
                    }
                    else
                    {
                        saveQuery.IsExists = isAllow;
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(saveQuery);
        }

        public JsonResult SaveQuery(SaveQuery saveQueryModel)
        {
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[9];
                if (string.IsNullOrWhiteSpace(saveQueryModel.QueryId) || saveQueryModel.QueryId == "0")
                    sqlParameters[0] = new SqlParameter("@Flg", 3);
                else
                    sqlParameters[0] = new SqlParameter("@Flg", 4);

                sqlParameters[1] = new SqlParameter("@QueryId", saveQueryModel.QueryId);
                sqlParameters[2] = new SqlParameter("@QueryCode", saveQueryModel.QueryCode);
                sqlParameters[3] = new SqlParameter("@QueryTitle", saveQueryModel.QueryName);
                sqlParameters[4] = new SqlParameter("@QueryDescription", saveQueryModel.QueryDesc);
                sqlParameters[5] = new SqlParameter("@QueryPrefix", saveQueryModel.QueryPrefix);
                sqlParameters[6] = new SqlParameter("@QueryFields", saveQueryModel.QueryFields);
                sqlParameters[7] = new SqlParameter("@QuerySufix", saveQueryModel.QuerySufix);
                sqlParameters[8] = new SqlParameter("@QUERYFLAG", saveQueryModel.QueryFlg);
                DataTable dtData = ObjDBConnection.CallStoreProcedure("QUERYMASTERSP", sqlParameters);
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(dtData.Rows[0][0].ToString()) && Convert.ToInt32(dtData.Rows[0][0].ToString()) > 0)
                    {
                        return Json(true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(false);
        }

        public JsonResult LoadData()
        {
            List<SaveQuery> saveQueries = new List<SaveQuery>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter("@Flg", 5);
                DataTable dtData = ObjDBConnection.CallStoreProcedure("QUERYMASTERSP", sqlParameters);
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    for (int i = 0; i < dtData.Rows.Count; i++)
                    {
                        saveQueries.Add(new Models.SaveQuery
                        {
                            QueryCode = dtData.Rows[i]["IQueryCode"].ToString(),
                            QueryDesc = dtData.Rows[i]["SQueryDesc"].ToString(),
                            QueryFields = dtData.Rows[i]["SQueryFields"].ToString(),
                            QueryFlg = dtData.Rows[i]["SQueryFlag"].ToString(),
                            QueryId = dtData.Rows[i]["IQueryId"].ToString(),
                            QueryName = dtData.Rows[i]["SQueryName"].ToString(),
                            QueryPrefix = dtData.Rows[i]["SQueryPrefix"].ToString(),
                            QuerySufix = dtData.Rows[i]["SQuerySufix"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(saveQueries);
        }

        public JsonResult EditQuery(int id)
        {
            SaveQuery saveQueries = new SaveQuery();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@Flg", 6);
                sqlParameters[1] = new SqlParameter("@QueryId", id);
                DataTable dtData = ObjDBConnection.CallStoreProcedure("QUERYMASTERSP", sqlParameters);
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    for (int i = 0; i < dtData.Rows.Count; i++)
                    {
                        saveQueries.QueryCode = dtData.Rows[i]["IQueryCode"].ToString();
                        saveQueries.QueryDesc = dtData.Rows[i]["SQueryDesc"].ToString();
                        saveQueries.QueryFields = dtData.Rows[i]["SQueryFields"].ToString();
                        saveQueries.QueryFlg = dtData.Rows[i]["SQueryFlag"].ToString();
                        saveQueries.QueryId = dtData.Rows[i]["IQueryId"].ToString();
                        saveQueries.QueryName = dtData.Rows[i]["SQueryName"].ToString();
                        saveQueries.QueryPrefix = dtData.Rows[i]["SQueryPrefix"].ToString();
                        saveQueries.QuerySufix = dtData.Rows[i]["SQuerySufix"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(saveQueries);
        }

        public JsonResult DeleteQuery(int id)
        {
            SaveQuery saveQueries = new SaveQuery();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@Flg", 7);
                sqlParameters[1] = new SqlParameter("@QueryId", id);
                DataTable dtData = ObjDBConnection.CallStoreProcedure("QUERYMASTERSP", sqlParameters);
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(dtData.Rows[0][0].ToString()) && Convert.ToInt32(dtData.Rows[0][0].ToString()) > 0)
                    {
                        return Json(true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(false);
        }

        public IActionResult ExportToExcel(int id)
        {
            SaveQuery saveQueries = new SaveQuery();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter("@Flg", 8);
                sqlParameters[1] = new SqlParameter("@QueryId", id);
                DataTable dtData = ObjDBConnection.CallStoreProcedure("QUERYMASTERSP", sqlParameters);
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    try
                    {
                        string[] columnNames = new string[dtData.Columns.Count];
                        DataTable dataTableData = new DataTable();
                        for (int i = 0; i < dtData.Columns.Count; i++)
                        {
                            columnNames[i] = dtData.Columns[i].ColumnName;
                            dataTableData.Columns.Add(dtData.Columns[i].ColumnName);
                        }
                        DataTable dataTableTotal = new DataTable();
                        if (dtData != null && dtData.Rows.Count > 0)
                        {
                            for (int x = 0; x < dtData.Rows.Count; x++)
                            {
                                DataRow drAdd = dataTableData.NewRow();
                                for (int y = 0; y < dtData.Columns.Count; y++)
                                {
                                    drAdd[dtData.Columns[y].ColumnName] = dtData.Rows[x][y].ToString();
                                }
                                dataTableData.Rows.Add(drAdd);
                            }
                        }

                        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

                        using (var workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add();
                            var currentRow = 0;

                            worksheet.Rows().AdjustToContents();
                            worksheet.Columns().AdjustToContents();
                            if (dataTableData != null && dataTableData.Columns.Count > 0)
                            {
                                currentRow++;
                                int columnIndex = 0;
                                int columnIndexJustigy = 1;
                                foreach (DataColumn columnName in dtData.Columns)
                                {
                                    columnIndex++;
                                    worksheet.Cell(currentRow, columnIndex).SetValue(columnName.ColumnName).Style.Font.SetBold().Font.SetFontSize(11);
                                }
                                columnIndexJustigy++;

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
                                                columnRowIndex++;
                                                columnColumnIndex++;
                                                worksheet.Row(currentRow).Style.Alignment.SetWrapText();
                                                decimal.TryParse(dataRow[i].ToString(), out decimal value);
                                                worksheet.Cell(currentRow, columnColumnIndex).SetValue(dataRow[i].ToString());
                                                if (value == 0 && dataRow[i].ToString() == "0")
                                                {
                                                    worksheet.Cell(currentRow, columnColumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                                }
                                                else if (value > 0)
                                                {
                                                    worksheet.Cell(currentRow, columnColumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                                                }
                                                else
                                                {
                                                    worksheet.Cell(currentRow, columnColumnIndex).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            worksheet.Columns().AdjustToContents();
                            worksheet.Rows().AdjustToContents();
                            worksheet.Style.Font.SetFontName("calibri");

                            using (var stream = new MemoryStream())
                            {
                                workbook.SaveAs(stream);

                                return File(
                                    stream.ToArray(),
                                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                    "RunQuery.xlsx");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(false);
        }

    }
}
