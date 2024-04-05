using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PIOAccount.Controllers
{
    public class GridMasterController : BaseController
    {
        DbConnection ObjDBConnection = new DbConnection();
        ProductHelpers ObjProductHelpers = new ProductHelpers();

        public IActionResult Index(long id)
        {
            try
            {
                GridMasterModel gridMasterModel = new GridMasterModel();
                gridMasterModel.TypeList = ObjProductHelpers.GetReportView();
                gridMasterModel.MenuList = ObjProductHelpers.GetMenuMasterDropdown();
                gridMasterModel.MultiYNList = ObjProductHelpers.GetProductYesNo();
                gridMasterModel.Gridtransaction = new GridTransactionGridModel();
                gridMasterModel.Gridtransaction.DataTypeList = ObjProductHelpers.GetDataType();
                gridMasterModel.Gridtransaction.AlignList = ObjProductHelpers.GetTextAlign();
                gridMasterModel.Gridtransaction.TotalYNList = ObjProductHelpers.GetProductYesNo();
                gridMasterModel.Gridtransaction.LinkYNList = ObjProductHelpers.GetProductYesNo();
                gridMasterModel.Gridtransaction.HideList = ObjProductHelpers.GetProductYesNo();
                gridMasterModel.Gridshort = new GridShortGridModel();
                gridMasterModel.Gridshort.DefaultYNList = ObjProductHelpers.GetProductYesNo();
                gridMasterModel.Gridtransaction.GrdATotYN = new bool[1];
                gridMasterModel.Gridtransaction.GrdALinkYN = new bool[1];
                gridMasterModel.Gridtransaction.GrdAHideYN = new bool[1];
                gridMasterModel.Gridshort.GrdBDefYN = new bool[1];
                if (id > 0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@GrdVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    DataSet dtSetGrid = ObjDBConnection.GetDataSet("GetGridMasterDetails", sqlParameters);
                    if (dtSetGrid != null && dtSetGrid.Tables.Count > 0)
                    {
                        DataTable DtGrdMst = dtSetGrid.Tables[0];
                        DataTable dtUniqueBDFld = dtSetGrid.Tables[1];
                        if (DtGrdMst != null && DtGrdMst.Rows.Count > 0)
                        {
                            gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
                            gridMasterModel.GrdFlg = 1;
                            gridMasterModel.GrdMnuVou = DbConnection.ParseInt32(DtGrdMst.Rows[0]["GrdMnuVou"].ToString());
                            gridMasterModel.GrdType = DtGrdMst.Rows[0]["GrdType"].ToString();
                            gridMasterModel.GrdName = DtGrdMst.Rows[0]["GrdName"].ToString();
                            gridMasterModel.GrdTitle = DtGrdMst.Rows[0]["GrdTitle"].ToString();
                            gridMasterModel.PageSize = DbConnection.ParseInt32(DtGrdMst.Rows[0]["PageSize"].ToString());
                            gridMasterModel.GrdMultiSelYN = DbConnection.ParseInt32(DtGrdMst.Rows[0]["GrdMultiSelYN"].ToString());
                            gridMasterModel.GrdDftYNo = DbConnection.ParseInt32(DtGrdMst.Rows[0]["GrdDftYNo"].ToString());
                            gridMasterModel.GrdQryFields = DtGrdMst.Rows[0]["GrdQryFields"].ToString();
                            gridMasterModel.GrdQryJoin = DtGrdMst.Rows[0]["GrdQryJoin"].ToString();
                            gridMasterModel.GrdQryOrderBy = DtGrdMst.Rows[0]["GrdQryOrderBy"].ToString();

                            gridMasterModel.Gridtransaction.GrdADbFld = new string[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdAColNm = new string[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdANewColNm = new string[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdAPosition = new int[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdADataType = new int[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdAWidth = new int[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdADecUpTo = new int[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdAAlign = new int[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdATotYN = new bool[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdALinkYN = new bool[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdAHideYN = new bool[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdCanGrow = new bool[DtGrdMst.Rows.Count];
                            gridMasterModel.Gridtransaction.GrdASuppressIFVal = new string[DtGrdMst.Rows.Count];

                            if (dtUniqueBDFld != null && dtUniqueBDFld.Rows.Count > 0)
                            {
                                gridMasterModel.Gridshort.GrdBDbFld = new string[dtUniqueBDFld.Rows.Count];
                                gridMasterModel.Gridshort.GrdBColNm = new string[dtUniqueBDFld.Rows.Count];
                                gridMasterModel.Gridshort.GrdBDefYN = new bool[dtUniqueBDFld.Rows.Count];
                                for (int i = 0; i < dtUniqueBDFld.Rows.Count; i++)
                                {
                                    gridMasterModel.Gridshort.GrdBDbFld[i] = dtUniqueBDFld.Rows[i]["GrdBDbFld"].ToString();
                                    gridMasterModel.Gridshort.GrdBColNm[i] = dtUniqueBDFld.Rows[i]["GrdBColNm"].ToString();
                                    if (DbConnection.ParseInt32(dtUniqueBDFld.Rows[i]["GrdBDefYN"].ToString()) == 1)
                                    {
                                        gridMasterModel.Gridshort.GrdBDefYN[i] = true;
                                    }
                                    else
                                    {
                                        gridMasterModel.Gridshort.GrdBDefYN[i] = false;
                                    }
                                }
                            }
                            else
                            {
                                gridMasterModel.Gridshort.GrdBDbFld = new string[0];
                                gridMasterModel.Gridshort.GrdBColNm = new string[0];
                                gridMasterModel.Gridshort.GrdBDefYN = new bool[1];
                            }

                            for (int i = 0; i < DtGrdMst.Rows.Count; i++)
                            {
                                gridMasterModel.Gridtransaction.GrdADbFld[i] = DtGrdMst.Rows[i]["GrdADbFld"].ToString();
                                gridMasterModel.Gridtransaction.GrdAColNm[i] = DtGrdMst.Rows[i]["GrdAColNm"].ToString();
                                gridMasterModel.Gridtransaction.GrdANewColNm[i] = DtGrdMst.Rows[i]["GrdANewColNm"].ToString();
                                gridMasterModel.Gridtransaction.GrdAPosition[i] = DbConnection.ParseInt32(DtGrdMst.Rows[i]["GrdAPosition"].ToString());
                                gridMasterModel.Gridtransaction.GrdADataType[i] = DbConnection.ParseInt32(DtGrdMst.Rows[i]["GrdADataType"].ToString());
                                gridMasterModel.Gridtransaction.GrdAWidth[i] = DbConnection.ParseInt32(DtGrdMst.Rows[i]["GrdAWidth"].ToString());
                                gridMasterModel.Gridtransaction.GrdADecUpTo[i] = DbConnection.ParseInt32(DtGrdMst.Rows[i]["GrdADecUpTo"].ToString());
                                gridMasterModel.Gridtransaction.GrdAAlign[i] = DbConnection.ParseInt32(DtGrdMst.Rows[i]["GrdAAlign"].ToString());
                                if (DbConnection.ParseInt32(DtGrdMst.Rows[i]["GrdATotYN"].ToString()) == 1)
                                {
                                    gridMasterModel.Gridtransaction.GrdATotYN[i] = true;
                                }
                                else
                                {
                                    gridMasterModel.Gridtransaction.GrdATotYN[i] = false;
                                }
                                if (DbConnection.ParseInt32(DtGrdMst.Rows[i]["GrdALinkYN"].ToString()) == 1)
                                {
                                    gridMasterModel.Gridtransaction.GrdALinkYN[i] = true;
                                }
                                else
                                {
                                    gridMasterModel.Gridtransaction.GrdALinkYN[i] = false;
                                }
                                if (DbConnection.ParseInt32(DtGrdMst.Rows[i]["GrdAHideYN"].ToString()) == 1)
                                {
                                    gridMasterModel.Gridtransaction.GrdAHideYN[i] = true;
                                }
                                else
                                {
                                    gridMasterModel.Gridtransaction.GrdAHideYN[i] = false;
                                }
                                if (DtGrdMst.Rows[i]["CanGrow"].ToString().ToLower() == "false")
                                {
                                    gridMasterModel.Gridtransaction.GrdCanGrow[i] = false;
                                }
                                else
                                {
                                    gridMasterModel.Gridtransaction.GrdCanGrow[i] = true;
                                }
                                gridMasterModel.Gridtransaction.GrdASuppressIFVal[i] = DtGrdMst.Rows[i]["GrdASuppressIFVal"].ToString();
                            }

                        }
                    }
                }
                return View(gridMasterModel);
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new GridMasterModel());
        }


        [HttpPost]
        public IActionResult Index(long id, GridMasterModel gridMasterModel)
        {
            try
            {
                gridMasterModel.TypeList = ObjProductHelpers.GetReportView();
                gridMasterModel.MultiYNList = ObjProductHelpers.GetProductYesNo();
                gridMasterModel.MenuList = ObjProductHelpers.GetMenuMasterDropdown();
                if (gridMasterModel.Gridtransaction == null)
                    gridMasterModel.Gridtransaction = new GridTransactionGridModel();
                gridMasterModel.Gridtransaction.DataTypeList = ObjProductHelpers.GetDataType();
                gridMasterModel.Gridtransaction.AlignList = ObjProductHelpers.GetTextAlign();
                gridMasterModel.Gridtransaction.TotalYNList = ObjProductHelpers.GetProductYesNo();
                gridMasterModel.Gridtransaction.LinkYNList = ObjProductHelpers.GetProductYesNo();
                gridMasterModel.Gridtransaction.HideList = ObjProductHelpers.GetProductYesNo();
                if (gridMasterModel.Gridshort == null)
                    gridMasterModel.Gridshort = new GridShortGridModel();
                gridMasterModel.Gridshort.DefaultYNList = ObjProductHelpers.GetProductYesNo();
                string WrongFiledName = "";
                //int flg = gridMasterModel.GrdFlg;
                SqlParameter[] sqlPara = new SqlParameter[2];
                string GrdSaveas = gridMasterModel.GrdSaveAs;
                sqlPara[0] = new SqlParameter("@GrdMnuVou", gridMasterModel.GrdMnuVou);
                sqlPara[1] = new SqlParameter("@GrdName", gridMasterModel.GrdName);
                DataTable dtgrdMst = ObjDBConnection.CallStoreProcedure("GetGridMasterDetailsCheck", sqlPara);
                if (dtgrdMst != null && dtgrdMst.Rows.Count > 0)
                {
                    int status = Convert.ToInt32(dtgrdMst.Rows[0][0].ToString());
                    if (status == 1)
                    {
                        gridMasterModel.GrdFlg = 0;
                    }
                    else
                    {
                        if (GrdSaveas == "1")
                        {
                            gridMasterModel.GrdSaveAs = "0";
                            SetErrorMessage("Layout Name Already Exist!");
                            ViewBag.FocusType = "1";
                            return View(gridMasterModel);
                        }
                        else
                        {
                            gridMasterModel.GrdFlg = 1;
                        }
                        
                    }
                    int flg = gridMasterModel.GrdFlg;
                    if (!string.IsNullOrWhiteSpace(gridMasterModel.GrdQryFields))
                    {
                        if (gridMasterModel.Gridtransaction.GrdADbFld != null)
                        {
                            for (int i = 0; i < gridMasterModel.Gridtransaction.GrdADbFld.Length; i++)
                            {
                                if (string.IsNullOrWhiteSpace(gridMasterModel.Gridtransaction.GrdADbFld[i]))
                                {
                                    gridMasterModel.Gridtransaction.GrdADbFld[i] = "";
                                }
                                if (!Regex.Match(gridMasterModel.GrdQryFields.ToLower(), @"\b" + gridMasterModel.Gridtransaction.GrdADbFld[i].ToLower() + @"\b").Success)
                                {
                                    WrongFiledName = gridMasterModel.Gridtransaction.GrdADbFld[i];
                                    break;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(WrongFiledName))
                        {
                            if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(gridMasterModel.GrdMnuVou).ToString()) && !string.IsNullOrWhiteSpace(gridMasterModel.GrdType) && !string.IsNullOrWhiteSpace(gridMasterModel.GrdName))
                            {
                                SqlParameter[] sqlParameters = new SqlParameter[11];
                                sqlParameters[0] = new SqlParameter("@GrdMnuVou", gridMasterModel.GrdMnuVou);
                                sqlParameters[1] = new SqlParameter("@GrdType", gridMasterModel.GrdType);
                                sqlParameters[2] = new SqlParameter("@GrdName", gridMasterModel.GrdName);
                                sqlParameters[3] = new SqlParameter("@GrdMultiSelYN", gridMasterModel.GrdMultiSelYN);
                                sqlParameters[4] = new SqlParameter("@GrdQryFields", gridMasterModel.GrdQryFields);
                                sqlParameters[5] = new SqlParameter("@GrdQryJoin", gridMasterModel.GrdQryJoin);
                                sqlParameters[6] = new SqlParameter("@GrdQryOrderBy", gridMasterModel.GrdQryOrderBy);
                                if (flg == 0)
                                {
                                    id = 0;
                                    sqlParameters[7] = new SqlParameter("@GrdVou", 0);
                                }
                                else
                                {
                                    sqlParameters[7] = new SqlParameter("@GrdVou", id);
                                }

                                sqlParameters[8] = new SqlParameter("@GrdDftYNo", gridMasterModel.GrdDftYNo);
                                sqlParameters[9] = new SqlParameter("@GrdTitle", gridMasterModel.GrdTitle);
                                sqlParameters[10] = new SqlParameter("@PageSize", gridMasterModel.PageSize);
                                DataTable DtGrid = ObjDBConnection.CallStoreProcedure("GridMaster_Insert", sqlParameters);
                                if (DtGrid != null && DtGrid.Rows.Count > 0)
                                {
                                    int masterId = DbConnection.ParseInt32(DtGrid.Rows[0][0].ToString());
                                    if (masterId == 1)
                                    {
                                        gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
                                        SetErrorMessage("Layout Name Already Exist!");
                                        ViewBag.FocusType = "1";
                                        return View(gridMasterModel);
                                    }
                                    if (masterId > 0)
                                    {
                                        if (gridMasterModel.Gridtransaction.GrdADbFld != null)
                                        {
                                            string[] aTotYN = gridMasterModel.GrdATotYNString.TrimEnd(',').Split(',');
                                            string[] canGrow = gridMasterModel.canGrowString.TrimEnd(',').Split(',');
                                            string[] aLinkYN = gridMasterModel.GrdALinkYNString.TrimEnd(',').Split(',');
                                            string[] aHideYN = gridMasterModel.GrdAHideYNString.TrimEnd(',').Split(',');
                                            for (int i = 0; i < gridMasterModel.Gridtransaction.GrdADbFld.Length; i++)
                                            {
                                                SqlParameter[] parameter = new SqlParameter[15];
                                                parameter[0] = new SqlParameter("@GrdAGrdVou", masterId);
                                                parameter[1] = new SqlParameter("@GrdADbFld", gridMasterModel.Gridtransaction.GrdADbFld[i]);
                                                parameter[2] = new SqlParameter("@GrdAColNm", gridMasterModel.Gridtransaction.GrdAColNm[i]);
                                                parameter[3] = new SqlParameter("@GrdANewColNm", gridMasterModel.Gridtransaction.GrdANewColNm[i]);
                                                parameter[4] = new SqlParameter("@GrdAPosition", gridMasterModel.Gridtransaction.GrdAPosition[i]);
                                                parameter[5] = new SqlParameter("@GrdADataType", gridMasterModel.Gridtransaction.GrdADataType[i]);
                                                parameter[6] = new SqlParameter("@GrdAWidth", gridMasterModel.Gridtransaction.GrdAWidth[i]);
                                                parameter[7] = new SqlParameter("@GrdADecUpTo", gridMasterModel.Gridtransaction.GrdADecUpTo[i]);
                                                parameter[8] = new SqlParameter("@GrdAAlign", gridMasterModel.Gridtransaction.GrdAAlign[i]);
                                                //parameter[9] = new SqlParameter("@GrdATotYN", gridMasterModel.Gridtransaction.GrdATotYN[i] ? "1" : "0");
                                                //parameter[10] = new SqlParameter("@GrdALinkYN", gridMasterModel.Gridtransaction.GrdALinkYN[i] ? "1" : "0");
                                                //parameter[11] = new SqlParameter("@GrdAHideYN", gridMasterModel.Gridtransaction.GrdAHideYN[i] ? "1" : "0");
                                                parameter[9] = new SqlParameter("@GrdATotYN", aTotYN[i]);
                                                parameter[10] = new SqlParameter("@GrdCanGrow", canGrow[i]);
                                                parameter[11] = new SqlParameter("@GrdALinkYN", aLinkYN[i]);
                                                parameter[12] = new SqlParameter("@GrdAHideYN", aHideYN[i]);
                                                parameter[13] = new SqlParameter("@GrdASuppressIFVal", gridMasterModel.Gridtransaction.GrdASuppressIFVal[i]);
                                                parameter[14] = new SqlParameter("@SrNo", (i + 1));
                                                DataTable DtGrdTrn = ObjDBConnection.CallStoreProcedure("GridTransaction_Insert", parameter);
                                            }
                                        }
                                        if (gridMasterModel.Gridshort.GrdBDbFld != null && gridMasterModel.Gridshort.GrdBDbFld[0] != null)
                                        {
                                            for (int i = 0; i < gridMasterModel.Gridshort.GrdBDbFld.Length; i++)
                                            {
                                                SqlParameter[] parameter = new SqlParameter[5];
                                                parameter[0] = new SqlParameter("@GrdBGrdVou", masterId);
                                                parameter[1] = new SqlParameter("@GrdBDbFld", gridMasterModel.Gridshort.GrdBDbFld[i]);
                                                parameter[2] = new SqlParameter("@GrdBColNm", gridMasterModel.Gridshort.GrdBColNm[i]);
                                                parameter[3] = new SqlParameter("@GrdBDefYN", gridMasterModel.Gridshort.GrdBDefYN[i] ? "1" : "0");
                                                parameter[4] = new SqlParameter("@SrNo", (i + 1));
                                                DataTable DtGrdTrn = ObjDBConnection.CallStoreProcedure("GridShort_Insert", parameter);
                                            }
                                        }
                                        int Status = DbConnection.ParseInt32(DtGrid.Rows[0][0].ToString());
                                        if (Status == 0)
                                        {
                                            gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
                                            SetErrorMessage("Dulplicate Type Details");
                                            ViewBag.FocusType = "1";
                                            return View(gridMasterModel);
                                        }
                                        else
                                        {
                                            if (id > 0)
                                            {
                                                SetSuccessMessage("Updated Sucessfully");
                                            }
                                            else
                                            {
                                                SetSuccessMessage("Inserted Sucessfully");
                                            }
                                            return RedirectToAction("index", "GridMaster", new { id = 0 });
                                        }
                                    }
                                    else
                                    {
                                        gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
                                        SetErrorMessage("Insert error");
                                        ViewBag.FocusType = "1";
                                        return View(gridMasterModel);
                                    }
                                }
                                else
                                {
                                    gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
                                    SetErrorMessage("Please Enter the Value");
                                    ViewBag.FocusType = "1";
                                    return View(gridMasterModel);
                                }
                            }
                            else
                            {
                                gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
                                SetErrorMessage("Please Enter the Value");
                                ViewBag.FocusType = "1";
                                return View(gridMasterModel);
                            }
                        }
                        else
                        {
                            gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
                            SetErrorMessage("“" + WrongFiledName + " ” field does not match in query");
                            ViewBag.FocusType = "1";
                            return View(gridMasterModel);
                        }
                    }
                    else
                    {
                        gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
                        SetErrorMessage("Please Enter Mandatory Field");
                        ViewBag.FocusType = "1";
                        return View(gridMasterModel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View(new GridMasterModel());
        }

        public IActionResult Delete(long id)
        {
            try
            {
                GridMasterModel gridMasterModel = new GridMasterModel();
                if (id > 0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@GrdVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 1);
                    DataTable DtGridMst = ObjDBConnection.CallStoreProcedure("GetGridMasterDetails", sqlParameters);
                    if (DtGridMst != null && DtGridMst.Rows.Count > 0)
                    {
                        SetSuccessMessage("Layout Deleted Sucessfully");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "GridMaster");
        }

        public IActionResult GetGridMasterList()
        {
            GetDataFromRunQuery(10029, 1);
            StringValues draw, orderColumn, orderDirection, startRecord, pageSize = "10", searchText = string.Empty;
            List<GridMasterModel> gridMasterList = new List<GridMasterModel>();
            int totalRecord = 0;
            try
            {
                Request.Form.TryGetValue("draw", out draw);
                Request.Form.TryGetValue("order[0][column]", out orderColumn);
                Request.Form.TryGetValue("order[0][dir]", out orderDirection);
                Request.Form.TryGetValue("start", out startRecord);
                Request.Form.TryGetValue("length", out pageSize);
                Request.Form.TryGetValue("search[value]", out searchText);

                string searchValue = !string.IsNullOrWhiteSpace(searchText.ToString()) ? searchText.ToString() : null;

                SqlParameter[] sqlParametersNew = new SqlParameter[5];
                sqlParametersNew[0] = new SqlParameter("@GrdVou", 0);
                sqlParametersNew[1] = new SqlParameter("@Flg", 2);
                sqlParametersNew[2] = new SqlParameter("@skiprecord", startRecord.ToString());
                sqlParametersNew[3] = new SqlParameter("@pagesize", pageSize.ToString());
                sqlParametersNew[4] = new SqlParameter("@searchvalue", searchValue);

                DataTable DtGridMaster = ObjDBConnection.CallStoreProcedure("GetGridMasterDetails", sqlParametersNew);
                if (DtGridMaster != null && DtGridMaster.Rows.Count > 0)
                {
                    for (int i = 0; i < DtGridMaster.Rows.Count; i++)
                    {
                        GridMasterModel gridMaster = new GridMasterModel();
                        gridMaster.GrdMnuVou = DbConnection.ParseInt32(DtGridMaster.Rows[i]["GrdMnuVou"].ToString());
                        gridMaster.MnuName = DtGridMaster.Rows[i]["MnuName"].ToString();
                        gridMaster.GrdType = DtGridMaster.Rows[i]["GrdType"].ToString();
                        gridMaster.GrdName = DtGridMaster.Rows[i]["GrdName"].ToString();
                        gridMaster.GrdMultiSelYN = DbConnection.ParseInt32(DtGridMaster.Rows[i]["GrdMultiSelYN"].ToString());
                        gridMaster.GrdVou = DbConnection.ParseInt32(DtGridMaster.Rows[i]["GrdVou"].ToString());
                        totalRecord = DbConnection.ParseInt32(DtGridMaster.Rows[i]["MaxRows"].ToString());
                        gridMasterList.Add(gridMaster);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new
            {
                draw = DbConnection.ParseInt32(draw),
                recordsTotal = DbConnection.ParseInt32(pageSize),
                recordsFiltered = totalRecord,
                data = gridMasterList != null ? gridMasterList : new List<GridMasterModel>(),
            });
        }


        public IActionResult GetDataFromRunQuery(long id, int isrunquery)
        {
            GridRunQueryModel objreturnList = new GridRunQueryModel();
            try
            {
                if (id > 0)
                {
                    SqlParameter[] sqlParameters = new SqlParameter[2];
                    sqlParameters[0] = new SqlParameter("@GrdVou", id);
                    sqlParameters[1] = new SqlParameter("@Flg", 2);
                    DataTable dtGrdMst = ObjDBConnection.CallStoreProcedure("GetGridMasterDetails", sqlParameters);
                    if (dtGrdMst != null && dtGrdMst.Rows.Count > 0)
                    {
                        string Fullquery = dtGrdMst.Rows[0]["GrdQryFields"].ToString() + " ";
                        Fullquery += dtGrdMst.Rows[0]["GrdQryJoin"].ToString() + " ";
                        Fullquery += dtGrdMst.Rows[0]["GrdQryOrderBy"].ToString() + " ";
                        sqlParameters = new SqlParameter[1];
                        sqlParameters[0] = new SqlParameter("@dynamicQuery", Fullquery);
                        DataTable dtWholeData = ObjDBConnection.CallStoreProcedure("ExecuteDynamicQuery", sqlParameters);
                        if (dtWholeData != null && dtWholeData.Rows.Count > 0)
                        {
                            List<string> columns = new List<string>();
                            if (isrunquery == 1)
                            {
                                for (int i = 0; i < dtWholeData.Columns.Count; i++)
                                {
                                    columns.Add(dtWholeData.Columns[i].ColumnName);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < dtGrdMst.Rows.Count; i++)
                                {
                                    columns.Add(dtGrdMst.Rows[i]["GrdADbFld"].ToString());
                                }
                            }


                            List<object> rowsList = new List<object>();
                            for (int i = 0; i < dtWholeData.Rows.Count; i++)
                            {
                                List<string> rows = new List<string>();
                                for (int j = 0; j < columns.Count; j++)
                                {
                                    rows.Add(dtWholeData.Rows[i][j].ToString());
                                }
                                rowsList.Add(rows);
                            }

                            objreturnList.ColumnName = columns;
                            objreturnList.RowsData = rowsList;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
            }
            return View(objreturnList);
        }

        [HttpPost]
        public IActionResult GetDataFromRunQueryFromQuery(DynamicRunqueryParaModel objReqPara)
        {
            GridRunQueryModel objreturnList = new GridRunQueryModel();
            objreturnList.Response = "-1";
            objreturnList.Message = "Fail";
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                int clientId = Convert.ToInt32(GetIntSession("clientId"));
                int yearId = Convert.ToInt32(GetIntSession("YearId"));
                objReqPara.Query = objReqPara.Query.Replace("@#CMPVOU#@", companyId.ToString());
                objReqPara.Query = objReqPara.Query.Replace("@#CLIVOU#@", clientId.ToString());
                objReqPara.Query = objReqPara.Query.Replace("@#YEARVOU#@", yearId.ToString());
                sqlParameters[0] = new SqlParameter("@dynamicQuery", objReqPara.Query);
                DataTable dtWholeData = ObjDBConnection.CallStoreProcedure("ExecuteDynamicQuery", sqlParameters);
                if (dtWholeData != null && dtWholeData.Rows.Count > 0)
                {
                    DataTable dtNew = dtWholeData.Clone();

                    List<string> columns = new List<string>();

                    if (!string.IsNullOrEmpty(objReqPara.ColumnName))
                    {
                        objReqPara.ColumnName = objReqPara.ColumnName.TrimEnd(',');
                        string[] split = objReqPara.ColumnName.Split(',');
                        if (split != null && split.Length > 0)
                        {
                            for (int i = 0; i < split.Length; i++)
                            {
                                if (!Regex.Match(objReqPara.Query.ToLower(), @"\b" + split[i].ToLower() + @"\b").Success)
                                {
                                    objreturnList.Response = "-1";
                                    objreturnList.Message = "“" + split[i] + " ” field does not match in query";
                                    return Json(objreturnList);
                                }
                            }

                            for (int j = 0; j < dtWholeData.Columns.Count; j++)
                            {
                                bool ismatch = false;
                                for (int i = 0; i < split.Length; i++)
                                {
                                    if (dtWholeData.Columns[j].ColumnName.ToLower() == split[i].ToLower())
                                    {
                                        ismatch = true;
                                    }
                                }
                                if (ismatch == false)
                                {
                                    if (dtNew.Columns.Contains(dtWholeData.Columns[j].ColumnName))
                                    {
                                        dtNew.Columns.Remove(dtWholeData.Columns[j].ColumnName);
                                        dtNew.AcceptChanges();
                                    }
                                    if (dtWholeData.Columns.Contains(dtWholeData.Columns[j].ColumnName))
                                    {
                                        dtWholeData.Columns.Remove(dtWholeData.Columns[j].ColumnName);
                                        dtWholeData.AcceptChanges();
                                    }
                                    j = 0;
                                }

                            }
                        }
                    }
                    dtNew.Merge(dtWholeData);

                    for (int i = 0; i < dtNew.Columns.Count; i++)
                    {
                        columns.Add(dtNew.Columns[i].ColumnName);
                    }
                    List<object> rowsList = new List<object>();
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        if (i < 10)
                        {
                            List<string> rows = new List<string>();
                            for (int j = 0; j < columns.Count; j++)
                            {
                                rows.Add(dtNew.Rows[i][j].ToString());
                            }
                            rowsList.Add(rows);
                        }
                        else
                        {
                            break;
                        }
                    }
                    objreturnList.ColumnName = columns;
                    objreturnList.RowsData = rowsList;
                    objreturnList.Response = "1";
                    objreturnList.Message = "Success";
                }
                else
                {
                    objreturnList.Response = "-1";
                    objreturnList.Message = "Data not found";
                }
            }
            catch (Exception ex)
            {
                objreturnList.Response = "-1";
                objreturnList.Message = ex.Message;
            }
            return Json(

                objreturnList
            );
        }

        //public JsonResult SaveAsGridMst(string grdnm, string mnuVou, string queryflds, GridMasterModel gridMasterModel, long id)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(mnuVou) && !string.IsNullOrWhiteSpace(grdnm))
        //        {
        //            //long id = gridMasterModel.GrdVou;
        //            SqlParameter[] sqlPara = new SqlParameter[2];
        //            sqlPara[0] = new SqlParameter("@GrdMnuVou", mnuVou);
        //            sqlPara[1] = new SqlParameter("@GrdName", grdnm);
        //            DataTable dtgrdMst = ObjDBConnection.CallStoreProcedure("GetGridMasterDetailsCheck", sqlPara);
        //            if (dtgrdMst != null && dtgrdMst.Rows.Count > 0)
        //            {
        //                int status = Convert.ToInt32(dtgrdMst.Rows[0][0].ToString());
        //                gridMasterModel.TypeList = ObjProductHelpers.GetReportView();
        //                gridMasterModel.MultiYNList = ObjProductHelpers.GetProductYesNo();
        //                gridMasterModel.MenuList = ObjProductHelpers.GetMenuMasterDropdown();
        //                if (gridMasterModel.Gridtransaction == null)
        //                    gridMasterModel.Gridtransaction = new GridTransactionGridModel();
        //                gridMasterModel.Gridtransaction.DataTypeList = ObjProductHelpers.GetDataType();
        //                gridMasterModel.Gridtransaction.AlignList = ObjProductHelpers.GetTextAlign();
        //                gridMasterModel.Gridtransaction.TotalYNList = ObjProductHelpers.GetProductYesNo();
        //                gridMasterModel.Gridtransaction.LinkYNList = ObjProductHelpers.GetProductYesNo();
        //                gridMasterModel.Gridtransaction.HideList = ObjProductHelpers.GetProductYesNo();
        //                if (gridMasterModel.Gridshort == null)
        //                    gridMasterModel.Gridshort = new GridShortGridModel();
        //                gridMasterModel.Gridshort.DefaultYNList = ObjProductHelpers.GetProductYesNo();
        //                string WrongFiledName = "";
        //                if (status == 1)
        //                {
        //                    gridMasterModel.GrdFlg = 0;
        //                }
        //                else
        //                {
        //                    gridMasterModel.GrdFlg = 1;
        //                }
        //                int flg = gridMasterModel.GrdFlg;


        //                if (!string.IsNullOrWhiteSpace(queryflds))
        //                {
        //                    if (gridMasterModel.Gridtransaction.GrdADbFld != null)
        //                    {
        //                        for (int i = 0; i < gridMasterModel.Gridtransaction.GrdADbFld.Length; i++)
        //                        {
        //                            if (string.IsNullOrWhiteSpace(gridMasterModel.Gridtransaction.GrdADbFld[i]))
        //                            {
        //                                gridMasterModel.Gridtransaction.GrdADbFld[i] = "";
        //                            }
        //                            if (!Regex.Match(gridMasterModel.GrdQryFields.ToLower(), @"\b" + gridMasterModel.Gridtransaction.GrdADbFld[i].ToLower() + @"\b").Success)
        //                            {
        //                                WrongFiledName = gridMasterModel.Gridtransaction.GrdADbFld[i];
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    if (string.IsNullOrEmpty(WrongFiledName))
        //                    {
        //                        if (!string.IsNullOrWhiteSpace(DbConnection.ParseInt32(gridMasterModel.GrdMnuVou).ToString()) && !string.IsNullOrWhiteSpace(gridMasterModel.GrdType) && !string.IsNullOrWhiteSpace(gridMasterModel.GrdName))
        //                        {
        //                            SqlParameter[] sqlParameters = new SqlParameter[11];
        //                            sqlParameters[0] = new SqlParameter("@GrdMnuVou", gridMasterModel.GrdMnuVou);
        //                            sqlParameters[1] = new SqlParameter("@GrdType", gridMasterModel.GrdType);
        //                            sqlParameters[2] = new SqlParameter("@GrdName", gridMasterModel.GrdName);
        //                            sqlParameters[3] = new SqlParameter("@GrdMultiSelYN", gridMasterModel.GrdMultiSelYN);
        //                            sqlParameters[4] = new SqlParameter("@GrdQryFields", gridMasterModel.GrdQryFields);
        //                            sqlParameters[5] = new SqlParameter("@GrdQryJoin", gridMasterModel.GrdQryJoin);
        //                            sqlParameters[6] = new SqlParameter("@GrdQryOrderBy", gridMasterModel.GrdQryOrderBy);
        //                            if (flg == 0)
        //                            {
        //                                sqlParameters[7] = new SqlParameter("@GrdVou", 0);
        //                            }
        //                            else
        //                            {
        //                                sqlParameters[7] = new SqlParameter("@GrdVou", id);
        //                            }

        //                            sqlParameters[8] = new SqlParameter("@GrdDftYNo", gridMasterModel.GrdDftYNo);
        //                            sqlParameters[9] = new SqlParameter("@GrdTitle", gridMasterModel.GrdTitle);
        //                            sqlParameters[10] = new SqlParameter("@PageSize", gridMasterModel.PageSize);
        //                            DataTable DtGrid = ObjDBConnection.CallStoreProcedure("GridMaster_Insert", sqlParameters);
        //                            if (DtGrid != null && DtGrid.Rows.Count > 0)
        //                            {
        //                                int masterId = DbConnection.ParseInt32(DtGrid.Rows[0][0].ToString());
        //                                if (masterId > 0)
        //                                {
        //                                    if (gridMasterModel.Gridtransaction.GrdADbFld != null)
        //                                    {
        //                                        string[] aTotYN = gridMasterModel.GrdATotYNString.TrimEnd(',').Split(',');
        //                                        string[] canGrow = gridMasterModel.canGrowString.TrimEnd(',').Split(',');
        //                                        string[] aLinkYN = gridMasterModel.GrdALinkYNString.TrimEnd(',').Split(',');
        //                                        string[] aHideYN = gridMasterModel.GrdAHideYNString.TrimEnd(',').Split(',');
        //                                        for (int i = 0; i < gridMasterModel.Gridtransaction.GrdADbFld.Length; i++)
        //                                        {
        //                                            SqlParameter[] parameter = new SqlParameter[15];
        //                                            parameter[0] = new SqlParameter("@GrdAGrdVou", masterId);
        //                                            parameter[1] = new SqlParameter("@GrdADbFld", gridMasterModel.Gridtransaction.GrdADbFld[i]);
        //                                            parameter[2] = new SqlParameter("@GrdAColNm", gridMasterModel.Gridtransaction.GrdAColNm[i]);
        //                                            parameter[3] = new SqlParameter("@GrdANewColNm", gridMasterModel.Gridtransaction.GrdANewColNm[i]);
        //                                            parameter[4] = new SqlParameter("@GrdAPosition", gridMasterModel.Gridtransaction.GrdAPosition[i]);
        //                                            parameter[5] = new SqlParameter("@GrdADataType", gridMasterModel.Gridtransaction.GrdADataType[i]);
        //                                            parameter[6] = new SqlParameter("@GrdAWidth", gridMasterModel.Gridtransaction.GrdAWidth[i]);
        //                                            parameter[7] = new SqlParameter("@GrdADecUpTo", gridMasterModel.Gridtransaction.GrdADecUpTo[i]);
        //                                            parameter[8] = new SqlParameter("@GrdAAlign", gridMasterModel.Gridtransaction.GrdAAlign[i]);
        //                                            //parameter[9] = new SqlParameter("@GrdATotYN", gridMasterModel.Gridtransaction.GrdATotYN[i] ? "1" : "0");
        //                                            //parameter[10] = new SqlParameter("@GrdALinkYN", gridMasterModel.Gridtransaction.GrdALinkYN[i] ? "1" : "0");
        //                                            //parameter[11] = new SqlParameter("@GrdAHideYN", gridMasterModel.Gridtransaction.GrdAHideYN[i] ? "1" : "0");
        //                                            parameter[9] = new SqlParameter("@GrdATotYN", aTotYN[i]);
        //                                            parameter[10] = new SqlParameter("@GrdCanGrow", canGrow[i]);
        //                                            parameter[11] = new SqlParameter("@GrdALinkYN", aLinkYN[i]);
        //                                            parameter[12] = new SqlParameter("@GrdAHideYN", aHideYN[i]);
        //                                            parameter[13] = new SqlParameter("@GrdASuppressIFVal", gridMasterModel.Gridtransaction.GrdASuppressIFVal[i]);
        //                                            parameter[14] = new SqlParameter("@SrNo", (i + 1));
        //                                            DataTable DtGrdTrn = ObjDBConnection.CallStoreProcedure("GridTransaction_Insert", parameter);
        //                                        }
        //                                    }
        //                                    if (gridMasterModel.Gridshort.GrdBDbFld != null && gridMasterModel.Gridshort.GrdBDbFld[0] != null)
        //                                    {
        //                                        for (int i = 0; i < gridMasterModel.Gridshort.GrdBDbFld.Length; i++)
        //                                        {
        //                                            SqlParameter[] parameter = new SqlParameter[5];
        //                                            parameter[0] = new SqlParameter("@GrdBGrdVou", masterId);
        //                                            parameter[1] = new SqlParameter("@GrdBDbFld", gridMasterModel.Gridshort.GrdBDbFld[i]);
        //                                            parameter[2] = new SqlParameter("@GrdBColNm", gridMasterModel.Gridshort.GrdBColNm[i]);
        //                                            parameter[3] = new SqlParameter("@GrdBDefYN", gridMasterModel.Gridshort.GrdBDefYN[i] ? "1" : "0");
        //                                            parameter[4] = new SqlParameter("@SrNo", (i + 1));
        //                                            DataTable DtGrdTrn = ObjDBConnection.CallStoreProcedure("GridShort_Insert", parameter);
        //                                        }
        //                                    }
        //                                    int Status = DbConnection.ParseInt32(DtGrid.Rows[0][0].ToString());
        //                                    if (Status == 0)
        //                                    {
        //                                        gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
        //                                        SetErrorMessage("Dulplicate Type Details");
        //                                        ViewBag.FocusType = "1";
        //                                        return Json(true);
        //                                    }
        //                                    else
        //                                    {
        //                                        if (id > 0)
        //                                        {
        //                                            SetSuccessMessage("Updated Sucessfully");
        //                                        }
        //                                        else
        //                                        {
        //                                            SetSuccessMessage("Inserted Sucessfully");
        //                                        }
        //                                        return Json(true);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
        //                                    SetErrorMessage("Insert error");
        //                                    ViewBag.FocusType = "1";
        //                                    return Json(false);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
        //                                SetErrorMessage("Please Enter the Value");
        //                                ViewBag.FocusType = "1";
        //                                return Json(false);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
        //                            SetErrorMessage("Please Enter the Value");
        //                            ViewBag.FocusType = "1";
        //                            return Json(false);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
        //                        SetErrorMessage("“" + WrongFiledName + " ” field does not match in query");
        //                        ViewBag.FocusType = "1";
        //                        return Json(false);
        //                    }
        //                }
        //                else
        //                {
        //                    gridMasterModel.GrdVou = DbConnection.ParseInt32(id);
        //                    SetErrorMessage("Please Enter Mandatory Field");
        //                    ViewBag.FocusType = "1";
        //                    return Json(false);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    return Json(false);

        //}

    }
}
