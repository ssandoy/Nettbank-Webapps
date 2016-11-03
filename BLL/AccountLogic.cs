using System.Collections.Generic;
using DAL;
using Model;

namespace BLL {
    public class AccountLogic {
        public List<Account> ListAccounts(string personalNumber) {
            var accountAccess = new AccountAccess();
            return accountAccess.ListAccounts(personalNumber);
        }

        public bool Login() {
            var accountAccess = new AccountAccess();
            return accountAccess.Login();
        }
    }
}