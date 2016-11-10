using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DAL
{
    public class AccountRepositoryStub : IAccountRepository
    {
        public List<Account> ListAccounts(string personalNumber)
        {
    
            var cList = new List<CustomerInfo>();
            var customer = new CustomerInfo()
            {
                PersonalNumber = personalNumber,
                FirstName = "Kjetil",
                LastName = "Olsen",
                Address = "Masterberggata 25"
            };


            for (int i = 0; i < 3; i++)
            {
                cList.Add(customer);
            }


            var list = new List<Account>();
            Account acc = new Account()
            {
                AccountNumber = "12345678909",
                Balance = 0,
                OwnerName = "Kjetil Olsen"
            };

            for (int i = 0; i < 3; i++)
            {
                list.Add(acc);
            }

            return list;
        }

        public bool DeleteAccount(string accountNumber)
        {
            var list = new List<Account>();
            Account acc = new Account()
            {
                AccountNumber = "12345678909",
                Balance = 0,
                OwnerName = "Kjetil Olsen"
            };

            for (int i = 0; i < 3; i++)
            {
                list.Add(acc);
            }
            Account ac = new Account()
            {
                AccountNumber = accountNumber,
                Balance = 0,
                OwnerName = "Kjetil Olsen"
            };

            list.Add(ac);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].AccountNumber.Equals(accountNumber))
                    return list.Remove(list[i]);
            }
            return false;
        }

        public EditableAccount GetUpdateableAccount(string accountNumber)
        {
            EditableAccount acc = new EditableAccount()
            {
                AccountNumber = accountNumber,
                OwnerPersonalNumber = "12345678901"
            };
            return acc;
        }

        public string UpdateAccount(EditableAccount a)
        {
            var account = new EditableAccount()
            {
                AccountNumber = "12345678991",
                OwnerPersonalNumber = "12345678901"
            };
            account = a;
            if (account.OwnerPersonalNumber == null)
            {
                return "Feil!";
            }
            if(account.AccountNumber != a.AccountNumber)
            {
                return "Feil!";
            }
            return "";
        }

        public bool AddAccount(string personalNumber)
        {
            if (personalNumber.Length == 11)
                return true;
            else
                return false;

        }




    }
}
