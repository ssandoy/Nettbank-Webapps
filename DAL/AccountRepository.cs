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

        public UpdateableAccount GetUpdateableAccount(string accountNumber) {
            using (var db = new DbModel()) {
                try {
                    var accounts = db.Accounts.Find(accountNumber);

                    if (accounts == null) {
                        return null;
                    }

                    var account = new UpdateableAccount() {
                        AccountNumber = accounts.AccountNumber,
                        OwnerPersonalNumber = accounts.Owner.PersonalNumber
                    };

                    return account;
                } catch (Exception exc) {
                    return null;
                }
            }
        }

        public string UpdateAccount(UpdateableAccount a) {
            using (var db = new DbModel()) {
                try {
                    var accounts = db.Accounts.Find(a.AccountNumber);
                    var newOwner = db.Customers.Find(a.OwnerPersonalNumber);

                    if (newOwner == null) {
                        return "Det finnes ingen kunde med det gitte personnummeret.";
                    }

                    accounts.Owner = newOwner;
                    db.SaveChanges();
                    return "";
                } catch (Exception exc) {
                    return "Feil: " + exc.Message;
                }
            }
        }
    }
}