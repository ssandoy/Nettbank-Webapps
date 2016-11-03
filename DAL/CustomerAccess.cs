using Model;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace DAL {
    public class CustomerAccess {
        private readonly HttpContext _context = HttpContext.Current;

        public bool Login() {
            if (_context.Session["loggedin"] == null) {
                _context.Session["loggedin"] = false;
            }
            else {
                return (bool) _context.Session["loggedin"];
            }
            return false;
        }

        public void Logout() {
            _context.Session["loggedin"] = false;
            _context.Session["CurrentUser"] = null;
        }

        public bool ValidateCustomer(FormCollection inList) {
            var customer = FindByPersonNr(inList["Personnumber"]);
            if (customer != null) {
                var password = Convert.ToBase64String(customer.Password);
                var rehash = CreateHash(inList["Password"], customer.Salt);
                var context = HttpContext.Current;
                if (password.Equals(rehash)) {
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

        public Customers FindByPersonNr(string personalNumber) {
            using (var db = new DbModel()) {
                var customers = db.Customers.ToList();
                return customers.FirstOrDefault(t => t.PersonalNumber == personalNumber);
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

        public CustomerInfo GetCustomerInfo(string personalNumber) {
            using (var db = new DbModel()) {
                var customer = db.Customers.ToList().FirstOrDefault(t => t.PersonalNumber == personalNumber);

                if (customer == null) {
                    return null;
                }

                var customerInfo = new CustomerInfo() {
                    PersonalNumber = customer.PersonalNumber,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Address = customer.Address + " " + customer.PostalNumber + " " + customer.PostalNumbers.PostalCity
                };
                return customerInfo;
            }
        }

        public bool insertCustomer() {
            using (var db = new DbModel()) {
                var customer = new Customers();
                customer.FirstName = "Ole Johan";
                customer.LastName = "Olsen";
                customer.PersonalNumber = "12126948141";
                customer.Address = "Steinåsen 4";
                string innPassord = "Sofa123456";
                string salt = CreateSalt(32);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(innPassord + salt);
                SHA256Managed SHA256String = new SHA256Managed();
                byte[] utdata = SHA256String.ComputeHash(bytes);
                customer.Salt = salt;
                customer.Password = utdata;
                PostalNumbers p = new PostalNumbers();
                p.PostalNumber = "8909";
                p.PostalCity = "Brønnøysund";
                customer.PostalNumbers = p;
                try {
                    db.Customers.Add(customer);
                    db.PostalNumbers.Add(p);
                    db.SaveChanges();
                    return true;
                } catch (Exception e) {
                    return false;
                }
            }
        }

        public string updateCustomer(CustomerAdmin customer) { //TODO: Fiks
            using (var db = new DbModel()) {
                try {
                    var customers = db.customers.Find(customer.personalNumber); //TODO: THIS WONT WORK WHEN CHANGED. Kanskje ikke ha mulighet for å endre personnummer?
                    customers.personalNumber = customer.personalNumber;
                    customers.firstName = customer.firstName;
                    customers.lastName = customer.lastName;

                    // Validerer navn.
                    if (customers.firstName == null || customers.lastName == null) {
                        return "Fornavn og etternavn må skrives inn.";
                    }


                    // Validerer personnummer
                    if (customers.personalNumber == null) { //TODO: TRENGS DISSE SIDEN VI HAR VIEWMODEL?
                        return "Kontoen du vil betale til eksisterer ikke";
                    }


                    db.Entry(customers).State = EntityState.Modified;
                    db.SaveChanges();
                    return "";
                } catch (Exception exc) {
                    return "Feil: " + exc.Message;
                }
            }
        }

        public List<CustomerAdmin> ListCustomers() //TODO: Fiks
        {
            using (var db = new DbModel())
            {
                List<CustomerAdmin> customers = (from p in db.customers
                                                 select
                                                 new CustomerAdmin() {
                                                     personalNumber = p.personalNumber,
                                                     firstName = p.firstName,
                                                     lastName = p.lastName,


                                                 }).ToList();

                return customers;

            }
}
    }
}