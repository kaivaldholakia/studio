using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Studio_Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studio_Mobile.Models
{
    public class UserMasterModel
    {

        public long UserVou { get; set; }
        public long otp { get; set; }
        public int UserEmpVou { get; set; }
        public List<CustomDropDown> EmployeeList { get; set; }
        public string EmpName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserId { get; set; }
        public int SocietyId { get; set; }
        public int Role { get; set; }
        public string UserCd { get; set; }
        public string UserPass { get; set; }
        public string UserMobNo { get; set; }
        public string UserEmail { get; set; }
        public DateTime UserDate { get; set; }
        public int ClientId { get; set; }
        public List<SelectListItem> GetClientLIst { get; set; }
        public string ProfilePicture { get; set; }
        public IFormFile profilePhoto { get; set; }
        public string URLPicture { get; set; }
        public int UserRolVou { get; set; }
        public List<SelectListItem> UserRoleLIst { get; set; }
        public int UserActYNVou { get; set; }
        public List<SelectListItem> ActiveYNList { get; set; }
        public List<SelectListItem> GetSocietylist { get; set; }
        public long SocietFilteryId { get; set; }
        public string Active { get; set; }

        // ForMObile
        public bool IsPasswordVerified { get; set; }
        public bool IsLogin { get; set; }
        public string LastLogin { get; set; }


    }
}
