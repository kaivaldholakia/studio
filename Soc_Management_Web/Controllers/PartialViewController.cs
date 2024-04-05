using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PIOAccount.Controllers
{
    public class PartialViewController : BaseController
    {
        DbConnection objDbConnection = new DbConnection();
        [HttpPost]
        public IActionResult MscMstPartialViewAdd(string name, string code, int position, string type, int id, string activeyn)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(code))
                {
                    long userId = GetIntSession("UserId");
                    int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
                    int clientId = Convert.ToInt32(GetIntSession("ClientId"));
                    SqlParameter[] sqlParameters = new SqlParameter[11];
                    sqlParameters[0] = new SqlParameter("@MscName", name);
                    sqlParameters[1] = new SqlParameter("@MscCode", code);
                    sqlParameters[2] = new SqlParameter("@MscType", type);
                    sqlParameters[3] = new SqlParameter("@MscPos", position);
                    sqlParameters[4] = new SqlParameter("@MscActYN", activeyn);
                    sqlParameters[5] = new SqlParameter("@MscVou", id);
                    sqlParameters[6] = new SqlParameter("@CmpVou", companyId);
                    sqlParameters[7] = new SqlParameter("@UsrVou", userId);
                    sqlParameters[8] = new SqlParameter("@Flg", 1);
                    sqlParameters[9] = new SqlParameter("@MscCmpCdN", companyId);
                    if (type == "ROL")
                    {
                        sqlParameters[10] = new SqlParameter("@MscCliVou", clientId);
                    }
                    else
                    {
                        sqlParameters[10] = new SqlParameter("@MscCliVou", 0);
                    }
                    DataTable DtMscMst = objDbConnection.CallStoreProcedure("MscMst_Insert", sqlParameters);
                    if (DtMscMst != null && DtMscMst.Rows.Count > 0)
                    {
                        int status = DbConnection.ParseInt32(DtMscMst.Rows[0][0].ToString());
                        if (type == "DSG")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/Designation/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Designation inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/Designation/Index" });
                                }
                            }
                        }
                        else if (type == "UNT")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/UnitMaster/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Unit inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/UnitMaster/Index" });
                                }
                            }
                        }
                        else if (type == "STA")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/StateMaster/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("State inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/StateMaster/Index" });
                                }
                            }
                        }
                        else if (type == "ROL")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/UserRoll/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("User Roll inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/UserRoll/Index" });
                                }
                            }
                        }
                        else if (type == "DEP")
                        {
                            if (status == -1)
                            {
                                return Json(new { result = false, Message = "Duplicate Code" });
                            }
                            else if (status == -2)
                            {
                                return Json(new { result = false, Message = "Duplicate Name" });
                            }
                            else
                            {
                                if (id > 0)
                                {
                                    SetSuccessMessage("Update Sucessfully");
                                    return Json(new { result = true, RedirectURL = "/DepMst/Index" });
                                }
                                else
                                {
                                    SetSuccessMessage("Department inserted successfully");
                                    return Json(new { result = true, RedirectURL = "/DepMst/Index" });
                                }
                            }
                        }
                    }

                }
                else
                {
                    return Json(new { result = false, Message = "Please Enter the Value" });
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { result = false });
        }
    }
}
