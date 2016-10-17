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

        public ActionResult RegisterTransaction(Transaction newTransaction) {
            return View();
        }
    }
}