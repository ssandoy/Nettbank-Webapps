using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using Model;

namespace DAL {
    public class AccountRepository : IAccountRepository {
        public List<Account> ListAccounts(string personalNumber) {
            using (var db = new DbModel()) {
                try {
                    var allAccounts = db.Accounts.Where(a => a.Owner.PersonalNumber == personalNumber);
                    var accounts = new List<Account>();
                    foreach (var a in allAccounts) {
                        accounts.Add(new Account {
                            AccountNumber = a.AccountNumber,
                            AvailableBalance = a.AvailableBalance,
                            OwnerName = a.Owner.FirstName + " " + a.Owner.LastName,
                            Balance = a.Balance
                        });
                    }
                    return accounts;
                } catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at DeleteAccoúnt()";
                    WriteToErrorLog(error);
                    return null;
                }
            }
        }

        public bool DeleteAccount(string accountNumber) {
            using (var db = new DbModel()) {
                try {
                    var deleteAccount = db.Accounts.Find(accountNumber);
                    if (deleteAccount == null) {
                        return false;
                    }
                    // Sletter transaksjoner som ikke er utført
                    var deleteTransactions = deleteAccount.Transactions.Where(t => t.TimeTransfered == null);
                    foreach (var t in deleteTransactions) {
                        db.Transactions.Remove(t);
                        //write to changelog
                        var tLog = new ChangeLog();
                        tLog.ChangedTime = (DateTime.Now).ToString("yyyyMMddHHmmss");
                        tLog.EventType = "Delete";
                        tLog.OriginalValue = t.ToString();
                        tLog.NewValue = "null";
                        var tContext = HttpContext.Current;
                        if (tContext.Session["CurrentAdmin"] != null) {
                            Admins changedby = (Admins)tContext.Session["CurrentAdmin"];
                            tLog.ChangedBy = changedby.FirstName + " " + changedby.LastName;
                        } else {
                            tLog.ChangedBy = "null";
                        }
                        WriteToChangeLog(tLog.toString());
                    }
                    // Kobler alle ikke-utførte transaksjoner fra kontoen slik at den kan slettes
                    deleteAccount.Transactions.RemoveAll(t => t.TimeTransfered != null);
                    // Sletter kontoen
                    db.Accounts.Remove(deleteAccount);
                    
                    //save to log
                    var log = new ChangeLog();
                    log.ChangedTime = (DateTime.Now).ToString("yyyyMMddHHmmss");
                    log.EventType = "Delete";
                    log.OriginalValue = deleteAccount.ToString();
                    log.NewValue = "null";
                    var context = HttpContext.Current;
                    if (context.Session["CurrentAdmin"] != null) {
                        Admins changedby = (Admins)context.Session["CurrentAdmin"];
                        log.ChangedBy = changedby.FirstName + " " + changedby.LastName;
                    } else {
                        log.ChangedBy = "null";
                    }
                    WriteToChangeLog(log.toString());
                    db.SaveChanges();
                    return true;
                } catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at DeleteAccoúnt()";
                    WriteToErrorLog(error);
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
                    string error = "Exception: " + exc.ToString() + " catched at GetUpdateableAccount()";
                    WriteToErrorLog(error);
                    return null;
                }
            }
        }

        public string UpdateAccount(EditableAccount a) {
            using (var db = new DbModel()) {
                try {
                    var accounts = db.Accounts.Find(a.AccountNumber);
                    string originalvalue = accounts.Owner.FirstName + " " + accounts.Owner.LastName;
                    var newOwner = db.Customers.Find(a.OwnerPersonalNumber);

                    if (newOwner == null) {
                        return "Det finnes ingen kunde med det gitte personnummeret.";
                    }

                    accounts.Owner = newOwner;
                    db.Entry(accounts).State = EntityState.Modified;
                    var log = new ChangeLog();
                    log.ChangedTime = (DateTime.Now).ToString("yyyyMMddHHmmss");
                    log.EventType = "Update";
                    log.OriginalValue = originalvalue;
                    log.NewValue = accounts.Owner.FirstName + " " + accounts.Owner.LastName;
                    var context = HttpContext.Current;
                    if (context.Session["CurrentAdmin"] != null) {
                        Admins changedby = (Admins)context.Session["CurrentAdmin"];
                        log.ChangedBy = changedby.FirstName + " " + changedby.LastName;
                    } else {
                        log.ChangedBy = "null";
                    }
                    WriteToChangeLog(log.toString());
                    db.SaveChanges();
                    return "";
                } catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at UpdateAccount()";
                    WriteToErrorLog(error);
                    return "Feil: " + exc.Message;
                }
            }
        }

        public bool AddAccount(string personalNumber) {
            using (var db = new DbModel()) {
                try {
                    var accounts = new Accounts() {
                        AccountNumber = "0411" + new Random().Next(9999999).ToString("0000000"),
                        AvailableBalance = 0,
                        Balance = 0,
                        PersonalNumber = personalNumber,
                        Transactions = new List<Transactions>()
                    };
                    // Validerer kontonummer
                    if (accounts.AccountNumber.Length != 11) {
                        return false;
                    }
                    // Validerer om eieren eksisterer
                    var owner = db.Customers.Find(accounts.PersonalNumber);
                    if (owner == null) {
                        return false;
                    }
                    db.Accounts.Add(accounts);
                    //write to changelog
                    var log = new ChangeLog();
                    log.ChangedTime = (DateTime.Now).ToString("yyyyMMddHHmmss");
                    log.EventType = "Create";
                    log.OriginalValue = accounts.ToString();
                    log.NewValue = "null";
                    var context = HttpContext.Current;
                    if (context.Session["CurrentAdmin"] != null) {
                        Admins changedby = (Admins)context.Session["CurrentAdmin"];
                        log.ChangedBy = changedby.FirstName + " " + changedby.LastName;
                    } else {
                        log.ChangedBy = "null";
                    }
                    WriteToChangeLog(log.toString());
                    db.SaveChanges();
                    return true;
                } catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at AddAccount()";
                    WriteToErrorLog(error);
                    return false;
                }
            }
        }

        public void WriteToChangeLog(string log) {
            string path = "ChangeLog.txt";
            var _Path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/"), path);
            if (!File.Exists(_Path)) {
                string createText = log + Environment.NewLine;
                File.WriteAllText(_Path, createText);
            } else {
                string appendText = log + Environment.NewLine;
                File.AppendAllText(_Path, appendText);
            }
        }

        public void WriteToErrorLog(string error) {
            string path = "ErrorLog.txt";
            var _Path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/"), path);
            if (!File.Exists(_Path)) {
                string createText = error + Environment.NewLine;
                File.WriteAllText(_Path, createText);
            } else {
                string appendText = error + Environment.NewLine;
                File.AppendAllText(_Path, appendText);
            }
        }
    }
}