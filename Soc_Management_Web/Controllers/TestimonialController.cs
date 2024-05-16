using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PIOAccount.Classes;
using PIOAccount.Controllers;
using PIOAccount.Models;
using Soc_Management_Web.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
namespace Soc_Management_Web.Controllers
{
    public class TestimonialController : BaseController
    {
		DbConnection ObjDBConnection = new DbConnection();
		ProductHelpers objProductHelper = new ProductHelpers();
		private readonly IWebHostEnvironment _hostingEnvironment;
		MasterDropdownHelper master = new MasterDropdownHelper();
		public TestimonialController(IWebHostEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
		}
		public IActionResult Index(long id)
		{

			Webcategorydetails model = new Webcategorydetails();
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

			if (id > 0)
			{

				SqlParameter[] sqlParameters = new SqlParameter[5];
				sqlParameters[0] = new SqlParameter("@id", id);
				sqlParameters[1] = new SqlParameter("@tittle", "");
				sqlParameters[2] = new SqlParameter("@Filepath", "");
				//sqlParameters[3] = new SqlParameter("@FileNames", "");
				sqlParameters[3] = new SqlParameter("@Discription", "");
				sqlParameters[4] = new SqlParameter("@FLG", "3");
				DataTable DtEmp = ObjDBConnection.CallStoreProcedure("usp_testimonialdetails_Insert", sqlParameters);
				if (DtEmp.Rows.Count > 0)
				{
		
					model.Id = id;
					model.tittle = DtEmp.Rows[0]["title"].ToString();
					model.Filepath = "http://localhost:8010/Testimonial/"+DtEmp.Rows[0]["ImageUrl"].ToString();
					model.description = DtEmp.Rows[0]["Discription"].ToString(); 

					//SetSuccessMessage("Inserted Sucessfully");
					//message = "Inserted Sucessfull";
					//datastatus = true;

				}
			}
			model.Caltlst = master.GetDropgen("Testimonial");
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

		[HttpPost]
		public async System.Threading.Tasks.Task<ActionResult> IndexAsync(long id, Webcategorydetails obj)
		{
			string obj1 = "";
			try
			{
				string desc = obj.description;
				bool isreturn = false;
				INIT(ref isreturn);
				if (obj.file != null && obj.file.Length > 0)
				{

					var uploadsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Testimonial");
					obj.filename = "img" + Guid.NewGuid().ToString() + Path.GetExtension(obj.file.FileName);

					using (var stream = new FileStream(Path.Combine(uploadsFolderPath, obj.filename), FileMode.CreateNew))
					{
						await obj.file.CopyToAsync(stream);
					}


					obj.iamgepath = Path.Combine(uploadsFolderPath, obj.filename);


				}
				long userId = GetIntSession("UserId");
				int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
				if (!string.IsNullOrWhiteSpace(Convert.ToString(obj.tittle)))
				{
					SqlParameter[] sqlParameters = new SqlParameter[5];
					sqlParameters[0] = new SqlParameter("@id", id);
					sqlParameters[1] = new SqlParameter("@tittle", obj.tittle);
					sqlParameters[2] = new SqlParameter("@Filepath", obj.filename);
					sqlParameters[3] = new SqlParameter("@Discription", desc);
				
					//sqlParameters[4] = new SqlParameter("@FileNames", obj.filename);
					sqlParameters[4] = new SqlParameter("@FLG", "1");
					DataTable DtCat = ObjDBConnection.CallStoreProcedure("usp_testimonialdetails_Insert", sqlParameters);

					if (DtCat != null && DtCat.Rows.Count > 0)
					{
						int status = DbConnection.ParseInt32(DtCat.Rows[0][0].ToString());
						if (status == -1)
						{
							id = status;
							obj1 = "Dulplicate Category Details";
							return RedirectToAction("index", "Testimonial", new { id = 0 });
						}
						else
						{
							string message = "";


							if (id > 0)
							{
								
								SetSuccessMessage("Update Sucessfully");
								return Json(new { msg = "Update Sucessfully" , status =true });
								}
							else
							{
								
								SetSuccessMessage("Inserted Sucessfully");
								return Json(new { msg = "Inserted Sucessfully", status = true });

							}
							
							//return RedirectToAction("index", "Testimonial", new { id = 0 });
						}
					}
					else
					{
						obj1 = "Inserted Sucessfully";
						return RedirectToAction("index", "Testimonial", new { id = id });
					}
				}
				else
				{
					obj1 = "Inserted Sucessfully";
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return RedirectToAction("index", "Testimonial");
			//return Json(new { obj1 });
		}

		public IActionResult Delete(long id)
		{
			try
			{
				CategoryModel catModel = new CategoryModel();
				if (id > 0)
				{
					long userId = GetIntSession("UserId");
					int companyId = Convert.ToInt32(GetIntSession("CompanyId"));
					SqlParameter[] sqlParameters = new SqlParameter[5];
					sqlParameters[0] = new SqlParameter("@id", id);
					sqlParameters[1] = new SqlParameter("@tittle", "");
					sqlParameters[2] = new SqlParameter("@Filepath", "");
					sqlParameters[3] = new SqlParameter("@discription", "tes");

					//sqlParameters[4] = new SqlParameter("@FileNames", obj.filename);
					sqlParameters[4] = new SqlParameter("@FLG", "2");
					DataTable DtCat = ObjDBConnection.CallStoreProcedure("usp_testimonialdetails_Insert", sqlParameters);
					if (DtCat != null && DtCat.Rows.Count > 0)
					{
						int @value = DbConnection.ParseInt32(DtCat.Rows[0][0].ToString());
						if (value == 0)
						{
							SetErrorMessage("You Can Not Delete Records This Record is Included Some Trasaction");
						}
						else
						{
							SetSuccessMessage("Testimonial Deleted Successfully");
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return RedirectToAction("index", "Testimonial");
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
					string currentURL = "/Testimonial/Index";
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
					getReportDataModel.ControllerName = "Testimonial";
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return PartialView("_reportView", getReportDataModel);
		}

	}
}
