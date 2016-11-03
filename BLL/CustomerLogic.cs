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
    public class CustomerLogic {

        private readonly HttpContext _context = HttpContext.Current;

        public bool Login()
        {
            if (_context.Session["loggedin"] == null)
            {
                _context.Session["loggedin"] = false;
            }
            else {
                return (bool)_context.Session["loggedin"];
            }
            return false;
        }

        public void Logout()
        {
            _context.Session["loggedin"] = false;
            _context.Session["CurrentUser"] = null;
        }


        public bool ValidateCustomer(FormCollection inList) {
            var customerAccess = new CustomerRepository();
            return customerAccess.ValidateCustomer(inList);
        }

        public Customers FindByPersonNr(string personalNumber) { //TODO: FJERN?
            var customerAccess = new CustomerRepository();
            return customerAccess.FindByPersonNr(personalNumber);
        }

        public List<CustomerInfo> ListCustomers()
        {
            var customerAccess = new CustomerRepository();
            return customerAccess.ListCustomers();
        }

        public string CreateHash(string password, string salt) {
            var customerAccess = new CustomerRepository();
            return customerAccess.CreateHash(password, salt);
        }

        public string CreateSalt(int size) {
            var customerAccess = new CustomerRepository();
            return customerAccess.CreateSalt(size);
        }

        public CustomerInfo GetCustomerInfo(string personalNumber) {
            var customerAccess = new CustomerRepository();
            return customerAccess.GetCustomerInfo(personalNumber);
        }

        public string UpdateCustomer(CustomerInfo c)
        {
            var customerAccess = new CustomerRepository();
            return customerAccess.UpdateCustomer(c);
        }

        public string AddCustomer(CustomerInfo c)
        {
            var customerAccess = new CustomerRepository();
            return customerAccess.AddCustomer(c);
        }

        public void insertCustomer() {
            var customerAccess = new CustomerRepository();
            customerAccess.insertCustomer();
        }

        public bool DeleteCustomer(string personalNumber)
        {
            var customerAccess = new CustomerRepository();
            return customerAccess.DeleteCustomer(personalNumber);
        }
    }
}
