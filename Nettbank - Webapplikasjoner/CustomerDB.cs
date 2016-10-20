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

        public bool Logout()
        {
            if (context.Session["loggedin"] == null)
            {
                context.Session["loggedin"] = false;
            }
            else
            {
                context.Session["loggedin"] = false;

            }
            context.Session["CurrentUser"] = null;
            return true;
        }

        public bool ValidateCustomer(FormCollection inList) //TODO: FIX parameterverdi
        {
            Customers customer = findByPersonNr(inList["Personnumber"]);
            if (customer != null)
            {
                string password = Convert.ToBase64String(customer.password);
                string ReHash = createHash(inList["Password"], customer.salt);
                HttpContext context = HttpContext.Current;
                if (password.Equals(ReHash)) // && inList["bankID"].Equals(generateBankId())
                {
                    context.Session["CurrentUser"] = customer;
                    Debug.WriteLine("Du er nå logget inn");
                    return true;
                }
                else
                {
                    //TODO: SETT SESSION TIL LOGIN FALSE OSV?
                    context.Session["loggedin"] = false;
                    context.Session["CurrentUser"] = null; //TODO: TRENGS DISSE?
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


        public bool insertCustomer()
        {
            using (var db = new DbModel())
            {
                var customer = new Customers();
                customer.firstName = "Sander";
                customer.lastName = "Sandøy";
                customer.personalNumber = "12345678902";
                customer.address = "Masterberggata 25";
                string innPassord = "Sofa123";
                string salt = createSalt(32); //TODO: Hvilken størrelse?
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(innPassord + salt);
                SHA256Managed SHA256String = new SHA256Managed();
                byte[] utdata = SHA256String.ComputeHash(bytes);
                customer.salt = salt;
                customer.password = utdata;
                PostalNumbers p = new PostalNumbers();
                p.postalNumber = "8900";
                p.postalCity = "Brønnøysund";
                customer.postalNumbers = p;

                var a = new Accounts();
                a.accountNumber = "12345678901";
                a.balance = 0;
                a.owner = customer;
                a.transactions = new List<Transactions>();
                try
                {
                    db.customers.Add(customer);
                    db.postalNumbers.Add(p);
                    db.accounts.Add(a);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }
    }
}