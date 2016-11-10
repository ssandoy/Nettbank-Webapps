using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public interface ITransactionLogic
    {
        List<Transaction> ListTransactions(string accountNumber);
        List<Transaction> ListExecutedTransactions(string accountNumber);
        string AddTransaction(Transaction t);
        bool DeleteTransaction(int id);
        Transaction FindTransanction(int id);
        string UpdateTransaction(Transaction t);
        List<Transaction> ListExecuteableTransactions();
        void ExecuteTransaction(int id);
    }
}