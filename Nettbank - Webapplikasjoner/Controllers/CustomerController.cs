using System.Web.Mvc;
using BLL;

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
            //temp
            var loggedIn = cL.Login();
            if (loggedIn) {
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
            } else {
                TempData["login"] = false;
                return RedirectToAction("Login");
            }
        }

        public ActionResult Logout() {
            var cL = new CustomerLogic();
            cL.Logout();
            return RedirectToAction("Login");
        }
    }
}