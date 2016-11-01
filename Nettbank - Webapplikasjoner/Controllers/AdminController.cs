using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nettbank___Webapplikasjoner.Models;

namespace Nettbank___Webapplikasjoner.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult ListCustomers()
        {
            if (Session["loggedin"] == null || !(bool)Session["loggedin"])
            {
                return RedirectToAction("Login", "Admin");
            }
            TempData["login"] = true;
            var db = new CustomerDB();
            List<CustomerAdmin> allCustomers = db.ListCustomers();
            return View(allCustomers);
        }



        public ActionResult Login()
        {

            var db = new AdminDB();
            bool loggedIn = db.Login();
            if (loggedIn)
            {
                TempData["login"] = true;
                return RedirectToAction("ListCustomers"); 
            }
            return View();
        }

        [HttpPost]
        public ActionResult ValidateAdmin(FormCollection inList)
        {
            var db = new AdminDB();
            bool loggedIn = db.ValidateAdmin(inList);
            if (loggedIn)
            {
                HttpContext context = System.Web.HttpContext.Current;
                context.Session["loggedin"] = true;
                TempData["login"] = true;
                return RedirectToAction("ListCustomers");
            }
            else
            {
                TempData["login"] = false;
                return RedirectToAction("Login");
            }
        }

    }
}