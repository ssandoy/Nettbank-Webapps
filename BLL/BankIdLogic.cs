using System;

namespace BLL {
    public class BankIdLogic {
        private static readonly string[] BankId = {
            "345281", "235152", "852352", "512512", "396934", "991231", "712314", "641241",
            "123211", "151255"
        };

        public static string GetBankId() {
            var rnd = new Random();
            var randomIndex = rnd.Next(0, BankId.Length);
            return BankId[randomIndex];
        }
    }
}