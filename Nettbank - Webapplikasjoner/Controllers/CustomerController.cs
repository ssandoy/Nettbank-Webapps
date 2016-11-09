using System.Web.Mvc;
using BLL;
using Model;

namespace Nettbank.Controllers {
    public class CustomerController : Controller {
        public ActionResult ListAccounts() {
            if (Session["loggedin"] == null || !(bool) Session["loggedin"]) {
                return RedirectToAction("Login", "Customer");
            }

            TempData["login"] = true;
            var aL = new AccountLogic();
            // var tL = new TransactionLogic();
            // tL.insertDoneTransaction();
            var cL = new CustomerLogic();
            var customerInfo = cL.GetCustomerInfo((string) Session["CurrentUser"]);
            ViewBag.CustomerInfo = customerInfo;
            var allAccounts = aL.ListAccounts(customerInfo.PersonalNumber);
            return View(allAccounts);
        }


        public ActionResult Login() {
            var cL = new CustomerLogic();
            //temp TODO: Fjern
            // cL.insertCustomer();
            bool loggedIn;
            if (Session["loggedin"] == null)
            {
                Session["loggedin"] = false;
            }
            loggedIn = (bool)Session["loggedin"];

            if (loggedIn)
            {
                TempData["login"] = true;
                return RedirectToAction("ListAccounts");
            }
            TempData["ID"] = BankIdLogic.GetBankId();
            ViewBag.bankID = TempData["ID"];
            return View();
        }


        [HttpPost]
        public ActionResult ValidateUser(FormCollection inList) {
            var cL = new CustomerLogic();
            var loggedIn = cL.ValidateCustomer(inList);
            if (loggedIn && inList["BankID"] == (inList["hiddenBankID"])) {
                var context = System.Web.HttpContext.Current;
                context.Session["loggedin"] = true;
                TempData["login"] = true;
                return RedirectToAction("ListAccounts");
            } else if (!loggedIn)
            {
                TempData["login"] = false;
                TempData["failure"] = "Feil passord eller personnummer";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["login"] = false;
                TempData["failure"] = "Feil BankID";
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout() {
            Session["loggedin"] = false;
            Session["CurrentUser"] = null;
            return RedirectToAction("Login");
        }

        public ActionResult ChangePassword()
        {
            if (Session["loggedin"] == null || !(bool)Session["loggedin"])
            {
                return RedirectToAction("Login", "Customer");
            }

            var _customerBLL = new CustomerLogic();
            var customerInfo = _customerBLL.GetCustomerInfo((string) Session["CurrentUser"]);

            var Customer = _customerBLL.GetCustomerInfo(customerInfo.PersonalNumber);

            return View(Customer);

        }

        [HttpPost]
        public ActionResult ChangePassword(FormCollection inList)
        {
            if (Session["loggedin"] == null || !(bool)Session["loggedin"])
            {
                return RedirectToAction("Login", "Customer");
            }

            var _customerBLL = new CustomerLogic();

            var change = _customerBLL.ChangePassword(inList);

            if (change)
            {
                return RedirectToAction("ListAccounts", "Customer");
            }
            else
            {
                var customer = _customerBLL.GetCustomerInfo(inList["personalnumber"]);
                return View(customer);
            }

        }
    }
}