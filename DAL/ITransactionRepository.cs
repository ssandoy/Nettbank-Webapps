using System.Collections.Generic;
using Model;

namespace DAL
{
    public interface ITransactionRepository
    {
        List<Transaction> ListTransactions(string accountNumber);
        List<Transaction> ListExecutedTransactions(string accountNumber);
        string AddTransaction(Transaction t);
        bool DeleteTransaction(int id);
        Transactions FindTransanction(int id);
        string UpdateTransaction(Transaction t);
        List<Transaction> ListExecuteableTransactions();
        void ExecuteTransaction(int id);
    }
}