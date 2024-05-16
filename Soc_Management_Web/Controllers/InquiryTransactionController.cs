using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using System;
using Soc_Management_Web.Models;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Net.Mail;
using System.Net;
using Soc_Management_Web.Classes;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using Soc_Management_Web.Common;

namespace Soc_Management_Web.Controllers
{
	public class InquiryTransactionController : BaseController
	{
		DbConnection ObjDBConnection = new DbConnection();
		ProductHelpers objProductHelper = new ProductHelpers();
		private readonly IWebHostEnvironment _hostingEnvironment;
		MasterDropdownHelper master = new MasterDropdownHelper();
		static string fontpath;
		public InquiryTransactionController(IWebHostEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
			string folderPath1 = Path.Combine(_hostingEnvironment.WebRootPath, "noto_sans");
			string fileName = "static/NotoSans-Regular.ttf";
			fontpath = Path.Combine(folderPath1, fileName);
		}
		public IActionResult Index(long id,string type=null)
		{
			InqueryMasterModel model = new InqueryMasterModel();
					
			bool isreturn = false;
			INIT(ref isreturn);

			if (!string.IsNullOrEmpty(type))
			{
				model.IsContact = true;
			}
			if (isreturn)
			{
				return RedirectToAction("index", "dashboard");
			}
			long userId = GetIntSession("UserId");
			long societyid = id;
			int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
			int administrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
			int clientId = 0;
			if (id > 0)
			{
				SqlParameter[] sqlParameters = new SqlParameter[2];
				sqlParameters[0] = new SqlParameter("@InqVou", id);
				sqlParameters[1] = new SqlParameter("@Flg", 5);
				DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_InquiryMst_Insert", sqlParameters);
				if (DtEmp != null && DtEmp.Rows.Count > 0)
				{
					model.InquiryVou = Convert.ToInt32(id);
					model.InquiryNo = Convert.ToInt32(DtEmp.Rows[0]["InqNo"].ToString());
					model.InquiryDate = DtEmp.Rows[0]["InqDt"].ToString();
					model.Customerid = Convert.ToInt32(DtEmp.Rows[0]["InqAccVou"].ToString());
					model.InqAmount = DtEmp.Rows[0]["InqAmt"].ToString();
					model.Remarks = DtEmp.Rows[0]["InqRem"].ToString();
					model.InquiryTitle = DtEmp.Rows[0]["InqTitle"].ToString();
					model.MobileNo = DtEmp.Rows[0]["Mob"].ToString();
					model.Address = DtEmp.Rows[0]["add1"].ToString();
					model.RefId = DtEmp.Rows[0]["InqRefBy"].ToString();
					model.InqStatusId = Convert.ToInt32(DtEmp.Rows[0]["InqDoneFlag"].ToString());
					model.InqSubTitle = DtEmp.Rows[0]["InqSubTittle"].ToString();
					model.DiscountAmount = Convert.ToDecimal(DtEmp.Rows[0]["DiscounAmount"].ToString());
					model.NetAmount = Convert.ToDecimal(DtEmp.Rows[0]["NetAmount"].ToString());
					model.manual = Convert.ToBoolean(DtEmp.Rows[0]["ManualFlag"].ToString());
					model.OdrNo = DtEmp.Rows[0]["OrdNo"].ToString();
				}
			}
			else
			{
				SqlParameter[] sqlParameters1 = new SqlParameter[1];
				sqlParameters1[0] = new SqlParameter("@Flag", 1);
				DataTable DtEmp1 = ObjDBConnection.CallStoreProcedure("DeleteDuplicaterecord", sqlParameters1);

				model.InquiryNo = objProductHelper.GetLstInqNo() + 1;
				model.InquiryDate = DateTime.Now.ToString("dd/MM/yyyy");
			}

			//  model.lstCustomer = objProductHelper.GetCustomerList();
			model.lstInqStatus = master.GetDropgen("Status");
			model.lstExtraitem = objProductHelper.GetlstExtraItemsDropdown();
			model.lstReference = objProductHelper.GetlstRefByDropdown();
			model.lstTermsAndCondition = objProductHelper.GetlstTearmAndConditionDropdown();

			model.lstJobmaster = objProductHelper.GetJobMasterLst();
			model.EventLst = objProductHelper.GetEventLst();
			return View(model);
		}

