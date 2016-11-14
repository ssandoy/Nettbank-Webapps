using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Model;
using MvcContrib.TestHelper.Ui;

namespace DAL
{
    public class CustomerRepositoryStub : ICustomerRepository
    {

        public bool ValidateCustomer(FormCollection inList)
        {
            var customer = FindByPersonNr(inList["Personnumber"]);
            if (customer != null)
            {
                var password = Convert.ToBase64String(customer.Password);
                var rehash = CreateHash(inList["Password"], customer.Salt);
                var context = HttpContext.Current;
                if (password.Equals(rehash))
                {
                    context.Session["CurrentUser"] = customer.PersonalNumber;
                    Debug.WriteLine("Du er nå logget inn");
                    return true;
                }
                else {
                    context.Session["loggedin"] = false;
                    context.Session["CurrentUser"] = null;
                    Debug.WriteLine("Kunne ikke logge inn");
                    return false;
                }
            }
            else {
                return false;
            }
        }

        public Customers FindByPersonNr(string personalNumber)
        {
            Customers customer = new Customers()
            {
                PersonalNumber = personalNumber,
                FirstName = "Kjetil",
                LastName = "Olsen",
                Address = "Masterberggata 25"

            };
            return customer;
        }

        public string CreateHash(string password, string salt)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            var sha256String = new SHA256Managed();
            var hash = sha256String.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public byte[] HashPassword(string password, string salt)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            var sha256String = new SHA256Managed();
            byte[] hash = sha256String.ComputeHash(bytes);
            return hash;
        }



        public string CreateSalt(int size)
        {
            var randomNumberGenerator = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var salt = new byte[size];
            randomNumberGenerator.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public CustomerInfo GetCustomerInfo(string personalNumber)
        {

            CustomerInfo customer = new CustomerInfo()
            {
                PersonalNumber = personalNumber,
                FirstName = "Kjetil",
                LastName = "Olsen",
                Address = "Masterberggata 25"

            };
            return customer;
        
        }



        public string UpdateCustomer(CustomerInfo customer) 
        {
           
              // Validerer navn.
              if (customer.FirstName == "" || customer.LastName == "")
              {
                  return "Fornavn og etternavn må skrives inn.";
              }

                    
               return "";
        }

        public List<CustomerInfo> ListCustomers()
        {
          
          var customers = new List<CustomerInfo>();
            var customer = new CustomerInfo()
            {
                PersonalNumber = "12345678901",
                FirstName = "Kjetil",
                LastName = "Olsen",
                Address = "Masterberggata 25"
            };
            for (int i = 0; i < 3; i++)
            {
                customers.Add(customer);
            }

              return customers;

        }

        public bool DeleteCustomer(string personalNumber)
        {
            if (personalNumber.IsNullOrEmpty())
            {
                return false;
            }
            return true;
        }

        public string AddCustomer(CustomerInfo customerInfo)
        { 

           if (customerInfo.FirstName == "" || customerInfo.LastName == "")
          {
              return "Fornavn og etternavn må skrives inn.";
          }

             return "";
        
            
        }

        public bool ChangePassword(FormCollection inList)
        {
            throw new NotImplementedException();
        }
    }
}
