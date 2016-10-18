using Nettbank___Webapplikasjoner.Models;
using System;
using System.Collections.Generic;
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

        /*public ActionResult Edit() {

        }

        public ActionResult Delete() {

        }*/

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
    }
}