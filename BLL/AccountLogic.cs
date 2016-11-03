﻿using System.Collections.Generic;
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
    }
}