using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nettbank___Webapplikasjoner.Models;
using System.Diagnostics;
using System.Data.Entity;

namespace Nettbank___Webapplikasjoner {
    public class AccessDb {

        public List<Account> listAccounts(String personalNumber) {
            using (var db = new DbModel())
            {
                var allAccounts = db.accounts.Where(a => a.owner.personalNumber == personalNumber);
                List<Account> Accounts = new List<Account>();
                foreach (var a in allAccounts)
                {
                    Accounts.Add(new Account {
                        accountNumber = a.accountNumber,
                        balance = a.balance
                        });
                }
                return Accounts;
            }
        }

        public List<Transaction> listTransactions(string accountNumber) {
            using (var db = new DbModel()) {
                var allTransactions = db.transactions.Where(t => t.accountNumber == accountNumber);
                var transactions = new List<Transaction>();
                foreach (var t in allTransactions) {
                    transactions.Add(new Transaction {
                        transactionId = t.transactionID,
                        amount = t.amount,
                        timeToBeTransfered = t.timeToBeTransfered,
                        timeTransfered = t.timeTransfered,
                        fromAccountNumber = t.accountNumber,
                        toAccountNumber = t.toAccountNumber,
                        comment = t.comment
                    });
                }
                return transactions;
            }
        }

        public bool addTransaction(Transaction t) {
            using (var db = new DbModel()) {
                try {
                    var newTransaction = new Transactions() {
                        amount = t.amount,
                        timeToBeTransfered = t.timeToBeTransfered,
                        timeTransfered = null,
                        toAccountNumber = t.toAccountNumber,
                        comment = t.comment
                    };

                    if (newTransaction.timeToBeTransfered == null) {
                        newTransaction.timeToBeTransfered = DateTime.Now;
                    }

                    var account = db.accounts.FirstOrDefault(a => a.accountNumber == t.fromAccountNumber);

                    if (account == null) {
                        return false;
                    }

                    account.transactions.Add(newTransaction);
                    db.SaveChanges();
                    return true;
                } catch (Exception exc) {
                    return false;
                }
            }
        }

        public bool insertCustomer() {
            using (var db = new DbModel()) {
                var customer = new Customers();
                customer.firstName = "Sander";
                customer.lastName = "Sandøy";
                customer.personalNumber = "12345678902";
                customer.address = "Masterberggata 25";
                string innPassord = "Sofa123";
                var algoritme = System.Security.Cryptography.SHA512.Create();
                byte[] inndata, utdata;
                inndata = System.Text.Encoding.ASCII.GetBytes(innPassord);
                utdata = algoritme.ComputeHash(inndata);

                customer.password = utdata;
                PostalNumbers p = new PostalNumbers();
                p.postalNumber = "8900";
                p.postalCity = "Brønnøysund";
                customer.postalNumbers = p;

                var a = new Accounts();
                a.accountNumber = "12345678901";
                a.balance = 0;
                a.owner = customer;
                a.transactions = new List<Transactions>();
                try {
                    db.customers.Add(customer);
                    db.postalNumbers.Add(p);
                    db.accounts.Add(a);
                    db.SaveChanges();
                    return true;
                } catch (Exception e) {
                    return false;
                }
            }
        }
    }
}