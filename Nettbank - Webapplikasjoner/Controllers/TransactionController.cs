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
            var db = new AccessDb();
            var transactions = db.listTransactions("12345678901");
            return View(transactions);
        }

        public ActionResult RegisterTransaction() {
            var adb = new AccessDb();
            adb.insertCustomer();
            Session["PersonalNumber"] = "12345678902";

            using (var db = new DbModel()) {
                string personalNumber = (string)Session["PersonalNumber"];
                List<Accounts> accounts = db.accounts.Where(a => a.personalNumber == personalNumber).ToList();
                List<SelectListItem> output = new List<SelectListItem>();
                foreach (var acc in accounts) {
                    output.Add(new SelectListItem { Text = acc.accountNumber, Value = acc.accountNumber });
                }
                ViewBag.AccountList = output;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterTransaction(Transaction newTransaction) {
            if (ModelState.IsValid) {
                var db = new AccessDb();
                if (db.addTransaction(newTransaction)) {
                    return RedirectToAction("ListTransactions");
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
                    return RedirectToAction("ListTransactions");
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