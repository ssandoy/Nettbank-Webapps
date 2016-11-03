﻿using System;
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
            var cL = new CustomerLogic();
            List<CustomerInfo> allCustomers = cL.ListCustomers();
            return View(allCustomers);
        }



        public ActionResult Login() {

            var aL = new AdminLogic();
            bool loggedIn = aL.Login();
            if (loggedIn) {
                TempData["login"] = true;
                return RedirectToAction("ListCustomers");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ValidateAdmin(FormCollection inList) {
            var aL = new AdminLogic();
            bool loggedIn = aL.ValidateAdmin(inList);
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

            var cL = new CustomerLogic();
            var oldCustomer = cL.GetCustomerInfo(personnumber);
            var newCustomer = new CustomerInfo() {
                PersonalNumber = oldCustomer.PersonalNumber,
                FirstName = oldCustomer.FirstName,
                LastName = oldCustomer.LastName,
                Address = oldCustomer.Address
                //TODO: ADD POSTALNUMBER ETC.  SENDE MED PERSONNUMMER I VIEWBAG SLIK AT DET OGSÅ KAN ENDRES?
            };

            return View(newCustomer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCustomer(CustomerInfo customer) //TODO: FIXME
        {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
                return RedirectToAction("Login", "Admin");
            }

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid) {
                var cL = new CustomerLogic();
                validationMessage = cL.UpdateCustomer(customer);
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