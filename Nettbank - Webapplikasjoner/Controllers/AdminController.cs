using System;
using System.Collections.Generic;
using System.Diagnostics;
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
           // aL.InsertAdmin();
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

        public ActionResult UpdateCustomer(string personalNumber) {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
                return RedirectToAction("Login");
            }

            var cL = new CustomerLogic();
            var oldCustomer = cL.GetCustomerInfo(personalNumber);
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


        public void Delete(string personalNumber) {
            var cL = new CustomerLogic();
            var deleteOK = cL.DeleteCustomer(personalNumber);
        }

        public ActionResult ListAccounts(string personalNumber) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            //if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
            //    return RedirectToAction("Login", "Admin");
            //} TODO: Legg til logginn-sjekk når det fungerer

            // Fyller dropdown listen med kunder via en ViewBag.
            var cL = new CustomerLogic();
            List<CustomerInfo> customers = cL.ListCustomers();
            var list = customers.Select(c => new SelectListItem {
                Text = c.FirstName + " " + c.LastName +
                       " (" + long.Parse(c.PersonalNumber).ToString("000000 00000") + ")",
                Value = c.PersonalNumber,
                Selected = (c.PersonalNumber == personalNumber)
            });

            ViewBag.CustomerList = list;

            if (personalNumber == null) {
                personalNumber = list.First(c => c.Value != null).Value;
            }

            return View();
        }

        public ActionResult ListAccountsPartial(string personalNumber) {
            var aL = new AccountLogic();
            List<Account> accounts = aL.ListAccounts(personalNumber);
            return View(accounts);
        }

        public ActionResult UpdateAccount(string accountNumber) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            //if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
            //    return RedirectToAction("Login", "Admin");
            //} TODO: Legg til logginn-sjekk når det fungerer

            var aL = new AccountLogic();
            var account = aL.GetUpdateableAccount(accountNumber);

            var cL = new CustomerLogic();
            List<CustomerInfo> customers = cL.ListCustomers();
            var list = customers.Select(
                        c => new SelectListItem {
                            Text = long.Parse(c.PersonalNumber).ToString("000000 00000") + " (" + c.FirstName + " " + c.LastName + ")",
                            Value = c.PersonalNumber,
                            Selected = account.OwnerPersonalNumber == c.PersonalNumber
                        }).ToList();

            ViewBag.CustomerList = list;
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAccount(EditableAccount updatedAccount) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            //if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
            //    return RedirectToAction("Login", "Admin");
            //} TODO: Legg til logginn-sjekk når det fungerer

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid) {
                var aL = new AccountLogic();
                validationMessage = aL.UpdateAccount(updatedAccount);
                if (validationMessage == "") {
                    return RedirectToAction("ListAccounts");
                }
            }

            var cL = new CustomerLogic();
            List<CustomerInfo> customers = cL.ListCustomers();
            var list = customers.Select(
                        c => new SelectListItem {
                            Text = long.Parse(c.PersonalNumber).ToString("000000 00000") + " (" + c.FirstName + " " + c.LastName + ")",
                            Value = c.PersonalNumber,
                            Selected = updatedAccount.OwnerPersonalNumber == c.PersonalNumber
                        }).ToList();

            ViewBag.CustomerList = list;
            ViewBag.ValidationMessage = validationMessage;

            return View(updatedAccount);
        }

        public void DeleteAccount(string accountNumber) {
            var aL = new AccountLogic();
            var deleteOK = aL.DeleteAccount(accountNumber);
        }

        public ActionResult AddAccount(string personalNumber) {
            var aL = new AccountLogic();
            if (aL.AddAccount(personalNumber)) {
                return RedirectToAction("ListAccounts", new { personalNumber = personalNumber });
            }
            return RedirectToAction("ListCustomers");
        }
    }
}
