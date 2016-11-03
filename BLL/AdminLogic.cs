using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL;

namespace BLL {
    public class AdminLogic {
        private readonly HttpContext _context = HttpContext.Current;

        public bool Login() {
            if (_context.Session["loggedin"] == null) {
                _context.Session["loggedin"] = false;
            } else {
                return (bool)_context.Session["loggedin"];
            }
            return false;
        }

        public void Logout() {
            _context.Session["loggedin"] = false;
            _context.Session["CurrentAdmin"] = null;
        }

        public bool ValidateAdmin(FormCollection inList) {
            var adminRepository = new AdminRepository();
            return adminRepository.ValidateAdmin(inList);
        }

        public Admins FindAdminByEmployeeNumber(string employeeNumber) {
            var adminRepository = new AdminRepository();
            return adminRepository.FindAdminByEmployeeNumber(employeeNumber);
        }

        public string CreateHash(string password, string salt) {
            var adminRepository = new AdminRepository();
            return adminRepository.CreateHash(password, salt);
        }

        public string CreateSalt(int size) {
            var adminRepository = new AdminRepository();
            return adminRepository.CreateSalt(size);
        }

        public bool InsertAdmin() {
            var adminRepository = new AdminRepository();
            return adminRepository.InsertAdmin();
        }
    }
}
