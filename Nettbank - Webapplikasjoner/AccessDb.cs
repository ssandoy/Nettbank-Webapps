using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nettbank___Webapplikasjoner.Models;

namespace Nettbank___Webapplikasjoner {
    public class AccessDb {

        public List<Account> allAccounts() {
            using (var db = new DbModel()) {
                List<Account> Accounts = db.accounts.Select(a => new Account {
                    accountNumber = a.accountNumber,
                    balance = a.balance
                }).ToList();
                return Accounts;
            }
        }

        public List<Transaction> listTransactions(string personalNumber) {
            using (var db = new DbModel()) {
                var allTransactions = db.transactions.Where(t => t.fromAccount.owner.personalNumber == personalNumber);
                var transactions = new List<Transaction>();
                foreach (var t in allTransactions) {
                    transactions.Add(new Transaction {
                        
                    });
                }

                return transactions;
            }
            return null;
        }
    }
}