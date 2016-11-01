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
    public class AdminDB
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
            context.Session["CurrentAdmin"] = null;
        }


        public bool ValidateAdmin(FormCollection inList)
        {
            Admins admin = findAdminByEmployeeNumber(inList["employeeNumber"]);
            if (admin != null)
            {
                string password = Convert.ToBase64String(admin.password);
                string reHash = createHash(inList["password"], admin.salt);
                HttpContext context = HttpContext.Current;
                if (password.Equals(reHash))
                {
                    context.Session["CurrentAdmin"] = admin;
                    Debug.WriteLine("Du er nå logget inn som Admin!");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Admins findAdminByEmployeeNumber(string employeeNumber)
        {
            using (var db = new DbModel())
            {
                List<Admins> admins = db.admins.ToList();
                for (int i = 0; i < admins.Count; i++)
                {
                    if (admins[i].employeeNumber == employeeNumber)
                    {
                        return admins[i];
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

        public bool insertAdmin()
        {
            using (var db = new DbModel())
            {
                var admin = new Admins();
                admin.firstName = "Sander Fagerland";
                admin.lastName = "Sandøy";
                admin.employeeNumber = "12345678901";
                admin.address = "Steinåsen 4";
                string innPassord = "Sofa1234";
                string salt = createSalt(32);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(innPassord + salt);
                SHA256Managed SHA256String = new SHA256Managed();
                byte[] utdata = SHA256String.ComputeHash(bytes);
                admin.salt = salt;
                admin.password = utdata;
                PostalNumbers p = new PostalNumbers();
                p.postalNumber = "8909";
                p.postalCity = "Brønnøysund";
                admin.postalNumbers = p;
                try
                {
                    db.admins.Add(admin);
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