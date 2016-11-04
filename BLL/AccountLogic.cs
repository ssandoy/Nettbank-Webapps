using System.Collections.Generic;
using System.Web;
using DAL;
using Model;

namespace BLL {
    public class AccountLogic {

        private readonly HttpContext _context = HttpContext.Current;

        public List<Account> ListAccounts(string personalNumber) {
            var accountAccess = new AccountRepository();
            return accountAccess.ListAccounts(personalNumber);
        }

        public bool Login()
        {
            if (_context.Session["loggedin"] == null)
            {
                _context.Session["loggedin"] = false;
            }
            else {
                return (bool)_context.Session["loggedin"];
            }
            return false;
        }

        public bool DeleteAccount(string accountNumber) {
            var accountAccess = new AccountRepository();
            return accountAccess.DeleteAccount(accountNumber);
        }

        public UpdateableAccount GetUpdateableAccount(string accountNumber) {
            var accountAccess = new AccountRepository();
            return accountAccess.GetUpdateableAccount(accountNumber);
        }

        public string UpdateAccount(UpdateableAccount updatedAccount) {
            var accountAccess = new AccountRepository();
            return accountAccess.UpdateAccount(updatedAccount);
        }
    }
}