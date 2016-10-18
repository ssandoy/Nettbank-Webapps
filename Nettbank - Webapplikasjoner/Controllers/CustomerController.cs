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
            string personnr = "12345678902";
            var db = new AccessDb();
            List<Account> allAccounts = db.listAccounts(personnr);
            return View(allAccounts);
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ValidateUser(FormCollection inList)
        {
            
            var db = new AccessDb();
            bool loggedIn = db.ValidateCustomer(inList);
            if (loggedIn)
            {
                //TODO: IMPLEMENT VIEWBAG.loggedIN  
                return RedirectToAction("ListAccounts");
            }
            return RedirectToAction("Login");
        }
    }   
}
