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
            var transactions = db.listTransactions(accountNumber);
            return View(transactions);
        }

        /*public ActionResult Details() {

        }

        public ActionResult Edit() {

        }

        public ActionResult Delete() {

        }*/

        public ActionResult RegisterTransaction() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterTransaction(Transaction newTransaction) {
            if (ModelState.IsValid) {
                var db = new AccessDb();
                if (db.addTransaction(newTransaction)) {
                    return RedirectToAction("ListTransaction");
                }
            }
            return View(newTransaction);
        }
    }
}