using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DAL
{
   public class TransactionRepositoryStub : ITransactionRepository
    {
        public List<Transaction> ListTransactions(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public List<Transaction> ListExecutedTransactions(string accountNumber)
        {
            throw new NotImplementedException();
        }

        public string AddTransaction(Transaction t)
        {
            throw new NotImplementedException();
        }

        public bool DeleteTransaction(int id)
        {
            throw new NotImplementedException();
        }

        public Transactions FindTransanction(int id)
        {
            throw new NotImplementedException();
        }

        public string UpdateTransaction(Transaction t)
        {
            throw new NotImplementedException();
        }

        public List<Transaction> ListExecuteableTransactions()
        {
            var list = new List<Transaction>();
            var transaction = new Transaction()
            {
                Amount = 100,
                FromAccountNumber = "12345678901",
                ToAccountNumber = "12345678902",
            };
            list.Add(transaction);
            list.Add(transaction);
            list.Add(transaction);

            return list;
        }

        public void ExecuteTransaction(int id)
        {
            throw new NotImplementedException();
        }
    }
}
