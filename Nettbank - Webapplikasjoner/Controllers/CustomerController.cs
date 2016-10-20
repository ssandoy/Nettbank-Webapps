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
        private string bankID;
        public ActionResult ListAccounts() //TODO: FIX SO IT SENDS PERSONNUMBER
        {
            ViewBag.loggedIn = TempData["login"];
            if (Session["loggedin"] != null)
            {
                bool loggetInn = (bool) Session["loggedin"];
                if (loggetInn)
                {
                    TempData["login"] = true;
                    var db = new AccountDB();
                    Customers c = (Customers)Session["CurrentUser"];
                    ViewBag.Customer = c;
                    List<Account> allAccounts = db.listAccounts(c.personalNumber);
                    return View(allAccounts);
                }
                else
                {
                   return RedirectToAction("Login");
                }
            }
               return RedirectToAction("Login");
            
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
            return View(); //Implisitt else
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
            db.Logout(); //TODO MAKE VOID
                return RedirectToAction("Login");
        }
    }   
}
