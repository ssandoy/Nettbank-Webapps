using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Nettbank___Webapplikasjoner.Models;

namespace Nettbank___Webapplikasjoner.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult ListAccounts() 
        {
            if (Session["loggedin"] == null || !(bool)Session["loggedin"])
            {
                return RedirectToAction("Login", "Customer");
            }
           
                    TempData["login"] = true;
                    var db = new AccountDB();
            var tdb = new TransactionDB();
           // tdb.insertDoneTransaction();
                    Customers c = (Customers)Session["CurrentUser"];
                    ViewBag.Customer = c;
                    List<Account> allAccounts = db.listAccounts(c.personalNumber);
                    return View(allAccounts);
                
        }
        

        public ActionResult Login() 
        {
           var db = new CustomerDB();
            bool loggedIn = db.Login();
            if (loggedIn)
            {
                TempData["login"] = true;
                return RedirectToAction("ListAccounts");
            }
             TempData["ID"] = BankIDGenerator.getBankID();
            ViewBag.bankID = TempData["ID"];
            return View();
        }


        [HttpPost]
        public ActionResult ValidateUser(FormCollection inList)
        {
            var db = new CustomerDB();
            bool loggedIn = db.ValidateCustomer(inList);
            if (loggedIn && inList["BankID"] == (inList["hiddenBankID"]))
            {
                HttpContext context = System.Web.HttpContext.Current;
                context.Session["loggedin"] = true;
                TempData["login"] = true;
                return RedirectToAction("ListAccounts");
            }
            else
            {
                TempData["login"] = false;
                return RedirectToAction("Login");
            }    
        }

        public ActionResult Logout()
        {
           var db = new CustomerDB();
           db.Logout(); 
           return RedirectToAction("Login");
        }
    }   
}
