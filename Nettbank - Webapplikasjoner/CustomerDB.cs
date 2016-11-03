using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Nettbank___Webapplikasjoner.Models;

namespace Nettbank___Webapplikasjoner
{
    public class CustomerDB
    {
        HttpContext context = HttpContext.Current;

        public bool Login()
        {
            if (context.Session["loggedin"] == null)
            {
                context.Session["loggedin"] = false;
            }
            else
            {
                return (bool)context.Session["loggedin"];
            }
            return false;
        }

        public void Logout()
        {
            context.Session["loggedin"] = false;
            context.Session["CurrentUser"] = null;
        }

        public bool ValidateCustomer(FormCollection inList) 
        {
            Customers customer = findByPersonNr(inList["Personnumber"]);
            if (customer != null)
            {
                string password = Convert.ToBase64String(customer.password);
                string ReHash = createHash(inList["Password"], customer.salt);
                HttpContext context = HttpContext.Current;
                if (password.Equals(ReHash))
                {
                    context.Session["CurrentUser"] = customer;
                    Debug.WriteLine("Du er nå logget inn");
                    return true;
                }
                else
                {
                    context.Session["loggedin"] = false;
                    context.Session["CurrentUser"] = null; 
                    Debug.WriteLine("Kunne ikke logge inn");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Customers findByPersonNr(string personnr)
        {
            using (var db = new DbModel())
            {
                List<Customers> customers = db.customers.ToList();
                for (int i = 0; i < customers.Count; i++)
                {
                    if (customers[i].personalNumber == personnr)
                    {
                        return customers[i];
                    }
                }
                return null;
            }
        }

        public string createHash(string password, string salt)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            SHA256Managed SHA256String = new SHA256Managed();
            byte[] hash = SHA256String.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public string createSalt(int size)
        {
            var RandomNumberGenerator = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] salt = new byte[size];
            RandomNumberGenerator.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }
}