using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nettbank___Webapplikasjoner.Models;

namespace Nettbank___Webapplikasjoner
{
    public class AccountDB
    {
        HttpContext context = HttpContext.Current;

        public List<Account> listAccounts(string personalNumber)
        {
            using (var db = new DbModel())
            {
                var allAccounts = db.accounts.Where(a => a.owner.personalNumber == personalNumber);
                List<Account> Accounts = new List<Account>();
                foreach (var a in allAccounts)
                {
                    Accounts.Add(new Account
                    {
                        accountNumber = a.accountNumber,
                        ownerName = a.owner.firstName + " " + a.owner.lastName,
                        balance = a.balance
                    });
                }
                return Accounts;
            }
        }


        public bool Login()
        {
            if (context.Session["loggedin"] == null)
            {
                context.Session["loggedin"] = false;
            }
            else
            {
                return (bool)context.Session["loggedin"];
            }
            return false;
        }


    
    }
}