using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studio_Mobile.Common
{
    public class Baseresponseclscsprop
    {
        public long satuscCode { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
        public bool modalstate { get; set; }

    }

    public class Baseresponseclscs
    {
        public static Baseresponseclscsprop? CreateResponse(Dictionary<string,IEnumerable<string>> data,long statusCode,string status,string message,bool modalState)
        {
            Baseresponseclscsprop baseresponseclscsprop = new Baseresponseclscsprop();
            baseresponseclscsprop.data = data;
            baseresponseclscsprop.message = message;
            baseresponseclscsprop.status = status;
            baseresponseclscsprop.modalstate = modalState;
            baseresponseclscsprop.satuscCode = statusCode;
            return baseresponseclscsprop;
        }
        public static Baseresponseclscsprop? CreateResponse(dynamic data, long statusCode, string status, string message, bool modalState)
        {
            Baseresponseclscsprop baseresponseclscsprop = new Baseresponseclscsprop();
            baseresponseclscsprop.data = data;
            baseresponseclscsprop.status = status;
            baseresponseclscsprop.message = message;
            baseresponseclscsprop.modalstate = modalState;
            baseresponseclscsprop.satuscCode = statusCode;
            return baseresponseclscsprop;
        }

    }
}
