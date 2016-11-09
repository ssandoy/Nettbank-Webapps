using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Model;

namespace DAL {
    public class TransactionRepository : ITransactionRepository
    {
        public List<Transaction> ListTransactions(string accountNumber) {
            using (var db = new DbModel()) {
                try
                {
                    var allTransactions = db.Transactions.Where(t => t.AccountNumber == accountNumber);
                    var transactions = new List<Transaction>();
                    foreach (var t in allTransactions)
                    {
                        if (t.TimeTransfered == null)
                        {
                            transactions.Add(new Transaction
                            {
                                TransactionId = t.TransactionId,
                                Amount = t.Amount,
                                TimeToBeTransfered = t.TimeToBeTransfered,
                                TimeTransfered = t.TimeTransfered,
                                FromAccountNumber = t.AccountNumber,
                                ToAccountNumber = t.ToAccountNumber,
                                Comment = t.Comment
                            });
                        }
                    }
                    return transactions;
                }
                catch (Exception exc)
                {
                    string error = "Exception: " + exc.ToString() + " catched at ListTransaction()";
                    writeToErrorLog(error);
                    return null;
                }
            }
        }

        public List<Transaction> ListExecutedTransactions(string accountNumber) {
            using (var db = new DbModel()) {
                try
                {
                    var allTransactions =
                        db.Transactions.Where(
                            t => t.AccountNumber == accountNumber || t.ToAccountNumber == accountNumber);
                    var transactions = new List<Transaction>();
                    foreach (var t in allTransactions)
                    {
                        if (t.TimeTransfered != null)
                        {
                            transactions.Add(new Transaction
                            {
                                TransactionId = t.TransactionId,
                                Amount = t.Amount,
                                TimeTransfered = t.TimeTransfered,
                                FromAccountNumber = t.AccountNumber,
                                ToAccountNumber = t.ToAccountNumber,
                                Comment = t.Comment
                            });
                        }
                    }
                    return transactions;
                }
                catch (Exception exc)
                {
                    string error = "Exception: " + exc.ToString() + " catched at ListExecutedTransaction()";
                    writeToErrorLog(error);
                    return null;
                }
            }
        }

