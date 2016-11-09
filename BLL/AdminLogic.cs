using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DAL;

namespace BLL {
    public class AdminLogic : IAdminLogic
    {

        private IAdminRepository _repository;
        private readonly HttpContext _context = HttpContext.Current;

        public AdminLogic()
        {
            _repository = new AdminRepository();   
        }

        public AdminLogic(IAdminRepository stub)
        {
            _repository = stub;
        }

        public bool ValidateAdmin(FormCollection inList) {
            return _repository.ValidateAdmin(inList);
        }

        public string CreateHash(string password, string salt) {
            return _repository.CreateHash(password, salt);
        }

        public string CreateSalt(int size) {
            return _repository.CreateSalt(size);
        }

        public bool InsertAdmin() {
            return _repository.InsertAdmin();
        }
    }
}
