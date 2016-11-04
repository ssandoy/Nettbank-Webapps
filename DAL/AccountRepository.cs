using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;

namespace DAL {
    public class AccountRepository {

        public List<Account> ListAccounts(string personalNumber) {
            using (var db = new DbModel()) {
                var allAccounts = db.Accounts.Where(a => a.Owner.PersonalNumber == personalNumber);
                var accounts = new List<Account>();
                foreach (var a in allAccounts) {
                    accounts.Add(new Account {
                        AccountNumber = a.AccountNumber,
                        OwnerName = a.Owner.FirstName + " " + a.Owner.LastName,
                        Balance = a.Balance
                    });
                }
                return accounts;
            }
        }

        public bool DeleteAccount(string accountNumber) {
            using (var db = new DbModel()) {
                try {
                    var deleteAccount = db.Accounts.Find(accountNumber);
                    db.Accounts.Remove(deleteAccount);
                    db.SaveChanges();
                    return true;
                } catch (Exception exc) {
                    return false;
                }
            }
        }
    }
}