		public IActionResult CustomerDropdown()
		{
			var customerList = objProductHelper.GetCustomerListOnly();
			var selectList = new SelectList(customerList, "Value", "Text");
			return Json(selectList);
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

		[HttpPost]
		public ActionResult Index(long id, InqueryMasterModel obj)
		{
			string obj1 = "";
			try
			{
				bool isreturn = false;
				INIT(ref isreturn);

				long userId = GetIntSession("UserId");
				int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
				if (!string.IsNullOrWhiteSpace(Convert.ToString(obj.InquiryNo)))
				{
					SqlParameter[] sqlParameters = new SqlParameter[16];
					sqlParameters[0] = new SqlParameter("@InqVou", id);
					sqlParameters[1] = new SqlParameter("@InqNo", obj.InquiryNo);
					sqlParameters[2] = new SqlParameter("@InqDt", obj.InquiryDate);
					sqlParameters[3] = new SqlParameter("@OdrNo", obj.OdrNo);
					sqlParameters[4] = new SqlParameter("@Custmrid", obj.Customerid);
					sqlParameters[5] = new SqlParameter("@RefId", obj.RefId != null ? obj.RefId : ""); 
					sqlParameters[6] = new SqlParameter("@InqTitl", obj.InquiryTitle);
					sqlParameters[7] = new SqlParameter("@InqRmks", obj.Remarks);
					sqlParameters[8] = new SqlParameter("@InqSts", obj.InqStatusId);
					sqlParameters[9] = new SqlParameter("@InqAmt", obj.InqAmount != null ? obj.InqAmount : 0);
					
					sqlParameters[10] = new SqlParameter("@DiscountAmount", obj.DiscountAmount != null ? obj.DiscountAmount : 0);

					sqlParameters[11] = new SqlParameter("@NetAmount", obj.NetAmount!=null? obj.NetAmount:0);
					sqlParameters[12] = new SqlParameter("@InqSubTitle", obj.InqSubTitle);
					sqlParameters[13] = new SqlParameter("@FLG", "1");
					sqlParameters[14] = new SqlParameter("@ManualFlag", obj.manual == true ? 1 : 0);
					sqlParameters[15] = new SqlParameter("@Mobile", obj.MobileNo);
					DataTable DtCat = ObjDBConnection.CallStoreProcedure("usp_InquiryMst_Insert", sqlParameters);
					if (DtCat != null && DtCat.Rows.Count > 0)
					{
						int status = DbConnection.ParseInt32(DtCat.Rows[0][0].ToString());
						if (status < 0)
						{
							id = status;
							obj1 = "Dulplicate Category Details";
							return RedirectToAction("index", "InquiryTransaction", new { id = id });
						}
						else
						{


							if (id > 0)
							{
								obj1 = "Update Sucessfully";
								SetSuccessMessage("Update Sucessfully");
							}
							else
							{
								id = status;
								obj1 = "Inserted Sucessfully";
								SetSuccessMessage("Inserted Sucessfully");

							}
							//Request.HttpContext.Session["id"] = id;
							return RedirectToAction("index", "InquiryTransaction", new { id = id });
						}
					}
					else
					{
						return RedirectToAction("index", "InquiryTransaction", new { id = 0 });
					}
				}
				else
				{
					return RedirectToAction("index", "InquiryTransaction", new { id = 0 });
				}
			}
			catch (Exception ex)
			{
				throw;
			}

			return RedirectToAction("index", "InquiryTransaction", new { id = 0 });
		}

		public IActionResult GetReportView(int gridMstId, int pageIndex, int pageSize, string searchValue, string columnName, string sortby)
		{
			GetReportDataModel getReportDataModel = new GetReportDataModel();
			try
			{
				if (gridMstId > 0)
				{
					#region User Rights
					long userId = GetIntSession("UserId");
					UserFormRightModel userFormRights = new UserFormRightModel();
					string currentURL = "/InquiryTransaction/Index";
					userFormRights = GetUserRights(userId, currentURL);
					if (userFormRights == null)
					{
						SetErrorMessage("You do not have right to access requested page. Please contact admin for more detail.");
					}
					ViewBag.userRight = userFormRights;
					#endregion

					int companyId = Convert.ToInt32(GetIntSession("CompanyId"));

					double startRecord = 0;
					if (pageIndex > 0)
					{
						startRecord = (pageIndex - 1) * pageSize;
					}
					getReportDataModel = GetReportData(gridMstId, pageIndex, pageSize, columnName, sortby, searchValue, companyId);
					if (getReportDataModel.IsError)
					{
						ViewBag.Query = getReportDataModel.Query;
						return PartialView("_reportView");
					}
					getReportDataModel.pageIndex = pageIndex;
					getReportDataModel.ControllerName = "InquiryTransaction";
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return PartialView("_reportView", getReportDataModel);
		}

		[HttpPost]
		public IActionResult GetExtraItem()
		{
			try
			{
				List<Extraitem> etrItmObj = new List<Extraitem>();
				SqlParameter[] sqlParameters = new SqlParameter[1];
				sqlParameters[0] = new SqlParameter("@Flg", 5);
				DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Usp_ExtraItems_Insert", sqlParameters);
				for (var item = 0; item < DtEmp.Rows.Count; item++)
				{
					Extraitem obj = new Extraitem();
					obj.EtrVou = Convert.ToInt32(DtEmp.Rows[item]["EitVou"].ToString());
					obj.extraitem = DtEmp.Rows[item]["EitNm"].ToString();
					obj.amount = Convert.ToInt32(DtEmp.Rows[item]["EitAmt"].ToString());
					etrItmObj.Add(obj);
				}
				return Json(new { str = etrItmObj });
			}
			catch (Exception ex)
			{
				throw;
			}


		}

		[HttpPost]
		public IActionResult GetNewInquiryNumber()
		{
			try
			{
				List<Extraitem> etrItmObj = new List<Extraitem>();
				SqlParameter[] sqlParameters = new SqlParameter[1];
				sqlParameters[0] = new SqlParameter("@Flg", 5);
				DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Usp_ExtraItems_Insert", sqlParameters);
				for (var item = 0; item < DtEmp.Rows.Count; item++)
				{
					Extraitem obj = new Extraitem();
					obj.EtrVou = Convert.ToInt32(DtEmp.Rows[item]["EitVou"].ToString());
					obj.extraitem = DtEmp.Rows[item]["EitNm"].ToString();
					obj.amount = Convert.ToInt32(DtEmp.Rows[item]["EitAmt"].ToString());
					etrItmObj.Add(obj);
				}
				return Json(new { str = etrItmObj });
			}
			catch (Exception ex)
			{
				throw;
			}


		}

		[HttpPost]
		public IActionResult GetMobAndAdrs(int id)
		{
			SqlParameter[] sqlParameters = new SqlParameter[3];
			sqlParameters[0] = new SqlParameter("@InqVou", "0");
			sqlParameters[1] = new SqlParameter("@Custmrid", id);
			sqlParameters[2] = new SqlParameter("@FLG", "3");
			DataTable DtCat = ObjDBConnection.CallStoreProcedure("usp_InquiryMst_Insert", sqlParameters);
			object obj = null;
			if (DtCat.Rows.Count > 0)
			{
				obj = new
				{
					Address = DtCat.Rows[0]["Address"].ToString(),
					Mobile = DtCat.Rows[0]["MobNo"].ToString(),
					refrence = DtCat.Rows[0]["AcrNm"].ToString()
				};
			}
			return Json(new { obj });
		}

		[HttpPost]
		public IActionResult GetRenderPartialView(string idNm = "")
		{
			InquerySubMasterModel objModel = new InquerySubMasterModel();
			if (idNm == "home-tab")
			{
				return PartialView("_InqJobView", objModel);
			}
			else
			{
				return null;
			}

		}

		// Tab Show Area
		public PartialViewResult listdata()
		{
			return PartialView("_ListEnquiry");
		}
		public PartialViewResult Adddata(long id = 0)
		{
			ViewBag.InqueryId = id;
			return PartialView("_AddEnquiry");
		}
		public PartialViewResult DetailstabLoad(long id = 0)
		{
			ViewBag.InqueryId = id;
			InquerySubMasterModel objModel = new InquerySubMasterModel();

			return PartialView("_InqJobView", objModel);

			// return PartialView("_JobDetails");
		}
		public PartialViewResult ExraitemsLoad(long id = 0)
		{
			try
			{
				List<Extraitem> extraItems = new List<Extraitem>();

				var sqlParameters = new SqlParameter[5];
				sqlParameters[0] = new SqlParameter("@ExtraItem", "");
				sqlParameters[1] = new SqlParameter("@Amount", 0);
				sqlParameters[2] = new SqlParameter("@InQId", id);
				sqlParameters[3] = new SqlParameter("@EIId", 0);
				sqlParameters[4] = new SqlParameter("@FLG", 3);

				using (DataTable dtEmp = ObjDBConnection.CallStoreProcedure("Add_Inq_ExtraItem_Temp", sqlParameters))
				{
					// Assuming there is a method to map DataTable to a List<Extraitem>
					extraItems = MapDataTableToExtraItemList(dtEmp);
				}

				ViewBag.InqueryId = id;
				return PartialView("_Exraitems", extraItems);
			}
			catch (Exception ex)
			{
				// Handle the exception, log it, or return an error view
				// Example: return View("Error", new ErrorViewModel { Message = ex.Message });
				throw;
			}
		}

		private List<Extraitem> MapDataTableToExtraItemList(DataTable dataTable)
		{
			List<Extraitem> extraItems = new List<Extraitem>();

			foreach (DataRow row in dataTable.Rows)
			{
				Extraitem extraItem = new Extraitem
				{
					IteValue = Convert.ToInt32(row["IteVou"]),
					extraitem = row["IteEitNm"].ToString(),
					Sl = Convert.ToInt32(row["IteSrNo"]),
					amount = Convert.ToInt32(row["IteEitAmt"]),
					// Map other properties accordingly
				};

				extraItems.Add(extraItem);
			}

			return extraItems;
		}

		//private List<Inclusive> MapDataTableToInclusiveToInclusive(DataTable dataTable)
		//{
		//    List<Inclusive> INEx = new List<Inclusive>();
		//        foreach (DataRow row in dataTable.Rows)
		//        {
		//            Inclusive INExa = new Inclusive
		//            {
		//                Id = Convert.ToInt32(row["IncVou"]),
		//                Tranid = Convert.ToInt32(row["IncInqVou"]),
		//                sl = Convert.ToInt32(row["incSrNo"]),
		//                Item = row["IncTncNm"].ToString(),
		//                description = row["IncTncType"].ToString()
		//            };

		//            INEx.Add(INExa);
		//        }


		//    return INEx;
		//}

		//private List<Enclusive> MapDataTableToInclusiveToEsclusive(DataTable dataTable)
		//{
		//    List<Enclusive> INEx = new List<Enclusive>();
		//       foreach (DataRow row in dataTable.Rows)
		//        {
		//        Enclusive INExa = new Enclusive
		//            {
		//                Id = Convert.ToInt32(row["IteVou"]),
		//                Tranid = Convert.ToInt32(row["IExInqVou"]),
		//                sl = Convert.ToInt32(row["IExSRNo"]),
		//                Item = row["IExTncNm"].ToString(),
		//                description = row["IExTncDesc"].ToString()
		//            };

		//            INEx.Add(INExa);
		//        }

		//    return INEx;
		//}
		[HttpPost]
		public JsonResult DeleteExtraItem(int extraItemId)
		{
			try
			{

				List<Extraitem> extraItems = new List<Extraitem>();

				var sqlParameters = new SqlParameter[5];
				sqlParameters[0] = new SqlParameter("@ExtraItem", "");
				sqlParameters[1] = new SqlParameter("@Amount", 0);
				sqlParameters[2] = new SqlParameter("@InQId", 2);
				sqlParameters[3] = new SqlParameter("@EIId", extraItemId);
				sqlParameters[4] = new SqlParameter("@FLG", 2);

				using (DataTable dtEmp = ObjDBConnection.CallStoreProcedure("Add_Inq_ExtraItem_Temp", sqlParameters))
				{
					return Json(new { success = true, message = "Record deleted successfully" });
				}


			}
			catch (Exception ex)
			{
				// Handle the exception, log it, or return an error view
				// Example: return View("Error", new ErrorViewModel { Message = ex.Message });
				throw;
			}

			return Json(new { success = true, message = "Record deleted successfully" });

		}

		[HttpPost]
		public JsonResult DeleteTermsAndCondition(int extraItemId)
		{
			try
			{

				List<Extraitem> extraItems = new List<Extraitem>();

				var sqlParameters = new SqlParameter[2];
				sqlParameters[0] = new SqlParameter("@InTcVou", extraItemId);

				sqlParameters[1] = new SqlParameter("@FLG", 2);

				using (DataTable dtEmp = ObjDBConnection.CallStoreProcedure("Add_Inq_Inclusive", sqlParameters))
				{
					return Json(new { success = true, message = "Record deleted successfully" });
				}


			}
			catch (Exception ex)
			{
				// Handle the exception, log it, or return an error view
				// Example: return View("Error", new ErrorViewModel { Message = ex.Message });
				throw;
			}

			return Json(new { success = true, message = "Record deleted successfully" });

		}


		public PartialViewResult InclusiveexclusivetabLoad(long id = 0)
		{
			List<InclusiveExclusiveModel> inlst = new List<InclusiveExclusiveModel>();
			try
			{

				var sqlParameters = new SqlParameter[6];
				sqlParameters[0] = new SqlParameter("@Tranid", id);
				sqlParameters[1] = new SqlParameter("@Id", 0);
				sqlParameters[2] = new SqlParameter("@Item", 2);
				sqlParameters[3] = new SqlParameter("@Description", "");
				sqlParameters[4] = new SqlParameter("@Type", 0);
				sqlParameters[5] = new SqlParameter("@FLG", 3);


				using (DataTable dt = ObjDBConnection.CallStoreProcedure("Add_Inq_InclusiveExclusive", sqlParameters))
				{
					foreach (DataRow row in dt.Rows)
					{
						InclusiveExclusiveModel INExa = new InclusiveExclusiveModel
						{
							Id = Convert.ToInt32(row["Id"]),
							Tranid = Convert.ToInt32(row["TranId"]),
							sl = Convert.ToInt32(row["Sl"]),
							Item = row["Item"].ToString(),
							description = row["Description"].ToString(),
							Type = row["Type"].ToString(),
						};
						inlst.Add(INExa);
					}
				}

				ViewBag.InqueryId = id;
				return PartialView("_InclusiveExclusive", inlst);
			}
			catch (Exception ex)
			{
				// Handle the exception, log it, or return an error view
				// Example: return View("Error", new ErrorViewModel { Message = ex.Message });
				throw;
			}
			ViewBag.InqueryId = id;
			return PartialView("_InclusiveExclusive", inlst);
		}
		public PartialViewResult HeaderFotterLoad(long id = 0)
		{
			InqHeaderAndFooterModel model = new InqHeaderAndFooterModel();
			ViewBag.InqueryId = id;
			SqlParameter[] sqlParameters = new SqlParameter[2];
			sqlParameters[0] = new SqlParameter("@inqid", id);
			sqlParameters[1] = new SqlParameter("@flag", 2);
			DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Update_Inq_sub_Mob_Foot", sqlParameters);
			if (DtEmp != null && DtEmp.Rows.Count > 0)
			{
				model.InqId = Convert.ToInt32(id);
				model.Subject = DtEmp.Rows[0]["InqSubj"].ToString();
				model.Hedaer = DtEmp.Rows[0]["InqHeader"].ToString();
				model.Mobile = DtEmp.Rows[0]["InqMobile"].ToString();
				model.Footer = DtEmp.Rows[0]["InqFooter"].ToString();
			}

			return PartialView("_HeaderFotter", model);
		}

		[HttpPost]
		public IActionResult SaveInqSubMobFooter(InqHeaderAndFooterModel data)
		{


			List<Extraitem> etrItmObj = new List<Extraitem>();
			SqlParameter[] sqlParameters = new SqlParameter[6];
			sqlParameters[0] = new SqlParameter("@inqid", data.InqId);
			sqlParameters[1] = new SqlParameter("@sub", data.Subject);
			sqlParameters[2] = new SqlParameter("@header", data.Hedaer);
			sqlParameters[3] = new SqlParameter("@footer", data.Footer);
			sqlParameters[4] = new SqlParameter("@mob", data.Mobile);
			sqlParameters[5] = new SqlParameter("@flag", 1);

			DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Update_Inq_sub_Mob_Foot", sqlParameters);
			if (DtEmp != null && DtEmp.Rows.Count > 0)
			{
				int status = DbConnection.ParseInt32(DtEmp.Rows[0][0].ToString());
			}
			return Json(new { data = "1" });
		}

		#region Terms & COndition
		public PartialViewResult TearmAndConditionLoad(long id = 0)
		{
			List<InqTermandCondition> obj = new List<InqTermandCondition>();
			ViewBag.InqueryId = id;

			List<InqTermandCondition> extraItems = new List<InqTermandCondition>();

			var sqlParameters = new SqlParameter[2];
			sqlParameters[0] = new SqlParameter("@InTcInqVou", id);
			sqlParameters[1] = new SqlParameter("@FLG", 3);

			using (DataTable dtEmp = ObjDBConnection.CallStoreProcedure("Add_Inq_Inclusive", sqlParameters))
			{
				// Assuming there is a method to map DataTable to a List<Extraitem>
				extraItems = MapDataTableToTermsConditionList(dtEmp);
			}
			return PartialView("_Termandconditionview", extraItems);
		}
		private List<InqTermandCondition> MapDataTableToTermsConditionList(DataTable dataTable)
		{
			List<InqTermandCondition> extraItems = new List<InqTermandCondition>();

			foreach (DataRow row in dataTable.Rows)
			{
				InqTermandCondition extraItem = new InqTermandCondition
				{
					InqId = Convert.ToInt32(row["InTcVou"]),
					SlNo = Convert.ToInt32(row["InTcSrNo"]),
					Terms = row["IntcTerm"].ToString(),
					Description = row["InTcTncDesc"].ToString(),
				};

				extraItems.Add(extraItem);
			}

			return extraItems;
		}

		#endregion
		// Save Term and condition data
		[HttpPost]
		public IActionResult SaveTermdata(InqTermandCondition data)
		{


			List<Extraitem> etrItmObj = new List<Extraitem>();
			SqlParameter[] sqlParameters = new SqlParameter[6];
			sqlParameters[0] = new SqlParameter("@InTcVou", data.TermInqId);
			sqlParameters[1] = new SqlParameter("@InTcInqVou", data.InqId);
			sqlParameters[2] = new SqlParameter("@IntcTerm", data.Terms);
			sqlParameters[3] = new SqlParameter("@InTcTncDesc", data.Description);
			sqlParameters[4] = new SqlParameter("@InTcTncType", "Terms & Condition");
			sqlParameters[5] = new SqlParameter("@FLG", 1);

			DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Add_Inq_Inclusive", sqlParameters);
			if (DtEmp != null && DtEmp.Rows.Count > 0)
			{
				int status = DbConnection.ParseInt32(DtEmp.Rows[0][0].ToString());
			}
			return Json(new { data = "1" });
		}

		[HttpPost]
		public JsonResult ExraItemSave(Extraitem data)
		{

			List<Extraitem> etrItmObj = new List<Extraitem>();
			SqlParameter[] sqlParameters = new SqlParameter[4];
			sqlParameters[0] = new SqlParameter("@ExtraItem", data.extraitem == null ? "" : data.extraitem);
			sqlParameters[1] = new SqlParameter("@Amount", data.amount);
			sqlParameters[2] = new SqlParameter("@InQId", data.InqId);
			sqlParameters[3] = new SqlParameter("@EIId", data.EtrVou);

			DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Add_Inq_ExtraItem", sqlParameters);
			string obj = DtEmp.Rows[0]["Result"].ToString();
			List<Extraitem> obj1 = new List<Extraitem>();
			return Json(new { obj });
		}

		[HttpPost]
		public JsonResult SaveJonDetails(PostJobDetails data)
		{
			CultureInfo culture = new CultureInfo("en-US");
			List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
			SqlParameter[] sqlParameters = new SqlParameter[29];
			sqlParameters[0] = new SqlParameter("@pjobentrychk ", data.pjobentrychk);
			sqlParameters[1] = new SqlParameter("@addoneday ", data.addoneday);
			sqlParameters[2] = new SqlParameter("@fullday ", data.fullday);
			sqlParameters[3] = new SqlParameter("@Job", data.Job);
			sqlParameters[4] = new SqlParameter("@Eventsid", data.Eventsid);
			sqlParameters[5] = new SqlParameter("@venueid", data.venueid);
			sqlParameters[6] = new SqlParameter("@ocasiondate", DateTime.ParseExact(data.ocasiondate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
			sqlParameters[7] = new SqlParameter("@fromtime", data.fromtime);
			sqlParameters[8] = new SqlParameter("@totime", data.totime);
			sqlParameters[9] = new SqlParameter("@qty", data.qty);
			sqlParameters[10] = new SqlParameter("@rate", data.rate);
			sqlParameters[11] = new SqlParameter("@amount", data.amount);
			sqlParameters[12] = new SqlParameter("@discountpercentage", data.discountpercentage);
			sqlParameters[13] = new SqlParameter("@discountamt ", data.discountamt);
			sqlParameters[14] = new SqlParameter("@totalnet ", data.totalnet);
			sqlParameters[15] = new SqlParameter("@remarks", data.remarks);
			sqlParameters[16] = new SqlParameter("@spnoes", data.spnoes);
			sqlParameters[17] = new SqlParameter("@status", data.status);
			sqlParameters[18] = new SqlParameter("@eventhrs", data.eventhrs);
			sqlParameters[19] = new SqlParameter("@IndId", data.IndId);
			sqlParameters[20] = new SqlParameter("@JobId", data.JobId);
			sqlParameters[21] = new SqlParameter("@Todate", DateTime.ParseExact(data.Todate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
			sqlParameters[22] = new SqlParameter("@venuelink", data.venuelink);
			sqlParameters[23] = new SqlParameter("@sl", data.sl);
			sqlParameters[24] = new SqlParameter("@VenueOneAddTo", data.VenueOneAddTo);
			sqlParameters[25] = new SqlParameter("@VenueToAddOne", data.VenueToAddOne);
			sqlParameters[26] = new SqlParameter("@VenueToAddTwo", data.VenueToAddTwo);
			sqlParameters[27] = new SqlParameter("@VenueToUrl", data.VenueToUrl);
			sqlParameters[28] = new SqlParameter("@Wedingceremonyoptions", data.Wedingceremonyoptions);

			DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Add_Inq_Jobdetails", sqlParameters);

			string obj = DtEmp.Rows[0]["Result"].ToString();
			return Json(new { obj });
		}

		[HttpGet]
		public JsonResult GetJobDetails(GetAlldetails data)
		{

			List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
			SqlParameter[] sqlParameters = new SqlParameter[3];
			sqlParameters[0] = new SqlParameter("@IndId", data.TranId);
			sqlParameters[1] = new SqlParameter("@Id", data.Id);
			sqlParameters[2] = new SqlParameter("@JobPrev", data.Type);
			DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SpGet_Inq_Jobdetails", sqlParameters);
			for (int i = 0; i < DtEmp.Rows.Count; i++)
			{
				PostJobDetails dt = new PostJobDetails();
				dt.JobId = Convert.ToInt32(DtEmp.Rows[i]["IntVou"].ToString());
				dt.sl = Convert.ToInt32(DtEmp.Rows[i]["SlNo"].ToString());
				dt.IndId = Convert.ToInt32(DtEmp.Rows[i]["IntInqVou"].ToString());
				dt.Job = Convert.ToInt32(DtEmp.Rows[i]["IntJobVou"].ToString());
				dt.JobName = DtEmp.Rows[i]["JobName"].ToString();
				dt.Eventsid = DtEmp.Rows[i]["IntEvnNm"].ToString();
				dt.venueid = DtEmp.Rows[i]["IntVenNm"].ToString();
				dt.ocasiondate = DtEmp.Rows[i]["IntOccDt"].ToString();
				dt.date = DtEmp.Rows[i]["IntOccDt"].ToString();
				dt.fromtime = DtEmp.Rows[i]["IntFrTm"].ToString();
				dt.totime = DtEmp.Rows[i]["IntToTm"].ToString();
				dt.qty = Convert.ToDecimal(DtEmp.Rows[i]["IntQty"].ToString());
				dt.rate = Convert.ToDecimal(DtEmp.Rows[i]["IntRt"].ToString());
				dt.amount = Convert.ToDecimal(DtEmp.Rows[i]["IntAmt"].ToString());
				dt.discountpercentage = Convert.ToDecimal(DtEmp.Rows[i]["IntDisPer"].ToString());
				dt.discountamt = Convert.ToDecimal(DtEmp.Rows[i]["IntDisAmt"].ToString());
				dt.totalnet = Convert.ToDecimal(DtEmp.Rows[i]["IntNetAmt"].ToString());
				dt.remarks = DtEmp.Rows[i]["IntRem"].ToString();
				dt.spnoes = DtEmp.Rows[i]["IntSpRem"].ToString();
				dt.status = DtEmp.Rows[i]["IntEveStat"].ToString();
				dt.fullday = Convert.ToInt32(DtEmp.Rows[i]["IntFullDayYN"].ToString());
				dt.eventhrs = DtEmp.Rows[i]["IntEveHrs"].ToString();
				dt.pjobentrychk = Convert.ToInt32(DtEmp.Rows[i]["PJobEntryCheck"].ToString());
				dt.addoneday = Convert.ToInt32(DtEmp.Rows[i]["AddOneDay"].ToString());
				dt.Todate = DtEmp.Rows[i]["ToDate"].ToString();
				string aa = DtEmp.Rows[i]["EnquiryAmt"].ToString();
				dt.InqAmount = Convert.ToDecimal(DtEmp.Rows[i]["EnquiryAmt"].ToString());
				dt.venuelink = DtEmp.Rows[i]["VenueLink"].ToString();
				dt.sl = Convert.ToInt32(DtEmp.Rows[i]["SlNo"].ToString());
				dt.VenueOneAddTo = DtEmp.Rows[i]["VenueOneAddTo"].ToString();
				dt.VenueToAddTwo = DtEmp.Rows[i]["VenueToAddTwo"].ToString();
				dt.VenueToAddOne = DtEmp.Rows[i]["VenueToAddOne"].ToString();
				dt.VenueToUrl = DtEmp.Rows[i]["VenueToUrl"].ToString();
				dt.Wedingceremonyoptions = DtEmp.Rows[i]["Optionswending"].ToString();
				etrItmObj.Add(dt);
			}
			return Json(new { etrItmObj });
		}

		[HttpPost]
		public JsonResult deletejobdeails(GetAlldetails data)
		{

			List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
			SqlParameter[] sqlParameters = new SqlParameter[3];
			sqlParameters[0] = new SqlParameter("@IndId", data.TranId);
			sqlParameters[1] = new SqlParameter("@Id", data.Id);
			sqlParameters[2] = new SqlParameter("@Type", data.Type);
			DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Delete_Inq_Jobdetails", sqlParameters);

			string obj = DtEmp.Rows[0]["Result"].ToString();
			return Json(new { obj });
		}

		[HttpGet]
		public JsonResult GetEnqueryDetails(GetAlldetails data)
		{

			List<PostJobDetails> etrItmObj = new List<PostJobDetails>();
			SqlParameter[] sqlParameters = new SqlParameter[3];
			sqlParameters[0] = new SqlParameter("@IndId", data.TranId);
			sqlParameters[1] = new SqlParameter("@Id", data.Id);
			sqlParameters[2] = new SqlParameter("@Type", data.Type);
			DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SpGet_Inq_details", sqlParameters);
			for (int i = 0; i < DtEmp.Rows.Count; i++)
			{
				PostJobDetails dt = new PostJobDetails();
				dt.JobId = Convert.ToInt32(DtEmp.Rows[i]["IntVou"].ToString());
				dt.sl = Convert.ToInt32(DtEmp.Rows[i]["SlNo"].ToString());
				dt.IndId = Convert.ToInt32(DtEmp.Rows[i]["IntInqVou"].ToString());
				dt.Job = Convert.ToInt32(DtEmp.Rows[i]["IntJobVou"].ToString());
				dt.JobName = DtEmp.Rows[i]["JobName"].ToString();
				dt.Eventsid = DtEmp.Rows[i]["IntEvnNm"].ToString();
				dt.venueid = DtEmp.Rows[i]["IntVenNm"].ToString();
				dt.ocasiondate = DtEmp.Rows[i]["IntOccDt"].ToString();
				dt.date = DtEmp.Rows[i]["IntOccDt"].ToString();
				dt.fromtime = DtEmp.Rows[i]["IntFrTm"].ToString();
				dt.totime = DtEmp.Rows[i]["IntToTm"].ToString();
				dt.qty = Convert.ToDecimal(DtEmp.Rows[i]["IntQty"].ToString());
				dt.rate = Convert.ToDecimal(DtEmp.Rows[i]["IntRt"].ToString());
				dt.amount = Convert.ToDecimal(DtEmp.Rows[i]["IntAmt"].ToString());
				dt.discountpercentage = Convert.ToDecimal(DtEmp.Rows[i]["IntDisPer"].ToString());
				dt.discountamt = Convert.ToDecimal(DtEmp.Rows[i]["IntDisAmt"].ToString());
				dt.totalnet = Convert.ToDecimal(DtEmp.Rows[i]["IntNetAmt"].ToString());
				dt.remarks = DtEmp.Rows[i]["IntRem"].ToString();
				dt.spnoes = DtEmp.Rows[i]["IntSpRem"].ToString();
				dt.status = DtEmp.Rows[i]["IntEveStat"].ToString();
				dt.fullday = Convert.ToInt32(DtEmp.Rows[i]["IntFullDayYN"].ToString());
				dt.eventhrs = DtEmp.Rows[i]["IntEveHrs"].ToString();
				dt.pjobentrychk = Convert.ToInt32(DtEmp.Rows[i]["PJobEntryCheck"].ToString());
				dt.addoneday = Convert.ToInt32(DtEmp.Rows[i]["AddOneDay"].ToString());
				dt.InqAmount = Convert.ToInt32(DtEmp.Rows[i]["EnquiryAmt"].ToString());
				etrItmObj.Add(dt);
			}
			return Json(new { etrItmObj });
		}

		public IActionResult Delete(long id)
		{
			try
			{
				CategoryModel catModel = new CategoryModel();
				if (id > 0)
				{
					SqlParameter[] sqlParameters = new SqlParameter[2];
					sqlParameters[0] = new SqlParameter("@InqVou", id);
					sqlParameters[1] = new SqlParameter("@Flg", 2);
					DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_InquiryMst_Insert", sqlParameters);
					if (DtEmp != null && DtEmp.Rows.Count > 0)
					{
						int @value = DbConnection.ParseInt32(DtEmp.Rows[0][0].ToString());
						if (value == 0)
						{
							SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
						}
						else
						{
							SetSuccessMessage("Inquery Deleted Successfully");
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return RedirectToAction("index", "InquiryTransaction");
		}

		[HttpPost]
		public JsonResult SaveInclusiveExclusive(InclusiveExclusiveModel data)
		{

			var sqlParameters = new SqlParameter[6];
			sqlParameters[0] = new SqlParameter("@Tranid", data.Tranid);
			sqlParameters[1] = new SqlParameter("@Id", data.Id);
			sqlParameters[2] = new SqlParameter("@Item", data.Item);
			sqlParameters[3] = new SqlParameter("@Description", data.description);
			sqlParameters[4] = new SqlParameter("@Type", data.Type);
			sqlParameters[5] = new SqlParameter("@FLG", data.flag);

			DataTable DtEmp = ObjDBConnection.CallStoreProcedure("Add_Inq_InclusiveExclusive", sqlParameters);

			string obj = DtEmp.Rows[0]["Result"].ToString();
			return Json(new { obj });
		}

		[HttpPost]
		public JsonResult GetSerialNo(string Types, int InqId)
		{
			string SlNo = "";
			try
			{

				List<Extraitem> extraItems = new List<Extraitem>();

				var sqlParameters = new SqlParameter[2];
				sqlParameters[0] = new SqlParameter("@Types", Types);
				sqlParameters[1] = new SqlParameter("@InqId", InqId);


				DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SPGet_SerialNo", sqlParameters);
				if (DtEmp != null && DtEmp.Rows.Count > 0)
				{
					SlNo = DtEmp.Rows[0]["Sl"].ToString();
				}


			}
			catch (Exception ex)
			{
				// Handle the exception, log it, or return an error view
				// Example: return View("Error", new ErrorViewModel { Message = ex.Message });
				throw;
			}

			return Json(new { success = true, result = SlNo });

		}
		[HttpGet]
		public JsonResult GetForAllSelection(string Types)
		{
			List<ForSelectAllModel> SelectAll = new List<ForSelectAllModel>();
			try
			{



				var sqlParameters = new SqlParameter[1];
				sqlParameters[0] = new SqlParameter("@Types", Types);
				DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SpGet_ForSelectAll", sqlParameters);
				if (DtEmp != null && DtEmp.Rows.Count > 0)
				{
					foreach (DataRow dr in DtEmp.Rows)
					{
						ForSelectAllModel select = new ForSelectAllModel();
						select.Amount = Convert.ToDecimal(dr["Amount"].ToString());
						select.Id = Convert.ToInt32(dr["Id"].ToString());
						select.Sl = Convert.ToInt32(dr["SL"].ToString());
						select.Item = dr["Item"].ToString();
						select.Description = dr["Description"].ToString();
						SelectAll.Add(select);
					}
				}


			}
			catch (Exception ex)
			{
				// Handle the exception, log it, or return an error view
				// Example: return View("Error", new ErrorViewModel { Message = ex.Message });
				throw;
			}

			return Json(new { success = true, result = SelectAll });

		}

		// Save Select All

		[HttpGet]
		public JsonResult SaveSelectAll(string Ids, string Type, int TranId)
		{
			try
			{



				var sqlParameters = new SqlParameter[3];
				sqlParameters[0] = new SqlParameter("@Ids", Ids);
				sqlParameters[1] = new SqlParameter("@Type", Type);
				sqlParameters[2] = new SqlParameter("@TranId", TranId);
				DataTable DtEmp = ObjDBConnection.CallStoreProcedure("SpSave_ForSelectAll", sqlParameters);

			}
			catch (Exception ex)
			{
				// Handle the exception, log it, or return an error view
				// Example: return View("Error", new ErrorViewModel { Message = ex.Message });
				throw;
			}

			return Json(new { success = true, result = "" });

		}

		//public ActionResult GeneratePDF()
		//{
		//    // Output file path
		//    string outputPath = "output.pdf";

		//    // Create a PdfWriter object
		//    using (var writer = new PdfWriter(outputPath))
		//    {
		//        // Create a PdfDocument object
		//        using (var pdf = new PdfDocument(writer))
		//        {
		//            // Create a Document object
		//            var document = new Document(pdf);

		//            // Add content to the document
		//            document.Add(new Paragraph("Hello, iTextSharp!"));
		//            document.Add(new Paragraph("This is a simple PDF generated using iTextSharp."));

		//            // Close the document
		//            document.Close();
		//        }
		//    }
		//}

		public PartialViewResult InqueryReport(int id = 0, string Headertype = "")
		{

			Document doc = new Document();
			string filePath = "";
			ReportFileInfo fileInfo = new ReportFileInfo();


			using (MemoryStream memoryStream = new MemoryStream())
			{
				List<InqueryReportModel> Inquery = getReportcontent(id);


				// Associate the Document with the MemoryStream
				PdfWriter pdfWriter = PdfWriter.GetInstance(doc, memoryStream);

				// Add an event handler to add page numbers
				pdfWriter.PageEvent = new PageNumberEventHandler();
				if (Headertype == "Letterpad")
				{
					MyPageEventHandler pageEventHandler = new MyPageEventHandler();
					pdfWriter.PageEvent = pageEventHandler;
					Headertype = "With Header";
				}
				doc.Open();

				PdfPTable tableLayout = new PdfPTable(5);

				tableLayout = Add_Content_To_PDF(tableLayout, Inquery, Headertype);
				tableLayout.HeaderRows = 7;
				doc.Add(tableLayout);

				// Close the Document to finish the PDF creation
				doc.Close();

				// Write the content of the MemoryStream to a file
				string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports");
				string fileName = Inquery[0].FileName.ToString() + ".pdf";

				filePath = Path.Combine(folderPath, fileName);
				fileInfo.FileName = fileName;
				fileInfo.FilePath = filePath;
				fileInfo.Id = id;
				fileInfo.Title = Inquery[0].InqTitle;
				fileInfo.Sendto = Inquery[0].AccEmail;
				System.IO.File.WriteAllBytes(filePath, memoryStream.ToArray());
			}



			return PartialView("_InqueryReport", fileInfo);
		}

		public List<InqueryReportModel> getReportcontent(long id)
		{
			List<InqueryReportModel> Inquery = new List<InqueryReportModel>();
			List<ExtraItemreport> extitemlst = new List<ExtraItemreport>();
			List<Exclusierepoer> exclusierepoers = new List<Exclusierepoer>();
			List<Inclusivereport> inclusivereports = new List<Inclusivereport>();
			List<tearmandcondition> tcl = new List<tearmandcondition>();

			ViewBag.InqueryId = id;
			var sqlParameters = new SqlParameter[1];
			sqlParameters[0] = new SqlParameter("@Id", id);
			DataSet ds = ObjDBConnection.CallStoreProcedureDS("GetQuatation", sqlParameters);
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
				if (ds.Tables[4].Rows.Count > 0)
				{
					foreach (DataRow item in ds.Tables[4].Rows)
					{
						tearmandcondition exclu = new tearmandcondition();
						exclu.item = item["IntcTerm"].ToString();
						exclu.desc = item["InTcTncDesc"].ToString();

						tcl.Add(exclu);
					}
				}
				foreach (DataRow item in ds.Tables[0].Rows)
				{
					InqueryReportModel inq = new InqueryReportModel();
					inq.exclusive = exclusierepoers;
					inq.Inclusive = inclusivereports;
					inq.ExtraItem = extitemlst;
					inq.tandc = tcl;
					inq.AccEmail = item["AccEmail"].ToString();
					inq.customerremarks = item["customerremarks"].ToString();
					inq.AccNm = item["AccNm"].ToString();
					inq.Add1 = item["Add1"].ToString();
					inq.Add2 = item["Add2"].ToString();
					inq.Heading = item["Heading"].ToString();
					inq.InqDt = item["InqDt"].ToString();
					inq.InqMobile = item["InqMobile"].ToString();
					inq.InqTitle = item["InqTitle"].ToString();
					inq.IntOccDt = item["IntOccDt"].ToString();
					inq.Todate = item["Todate"].ToString();
					inq.IntFrTm = item["IntFrTm"].ToString();
					inq.IntToTm = item["IntToTm"].ToString();
					inq.AccAdd1 = item["AccAdd1"].ToString();
					inq.IntEvnNm = item["IntEvnNm"].ToString();
					inq.JobNm = item["JobNm"].ToString();
					inq.IntVenNm = item["IntVenNm"].ToString();
					inq.IntQty = Convert.ToDecimal(item["IntQty"].ToString());
					inq.IntRt = Convert.ToDecimal(item["IntRt"].ToString());
					inq.IntAmt = Convert.ToDecimal(item["IntAmt"].ToString());
					inq.disamt = Convert.ToDecimal(item["disamt"].ToString());
					inq.disper = Convert.ToDecimal(item["disper"].ToString());

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

		// for pdf report





		private PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, List<InqueryReportModel> Inquery, string Headertype)
		{



			List<InqueryReportModel> Inquery1 = new List<InqueryReportModel>();

			var uniqu = Inquery.Select(p => p.IntEvnNm)
								 .Distinct();
			float[] headers = { 15, 40, 10, 20, 15 };  //Header Widths
			tableLayout.SetWidths(headers);        //Set the pdf headers
			tableLayout.WidthPercentage = 100;       //Set the PDF File witdh percentage
			if (Headertype == "Without Header")
			{
				tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 17, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = -4, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_CENTER });
				tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 35, HorizontalAlignment = Element.ALIGN_CENTER });
				tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 10, HorizontalAlignment = Element.ALIGN_CENTER });
				tableLayout.AddCell(new PdfPCell(new Phrase("  ", new Font(Font.FontFamily.HELVETICA, 10, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 0, HorizontalAlignment = Element.ALIGN_CENTER });

			}
			//Add Title to the PDF file at the top
			if (Headertype == "With Header")
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

			tableLayout.AddCell(new PdfPCell(new Phrase("Quotation", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.DarkBlue)))) { Colspan = 5, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });
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

				AddCellToBodyNobold(tableLayout, "Customer Remarks :");
				AddCellToBody4(tableLayout, Inquery1[0].customerremarks);

				decimal netamount = 0;
				int a = Inquery1.Count;
				int b = 0;
				foreach (var item1 in Inquery1)
				{
					b = b + 1;


					if (a == b)
					{
						AddCellToBody2(tableLayout, item1.JobNm);
						AddCellToBodyBotoomborder(tableLayout, "Qty :" + item1.IntQty);
						AddCellToBodyBotoomborder(tableLayout, "Rate :" + Convert.ToDecimal(item1.IntRt).ToString("N2"));
						AddCellToBodyBotoomborder(tableLayout, Convert.ToDecimal(item1.IntQty * item1.IntRt).ToString("N2"));
					}
					else
					{
						AddCellToBody2(tableLayout, item1.JobNm);
						AddCellToBody(tableLayout, "Qty :" + item1.IntQty);
						AddCellToBody(tableLayout, "Rate :" + item1.IntRt);
						AddCellToBody(tableLayout, Convert.ToDecimal(item1.IntQty * item1.IntRt).ToString("N2"));
					}
					AddCellToBody(tableLayout, "");
					AddCellToBody(tableLayout, "");
					AddCellToBody(tableLayout, "");
					if(item1.disamt>0)
					{
						AddCellToBody3(tableLayout, "Discount ( " + item1.disper.ToString() + " %)");
						AddCellToBody3(tableLayout, Convert.ToDecimal(item1.disamt).ToString("N2"));
					}
					else
					{
						AddCellToBody3(tableLayout, "");
						AddCellToBody3(tableLayout, "");
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

			if (Inquery[0].Inclusive.Any(inclusive => !string.IsNullOrEmpty(inclusive.IncTncDesc)))
			{
				tableLayout.AddCell(new PdfPCell(new Phrase("All Below Packages Inclusive of", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 3, PaddingTop = 3, HorizontalAlignment = Element.ALIGN_LEFT });
			}

			foreach (var item in Inquery[0].Inclusive)
			{
				if (!string.IsNullOrEmpty(item.IncTncDesc))
				{
					tableLayout.AddCell(new PdfPCell(new Phrase("* " + item.IncTncDesc, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT });
				}
			}
			if (Inquery[0].exclusive.Any(exclusive => !string.IsNullOrEmpty(exclusive.IExTncDesc)))
			{
				tableLayout.AddCell(new PdfPCell(new Phrase("All Below Packages Exclusive of", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 3, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });
			}
			foreach (var item in Inquery[0].exclusive)
			{
				if (!string.IsNullOrEmpty(item.IExTncDesc))
				{
					tableLayout.AddCell(new PdfPCell(new Phrase("* " + item.IExTncDesc, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingBottom = 1, HorizontalAlignment = Element.ALIGN_LEFT });
				}
			}
			if (Inquery[0].ExtraItem.Count > 0)
			{
				tableLayout.AddCell(new PdfPCell(new Phrase("Extra Item", new Font(Font.FontFamily.HELVETICA, 14, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 3, PaddingBottom = 2, Top = 2, HorizontalAlignment = Element.ALIGN_LEFT });
			}
			foreach (var item in Inquery[0].ExtraItem)
			{
				tableLayout.AddCell(new PdfPCell(new Phrase(item.IteEitNm, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

				tableLayout.AddCell(new PdfPCell(new Phrase("Rs : " + Convert.ToDecimal(item.IteEitAmt).ToString("N2"), new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

				tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 1, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });


			}

			if (Inquery[0].tandc.Count > 0)
			{
				tableLayout.AddCell(new PdfPCell(new Phrase("Notes ", new Font(Font.FontFamily.HELVETICA, 14, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 3, PaddingBottom = 2, Top = 2, HorizontalAlignment = Element.ALIGN_LEFT });
			}
			foreach (var item in Inquery[0].tandc)
			{
				tableLayout.AddCell(new PdfPCell(new Phrase(item.item, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

				tableLayout.AddCell(new PdfPCell(new Phrase(item.desc, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 3, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });




			}

			//tableLayout.AddCell(new PdfPCell(new Phrase("Customer Remarks ", new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 10, Border = 0, PaddingTop = 15, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_LEFT });
			//tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].customerremarks, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 10, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

			tableLayout.AddCell(new PdfPCell(new Phrase(Inquery[0].InqFooter, new Font(Font.FontFamily.HELVETICA, 12, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 5, Border = 0, PaddingTop = 15, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_LEFT });

			tableLayout.AddCell(new PdfPCell(new Phrase("Mobile-" + Inquery[0].FooterMobile, new Font(Font.FontFamily.HELVETICA, 8, 0, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 3, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });

			tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(System.Drawing.Color.Black)))) { Colspan = 2, Border = 0, PaddingBottom = 2, HorizontalAlignment = Element.ALIGN_LEFT });




			return tableLayout;
		}

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


		private static void AddCellToBody4(PdfPTable tableLayout, string cellText)
		{
			tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 0, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Colspan = 4, Border = 0, PaddingBottom = 0, Padding = 2, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
			//BaseFont baseFont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
			//Font textFont = new Font(baseFont, 9, Font.NORMAL, BaseColor.BLACK);

			//tableLayout.AddCell(new PdfPCell(new Phrase(cellText, textFont))
			//{
			//    Colspan = 10,
			//    Border = 0,
			//    PaddingTop = 15,
			//    PaddingBottom = 5,
			//    HorizontalAlignment = Element.ALIGN_LEFT
			//});
		}
		private static void AddCellToBody3(PdfPTable tableLayout, string cellText)
		{
			tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = 0, PaddingBottom = 0, Padding = 2, BackgroundColor = iTextSharp.text.BaseColor.WHITE });
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

		[HttpPost]
		public JsonResult SendMail(ReportFileInfo dt)
		{
			// Sender's Gmail credentials
			string senderEmail = "email2pioneer@gmail.com";
			string senderPassword = "bswslprzydbebgsq";

			// Recipient's email address
			string recipientEmail = dt.Sendto;
			// Creating the MailMessage object
			MailMessage mail = new MailMessage(senderEmail, recipientEmail);
			mail.Subject = dt.Title;
			mail.Body = "Please find attachment inquiry/quotation details bellow";
			mail.IsBodyHtml = true;
			string folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Reports");
			string fullpath = Path.Combine(folderPath, dt.FileName);
			Attachment at = new Attachment(fullpath);
			mail.Attachments.Add(at);
			// Creating the SmtpClient object
			SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
			smtpClient.Port = 587;
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
			smtpClient.EnableSsl = true;

			try
			{
				// Sending the email
				smtpClient.Send(mail);

			}
			catch (Exception ex)
			{

				return Json(new { success = true, result = "Fail" });
			}
			return Json(new { success = true, result = "Send" });

		}



	}

}

