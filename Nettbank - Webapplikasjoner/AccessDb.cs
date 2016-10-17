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
    }
}