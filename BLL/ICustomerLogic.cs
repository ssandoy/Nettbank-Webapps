using System.Collections.Generic;
using System.Web.Mvc;
using DAL;
using Model;

namespace BLL
{
    public interface ICustomerLogic
    {
        bool ValidateCustomer(FormCollection inList);
        List<CustomerInfo> ListCustomers();
        string CreateHash(string password, string salt);
        string CreateSalt(int size);
        CustomerInfo GetCustomerInfo(string personalNumber);
        string UpdateCustomer(CustomerInfo c);
        string AddCustomer(CustomerInfo c);
        bool DeleteCustomer(string personalNumber);
        bool ChangePassword(FormCollection inList);

    }
}