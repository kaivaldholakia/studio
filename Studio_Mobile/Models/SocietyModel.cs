using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocietyManagementWeb.Models
{
    public class SocietyModel
    {
        public long SocietyId { get; set; } = 0;
        public string RegisteredName { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int CityId { get; set; }
        public string BlockCount { get; set; }
        public string Units { get; set; } 
        public int ClientId { get; set; }
        public List<SelectListItem> GetClientlist { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> CityList { get; set; }
    }

    public class BlockModel
    {
       
        public long BlockID { get; set; }
        public long SocietyId { get; set; }
        public long SocietyFilter { get; set; }
        public string Name { get; set; }
        public int TotalFloor { get; set; }
        public int TotalFlatPerFloor { get; set; }
        public List<SelectListItem> GetSocietylist { get; set; }
    }

    public class UnitModel
    {
        public long UnitID { get; set; }
        public long BlockID { get; set; }
        public long SocietyId { get; set; }
        public string Name { get; set; }
        public string FloorNo { get; set; }
        public string UnitNo { get; set; }
        public string Area { get; set; }
        public string SqrFt { get; set; }
        public bool Status { get; set; }
    
        public decimal SquareFoot { get; set; }
        public int RoomType { get; set; }

        public long BlockFilterID { get; set; }
        public long SocietFilteryId { get; set; }
        public List<SelectListItem> GetGetRoomType { get; set; }
        public List<SelectListItem> GetSocietylist { get; set; }
        public List<SelectListItem> GetBlocklist { get; set; }
    }
}
