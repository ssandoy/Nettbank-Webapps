using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Model;

namespace DAL {
    public class AdminRepository {
        public bool ValidateAdmin(FormCollection inList) {
            var admin = FindAdminByEmployeeNumber(inList["employeeNumber"]);
            if (admin != null) {
                var password = Convert.ToBase64String(admin.password);
                var reHash = CreateHash(inList["password"], admin.salt);
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
                var admins = db.Admins.ToList();
                return admins.FirstOrDefault(t => t.employeeNumber == employeeNumber);
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
                    firstName = "Sander Fagerland",
                    lastName = "Sandøy",
                    employeeNumber = "12345678901",
                    address = "Steinåsen 4"
                };
                var innPassord = "Sofa1234";
                var salt = CreateSalt(32);
                var bytes = System.Text.Encoding.UTF8.GetBytes(innPassord + salt);
                var sha256String = new SHA256Managed();
                var utdata = sha256String.ComputeHash(bytes);
                admin.salt = salt;
                admin.password = utdata;
                var p = new PostalNumbers {
                    PostalNumber = "8909",
                    PostalCity = "Brønnøysund"
                };
                admin.postalNumbers = p;
                try {
                    db.Admins.Add(admin);
                    db.SaveChanges();
                    return true;
                } catch (Exception e) {
                    return false;
                }
            }
        }
    }
}