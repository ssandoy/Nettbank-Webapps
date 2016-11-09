using System.Web.Mvc;
using DAL;

namespace BLL
{
    public interface IAdminLogic
    {
        bool ValidateAdmin(FormCollection inList);
        Admins FindAdminByEmployeeNumber(string employeeNumber);
        string CreateHash(string password, string salt);
        string CreateSalt(int size);
    }
}