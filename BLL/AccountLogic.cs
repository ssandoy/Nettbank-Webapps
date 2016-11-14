using System.Collections.Generic;
using System.Web;
using DAL;
using Model;

namespace BLL {
    public class AccountLogic : IAccountLogic
    {

        private IAccountRepository _repository;

        public AccountLogic()
        {
            _repository = new AccountRepository(); 
        }

        public AccountLogic(IAccountRepository stub)
        {
            _repository = stub;
        }


        private readonly HttpContext _context = HttpContext.Current;

        public List<Account> ListAccounts(string personalNumber) {
            return _repository.ListAccounts(personalNumber);
        }


        public bool DeleteAccount(string accountNumber) {
            return _repository.DeleteAccount(accountNumber);
        }

        public EditableAccount GetUpdateableAccount(string accountNumber) {
            return _repository.GetUpdateableAccount(accountNumber);
        }

        public string UpdateAccount(EditableAccount updatedAccount) {
            return _repository.UpdateAccount(updatedAccount);
        }

        public bool AddAccount(string personalNumber) {
            return _repository.AddAccount(personalNumber);
        }
    }
}