using System;

namespace BLL {
    public class BankIdLogic {
      

        public static string GetBankId() {
            var rnd = new Random();
            var x = rnd.Next(0, 1000000);
            string s = x.ToString("000000");
            return s;
        }
    }
}