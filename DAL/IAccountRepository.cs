using System.Collections.Generic;
using Model;

namespace DAL
{
    public interface IAccountRepository
    {
        List<Account> ListAccounts(string personalNumber);
        bool DeleteAccount(string accountNumber);
        EditableAccount GetUpdateableAccount(string accountNumber);
        string UpdateAccount(EditableAccount a);
        bool AddAccount(string personalNumber);
    }
}