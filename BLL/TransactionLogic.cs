using System.Collections.Generic;
using DAL;
using Model;

namespace BLL {
    public class TransactionLogic {
        public List<Transaction> ListTransactions(string accountNumber) {
            var transactionAccess = new TransactionRepository();
            return transactionAccess.ListTransactions(accountNumber);
        }

        public List<Transaction> ListExecutedTransactions(string accountNumber) {
            var transactionAccess = new TransactionRepository();
            return transactionAccess.ListExecutedTransactions(accountNumber);
        }

        public string AddTransaction(Transaction t) {
            var transactionAccess = new TransactionRepository();
            return transactionAccess.AddTransaction(t);
        }

        public bool DeleteTransaction(int id) {
            var transactionAccess = new TransactionRepository();
            return transactionAccess.DeleteTransaction(id);
        }

        public Transactions FindTransanction(int id) {
            var transactionAccess = new TransactionRepository();
            return transactionAccess.FindTransanction(id);
        }

        public string UpdateTransaction(Transaction t) {
            var transactionAccess = new TransactionRepository();
            return transactionAccess.UpdateTransaction(t);
        }

        public List<Transaction> ListExecuteableTransactions() {
            var transactionAccess = new TransactionRepository();
            return transactionAccess.ListExecuteableTransactions();
        }

        public void ExecuteTransaction(int id) {
            var transactionAccess = new TransactionRepository();
            transactionAccess.ExecuteTransaction(id);
        }
    }
}
