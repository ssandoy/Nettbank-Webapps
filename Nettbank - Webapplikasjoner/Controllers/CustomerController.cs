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
        public ActionResult ListAccounts() 
        {
            ViewBag.loggedIn = TempData["login"];
            if (Session["loggedin"] != null)
            {
                bool loggetInn = (bool) Session["loggedin"];
                if (loggetInn)
                {
                    TempData["login"] = true; //TODO: BENNYTTE SESSION-VERDI I STEDET?
                    string personnr = "12345678902";
                    var db = new AccessDb();
                    ViewBag.Customer = (Customers)Session["CurrentUser"];
                    List<Account> allAccounts = db.listAccounts(personnr);
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
           var db = new AccessDb();
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
            var db = new AccessDb();
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
            var db = new AccessDb();
            bool loggedOut = db.Logout(); //TODO MAKE VOID
                return RedirectToAction("Login");
        }
    }   
}
