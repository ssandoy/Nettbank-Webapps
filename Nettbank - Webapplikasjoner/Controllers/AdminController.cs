using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Model;

namespace Nettbank.Controllers {
    public class AdminController : Controller
    {

        private IAdminLogic _adminBLL;
        private ICustomerLogic _customerBLL;
        private IAccountLogic _accountBLL;

        public AdminController()
        {
            _adminBLL = new AdminLogic();
            _customerBLL = new CustomerLogic();
            _accountBLL = new AccountLogic();
        }

        public AdminController(IAdminLogic adminstub, ICustomerLogic customerstub, IAccountLogic accountstub) 
        {
            _adminBLL = adminstub;
            _customerBLL = customerstub;
            _accountBLL = accountstub;
        }

        public ActionResult ListCustomers() {
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"])
            {
                return RedirectToAction("Login", "Admin");
            }
            TempData["login"] = true;
            List<CustomerInfo> allCustomers = _customerBLL.ListCustomers();
            return View(allCustomers);
        }

        public ActionResult Login() {

           // aL.InsertAdmin();
            bool loggedIn;
            if (Session["adminloggedin"] == null)
            {
                Session["adminloggedin"] = false;
            }
                loggedIn = (bool) Session["adminloggedin"];
            
           
            if (loggedIn) {
                TempData["login"] = true;
                return RedirectToAction("ListCustomers");
            }
            return View();

        }

        public ActionResult Logout()
        {
            Session["adminloggedin"] = false;
            Session["CurrentAdmin"] = null;
            return RedirectToAction("Login");
        }


        [HttpPost]
        public ActionResult ValidateAdmin(FormCollection inList) {
            bool loggedIn = _adminBLL.ValidateAdmin(inList);
            if (loggedIn) {
               Session["adminloggedin"] = true;
                TempData["login"] = true;
                return RedirectToAction("ListCustomers");
            } else {
                TempData["login"] = false;
                return RedirectToAction("Login");
            }
        }

        public ActionResult UpdateCustomer(string personalNumber) { 
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
            }

            var oldCustomer = _customerBLL.GetCustomerInfo(personalNumber);
            var newCustomer = new CustomerInfo() {
                PersonalNumber = oldCustomer.PersonalNumber,
                FirstName = oldCustomer.FirstName,
                LastName = oldCustomer.LastName,
                Address = oldCustomer.Address,
                PostalNumber = oldCustomer.PostalNumber,
                PostalCity = oldCustomer.PostalCity
            };

            return View(newCustomer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCustomer(CustomerInfo customer) //TODO: FIXME
        {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login", "Admin");
            }

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid) {
                validationMessage = _customerBLL.UpdateCustomer(customer);
                if (validationMessage == "") {
                    return RedirectToAction("ListCustomers");
                }
            }

            ViewBag.ValidationMessage = validationMessage;

            return View(customer);
        }

        public ActionResult RegisterCustomer()
        {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"])
            {
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCustomer(CustomerInfo newCustomer)
        {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"])
            {
                return RedirectToAction("Login", "Customer");
            }

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid)
            {
                validationMessage = _customerBLL.AddCustomer(newCustomer);
                if (validationMessage == "")
                {
                    return RedirectToAction("ListCustomers");
                }
            }

            return View(newCustomer);
        }



        public void Delete(string personalNumber) {
            var deleteOK = _customerBLL.DeleteCustomer(personalNumber);
        }

        public ActionResult ListAccounts(string personalNumber) { //TODO: HOW TO TEST?
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login", "Admin");
            }

            // Fyller dropdown listen med kunder via en ViewBag.
            List<CustomerInfo> customers = _customerBLL.ListCustomers();
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
            List<Account> accounts = _accountBLL.ListAccounts(personalNumber);
            return View(accounts);
        }

        public ActionResult UpdateAccount(string accountNumber) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
               return RedirectToAction("Login", "Admin");
            } 
            
            var account = _accountBLL.GetUpdateableAccount(accountNumber);

            List<CustomerInfo> customers = _customerBLL.ListCustomers();
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
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login", "Admin");
            } 

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid) {
                validationMessage = _accountBLL.UpdateAccount(updatedAccount);
                if (validationMessage == "") {
                    return RedirectToAction("ListAccounts");
                }
            }

            List<CustomerInfo> customers = _customerBLL.ListCustomers();
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

        public void DeleteAccount(string accountNumber) { //TODO: MAKE BOOL?
            var deleteOK = _accountBLL.DeleteAccount(accountNumber);
        }

        public ActionResult AddAccount(string personalNumber) {
            if (_accountBLL.AddAccount(personalNumber)) {
                return RedirectToAction("ListAccounts", new { personalNumber = personalNumber });
            }
            return RedirectToAction("ListCustomers");
        }
    }
}
