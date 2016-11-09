using System.Web.Mvc;

namespace DAL
{
    public interface IAdminRepository
    {
        bool ValidateAdmin(FormCollection inList);
        Admins FindAdminByEmployeeNumber(string employeeNumber);
        string CreateHash(string password, string salt);
        string CreateSalt(int size);

    }
}