using Nettbank___Webapplikasjoner.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nettbank___Webapplikasjoner.Controllers
{
    public class TransactionController : Controller
    {
        public ActionResult ListTransactions(string accountNumber) {
            if (Session["loggedin"] != null) {
                bool loggetInn = (bool) Session["loggedin"];
                if (loggetInn) {
                    TempData["login"] = true;
                    using (var dbm = new DbModel()) {
                        Customers c = (Customers) Session["CurrentUser"];
                        string personalNumber = c.personalNumber;
                        List<Accounts> accounts = dbm.accounts.Where(a => a.personalNumber == personalNumber).ToList();
                        List<SelectListItem> output = new List<SelectListItem>();
                        foreach (var acc in accounts) {
                            if (acc.accountNumber == accountNumber) {
                                output.Add(new SelectListItem { Text = Int64.Parse(acc.accountNumber).ToString("0000 00 00000") + " (" + acc.balance + " kr)", Value = acc.accountNumber, Selected = true });
                            } else {
                                output.Add(new SelectListItem { Text = Int64.Parse(acc.accountNumber).ToString("0000 00 00000") + " (" + acc.balance + " kr)", Value = acc.accountNumber});
                            }
                        }
                        ViewBag.AccountList = output;
                        if (accountNumber == null) {
                            accountNumber = output[0].Value;
                        }
                    }
                    var db = new AccessDb();
                    var transactions = db.listTransactions(accountNumber);
                    return View(transactions);
                }
            }
            return RedirectToAction("Login", "Customer");
        }

        public ActionResult ShowStatement(string accountNumber) {
            if (Session["loggedin"] != null) {
                bool loggetInn = (bool)Session["loggedin"];
                if (loggetInn) {
                    TempData["login"] = true;
                    using (var dbm = new DbModel()) {
                        Customers c = (Customers)Session["CurrentUser"];
                        string personalNumber = c.personalNumber;
                        List<Accounts> accounts = dbm.accounts.Where(a => a.personalNumber == personalNumber).ToList();
                        List<SelectListItem> output = new List<SelectListItem>();
                        foreach (var acc in accounts) {
                            if (acc.accountNumber == accountNumber) {
                                output.Add(new SelectListItem { Text = Int64.Parse(acc.accountNumber).ToString("0000 00 00000") + " (" + acc.balance + " kr)", Value = acc.accountNumber, Selected = true });
                            } else {
                                output.Add(new SelectListItem { Text = Int64.Parse(acc.accountNumber).ToString("0000 00 00000") + " (" + acc.balance + " kr)", Value = acc.accountNumber });
                            }
                        }
                        ViewBag.AccountList = output;
                        if (accountNumber == null) {
                            accountNumber = output[0].Value;
                        }
                    }
                    var db = new AccessDb();
                    var transactions = db.listExecutedTransactions(accountNumber);
                    return View(transactions);
                }
            }
            return RedirectToAction("Login", "Customer");
        }

        public ActionResult RegisterTransaction() {
            if (Session["loggedin"] != null)
            {
                bool loggetInn = (bool) Session["loggedin"];
                if (loggetInn)
                {
                    TempData["login"] = true;
                    using (var db = new DbModel())
                    {
                        Customers c = (Customers)Session["CurrentUser"];
                        string personalNumber = c.personalNumber;
                        List<Accounts> accounts = db.accounts.Where(a => a.personalNumber == personalNumber).ToList();
                        List<SelectListItem> output = new List<SelectListItem>();
                        foreach (var acc in accounts)
                        {
                            output.Add(new SelectListItem {Text = acc.accountNumber, Value = acc.accountNumber});
                        }
                        ViewBag.AccountList = output;
                    }
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterTransaction(Transaction newTransaction) {
            if (ModelState.IsValid) {
                var db = new AccessDb();
                if (db.addTransaction(newTransaction)) {
                    return RedirectToAction("ListTransactions", new { accountNumber=newTransaction.fromAccountNumber}); 
                }
            }
            return View(newTransaction);
        }

        
        public ActionResult UpdateTransaction(int id)
        {
            var db = new AccessDb();
            Transactions transactionDb = db.findTransanction(id);
            Transaction transaction = new Transaction()
            {
                transactionId = transactionDb.transactionID,
                amount = transactionDb.amount,
                fromAccountNumber = transactionDb.accountNumber,
                toAccountNumber = transactionDb.toAccountNumber,
                timeToBeTransfered = transactionDb.timeToBeTransfered,
                timeTransfered = transactionDb.timeTransfered,
                comment = transactionDb.comment
            };
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var db = new AccessDb();
                if(db.updateTransaction(transaction))
                    return RedirectToAction("ListTransactions", new {accountNumber=transaction.fromAccountNumber});
            }
            return View(transaction);
        }

        public void Delete(int id)
        {
            var db = new AccessDb();
            bool deleteOK = db.deleteTransaction(id);
            //TODO: FIX CHECK OM SUCCESSFUL
        }
    }
}