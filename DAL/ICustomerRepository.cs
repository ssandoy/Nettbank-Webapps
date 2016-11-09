using System.Collections.Generic;
using System.Web.Mvc;
using Model;

namespace DAL
{
    public interface ICustomerRepository
    {
        bool ValidateCustomer(FormCollection inList);
        Customers FindByPersonNr(string personalNumber);
        string CreateHash(string password, string salt);
        byte[] HashPassword(string password, string salt);
        string CreateSalt(int size);
        CustomerInfo GetCustomerInfo(string personalNumber);
        string UpdateCustomer(CustomerInfo customer);
        List<CustomerInfo> ListCustomers();
        bool DeleteCustomer(string personalNumber);
        string AddCustomer(CustomerInfo customerInfo);
        
    }
}