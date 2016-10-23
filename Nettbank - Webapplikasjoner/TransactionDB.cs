using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Nettbank___Webapplikasjoner.Models;

namespace Nettbank___Webapplikasjoner
{
    public class TransactionDB
    {

        public List<Transaction> listTransactions(string accountNumber)
        {
            using (var db = new DbModel())
            {
                var allTransactions = db.transactions.Where(t => t.accountNumber == accountNumber);
                var transactions = new List<Transaction>();
                foreach (var t in allTransactions)
                {
                    transactions.Add(new Transaction
                    {
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

        public List<Transaction> listExecutedTransactions(string accountNumber)
        {
            using (var db = new DbModel())
            {
                var allTransactions = db.transactions.Where(t => t.accountNumber == accountNumber || t.toAccountNumber == accountNumber);
                var transactions = new List<Transaction>();
                foreach (var t in allTransactions)
                {
                    if (t.timeTransfered != null)
                    {
                        transactions.Add(new Transaction
                        {
                            transactionId = t.transactionID,
                            amount = t.amount,
                            timeTransfered = t.timeTransfered,
                            fromAccountNumber = t.accountNumber,
                            toAccountNumber = t.toAccountNumber,
                            comment = t.comment
                        });
                    }
                }
                return transactions;
            }
        }

        public string addTransaction(Transaction t) {
            using (var db = new DbModel()) {
                try {
                    var newTransaction = new Transactions() {
                        amount = t.amount,
                        timeToBeTransfered = t.timeToBeTransfered,
                        timeTransfered = null,
                        toAccountNumber = t.toAccountNumber,
                        comment = t.comment
                    };

                    // Validerer beløp.
                    if (newTransaction.amount <= 0) {
                        return "Beløpet må være positivt.";
                    }

                    // Validerer utførelsesdato.
                    if (newTransaction.timeToBeTransfered == null) {
                        newTransaction.timeToBeTransfered = DateTime.Now;
                    } else if (newTransaction.timeToBeTransfered.Value.CompareTo(DateTime.Now) < 0) {
                        return "Utførelsesdatoen på være en dato i fremtiden. La feltet stå tomt for å utføre betalingen umiddelbart.";
                    }

                    // Setter og validerer fra-kontonummer.
                    var account = db.accounts.FirstOrDefault(a => a.accountNumber == t.fromAccountNumber);
                    if (account == null) {
                        return "Kontoen du vil betale fra eksisterer ikke";
                    }

                   /* // Validerer til-kontonummer.
                    if (db.accounts.FirstOrDefault(a => a.accountNumber == newTransaction.toAccountNumber) == null) {
                        return "Kontoen du vil betale til eksisterer ikke";
                    }*/

                    account.transactions.Add(newTransaction);
                    db.SaveChanges();
                    return "";
                } catch (Exception exc) {
                    return "Feil: " + exc.Message;
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

        public Transactions findTransanction(int id)
        {
            using (var db = new DbModel())
            {
                try
                {
                    Transactions transaction = db.transactions.Find(id);
                    if (transaction != null)
                        return transaction;
                    else
                        return null;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public bool updateTransaction(Transaction t)
        {
            using (var db = new DbModel())
            {
                try
                {
                    Transactions transactions = db.transactions.Find(t.transactionId);
                    transactions.toAccountNumber = t.toAccountNumber;
                    transactions.accountNumber = t.fromAccountNumber;
                    transactions.amount = t.amount;
                    transactions.comment = t.comment;
                    if (t.timeToBeTransfered != null)
                    {
                        transactions.timeToBeTransfered = t.timeToBeTransfered;
                    }
                    db.Entry(transactions).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

    }
}