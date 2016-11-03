﻿using System.Linq;
using System.Web.Mvc;
using BLL;
using Model;

namespace Nettbank.Controllers {
    public class TransactionController : Controller {
        public ActionResult ListTransactions(string accountNumber) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool) Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
            }

            TempData["login"] = true;

            // Fyller dropdown listen med kontoer via en ViewBag.
            var personalNumber = (string) Session["CurrentUser"];
            var aL = new AccountLogic();
            var accounts = aL.ListAccounts(personalNumber);
            var list = accounts.Select(acc => new SelectListItem {
                Text = long.Parse(acc.AccountNumber).ToString("0000 00 00000") +
                       " (" + acc.Balance + " kr)",
                Value = acc.AccountNumber,
                Selected = (acc.AccountNumber == accountNumber)
            });

            ViewBag.AccountList = list;

            if (accountNumber == null) {
                accountNumber = list.First(acc => acc.Value != null).Value;
            }

            return View();
        }

        public ActionResult ListPartial(string accountNumber) {
            var tL = new TransactionLogic();
            var transactions = tL.ListTransactions(accountNumber);
            return View(transactions);
        }

        public ActionResult ShowStatement(string accountNumber) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool) Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
            }

            var personalNumber = (string) Session["CurrentUser"];
            var aL = new AccountLogic();
            var accounts = aL.ListAccounts(personalNumber);
            var list = accounts.Select(acc => new SelectListItem {
                Text = long.Parse(acc.AccountNumber).ToString("0000 00 00000") +
                       " (" + acc.Balance + " kr)",
                Value = acc.AccountNumber,
                Selected = (acc.AccountNumber == accountNumber)
            });

            ViewBag.AccountList = list;

            if (accountNumber == null) {
                accountNumber = list.First(acc => acc.Value != null).Value;
            }

            return View();
        }

        public ActionResult StatementPartial(string accountNumber) {
            ViewBag.AccountNumber = accountNumber;
            var tL = new TransactionLogic();
            var transactions = tL.ListExecutedTransactions(accountNumber);
            return View(transactions);
        }

        public ActionResult RegisterTransaction() {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool) Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
            }

            var cL = new CustomerLogic();
            var customerInfo = cL.GetCustomerInfo((string) Session["CurrentUser"]);
            var personalNumber = customerInfo.PersonalNumber;
            var aL = new AccountLogic();
            var accounts = aL.ListAccounts(personalNumber);
            var list =
                accounts.Select(acc => new SelectListItem {Text = acc.AccountNumber, Value = acc.AccountNumber})
                    .ToList();

            ViewBag.CustomerInfo = customerInfo;
            ViewBag.AccountList = list;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterTransaction(Transaction newTransaction) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool) Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
            }

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid) {
                var tL = new TransactionLogic();
                validationMessage = tL.AddTransaction(newTransaction);
                if (validationMessage == "") {
                    return RedirectToAction("ListTransactions", new {accountNumber = newTransaction.FromAccountNumber});
                }
            }

            // Implisitt else. Hvis registreringen feilet, lastes siden inn på nytt.
            var cL = new CustomerLogic();
            var customerInfo = cL.GetCustomerInfo((string)Session["CurrentUser"]);
            var personalNumber = customerInfo.PersonalNumber;
            var aL = new AccountLogic();
            var accounts = aL.ListAccounts(personalNumber);
            var list =
                accounts.Select(acc => new SelectListItem {Text = acc.AccountNumber, Value = acc.AccountNumber})
                    .ToList();

            ViewBag.CustomerInfo = customerInfo;
            ViewBag.AccountList = list;
            ViewBag.ValidationMessage = validationMessage;

            return View(newTransaction);
        }

        /*public ActionResult UpdateTransaction(int id) { TODO: Metoden bruker Transactions. Det er ikke lov. Må bruke Transaction.
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool) Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
            }

            var tL = new TransactionLogic();
            var oldTransaction = tL.FindTransanction(id);
            var newTransaction = new Transaction() {
                TransactionId = oldTransaction.TransactionID,
                Amount = oldTransaction.Amount,
                FromAccountNumber = oldTransaction.AccountNumber,
                ToAccountNumber = oldTransaction.ToAccountNumber,
                TimeToBeTransfered = oldTransaction.TimeToBeTransfered,
                Comment = oldTransaction.Comment
            };

            return View(newTransaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTransaction(Transaction transaction) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool) Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
            }

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid) {
                var tDb = new TransactionDB();
                validationMessage = tDb.updateTransaction(transaction);
                if (validationMessage == "") {
                    return RedirectToAction("ListTransactions", new {accountNumber = transaction.FromAccountNumber});
                }
            }

            ViewBag.ValidationMessage = validationMessage;

            return View(transaction);
        }*/

        public void Delete(int id) {
            var tL = new TransactionLogic();
            var deleteOK = tL.DeleteTransaction(id);
        }
    }
}