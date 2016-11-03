using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DAL;
using Model;

namespace BLL {
    public class CustomerLogic {
        public bool Login() {
            var customerAccess = new CustomerAccess();
            return customerAccess.Login();
        }

        public void Logout() {
            var customerAccess = new CustomerAccess();
            customerAccess.Logout();
        }

        public bool ValidateCustomer(FormCollection inList) {
            var customerAccess = new CustomerAccess();
            return customerAccess.ValidateCustomer(inList);
        }

        public Customers FindByPersonNr(string personalNumber) {
            var customerAccess = new CustomerAccess();
            return customerAccess.FindByPersonNr(personalNumber);
        }

        public string CreateHash(string password, string salt) {
            var customerAccess = new CustomerAccess();
            return customerAccess.CreateHash(password, salt);
        }

        public string CreateSalt(int size) {
            var customerAccess = new CustomerAccess();
            return customerAccess.CreateSalt(size);
        }

        public CustomerInfo GetCustomerInfo(string personalNumber) {
            var customerAccess = new CustomerAccess();
            return customerAccess.GetCustomerInfo(personalNumber);
        }

        public void insertCustomer() {
            var customerAccess = new CustomerAccess();
            customerAccess.insertCustomer();
        }
    }
}
