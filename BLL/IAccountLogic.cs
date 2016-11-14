using System.Collections.Generic;
using Model;

namespace BLL
{
    public interface IAccountLogic
    {
        List<Account> ListAccounts(string personalNumber);
        bool DeleteAccount(string accountNumber);
        EditableAccount GetUpdateableAccount(string accountNumber);
        string UpdateAccount(EditableAccount updatedAccount);
        bool AddAccount(string personalNumber);
    }
}