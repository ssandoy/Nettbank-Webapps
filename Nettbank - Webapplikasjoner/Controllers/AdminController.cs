﻿using System;
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
        private ITransactionLogic _transactionBLL;

        public AdminController()
        {
            _adminBLL = new AdminLogic();
            _customerBLL = new CustomerLogic();
            _accountBLL = new AccountLogic();
            _transactionBLL = new TransactionLogic();
        }

        public AdminController(IAdminLogic adminstub, ICustomerLogic customerstub, 
            IAccountLogic accountstub, ITransactionLogic transactionstub) 
        {
            _adminBLL = adminstub;
            _customerBLL = customerstub;
            _accountBLL = accountstub;
            _transactionBLL = transactionstub;
        }

        public ActionResult ListCustomers() {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
            }
            TempData["login"] = true;
            List<CustomerInfo> allCustomers = _customerBLL.ListCustomers();
            return View(allCustomers);
        }

        public ActionResult Login() {

         //   _adminBLL.InsertAdmin(); TODO: Fjern?
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
            }
            else 
            {
                TempData["login"] = false;
                TempData["failure"] = "Feil passord eller ansattnummer";
                return RedirectToAction("Login");
            }
        }

        public ActionResult UpdateCustomer(string personalNumber) {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
            }

            // Hindrer at man skrive URL rett inn i nettleseren uten å gi et personnummber
            if (personalNumber == null) {
                return RedirectToAction("ListCustomers");
            }

            var oldCustomer = _customerBLL.GetCustomerInfo(personalNumber);
            var newCustomer = new CustomerInfo() {
                PersonalNumber = oldCustomer.PersonalNumber,
                FirstName = oldCustomer.FirstName,
                LastName = oldCustomer.LastName,
                Address = oldCustomer.Address,
                PostalNumber = oldCustomer.PostalNumber,
                PostalCity = oldCustomer.PostalCity,
                Password =  oldCustomer.Password
            };

            return View(newCustomer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCustomer(CustomerInfo customer)
        {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
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
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCustomer(CustomerInfo newCustomer)
        {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
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



        public JsonResult Delete(string personalNumber) {
            // Sjekker om admin er logget inn.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
            var ok = _customerBLL.DeleteCustomer(personalNumber);
            return Json(ok ? new { result = true } : new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListAccounts(string personalNumber) { 
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
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
            return View();
        }

        public ActionResult ListAccountsPartial(string personalNumber) {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
            }
            List<Account> accounts = _accountBLL.ListAccounts(personalNumber);
            return View(accounts);
        }

        public ActionResult UpdateAccount(string accountNumber) {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
            }

            // Hindrer at man skrive URL rett inn i nettleseren uten å gi et kontonummer
            if (accountNumber == null) {
                return RedirectToAction("ListAccounts");
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
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
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

        public JsonResult DeleteAccount(string id) {
            // Sjekker om admin er logget inn.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
            var ok = _accountBLL.DeleteAccount(id);
            return Json(ok ? new {result = true} : new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddAccount(string personalNumber) {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
            }
            if (_accountBLL.AddAccount(personalNumber)) {
                return RedirectToAction("ListAccounts", new { personalNumber = personalNumber });
            }
            return RedirectToAction("ListCustomers");
        }

        public ActionResult ListTransactions() {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
            }

            List<Transaction> executeableTransactions = _transactionBLL.ListExecuteableTransactions();
            return View(executeableTransactions);
        }

        public ActionResult ExecuteTransaction(int transactionId) {
            // Sjekker om admin er logget inn, og hvis ikke sender admin til forsiden.
            if (Session["adminloggedin"] == null || !(bool)Session["adminloggedin"]) {
                return RedirectToAction("Login");
            }

            _transactionBLL.ExecuteTransaction(transactionId);
            return RedirectToAction("ListTransactions");
        }
    }
}
