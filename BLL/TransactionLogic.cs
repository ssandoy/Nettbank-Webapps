using System.Collections.Generic;
using DAL;
using Model;

namespace BLL {
    public class TransactionLogic {
        public List<Transaction> ListTransactions(string accountNumber) {
            var transactionAccess = new TransactionAccess();
            return transactionAccess.ListTransactions(accountNumber);
        }

        public List<Transaction> ListExecutedTransactions(string accountNumber) {
            var transactionAccess = new TransactionAccess();
            return transactionAccess.ListExecutedTransactions(accountNumber);
        }

        public string AddTransaction(Transaction t) {
            var transactionAccess = new TransactionAccess();
            return transactionAccess.AddTransaction(t);
        }

        public bool DeleteTransaction(int id) {
            var transactionAccess = new TransactionAccess();
            return transactionAccess.DeleteTransaction(id);
        }

        public Transactions FindTransanction(int id) {
            var transactionAccess = new TransactionAccess();
            return transactionAccess.FindTransanction(id);
        }

        public string UpdateTransaction(Transaction t) {
            var transactionAccess = new TransactionAccess();
            return transactionAccess.UpdateTransaction(t);
        }
    }
}
