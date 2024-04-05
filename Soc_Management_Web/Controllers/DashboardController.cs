using Microsoft.AspNetCore.Mvc;
using PIOAccount.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PIOAccount.Classes;

namespace PIOAccount.Controllers
{
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            try
            {


            }
            catch (Exception ex)
            {
                throw;
            }
            return View();
        }

        [HttpPost]
        public IActionResult GetYearList(int companyId)
        {
            try
            {
                int isadministrator = Convert.ToInt32(GetIntSession("IsAdministrator"));
                string clientId = HttpContext.Request.Cookies["ClientId"].ToString();
                var list = DbConnection.GetYearList(Convert.ToInt32(companyId));
                Response.Cookies.Delete("CompanyId");
                Response.Cookies.Delete("YearId");
                Response.Cookies.Append("CompanyId", companyId.ToString());
                if (list.Count <= 0)
                {
                    SetErrorMessage("No year found!");
                }
                var companyList = DbConnection.GetCompanyList(Convert.ToInt32(clientId), isadministrator);
                string yearId = list.Where(x => x.Selected).Select(x => x.Value).FirstOrDefault();
                Response.Cookies.Append("YearId", yearId);
                Response.Cookies.Append("CompanyName", companyList.Where(x => x.Value == companyId.ToString()).Select(x => x.Text).FirstOrDefault());
                Response.Cookies.Append("YearName", list.Where(x => x.Value == yearId.ToString()).Select(x => x.Text).FirstOrDefault());
                return Json(new { result = true, data = list });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult ChangeYear(int yearId)
        {
            try
            {
                Response.Cookies.Delete("YearId");
                Response.Cookies.Append("YearId", yearId.ToString());
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("index", "dashboard");
        }

        public IActionResult Buttons()
        {
            try
            {
                int UserId = Convert.ToInt32(GetIntSession("UserId"));
                int ClientId = Convert.ToInt32(GetIntSession("ClientId"));
                var buttonList = GetUserRightsList(UserId, ClientId);
                if (buttonList != null && buttonList.Count > 0)
                    return Json(new { data = buttonList, result = true });
                else
                    return Json(new { result = false });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
