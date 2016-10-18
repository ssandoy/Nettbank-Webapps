﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Nettbank___Webapplikasjoner.Models;

namespace Nettbank___Webapplikasjoner {
    public class AccessDb {

        public List<Account> listAccounts(string personalNumber) {
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
                var allTransactions = db.transactions.Where(t => t.fromAccount.accountNumber == accountNumber);
                var transactions = new List<Transaction>();
                foreach (var t in allTransactions) {
                    transactions.Add(new Transaction {
                        amount = t.amount,
                        executed = t.executed,
                        timeToBeTransfered = t.timeToBeTransfered,
                        timeTransfered = t.timeTransfered,
                        fromAccountNumber = t.fromAccount.accountNumber,
                        toAccountNumber = t.toAccount.accountNumber
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
                customer.postalNumber = p;
               
                var a = new Accounts();
                a.accountNumber = "123456";
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

        public bool addTransaction(Transaction t) {
            using (var db = new DbModel()) {
                try {
                    var newTransaction = new Transactions() {
                        amount = t.amount,
                        executed = false,
                        timeToBeTransfered = t.timeToBeTransfered,
                        timeTransfered = DateTime.MinValue,
                        fromAccount = validateAccountNumber(t.fromAccountNumber),
                        toAccount = validateAccountNumber(t.toAccountNumber)
                    };

                    if (newTransaction.fromAccount == null || newTransaction.toAccount == null) {
                        return false;
                    }

                    db.transactions.Add(newTransaction);
                    db.SaveChanges();
                    return true;
                } catch (Exception exc) {
                    return false;
                }
            }
        }

        /*private DateTime validateDate(DateTime time) {
            if (time.CompareTo(DateTime.Now) < 0)
        }*/

        private Accounts validateAccountNumber(string accountNumber) {
            using (var db = new DbModel()) {
                return db.accounts.FirstOrDefault(a => a.accountNumber == accountNumber);
            }
        }
            else
            {
                return false;
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