using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL;
using Model;

namespace BLL {
    public class CustomerLogic : ICustomerLogic
    {

        private ICustomerRepository _repository;

        public CustomerLogic()
        {
            _repository = new CustomerRepository();
        }

        public CustomerLogic(ICustomerRepository stub)
        {
            _repository = stub;
        }

        public bool ValidateCustomer(FormCollection inList) {
            return _repository.ValidateCustomer(inList);
        }

        public Customers FindByPersonNr(string personalNumber) { //TODO: FJERN?
            return _repository.FindByPersonNr(personalNumber);
        }

        public List<CustomerInfo> ListCustomers()
        {
            return _repository.ListCustomers();
        }

        public string CreateHash(string password, string salt) {
            return _repository.CreateHash(password, salt);
        }

        public string CreateSalt(int size) {
            return _repository.CreateSalt(size);
        }

        public CustomerInfo GetCustomerInfo(string personalNumber) {
            return _repository.GetCustomerInfo(personalNumber);
        }

        public string UpdateCustomer(CustomerInfo c)
        {
            return _repository.UpdateCustomer(c);
        }

        public string AddCustomer(CustomerInfo c)
        {
            return _repository.AddCustomer(c);
        }

        public bool DeleteCustomer(string personalNumber)
        {
            return _repository.DeleteCustomer(personalNumber);
        }
    }
}
