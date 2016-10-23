using Nettbank___Webapplikasjoner.Models;
using System.Linq;
using System.Web.Mvc;

namespace Nettbank___Webapplikasjoner.Controllers
{
    public class TransactionController : Controller {
        public ActionResult ListTransactions(string accountNumber) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
                            }

            TempData["login"] = true;

            // Fyller dropdown listen med kontoer via en ViewBag.
            var personalNumber = ((Customers)Session["CurrentUser"]).personalNumber;
            var aDb = new AccountDB();
            var accounts = aDb.listAccounts(personalNumber);
            var list = accounts.Select(acc => new SelectListItem {Text = long.Parse(acc.accountNumber).ToString("0000 00 00000") +
                    " (" + acc.balance + " kr)", Value = acc.accountNumber, Selected = (acc.accountNumber == accountNumber)});
            
            ViewBag.AccountList = list;

                        if (accountNumber == null) {
                accountNumber = list.First(acc => acc.Value != null).Value;
                        }
                    
            return View();
            
        }

        public ActionResult ListPartial(string accountNumber) {
            var tDb = new TransactionDB();
            var transactions = tDb.listTransactions(accountNumber);
                    return View(transactions);
                }

        public ActionResult ShowStatement(string accountNumber) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
                        }

            var personalNumber = ((Customers)Session["CurrentUser"]).personalNumber;
            var aDb = new AccountDB();
            var accounts = aDb.listAccounts(personalNumber);
            var list = accounts.Select(acc => new SelectListItem {Text = long.Parse(acc.accountNumber).ToString("0000 00 00000") +
                    " (" + acc.balance + " kr)", Value = acc.accountNumber, Selected = (acc.accountNumber == accountNumber)});

            ViewBag.AccountList = list;

                     if (accountNumber == null) {
                accountNumber = list.First(acc => acc.Value != null).Value;
                     }
                    
            return View();
        }

        public ActionResult StatementPartial(string accountNumber) {
            ViewBag.AccountNumber = accountNumber;
            var tDb = new TransactionDB();
            var transactions = tDb.listExecutedTransactions(accountNumber);
                    return View(transactions);
                }

        public ActionResult RegisterTransaction() {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
            return RedirectToAction("Login", "Customer");
        }

            var customer = (Customers) Session["CurrentUser"];
            var personalNumber = customer.personalNumber;
            var aDb = new AccountDB();
            var accounts = aDb.listAccounts(personalNumber);
            var list = accounts.Select(acc => new SelectListItem {Text = acc.accountNumber, Value = acc.accountNumber}).ToList();
                    
            ViewBag.Customer = customer;
            ViewBag.AccountList = list;
                    
                    return View();
                }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterTransaction(Transaction newTransaction) { 
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
            }

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid) {
                var tDb = new TransactionDB();
                validationMessage = tDb.addTransaction(newTransaction);
                if (validationMessage == "") {
                    return RedirectToAction("ListTransactions", new {accountNumber = newTransaction.fromAccountNumber});
                        }
                    }
                 
            // Implisitt else. Hvis registreringen feilet, lastes siden inn på nytt.
            var customer = (Customers)Session["CurrentUser"];
            var personalNumber = customer.personalNumber;
            var aDb = new AccountDB();
            var accounts = aDb.listAccounts(personalNumber);
            var list = accounts.Select(acc => new SelectListItem {Text = acc.accountNumber, Value = acc.accountNumber}).ToList();

            ViewBag.Customer = customer;
            ViewBag.AccountList = list;
            ViewBag.ValidationMessage = validationMessage;
                    
                    return View(newTransaction);
                }

        public ActionResult UpdateTransaction(int id) { 
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
            return RedirectToAction("Login", "Customer");
        }

            var tDb = new TransactionDB();
            var oldTransaction = tDb.findTransanction(id);
            var newTransaction = new Transaction() {
                transactionId = oldTransaction.transactionID,
                amount = oldTransaction.amount,
                fromAccountNumber = oldTransaction.accountNumber,
                toAccountNumber = oldTransaction.toAccountNumber,
                timeToBeTransfered = oldTransaction.timeToBeTransfered,
                comment = oldTransaction.comment
                    };

            return View(newTransaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTransaction(Transaction transaction) {
            // Sjekker om brukeren er logget inn, og hvis ikke sender brukeren til forsiden.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
                    }

            // Meldingen som vises hvis ModelState ikke er gyldig.
            var validationMessage = "Du har skrevet inn ugyldige verdier.";

            if (ModelState.IsValid) {
                var tDb = new TransactionDB();
                validationMessage = tDb.updateTransaction(transaction);
                if (validationMessage == "") {
                    return RedirectToAction("ListTransactions", new {accountNumber = transaction.fromAccountNumber});
                }
            }

            ViewBag.ValidationMessage = validationMessage;

            return View(transaction);
        }

        public void Delete(int id)
        {
            var tDb = new TransactionDB();
            bool deleteOK = tDb.deleteTransaction(id);
        }
    }
}