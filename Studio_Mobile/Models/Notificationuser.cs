using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studio_Mobile.Models
{
    public class Notificationuser
    {
        public long  Id { get; set; }
        public long jobid { get; set; }
        public string jobName { get; set; }
        public string category { get; set; }
        public string Status { get; set; }
        public string location { get; set; }

        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string ToDate { get; set; }
        public string FromDate { get; set; }

        public int IsAttendent { get; set; }

    }
}
