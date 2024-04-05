using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocietyManagementApi.Helper
{
    public static class RendomKey
    {
        public static int getrenadomkey()
        {
            Random rnd = new Random();
            string randomNumber = (rnd.Next(100000, 999999)).ToString();
           return Convert.ToInt32(randomNumber);
        }
    }
}
