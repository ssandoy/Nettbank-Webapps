﻿using Nettbank___Webapplikasjoner.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nettbank___Webapplikasjoner.Controllers
{
    public class TransactionController : Controller {
        public ActionResult ListTransactions(string accountNumber) {
            // Checks if the user isn't logged in, and if so redirects the user to the login page.
            if (Session["loggedin"] == null || !(bool)Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
            }

            TempData["login"] = true;

            // Populates the dropdown list with accounts through a ViewBag.
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
            // Checks if the user isn't logged in, and if so redirects the user to the login page.
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

        public ActionResult RegisterTransaction() { //TODO: ENDRE TIL Å BRUKE DB-KLASSER
            if (Session["loggedin"] != null)
            {
                bool loggetInn = (bool) Session["loggedin"];
                if (loggetInn)
                {
                    TempData["login"] = true;
                    
                        Customers c = (Customers)Session["CurrentUser"];
                        string personalNumber = c.personalNumber;
                        AccountDB adb = new AccountDB();
                        List<Account> accounts = adb.listAccounts(personalNumber);
                        List<SelectListItem> output = new List<SelectListItem>();
                        foreach (var acc in accounts)
                        {
                            output.Add(new SelectListItem {Text = acc.accountNumber, Value = acc.accountNumber});
                        }
                        ViewBag.AccountList = output;
                        ViewBag.Customer = c;
                    
                    return View();
                }
            }
            return RedirectToAction("Login", "Customer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterTransaction(Transaction newTransaction) { 
            if (Session["loggedin"] != null)
            {
                bool loggetInn = (bool) Session["loggedin"];
                if (loggetInn)
                {
                    if (ModelState.IsValid)
                    {
                        var db = new TransactionDB();
                        if (db.addTransaction(newTransaction))
                        {
                            return RedirectToAction("ListTransactions",
                                new {accountNumber = newTransaction.fromAccountNumber});
                        }
                    }
                 
                        AccountDB adb = new AccountDB();
                        Customers c = (Customers) Session["CurrentUser"];
                        string personalNumber = c.personalNumber;
                        List<Account> accounts = adb.listAccounts(personalNumber);
                        List<SelectListItem> output = new List<SelectListItem>();
                        foreach (var acc in accounts)
                        {
                            output.Add(new SelectListItem {Text = acc.accountNumber, Value = acc.accountNumber});
                        }
                        ViewBag.AccountList = output;
                        ViewBag.Customer = c;
                    
                    return View(newTransaction);
                }
            }
            return RedirectToAction("Login", "Customer");
        }

        
        public ActionResult UpdateTransaction(int id)  //TODO: FIX LOGIN-CHECK
        {
            if (Session["loggedin"] != null)
            {
                bool loggetInn = (bool) Session["loggedin"];
                if (loggetInn)
                {
                    var db = new TransactionDB();
                    Transactions transactionDb = db.findTransanction(id);
                    Transaction transaction = new Transaction()
                    {
                        transactionId = transactionDb.transactionID,
                        amount = transactionDb.amount,
                        fromAccountNumber = transactionDb.accountNumber,
                        toAccountNumber = transactionDb.toAccountNumber,
                        timeToBeTransfered = transactionDb.timeToBeTransfered,
                        comment = transactionDb.comment
                    };
                    return View(transaction);
                }
            }
            return RedirectToAction("Login", "Customer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTransaction(Transaction transaction)
        {
            if (Session["loggedin"] != null)
            {
                bool loggetInn = (bool) Session["loggedin"];
                if (loggetInn)
                {
                    if (ModelState.IsValid)
                    {
                        var db = new TransactionDB();
                        if (db.updateTransaction(transaction))
                            return RedirectToAction("ListTransactions",
                                new {accountNumber = transaction.fromAccountNumber});
                    }
                    return View(transaction);
                }
            }
            return RedirectToAction("Login", "Customer");
        }

        public void Delete(int id)
        {
            var db = new TransactionDB();
            bool deleteOK = db.deleteTransaction(id);
            //TODO: FIX CHECK OM SUCCESSFUL
        }
    }
}