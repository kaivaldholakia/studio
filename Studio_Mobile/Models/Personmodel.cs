using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studio_Mobile.Models
{
    public class Personmodel
    {
        public int Id { get; set; }

        public string customer { get; set; }
        public string fulladdress { get; set; }
        public string area { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string pan { get; set; }
    }

    public class Personmodelevent
    {
       
        public string person { get; set; }
        public string mobile { get; set; }
        public string add1Loc1 { get; set; }
        public string add1Loc2 { get; set; }
        public string add2Loc1 { get; set; }
        public string add2Loc2 { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string eventName { get; set; }
        public string category { get; set; }
        public string meetperson { get; set; }
    }
}
