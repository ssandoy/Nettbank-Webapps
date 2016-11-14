using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Model;

namespace DAL {
    public class AdminRepository : IAdminRepository
    {

        public bool ValidateAdmin(FormCollection inList) {
            var admin = FindAdminByEmployeeNumber(inList["EmployeeNumber"]);
            if (admin != null) {
                var password = Convert.ToBase64String(admin.Password);
                var reHash = CreateHash(inList["Password"], admin.Salt);
                var context = HttpContext.Current;
                if (password.Equals(reHash)) {
                    context.Session["CurrentAdmin"] = admin;
                    Debug.WriteLine("Du er nå logget inn som Admin!");
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        public Admins FindAdminByEmployeeNumber(string employeeNumber) {
            using (var db = new DbModel()) {
                try
                {
                    var admins = db.Admins.ToList();
                    return admins.FirstOrDefault(t => t.EmployeeNumber == employeeNumber);
                }
                catch (Exception exc)
                {
                    string error = "Exception: " + exc.ToString() + " catched at FindAdminByEmployeeNumber()";
                    writeToErrorLog(error);
                    return null;
                }
            }
        }

        public string CreateHash(string password, string salt) {
            var bytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            var sha256String = new SHA256Managed();
            var hash = sha256String.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public string CreateSalt(int size) {
            var randomNumberGenerator = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var salt = new byte[size];
            randomNumberGenerator.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public bool InsertAdmin() {
            using (var db = new DbModel()) {
                var admin = new Admins {
                    FirstName = "Sander Fagerland",
                    LastName = "Sandøy",
                    EmployeeNumber = "12345678901",
                    Address = "Steinåsen 4"
                };
                var innPassord = "Sofa1234";
                var salt = CreateSalt(32);
                var bytes = System.Text.Encoding.UTF8.GetBytes(innPassord + salt);
                var sha256String = new SHA256Managed();
                var utdata = sha256String.ComputeHash(bytes);
                admin.Salt = salt;
                admin.Password = utdata;
                admin.PostalNumbers = db.PostalNumbers.Find("8909");
                try {
                    db.Admins.Add(admin);
                    db.SaveChanges();
                    return true;
                } catch (Exception e) {
                    string error = "Exception: " + e.ToString() + " catched at InsertAdmin()";
                    writeToErrorLog(error);
                    return false;
                }
            }
        }
        public void writeToErrorLog(string error)
        {
            string path = "ErrorLog.txt";
            var _Path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/"), path);
            if (!File.Exists(_Path))
            {
                string createText = error + Environment.NewLine;
                File.WriteAllText(_Path, createText);
            }
            else
            {
                string appendText = error + Environment.NewLine;
                File.AppendAllText(_Path, appendText);
            }

        }
    }
}