using Microsoft.AspNetCore.Mvc.Rendering;
using PIOAccount.Classes;
using PIOAccount.Models;
using SocietyManagementWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SocietyManagementWeb.Classes
{
    public class AccountMasterHelpers
    {

        public DbConnection ObjDBConnection = new DbConnection();
        public List<SelectListItem> GetGroupNameDropdown(int companyId)
        {
            List<SelectListItem> AccountGroup = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@AgrVOU", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@cmpvou", companyId);
                DataTable DtGroup = ObjDBConnection.CallStoreProcedure("GetAccountGroupDetails", sqlParameters);
                if (DtGroup != null && DtGroup.Rows.Count > 0)
                {
                    for (int i = 0; i < DtGroup.Rows.Count; i++)
                    {
                        SelectListItem AccountGroupItem = new SelectListItem();
                        AccountGroupItem.Text = DtGroup.Rows[i]["AgrName"].ToString();
                        AccountGroupItem.Value = DtGroup.Rows[i]["AgrVou"].ToString();
                        AccountGroup.Add(AccountGroupItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return AccountGroup;
        }
        public List<SelectListItem> GetPurchaseOrderDropdown(int companyId)
        {
            List<SelectListItem> AccountGroup = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@OrmVOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "PORD");
                sqlParameters[2] = new SqlParameter("@cmpvou", companyId);
                DataTable DtGroup = ObjDBConnection.CallStoreProcedure("GetOrderDetails", sqlParameters);
                if (DtGroup != null && DtGroup.Rows.Count > 0)
                {
                    for (int i = 0; i < DtGroup.Rows.Count; i++)
                    {
                        SelectListItem AccountGroupItem = new SelectListItem();
                        AccountGroupItem.Text = DtGroup.Rows[i]["OrmVNo"].ToString();
                        AccountGroupItem.Value = DtGroup.Rows[i]["OrmVou"].ToString();
                        AccountGroup.Add(AccountGroupItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return AccountGroup;
        }
        public  List<CustomDropDown> GetCityNameCustomDropdown(int CompanyId)
        {
            List<CustomDropDown> CityNameList = new List<CustomDropDown>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@CtyVOU", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtCity = ObjDBConnection.CallStoreProcedure("GetCityDetails", sqlParameters);
                if (DtCity != null && DtCity.Rows.Count > 0)
                {
                    for (int i = 0; i < DtCity.Rows.Count; i++)
                    {
                        CustomDropDown cityItem = new CustomDropDown();
                        cityItem.Text = DtCity.Rows[i]["CtyNm"].ToString();
                        cityItem.Value = DtCity.Rows[i]["CtyVou"].ToString();
                        cityItem.Value1 = DtCity.Rows[i]["CtyState"].ToString();
                        cityItem.Value2 = DtCity.Rows[i]["CtyStaVou"].ToString();
                        CityNameList.Add(cityItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return CityNameList;
        }
        public List<SelectListItem> GetDesignationDropdown(int CompanyId)
        {
            List<SelectListItem> CityName = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "DSG");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtCity = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtCity != null && DtCity.Rows.Count > 0)
                {
                    for (int i = 0; i < DtCity.Rows.Count; i++)
                    {
                        SelectListItem CityItem = new SelectListItem();
                        CityItem.Text = DtCity.Rows[i]["MScNm"].ToString();
                        CityItem.Value = DtCity.Rows[i]["MscVou"].ToString();
                        CityName.Add(CityItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return CityName;
        }

        public List<SelectListItem> GetDepartmentMscDropdown(int CompanyId)
        {
            List<SelectListItem> DepName = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "DEP");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtDep = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtDep != null && DtDep.Rows.Count > 0)
                {
                    for (int i = 0; i < DtDep.Rows.Count; i++)
                    {
                        SelectListItem DepItem = new SelectListItem();
                        DepItem.Text = DtDep.Rows[i]["MScNm"].ToString();
                        DepItem.Value = DtDep.Rows[i]["MscVou"].ToString();
                        DepName.Add(DepItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return DepName;
        }

        public List<SelectListItem> GetUnitDropdown(int CompanyId)
        {
            List<SelectListItem> CityName = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "UNT");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtCity = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtCity != null && DtCity.Rows.Count > 0)
                {
                    for (int i = 0; i < DtCity.Rows.Count; i++)
                    {
                        SelectListItem CityItem = new SelectListItem();
                        CityItem.Text = DtCity.Rows[i]["MScNm"].ToString();
                        CityItem.Value = DtCity.Rows[i]["MscVou"].ToString();
                        CityName.Add(CityItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return CityName;
        }

        public List<SelectListItem> GetStateDropdown(int CompanyId)
        {
            List<SelectListItem> CityName = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "STA");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtCity = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtCity != null && DtCity.Rows.Count > 0)
                {
                    for (int i = 0; i < DtCity.Rows.Count; i++)
                    {
                        SelectListItem CityItem = new SelectListItem();
                        CityItem.Text = DtCity.Rows[i]["MScNm"].ToString();
                        CityItem.Value = DtCity.Rows[i]["MscVou"].ToString();
                        CityName.Add(CityItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return CityName;
        }

        public List<SelectListItem> GetCountryDropdown(int CompanyId)
        {
            List<SelectListItem> CityName = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "COU");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtCity = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtCity != null && DtCity.Rows.Count > 0)
                {
                    for (int i = 0; i < DtCity.Rows.Count; i++)
                    {
                        SelectListItem CityItem = new SelectListItem();
                        CityItem.Text = DtCity.Rows[i]["MScNm"].ToString();
                        CityItem.Value = DtCity.Rows[i]["MscVou"].ToString();
                        CityName.Add(CityItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return CityName;
        }

        public List<SelectListItem> GetGradeDropdown(int CompanyId)
        {
            List<SelectListItem> GradeName = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "GRD");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtGrade = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtGrade != null && DtGrade.Rows.Count > 0)
                {
                    for (int i = 0; i < DtGrade.Rows.Count; i++)
                    {
                        SelectListItem GradeItem = new SelectListItem();
                        GradeItem.Text = DtGrade.Rows[i]["MScNm"].ToString();
                        GradeItem.Value = DtGrade.Rows[i]["MscVou"].ToString();
                        GradeName.Add(GradeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return GradeName;
        }

        public List<SelectListItem> GetSpacificationDropdown(int CompanyId)
        {
            List<SelectListItem> SpacifiName = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "SPE");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtSpec = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtSpec != null && DtSpec.Rows.Count > 0)
                {
                    for (int i = 0; i < DtSpec.Rows.Count; i++)
                    {
                        SelectListItem SpacifiItem = new SelectListItem();
                        SpacifiItem.Text = DtSpec.Rows[i]["MScNm"].ToString();
                        SpacifiItem.Value = DtSpec.Rows[i]["MscVou"].ToString();
                        SpacifiName.Add(SpacifiItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return SpacifiName;
        }
        public List<SelectListItem> GetPrdTypeDropdown(int CompanyId)
        {
            List<SelectListItem> PrdType = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "PTY");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtCity = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtCity != null && DtCity.Rows.Count > 0)
                {
                    for (int i = 0; i < DtCity.Rows.Count; i++)
                    {
                        SelectListItem PrdTypeItem = new SelectListItem();
                        PrdTypeItem.Text = DtCity.Rows[i]["MScNm"].ToString();
                        PrdTypeItem.Value = DtCity.Rows[i]["MscVou"].ToString();
                        PrdType.Add(PrdTypeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PrdType;
        }
        public List<CustomDropDown> GetEmployeeCustomDropdown(int CompanyId, int isAdministrator)
        {
            List<CustomDropDown> EmployeeList = new List<CustomDropDown>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@EmpVou", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 2);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetEmployeeDetails", sqlParameters);
                if (DtEmp != null && DtEmp.Rows.Count > 0)
                {
                    for (int i = 0; i < DtEmp.Rows.Count; i++)
                    {
                        CustomDropDown EmployeeItem = new CustomDropDown();
                        EmployeeItem.Text = DtEmp.Rows[i]["EmpNm"].ToString();
                        EmployeeItem.Value = DtEmp.Rows[i]["EmpVou"].ToString();
                        EmployeeItem.Value1 = DtEmp.Rows[i]["EmpMob"].ToString();
                        EmployeeItem.Value2 = DtEmp.Rows[i]["EmpEmail"].ToString();
                        EmployeeItem.Value3 = DtEmp.Rows[i]["EmpPhoto"].ToString();
                        EmployeeList.Add(EmployeeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return EmployeeList;
        }

        public List<CustomDropDown> GetOperatorCustomDropdown(int CompanyId, int isAdministrator)
        {
            List<CustomDropDown> EmployeeList = new List<CustomDropDown>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@EmpVou", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 3);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetEmployeeDetails", sqlParameters);
                if (DtEmp != null && DtEmp.Rows.Count > 0)
                {
                    for (int i = 0; i < DtEmp.Rows.Count; i++)
                    {
                        CustomDropDown EmployeeItem = new CustomDropDown();
                        EmployeeItem.Text = DtEmp.Rows[i]["EmpNm"].ToString();
                        EmployeeItem.Value = DtEmp.Rows[i]["EmpVou"].ToString();
                        EmployeeItem.Value1 = DtEmp.Rows[i]["EmpMob"].ToString();
                        EmployeeItem.Value2 = DtEmp.Rows[i]["EmpEmail"].ToString();
                        EmployeeItem.Value3 = DtEmp.Rows[i]["EmpPhoto"].ToString();
                        EmployeeList.Add(EmployeeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return EmployeeList;
        }

        public List<SelectListItem> GetOperatorCustomDropdown_New(int CompanyId, int isAdministrator)
        {
            List<SelectListItem> EmployeeList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@EmpVou", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 3);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetEmployeeDetails", sqlParameters);
                if (DtEmp != null && DtEmp.Rows.Count > 0)
                {
                    for (int i = 0; i < DtEmp.Rows.Count; i++)
                    {
                        SelectListItem EmployeeItem = new SelectListItem();
                        EmployeeItem.Text = DtEmp.Rows[i]["EmpNm"].ToString();
                        EmployeeItem.Value = DtEmp.Rows[i]["EmpVou"].ToString();
                        EmployeeList.Add(EmployeeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return EmployeeList;
        }

        public List<CustomDropDown> GetSupervisorCustomDropdown(int CompanyId, int isAdministrator)
        {
            List<CustomDropDown> EmployeeList = new List<CustomDropDown>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@EmpVou", 0);
                sqlParameters[1] = new SqlParameter("@Flg", 4);
                sqlParameters[2] = new SqlParameter("@skiprecord", 0);
                sqlParameters[3] = new SqlParameter("@pagesize", 0);
                sqlParameters[4] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtEmp = ObjDBConnection.CallStoreProcedure("GetEmployeeDetails", sqlParameters);
                if (DtEmp != null && DtEmp.Rows.Count > 0)
                {
                    for (int i = 0; i < DtEmp.Rows.Count; i++)
                    {
                        CustomDropDown EmployeeItem = new CustomDropDown();
                        EmployeeItem.Text = DtEmp.Rows[i]["EmpNm"].ToString();
                        EmployeeItem.Value = DtEmp.Rows[i]["EmpVou"].ToString();
                        EmployeeItem.Value1 = DtEmp.Rows[i]["EmpMob"].ToString();
                        EmployeeItem.Value2 = DtEmp.Rows[i]["EmpEmail"].ToString();
                        EmployeeItem.Value3 = DtEmp.Rows[i]["EmpPhoto"].ToString();
                        EmployeeList.Add(EmployeeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return EmployeeList;
        }

        public List<SelectListItem> GetUserRoleDropdown(int CompanyId)
        {
            List<SelectListItem> UserRoleList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[5];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "ROL");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
               // sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtRole = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtRole != null && DtRole.Rows.Count > 0)
                {
                    for (int i = 0; i < DtRole.Rows.Count; i++)
                    {
                        SelectListItem UserRoleItem = new SelectListItem();
                        UserRoleItem.Text = DtRole.Rows[i]["MScNm"].ToString();
                        UserRoleItem.Value = DtRole.Rows[i]["MscVou"].ToString();
                        UserRoleList.Add(UserRoleItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return UserRoleList;
        }

        public List<SelectListItem> GetMachineMasterDropdown(int CompanyId,string jobType)
        {
            List<SelectListItem> MachTypeList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@MacVou", 0);
                sqlParameters[1] = new SqlParameter("@MacMtyCd", jobType);
                sqlParameters[2] = new SqlParameter("@Flg", 3);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtMacTy = ObjDBConnection.CallStoreProcedure("GetMachineDetails", sqlParameters);
                if (DtMacTy != null && DtMacTy.Rows.Count > 0)
                {
                    for (int i = 0; i < DtMacTy.Rows.Count; i++)
                    {
                        SelectListItem MachTypeItem = new SelectListItem();
                        MachTypeItem.Text = DtMacTy.Rows[i]["MacNm"].ToString();
                        MachTypeItem.Value = DtMacTy.Rows[i]["MacVou"].ToString();
                        MachTypeList.Add(MachTypeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return MachTypeList;
        }

        public List<SelectListItem> GetNBMasterDropdown(int CompanyId)
        {
            List<SelectListItem> NBList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@NbsVou", 0);
                sqlParameters[1] = new SqlParameter("@skiprecord", 0);
                sqlParameters[2] = new SqlParameter("@pagesize", 0);
                sqlParameters[3] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtNB = ObjDBConnection.CallStoreProcedure("GetNBSCHMasterDetails", sqlParameters);
                if (DtNB != null && DtNB.Rows.Count > 0)
                {
                    for (int i = 0; i < DtNB.Rows.Count; i++)
                    {
                        SelectListItem NBItem = new SelectListItem();
                        NBItem.Text = DtNB.Rows[i]["NbsNB"].ToString();
                        NBItem.Value = DtNB.Rows[i]["NbsNB"].ToString();
                        NBList.Add(NBItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return NBList;
        }

        public List<SelectListItem> GetSCHMasterDropdown(int CompanyId)
        {
            List<SelectListItem> SCHList = new List<SelectListItem>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[4];
                sqlParameters[0] = new SqlParameter("@NbsVou", 0);
                sqlParameters[1] = new SqlParameter("@skiprecord", 0);
                sqlParameters[2] = new SqlParameter("@pagesize", 0);
                sqlParameters[3] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtSCH = ObjDBConnection.CallStoreProcedure("GetSCHMasterDetails", sqlParameters);
                if (DtSCH != null && DtSCH.Rows.Count > 0)
                {
                    for (int i = 0; i < DtSCH.Rows.Count; i++)
                    {
                        SelectListItem SCHItem = new SelectListItem();
                        SCHItem.Text = DtSCH.Rows[i]["NbsSCH"].ToString();
                        SCHItem.Value = DtSCH.Rows[i]["NbsSCH"].ToString();
                        SCHList.Add(SCHItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return SCHList;
        }

        public List<CustomDropDown> GetTranTypeCustomDropdown(int CompanyId, int isAdministrator)
        {
            List<CustomDropDown> TranType = new List<CustomDropDown>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[6];
                sqlParameters[0] = new SqlParameter("@VOU", 0);
                sqlParameters[1] = new SqlParameter("@Type", "TRN");
                sqlParameters[2] = new SqlParameter("@Flg", 2);
                sqlParameters[3] = new SqlParameter("@skiprecord", 0);
                sqlParameters[4] = new SqlParameter("@pagesize", 0);
                sqlParameters[5] = new SqlParameter("@CmpVou", CompanyId);
                DataTable DtTrans = ObjDBConnection.CallStoreProcedure("GetMscMstDetails", sqlParameters);
                if (DtTrans != null && DtTrans.Rows.Count > 0)
                {
                    for (int i = 0; i < DtTrans.Rows.Count; i++)
                    {
                        CustomDropDown TranTypeItem = new CustomDropDown();
                        TranTypeItem.Text = DtTrans.Rows[i]["MScNm"].ToString();
                        TranTypeItem.Value = DtTrans.Rows[i]["MscVou"].ToString();
                        TranTypeItem.Value1 = DtTrans.Rows[i]["MscCd"].ToString();
                        TranType.Add(TranTypeItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return TranType;
        }

        public List<SelectListItem> GetAccountType()
        {
            List<SelectListItem> AccTypeList = new List<SelectListItem>();
            try
            {
                AccTypeList.Add(new SelectListItem
                {
                    Text = "Party",
                    Value = "1"
                });

                AccTypeList.Add(new SelectListItem
                {
                    Text = "Income/Expenses",
                    Value = "2"
                });
                AccTypeList.Add(new SelectListItem
                {
                    Text = "Transport",
                    Value = "3"
                });
                AccTypeList.Add(new SelectListItem
                {
                    Text = "Other",
                    Value = "4"
                });
                AccTypeList.Add(new SelectListItem
                {
                    Text = "Sales Person",
                    Value = "5"
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return AccTypeList;
        }

        public List<SelectListItem> GetMemberRole()
        {
            List<SelectListItem> AccTypeList = new List<SelectListItem>();
            try
            {
                AccTypeList.Add(new SelectListItem
                {
                    Text = "Admin",
                    Value = "1"
                });

                AccTypeList.Add(new SelectListItem
                {
                    Text = "Member",
                    Value = "2"
                });
                AccTypeList.Add(new SelectListItem
                {
                    Text = "Gate Keeper",
                    Value = "3"
                });


                AccTypeList.Add(new SelectListItem
                {
                    Text = "Guest",
                    Value = "4"
                });
                AccTypeList.Add(new SelectListItem
                {
                    Text = "Society Staff",
                    Value = "5"
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return AccTypeList;
        }

    }
}
