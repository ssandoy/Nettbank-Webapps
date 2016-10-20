using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nettbank___Webapplikasjoner
{
    public static class BankIDGenerator
    {
        private static string[] bankID = { "345281", "235152", "852352", "512512", "396934", "991231", "712314" , "641241", "123211" , "151255" };

        public static string getBankID()
        {
          Random rnd = new Random();
            int randomIndex = rnd.Next(0, bankID.Length);
            return bankID[randomIndex];
        } 

    }
}