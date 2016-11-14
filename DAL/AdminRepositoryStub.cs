using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DAL
{
    public class AdminRepositoryStub : IAdminRepository
    {


       public bool ValidateAdmin(FormCollection inList) 
        {
            var admin = FindAdminByEmployeeNumber(inList["EmployeeNumber"]);
            if (admin != null)
            {
                return true;
            }
              else
            {
                return false;
            }
        }

        public Admins FindAdminByEmployeeNumber(string employeeNumber)
        {
            if (employeeNumber == null)
                return null;
            Admins admin = new Admins()
            {
                EmployeeNumber = employeeNumber,
                FirstName = "Kjetil",
                LastName = "Olsen",
                PostalNumber = "8909"
            };
            return admin;
        }

        public string CreateHash(string password, string salt)
        {
            if (password == "" && salt == "")
            {
                return "FAIL";
            }
            return "SUCCESS";
        }

        public string CreateSalt(int size)
        {
            if (size != 32)
                return "FAIL";
            return "SUCCESS";
        }

        public bool InsertAdmin() {
            throw new NotImplementedException();
        }
    }
}

