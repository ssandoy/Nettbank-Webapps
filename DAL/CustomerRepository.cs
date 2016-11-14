using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace DAL {
    public class CustomerRepository : ICustomerRepository
    {
      
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
                try
                {
                    var customers = db.Customers.ToList();
                    return customers.FirstOrDefault(t => t.PersonalNumber == personalNumber);
                }
                catch (Exception exc)
                {
                    string error = "Exception: " + exc.ToString() + " catched at FindByPersonNr()";
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

        public byte[] HashPassword(string password, string salt)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            var sha256String = new SHA256Managed();
            byte[] hash = sha256String.ComputeHash(bytes);
            return hash;
        }



        public string CreateSalt(int size) {
            var randomNumberGenerator = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var salt = new byte[size];
            randomNumberGenerator.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public CustomerInfo GetCustomerInfo(string personalNumber) {
            using (var db = new DbModel()) {
                try
                {
                    var customer = db.Customers.ToList().FirstOrDefault(t => t.PersonalNumber == personalNumber);

                    if (customer == null)
                    {
                        return null;
                    }

                    var customerInfo = new CustomerInfo()
                    {
                        PersonalNumber = customer.PersonalNumber,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Password = Convert.ToBase64String(customer.Password),
                        Address = customer.Address,
                        PostalNumber = customer.PostalNumber,
                        PostalCity = customer.PostalNumbers.PostalCity
                    };
                    return customerInfo;
                }
                catch (Exception exc)
                {
                    string error = "Exception: " + exc.ToString() + " catched at GetCustomerInfo()";
                    writeToErrorLog(error);
                    return null;
                }
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
                customer.PostalNumbers = db.PostalNumbers.Find("8909");
                try {
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    return true;
                } catch (Exception e) {
                    string error = "Exception: " + e.ToString() + " catched at insertCustomer()";
                    writeToErrorLog(error);
                    return false;
                }
            }
        }

        public string UpdateCustomer(CustomerInfo customer) {
            PostalNumbers p;
            using (var db = new DbModel()) {
                try {
                    var customers = db.Customers.Find(customer.PersonalNumber);
                    customers.PersonalNumber = customer.PersonalNumber;
                    customers.FirstName = customer.FirstName;
                    customers.LastName = customer.LastName;
                    customers.Address = customer.Address;
                    if (db.PostalNumbers.Find(customer.PostalNumber) == null)
                    {
                        p = new PostalNumbers();
                        p.PostalNumber = customer.PostalNumber;
                        p.PostalCity = customer.PostalCity;
                        db.PostalNumbers.Add(p);
                    }
                    else
                    {
                        p = db.PostalNumbers.Find(customer.PostalNumber);
                    }
                    customers.PostalNumbers = p;
                    customers.PostalNumber = p.PostalNumber;

                    // Validerer navn.
                    if (customers.FirstName == null || customers.LastName == null) {
                        return "Fornavn og etternavn må skrives inn.";
                    }   

                    // Validerer personnummer
                    if (customers.PersonalNumber == null) { 
                        return "Kontoen du vil betale til eksisterer ikke";
                    }

                    db.Entry(customers).State = EntityState.Modified;
                    //write to changelog
                    var log = new ChangeLog();
                    log.ChangedTime = (DateTime.Now).ToString("yyyyMMddHHmmss");
                    log.EventType = "Update";
                    log.OriginalValue = customers.ToString();
                    log.NewValue = customer.ToString();
                    var context = HttpContext.Current;
                    if (context.Session["CurrentAdmin"] != null)
                    {
                        Admins changedby = (Admins)context.Session["CurrentAdmin"];
                        log.ChangedBy = changedby.FirstName + " " + changedby.LastName;
                    }
                    else
                    {
                        log.ChangedBy = "null";
                    }
                    db.SaveChanges();
                    WriteToChangeLog(log.toString());
                    return "";
                } catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at UpdateCustomer()";
                    writeToErrorLog(error);
                    return "Feil: " + exc.Message;
                }
            }
        }

        public List<CustomerInfo> ListCustomers() {
            using (var db = new DbModel()) {
                try
                {
                    List<CustomerInfo> customers = (from p in db.Customers
                        select
                        new CustomerInfo()
                        {
                            PersonalNumber = p.PersonalNumber,
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            Address = p.Address + " " + p.PostalNumber + " " + p.PostalNumbers.PostalCity
                        }).ToList();

                    return customers;
                }
                catch (Exception exc)
                {
                    string error = "Exception: " + exc.ToString() + " catched at ListCustomers()";
                    writeToErrorLog(error);
                    return null;
                }
            }

        }

        public bool DeleteCustomer(string personalNumber) {
            using (var db = new DbModel()) {
                try {
                    var deleteCustomer = db.Customers.Find(personalNumber);
                    if (deleteCustomer == null) {
                        return false;
                    }
                    // Rydder kontoene for transaksjoner TODO: Skal dette logges?
                    var deleteAccounts = db.Accounts.Where(a => a.PersonalNumber == personalNumber);
                    foreach (var a in deleteAccounts) {
                        // Sletter transaksjoner som ikke er utført TODO: Skal dette logges?
                        var deleteTransactions = a.Transactions.Where(t => t.TimeTransfered == null);
                        db.Transactions.RemoveRange(deleteTransactions);
                        // Kobler alle ikke-utførte transaksjoner fra kontoen slik at den kan slettes TODO: Skal dette logges?
                        a.Transactions.RemoveAll(t => t.TimeTransfered != null);
                    }
                    // Sletter kontoene TODO: Skal dette logges?
                    db.Accounts.RemoveRange(deleteAccounts);
                    // Sletter kunden
                    db.Customers.Remove(deleteCustomer);
                    //write to changelog
                    var log = new ChangeLog();
                    log.ChangedTime = (DateTime.Now).ToString("yyyyMMddHHmmss");
                    log.EventType = "Delete";
                    log.OriginalValue = deleteCustomer.ToString();
                    log.NewValue = "null";
                    var context = HttpContext.Current;
                    if (context.Session["CurrentAdmin"] != null) {
                        Admins changedby = (Admins)context.Session["CurrentAdmin"];
                        log.ChangedBy = changedby.FirstName + " " + changedby.LastName;
                    } else {
                        log.ChangedBy = "null";
                    }
                    db.SaveChanges();
                    WriteToChangeLog(log.toString());
                    return true;
                } catch (Exception exc) {
                    string error = "Exception: " + exc.ToString() + " catched at DeleteCustomer()";
                    writeToErrorLog(error);
                    return false;
                }
            }
        }

        public string AddCustomer(CustomerInfo customerInfo) {
            PostalNumbers p;
            using (var db = new DbModel()) {
                try {
                    if (db.PostalNumbers.Find(customerInfo.PostalNumber) == null)
                    {
                        p = new PostalNumbers();
                        p.PostalNumber = customerInfo.PostalNumber;
                        p.PostalCity = customerInfo.PostalCity;
                        db.PostalNumbers.Add(p);
                    } else {
                        p = db.PostalNumbers.Find(customerInfo.PostalNumber);
                    }
                    string salt = CreateSalt(32);
                    var newCustomer = new Customers() {
                        PersonalNumber = customerInfo.PersonalNumber,
                        FirstName = customerInfo.FirstName,
                        LastName = customerInfo.LastName,
                        Address = customerInfo.Address,
                        Salt = salt,  
                        Password = HashPassword(customerInfo.Password, salt),
                        PostalNumbers = p,
                        PostalNumber =  p.PostalNumber
                    };

                    if (customerInfo.FirstName == null || customerInfo.LastName == null) {
                        return "Fornavn og etternavn må skrives inn.";
                    }

                    db.Customers.Add(newCustomer);
                    //write to changelog
                    var log = new ChangeLog();
                    log.ChangedTime = (DateTime.Now).ToString("yyyyMMddHHmmss");
                    log.EventType = "Create";
                    log.OriginalValue = newCustomer.ToString();
                    log.NewValue = "null";
                    var context = HttpContext.Current;
                    if (context.Session["CurrentAdmin"] != null)
                    {
                        Admins changedby = (Admins)context.Session["CurrentAdmin"];
                        log.ChangedBy = changedby.FirstName + " " + changedby.LastName;
                    }
                    else
                    {
                        log.ChangedBy = "null";
                    }
                    db.SaveChanges();
                    WriteToChangeLog(log.toString());
                    return "";
                }
                catch (Exception exc)
                {
                    string error = "Exception: " + exc.ToString() + " catched at AddCustomer()";
                    writeToErrorLog(error);
                    return "Feil: " + exc.Message;
                }
            }
        }

        public bool ChangePassword(FormCollection inList)
        {
            string salt = CreateSalt(32);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(inList["newPassword"] + salt);
            SHA256Managed SHA256String = new SHA256Managed();
            byte[] newPassword = SHA256String.ComputeHash(bytes);
            
            using (var db = new DbModel())
            {
                try
                {
                    var customers = db.Customers.Find(inList["personalnumber"]);
                    if(customers != null)
                    { 
                    var password = Convert.ToBase64String(customers.Password);
                    var rehash = CreateHash(inList["oldPassword"], customers.Salt);
                    if (password.Equals(rehash))
                    {
                        customers.Salt = salt;
                        customers.Password = newPassword;
                    }

                    db.Entry(customers).State = EntityState.Modified;
                        var log = new ChangeLog();
                        log.ChangedTime = (DateTime.Now).ToString("yyyyMMddHHmmss");
                        log.EventType = "Update";
                        log.OriginalValue = password;
                        log.NewValue = inList["newPassword"];
                        var context = HttpContext.Current;
                        if (context.Session["CurrentAdmin"] != null)
                        {
                            Admins changedby = (Admins)context.Session["CurrentAdmin"];
                            log.ChangedBy = changedby.FirstName + " " + changedby.LastName;
                        }
                        else
                        {
                            log.ChangedBy = "null";
                        }
                        db.SaveChanges();
                        WriteToChangeLog(log.toString());
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception exception)
                {
                    string error = "Exception: " + exception.ToString() + " catched at ChangePassword()";
                    writeToErrorLog(error);
                    return false;
                }
            }
        }

        public void WriteToChangeLog(string log)
        {
            string path = "ChangeLog.txt";
            var _Path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/"), path);
            if (!File.Exists(_Path))
            {
                string createText = log + Environment.NewLine;
                File.WriteAllText(_Path, createText);
            }
            else
            {
                string appendText = log + Environment.NewLine;
                File.AppendAllText(_Path, appendText);
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