using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BLL;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using MvcContrib.TestHelper;
using Nettbank.Controllers;

namespace Enhetstest
{
    [TestClass]
    public class AdminControllerTest
    {
        [TestMethod]
        public void thatListCustomersReturnsExpectedResults()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var expectedResult = new List<CustomerInfo>();
            var customer = new CustomerInfo()
            {
                PersonalNumber = "12345678901",
                FirstName = "Kjetil",
                LastName = "Olsen",
                Address = "Masterberggata 25"
            };
            expectedResult.Add(customer);
            expectedResult.Add(customer);
            expectedResult.Add(customer);


            var actionResult = (ViewResult) controller.ListCustomers();
            var result = (List<CustomerInfo>) actionResult.Model;

            Assert.AreEqual(actionResult.ViewName, "");

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].PersonalNumber, result[i].PersonalNumber);
                Assert.AreEqual(expectedResult[i].FirstName, result[i].FirstName);
                Assert.AreEqual(expectedResult[i].LastName, result[i].LastName);
                Assert.AreEqual(expectedResult[i].Address, result[i].Address);
            }
        }

        [TestMethod]
        public void thatListCustomersFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);


            var result = (RedirectToRouteResult)controller.ListCustomers();

            //Assert
            Assert.AreEqual(result.RouteValues.Values.First(), "Login");
        }

        [TestMethod]
        public void thatListAccountsFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            var actionResult = (RedirectToRouteResult)controller.ListAccounts("");

            //Assert
            Assert.AreEqual(actionResult.RouteName, "");
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "Login");
        }

        [TestMethod]
        public void thatListAccountsPartialReturnsExpectedResult()
        {
            var SessionMock = new TestControllerBuilder();
var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var expectedResult = new List<Account>();

            Account acc = new Account()
            {
                AccountNumber = "12345678909",
                Balance = 0,
                OwnerName = "Kjetil Olsen"
            };

            for (int i = 0; i < 3; i++)
            {
                expectedResult.Add(acc);
            }

            var actionResult = (ViewResult) controller.ListAccountsPartial("12345678901");
            var result = (List<Account>) actionResult.Model;

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(result[i].AccountNumber, expectedResult[i].AccountNumber);
                Assert.AreEqual(result[i].Balance, expectedResult[i].Balance);
                Assert.AreEqual(result[i].OwnerName, expectedResult[i].OwnerName);
            }

        }

        [TestMethod]
        public void thatListAccountsPartialFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            var actionResult = (RedirectToRouteResult)controller.ListAccountsPartial("");

            //Assert
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "Login");
        }

        [TestMethod]
        public void thatDeleteAccountDeletesAccount()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                            new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var delete = controller.DeleteAccount("12345567");
            Assert.AreEqual("{ result = True }", delete.Data.ToString());

        }

        [TestMethod]
        public void thatDeleteAccountFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                            new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);


            var delete = controller.DeleteAccount("12345567");
            Assert.AreEqual("{ result = False }", delete.Data.ToString());

        }
        [TestMethod]
        public void thatDeleteAccountFailsWithIllegalAccountNumber()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                            new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);


            var delete = controller.DeleteAccount("");
            Assert.AreEqual("{ result = False }", delete.Data.ToString());

        }


        [TestMethod]
        public void thatUpdateAccountReturnsAccount()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;
            EditableAccount account = new EditableAccount()
            {
                AccountNumber = "9876543210",
                OwnerPersonalNumber = "12345678901"
            };

            var actionResult = (ViewResult) controller.UpdateAccount("9876543210");
            var result = (EditableAccount) actionResult.Model;


            Assert.AreEqual(account.AccountNumber, result.AccountNumber);
            Assert.AreEqual(account.OwnerPersonalNumber, result.OwnerPersonalNumber);

        }

        [TestMethod]
        public void thatUpdateAccountFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);


            var result = (RedirectToRouteResult)controller.UpdateAccount(("9876543210"));

            //Assert
            Assert.AreEqual(result.RouteValues.Values.First(), "Login");

        }

        [TestMethod]
        public void thatPostUpdateAccountUpdatesAccountAndReturnsToListAccounts()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var account = new EditableAccount()
            {
                AccountNumber = "55555555555",
                OwnerPersonalNumber = "12345678902"
            };

            var actionResult = (RedirectToRouteResult) controller.UpdateAccount(account);

            Assert.AreEqual(actionResult.RouteName, "");
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "ListAccounts");

        }

        [TestMethod]
        public void thatPostUpdateAccountFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);


            var account = new EditableAccount();
            var result = (RedirectToRouteResult)controller.UpdateAccount(account);

            //Assert
            Assert.AreEqual(result.RouteValues.Values.First(), "Login");

        }

        [TestMethod]
        public void thatPostUpdateAccountValidationFails()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var account = new EditableAccount();
            controller.ViewData.ModelState.AddModelError("Accountnumber", "Accountnumber ikke oppgitt");

            var result = (ViewResult) controller.UpdateAccount(account);

            Assert.IsTrue(result.ViewData.ModelState.Count == 1);
            Assert.AreEqual(result.ViewName, "");


        }

        [TestMethod]
        public void thatPostUpdateAccountFails() //TODO: BEDRE NAVN
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                 new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var account = new EditableAccount();
            account.AccountNumber = "";

            var result = (ViewResult)controller.UpdateAccount(account);
            // Assert
            Assert.AreEqual(result.ViewName, "");

        }

        [TestMethod]
        public void thatAddAccountAddsAccount() 
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var actionResult = (RedirectToRouteResult)controller.AddAccount("12345678901");

            // Assert
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "12345678901");
            Assert.AreEqual(actionResult.RouteValues.Values.ElementAt(1), "ListAccounts");

        }

        [TestMethod]
        public void thatAddAccountFailsWithIllegalPersonalNumber()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            //Illegal personalnumber
            var actionResult = (RedirectToRouteResult)controller.AddAccount("123478901");

            // Assert
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "ListCustomers");
        }

        [TestMethod]
        public void thatAddAccountFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            var actionResult = (RedirectToRouteResult)controller.AddAccount("123478901");

            Assert.AreEqual(actionResult.RouteValues.Values.First(), "Login");
        }

        [TestMethod]
        public void thatValidateAdminLogsInAdmin()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            
            FormCollection inList = new FormCollection();
            inList.Add("EmployeeNumber", "12345678901");
            var actionResult = (RedirectToRouteResult) controller.ValidateAdmin(inList);

            // Assert
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "ListCustomers");


        }

        [TestMethod]
        public void thatValidateAdminFailstoLogInAdmin()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            FormCollection inList = new FormCollection();
            var actionResult = (RedirectToRouteResult)controller.ValidateAdmin(inList);

            // Assert
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "Login");


        }

        [TestMethod]
        public void thatLoginRedirectsCorrectlyWhenLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            controller.Session["adminloggedin"] = true;

            var result = (RedirectToRouteResult) controller.Login();

            // Assert
            Assert.AreEqual(result.RouteValues.Values.First(), "ListCustomers");

        }

        [TestMethod]
        public void thatLoginRedirectsCorrectlyWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);


            var result = (ViewResult)controller.Login();

            // Assert
            Assert.AreEqual(result.ViewName, "");

        }

        [TestMethod]
        public void thatLogoutRedirectsCorrectlyAndLogsOut()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;
            var result = (RedirectToRouteResult)controller.Logout();

            // Assert
            Assert.AreEqual(result.RouteValues.Values.First(), "Login");
            Assert.AreEqual(controller.Session["adminloggedin"], false);

        }

        [TestMethod]
        public void thatRegisterCustomerReturnsView()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var result = (ViewResult) controller.RegisterCustomer();

            Assert.AreEqual(result.ViewName, "");

        }

        [TestMethod]
        public void thatRegisterCustomerRedirectsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            

            var result = (RedirectToRouteResult)controller.RegisterCustomer();

            Assert.AreEqual(result.RouteValues.Values.First(), "Login");

        }

        [TestMethod]
        public void thatPostRegisterCustomerRegistersCustomer()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;
            var customer = new CustomerInfo()
            {
                FirstName = "Kjetil",
                LastName = "Olsen"
            };

            var result = (RedirectToRouteResult) controller.RegisterCustomer(customer);

            Assert.AreEqual(result.RouteValues.Values.First(), "ListCustomers");
        }

        [TestMethod]
        public void thatPostRegisterCustomerValidationFails()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var customer = new CustomerInfo();
            controller.ViewData.ModelState.AddModelError("FirstName", "Fornavn ikke oppgitt");

            var result = (ViewResult)controller.RegisterCustomer(customer);

            Assert.IsTrue(result.ViewData.ModelState.Count == 1);
            Assert.AreEqual(result.ViewName, "");

        }

        [TestMethod]
        public void thatPostRegisterCustomerFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);


            var customer = new CustomerInfo();
            var result = (RedirectToRouteResult)controller.RegisterCustomer(customer);

            //Assert
            Assert.AreEqual(result.RouteValues.Values.First(), "Login");
        }

        [TestMethod]
        public void thatUpdateCustomerReturnsView()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var result = (ViewResult)controller.UpdateCustomer("");

            Assert.AreEqual(result.ViewName, "");

        }

        [TestMethod]
        public void thatUpdateCustomerRedirectsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);


            var result = (RedirectToRouteResult)controller.UpdateCustomer("");

            Assert.AreEqual(result.RouteValues.Values.First(), "Login");

        }

        [TestMethod]
        public void thatPostRegisterCustomerFails()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var customer = new CustomerInfo();
            customer.FirstName = "";

            var result = (ViewResult)controller.RegisterCustomer(customer);
            // Assert
            Assert.AreEqual(result.ViewName, "");
        }

        [TestMethod]
        public void thatPostUpdateCustomerUpdatesCustomerAndReturnsToListCustomer()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var customer = new CustomerInfo()
            {
                FirstName = "Kjetil",
                LastName = "Olsen"
            };

            var result = (RedirectToRouteResult)controller.UpdateCustomer(customer);

            Assert.AreEqual(result.RouteName, "");
            Assert.AreEqual(result.RouteValues.Values.First(), "ListCustomers");

        }

        [TestMethod]
        public void thatPostUpdateCustomerValidationFails()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var customer = new CustomerInfo();
            controller.ViewData.ModelState.AddModelError("FirstName", "Fornavn ikke oppgitt");

            var result = (ViewResult)controller.UpdateCustomer(customer);

            Assert.IsTrue(result.ViewData.ModelState.Count == 1);
            Assert.AreEqual(result.ViewName, "");

        }

        [TestMethod]
        public void thatPostUpdateCustomerFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);


            var customer = new CustomerInfo();
            var result = (RedirectToRouteResult)controller.UpdateCustomer(customer);

            //Assert
            Assert.AreEqual(result.RouteValues.Values.First(), "Login");
        }

        [TestMethod]
        public void thatPostUpdateCustomerFails()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var customer = new CustomerInfo();
            customer.FirstName = "";

            var result = (ViewResult)controller.UpdateCustomer(customer);
            // Assert
            Assert.AreEqual(result.ViewName, "");
        }


        [TestMethod]
        public void thatListExecutableTranstactionsReturnsExpectedResults()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var expectedResult = new List<Transaction>();
            var transaction = new Transaction()
            {
                Amount = 100,
                FromAccountNumber = "12345678901",
                ToAccountNumber = "12345678902",
            };
            expectedResult.Add(transaction);
            expectedResult.Add(transaction);
            expectedResult.Add(transaction);


            var actionResult = (ViewResult)controller.ListTransactions();
            var result = (List<Transaction>)actionResult.Model;

            Assert.AreEqual(actionResult.ViewName, "");

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i].Amount, result[i].Amount);
                Assert.AreEqual(expectedResult[i].FromAccountNumber, result[i].FromAccountNumber);
                Assert.AreEqual(expectedResult[i].ToAccountNumber, result[i].ToAccountNumber);
            }
        }

        [TestMethod]
        public void thatListExecutableTransactionsFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            var actionResult = (RedirectToRouteResult)controller.ListTransactions();

            //Assert
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "Login");
        }

        [TestMethod]
        public void thatDeleteCustomerDeletesCustomer()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                            new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var delete = controller.Delete("12345670");
            Assert.AreEqual("{ result = True }", delete.Data.ToString());

        }

        [TestMethod]
        public void thatDeleteCustomerFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                            new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            var delete = controller.Delete("12345670");
            Assert.AreEqual("{ result = False }", delete.Data.ToString());

        }

        [TestMethod]
        public void thatDeleteCustomerFailsWithIllegalPersonalNumber()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                            new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            var delete = controller.Delete("");
            Assert.AreEqual("{ result = False }", delete.Data.ToString());
        }

        [TestMethod]
        public void thatExecuteTransactionExecutesTransaction()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                            new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);
            controller.Session["adminloggedin"] = true;

            var result = (RedirectToRouteResult)controller.ExecuteTransaction(1);

            Assert.AreEqual(result.RouteName, "");
            Assert.AreEqual(result.RouteValues.Values.First(), "ListTransactions");

        }

        [TestMethod]
        public void thatExecuteTransactionFailsWhenNotLoggedIn()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            var actionResult = (RedirectToRouteResult)controller.ExecuteTransaction(1);

            //Assert
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "Login");
        }

        [TestMethod]
        public void thatExecuteTransactionFailsWithIllegalTransactionId()
        {
            var SessionMock = new TestControllerBuilder();
            var controller = new AdminController(new AdminLogic(new AdminRepositoryStub()),
                new CustomerLogic(new CustomerRepositoryStub()), new AccountLogic(new AccountRepositoryStub()), new TransactionLogic(new TransactionRepositoryStub()));
            SessionMock.InitializeController(controller);

            var actionResult = (RedirectToRouteResult)controller.ExecuteTransaction(1);

            //Assert
            Assert.AreEqual(actionResult.RouteValues.Values.First(), "Login");
        }


    }
}