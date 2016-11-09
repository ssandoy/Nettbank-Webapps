using System.Collections.Generic;
using DAL;
using Model;

namespace BLL {
    public class TransactionLogic : ITransactionLogic
    {
        private ITransactionRepository _repository;

        public TransactionLogic()
        {
            _repository = new TransactionRepository();
        }

        public TransactionLogic(ITransactionRepository stub)
        {
            _repository = stub;
        }

        public List<Transaction> ListTransactions(string accountNumber) {
            return _repository.ListTransactions(accountNumber);
        }

        public List<Transaction> ListExecutedTransactions(string accountNumber) {
            return _repository.ListExecutedTransactions(accountNumber);
        }

        public string AddTransaction(Transaction t) {
            return _repository.AddTransaction(t);
        }

        public bool DeleteTransaction(int id) {
            return _repository.DeleteTransaction(id);
        }

        public Transactions FindTransanction(int id) {
            return _repository.FindTransanction(id);
        }

        public string UpdateTransaction(Transaction t) {
            return _repository.UpdateTransaction(t);
        }

        public List<Transaction> ListExecuteableTransactions() {
            return _repository.ListExecuteableTransactions();
        }

        public void ExecuteTransaction(int id) {
            _repository.ExecuteTransaction(id);
        }
    }
}
