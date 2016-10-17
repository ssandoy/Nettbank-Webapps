using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Nettbank___Webapplikasjoner.Models;

namespace Nettbank___Webapplikasjoner.Controllers
{
    public class CustomerController : Controller
    {
        public ActionResult ListAccounts()
        {
            Session["personnummer"] = "12345678902";
            string personnr = (string)Session["personnummer"];
            var db = new AccessDb();
            List<Account> allAccounts = db.listAccounts(personnr);
            return View(allAccounts);
        }

    }
}