        public string AddTransaction(Transaction t) {
            using (var db = new DbModel()) {
                try {
                    var newTransaction = new Transactions() {
                        Amount = t.Amount,
                        TimeToBeTransfered = t.TimeToBeTransfered,
                        TimeTransfered = null,
                        ToAccountNumber = t.ToAccountNumber,
                        Comment = t.Comment
                    };

                    // Validerer beløp.
                    if (newTransaction.Amount <= 0) {
                        return "Beløpet må være positivt.";
                    }

                    // Validerer utførelsesdato.
                    if (newTransaction.TimeToBeTransfered == null) {
                        newTransaction.TimeToBeTransfered = DateTime.Now;
                    }
                    else if (newTransaction.TimeToBeTransfered.Value.CompareTo(DateTime.Now) < 0) {
                        return
                            "Utførelsesdatoen på være en dato i fremtiden. La feltet stå tomt for å utføre betalingen umiddelbart.";
                    }

                    // Validerer fra-kontonummer.
                    var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == t.FromAccountNumber);
                    if (account == null) {
                        return "Kontoen du vil betale fra eksisterer ikke";
                    }

                    // Validerer disponibel saldo på fra-konto
                    if (account.AvailableBalance < newTransaction.Amount) {
                        return "Kontoen du vil betale fra har ikke nok disponibel saldo";
                    }

                    // Oppdaterer disponibel saldo
                    account.AvailableBalance -= newTransaction.Amount;
                    db.Entry(account).State = EntityState.Modified;

                    // Validerer til-kontonummer.
                    /* Kommentert bort ettersom vi vil tillate at det betales til andre banker selv om de
                     * ikke eksisterer innad i prosjektet. Man kan fjerne kommentaren dersom man kun vil
                     * tillate transaksjoner mellom kontoer i databasen.
                    if (db.accounts.FirstOrDefault(a => a.accountNumber == newTransaction.toAccountNumber) == null) {
                        return "Kontoen du vil betale til eksisterer ikke";
                    }*/

                    account.Transactions.Add(newTransaction);
                    db.SaveChanges();
                    return "";
                }
                catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at AddTransaction()";
                    writeToErrorLog(error);
                    return "Feil: " + exc.Message;
                }
            }
        }

        public bool DeleteTransaction(int id) {
            using (var db = new DbModel()) {
                try {
                    var deleteTransaction = db.Transactions.Find(id);
                    db.Transactions.Remove(deleteTransaction);

                    // Oppdaterer disponibel saldo
                    var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == deleteTransaction.AccountNumber);
                    if (account != null) {
                        account.AvailableBalance += deleteTransaction.Amount;
                    }

                    db.SaveChanges();
                    return true;
                }
                catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at DeleteTransaction()";
                    writeToErrorLog(error);
                    return false;
                }
            }
        }

        public Transactions FindTransanction(int id) {
            using (var db = new DbModel()) {
                try {
                    var transaction = db.Transactions.Find(id);
                    return transaction ?? null;
                }
                catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at FindTransaction()";
                    writeToErrorLog(error);
                    return null;
                }
            }
        }

        public string UpdateTransaction(Transaction t) {
            using (var db = new DbModel()) {
                try {
                    var transactions = db.Transactions.Find(t.TransactionId);
                    transactions.ToAccountNumber = t.ToAccountNumber;
                    transactions.Comment = t.Comment;

                    // Setter og validerer utførelsesdato.
                    if (t.TimeToBeTransfered != null) {
                        if (t.TimeToBeTransfered.Value.CompareTo(DateTime.Now) > 0) {
                            transactions.TimeToBeTransfered = t.TimeToBeTransfered;
                        }
                        else {
                            return
                                "Utførelsesdatoen på være en dato i fremtiden. La feltet stå tomt for å ikke endre på datoen.";
                        }
                    }

                    // Validerer til-kontonummer.
                    /* Kommentert bort ettersom vi vil tillate at det betales til andre banker selv om de
                     * ikke eksisterer innad i prosjektet. Man kan fjerne kommentaren dersom man kun vil
                     * tillate transaksjoner mellom kontoer i databasen.
                    if (db.accounts.FirstOrDefault(a => a.accountNumber == t.toAccountNumber) == null) {
                        return "Kontoen du vil betale til eksisterer ikke";
                    }*/

                    // Validerer og setter fra-kontonummer hvis det er endret.
                    Accounts oldAccount = transactions.Account;
                    Accounts newAccount = null;
                    if (transactions.AccountNumber != t.FromAccountNumber) {
                        newAccount = db.Accounts.FirstOrDefault(a => a.AccountNumber == t.FromAccountNumber);

                        if (newAccount == null) {
                            return "Kontoen du vil betale fra eksisterer ikke";
                        }

                        oldAccount.Transactions.Remove(transactions);
                        newAccount.Transactions.Add(transactions);
                    }
                    else {
                        db.Entry(transactions).State = EntityState.Modified;
                    }

                    // Oppdaterer disponibel saldo
                    oldAccount.AvailableBalance += transactions.Amount;
                    if (newAccount == null) {
                        oldAccount.AvailableBalance -= t.Amount;
                    } else {
                        newAccount.AvailableBalance -= t.Amount;
                    }

                    // Oppdaterer og validerer beløp.
                    transactions.Amount = t.Amount;
                    if (transactions.Amount <= 0) {
                        return "Beløpet må være positivt.";
                    }

                    db.SaveChanges();
                    return "";
                }
                catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at UpdateTransaction()";
                    writeToErrorLog(error);
                    return "Feil: " + exc.Message;
                }
            }
        }

        public List<Transaction> ListExecuteableTransactions() {
            using (var db = new DbModel()) {
                var allTransactions = db.Transactions.Where(t => t.TimeTransfered == null);
                var transactions = new List<Transaction>();
                foreach (var t in allTransactions) {  
                    transactions.Add(new Transaction {
                        TransactionId = t.TransactionId,
                        Amount = t.Amount,
                        TimeToBeTransfered = t.TimeToBeTransfered,
                        TimeTransfered = t.TimeTransfered,
                        FromAccountNumber = t.AccountNumber,
                        ToAccountNumber = t.ToAccountNumber,
                        Comment = t.Comment
                    });
                }
                return transactions.OrderBy(t => t.TimeToBeTransfered).ToList();
            }
        }

        public void ExecuteTransaction(int id) {
            using (var db = new DbModel()) {
                try {
                    // Find the transaction and check that it exists
                    var transaction = db.Transactions.Find(id);
                    if (transaction == null) {
                        return;
                    }
                    // Find the accounts and check that the fromAccount exists and that it's good for the amount
                    var fromAccount = db.Accounts.Find(transaction.AccountNumber);
                    var toAccount = db.Accounts.Find(transaction.ToAccountNumber);
                    if (fromAccount == null || fromAccount.Balance < transaction.Amount) {
                        db.Transactions.Remove(transaction);
                        db.Entry(transaction).State = EntityState.Deleted;
                        db.SaveChanges();
                        return;
                    }
                    // Transfers the amount and adds the transferedTime
                    fromAccount.Balance -= transaction.Amount;
                    db.Entry(fromAccount).State = EntityState.Modified;
                    if (toAccount != null) {
                        // Transfers the money to toAccount if it exists. 
                        toAccount.Balance += transaction.Amount;
                        toAccount.AvailableBalance += transaction.Amount;
                        db.Entry(toAccount).State = EntityState.Modified;
                        // If it doesn't, no account will receive the money. This simulates that toAccount is an account from another bank.
                    }
                    transaction.TimeTransfered = DateTime.Now;
                    db.Entry(transaction).State = EntityState.Modified;
                    db.SaveChanges();
                    return;
                } catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at ExecuteTransaction()";
                    writeToErrorLog(error);
                    return;
                }
            }
        }

        public void writeToErrorLog(string error)
        {
            string path = "ErrorLog.txt";
            var _Path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/"), path);
            if (!File.Exists(_Path))
            {
                string createText = error + Environment.NewLine;
                File.WriteAllText(_Path, createText);
            }
            else
            {
                string appendText = error + Environment.NewLine;
                File.AppendAllText(_Path, appendText);
            }

        }
    }
}