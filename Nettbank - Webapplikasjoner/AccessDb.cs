using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Nettbank___Webapplikasjoner.Models;
using System.Diagnostics;
using System.Data.Entity;

namespace Nettbank___Webapplikasjoner {
    public class AccessDb {

        public List<Account> listAccounts(string personalNumber) {
            using (var db = new DbModel())
            {
                insertCustomer();
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

        public bool insertCustomer()
        {
            using (var db = new DbModel())
            {
                var customer = new Customers();
                customer.firstName = "Sander";
                customer.lastName = "Sandøy";
                customer.personalNumber = "12345678902";
                customer.address = "Masterberggata 25";
                string innPassord = "Sofa123";
                string salt = createSalt(32); //TODO: Hvilken størrelse?
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(innPassord + salt);
                SHA256Managed SHA256String = new SHA256Managed();
                byte[] utdata = SHA256String.ComputeHash(bytes);
                customer.salt = salt;
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
                try
                {
                    db.customers.Add(customer);
                    db.postalNumbers.Add(p);
                    db.accounts.Add(a);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public bool ValidateCustomer(FormCollection inList) //TODO: FIX parameterverdi
        {
            Customers customer = findByPersonNr(inList["personnumber"]);
            if (customer != null)
            {
                string password = Convert.ToBase64String(customer.password);
                string ReHash = createHash(inList["password"], customer.salt);
                if (password.Equals(ReHash))
                {
                    //TODO: SETT SESSION TIL LOGIN TRUE OSV.?
                    return true;
                }
                else
                {
                    //TODO: SETT SESSION TIL LOGIN FALSE OSV?
                    return false;
                }
            }
            else
            {
                return false;
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

        public bool deleteTransaction(int id)
        {
            using (var db = new DbModel())
            {
                try
                {
                    Transactions deleteTransaction = db.transactions.Find(id);
                    db.transactions.Remove(deleteTransaction);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception feil)
                {
                    return false;
                }
            }
        }

        public Customers findByPersonNr(string personnr)
        {
            using (var db = new DbModel())
            {
                List<Customers> customers = db.customers.ToList();
                for (int i = 0; i < customers.Count; i++)
                {
                    if (customers[i].personalNumber == personnr)
                    {
                        return customers[i];
                    }
                }
                return null;
            }
        }

        public string createHash(string password, string salt)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            SHA256Managed SHA256String = new SHA256Managed();
            byte[] hash = SHA256String.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public string createSalt(int size)
        {
            var RandomNumberGenerator = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] salt = new byte[size];
            RandomNumberGenerator.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }
}