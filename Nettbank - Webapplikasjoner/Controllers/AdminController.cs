using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Model;

namespace Nettbank.Controllers {
    public class AdminController : Controller {
        public ActionResult ListCustomers() {
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) //TODO: ENDRE TIL session["adminLoggedIn"]
            {
                return RedirectToAction("Login", "Admin");
            }
            TempData["login"] = true;
            var db = new CustomerDB();
            List<CustomerAdmin> allCustomers = db.ListCustomers();
            return View(allCustomers);
        }



        public ActionResult Login() {

            var db = new AdminDB();
            bool loggedIn = db.Login();
            if (loggedIn) {
                TempData["login"] = true;
                return RedirectToAction("ListCustomers");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ValidateAdmin(FormCollection inList) {
            var db = new AdminDB();
            bool loggedIn = db.ValidateAdmin(inList);
            if (loggedIn) {
                HttpContext context = System.Web.HttpContext.Current;
                context.Session["loggedin"] = true;
                TempData["login"] = true;
                return RedirectToAction("ListCustomers");
            } else {
                TempData["login"] = false;
                return RedirectToAction("Login");
            }
        }

        public ActionResult UpdateCustomer(string personnumber) {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
                return RedirectToAction("Login");
            }

            var cDb = new CustomerLogic();
            var oldCustomer = cDb.FindByPersonNr(personnumber);
            var newCustomer = new CustomerAdmin() {
                PersonalNumber = oldCustomer.personalNumber,
                FirstName = oldCustomer.firstName,
                LastName = oldCustomer.lastName,
                //TODO: ADD POSTALNUMBER ETC.  SENDE MED PERSONNUMMER I VIEWBAG SLIK AT DET OGSÅ KAN ENDRES?
            };

            return View(newCustomer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCustomer(CustomerAdmin customer) //TODO: FIXME
        {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
                return RedirectToAction("Login", "Admin");
            }

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid) {
                var cDb = new CustomerDB();
                validationMessage = cDb.updateCustomer(customer);
                if (validationMessage == "") {
                    return RedirectToAction("ListCustomers");
                }
            }

            ViewBag.ValidationMessage = validationMessage;

            return View(customer);
        }


        public ActionResult Delete() {
            throw new NotImplementedException();
        }
    }
}