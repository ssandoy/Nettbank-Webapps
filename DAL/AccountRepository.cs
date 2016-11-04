using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
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

        public EditableAccount GetUpdateableAccount(string accountNumber) {
            using (var db = new DbModel()) {
                try {
                    var accounts = db.Accounts.Find(accountNumber);

                    if (accounts == null) {
                        return null;
                    }

                    var account = new EditableAccount() {
                        AccountNumber = accounts.AccountNumber,
                        OwnerPersonalNumber = accounts.Owner.PersonalNumber
                    };

                    return account;
                } catch (Exception exc) {
                    return null;
                }
            }
        }

        public string UpdateAccount(EditableAccount a) {
            using (var db = new DbModel()) {
                try {
                    var accounts = db.Accounts.Find(a.AccountNumber);
                    var newOwner = db.Customers.Find(a.OwnerPersonalNumber);

                    if (newOwner == null) {
                        return "Det finnes ingen kunde med det gitte personnummeret.";
                    }

                    accounts.Owner = newOwner;
                    db.Entry(accounts).State = EntityState.Modified;
                    db.SaveChanges();
                    return "";
                } catch (Exception exc) {
                    return "Feil: " + exc.Message;
                }
            }
        }

        public string AddAccount(EditableAccount newAccount) {
            using (var db = new DbModel()) {
                try {
                    var accounts = new Accounts() {
                        AccountNumber = newAccount.AccountNumber,
                        Balance = 0,
                        PersonalNumber = newAccount.OwnerPersonalNumber,
                        Transactions = new List<Transactions>()
                    };

                    // Validerer kontonummer
                    if (accounts.AccountNumber.Length != 11) {
                        return "Kontonummeret må være på 11 siffer.";
                    } 

                    // Validerer om eieren eksisterer
                    var owner = db.Customers.Find(accounts.PersonalNumber);
                    if (owner == null) {
                        return "Det finnes ingen kunde med det gitte personnummeret.";
                    }

                    db.Accounts.Add(accounts);
                    db.SaveChanges();
                    return "";
                } catch (Exception exc) {
                    return "Feil: " + exc.Message;
                }
            }
        }
    }
}