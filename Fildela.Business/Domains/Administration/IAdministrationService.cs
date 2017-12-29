using Fildela.Business.Domains.Administration.Models;

namespace Fildela.Business.Domains.Administration
{
    public interface IAdministrationService
    {
        void InsertAdminLog(AdminLogModel adminLog);

        bool AdminLogExist(string ipAddress, string type);

        int AdminLogExistCount(string ipAddress, string type);

        bool AdminLogExistAuthenticated(string email, string type);

        AdministrationAccountsDTOModel GetUsersAdministration();

        AdministrationNewsDTOModel GetNewsAdministration();

        AdministrationEmailsDTOModel GetEmailAdministration();

        AdministrationLogsDTOModel GetLogsAdministration();

        string GetUserFullName(int userID);
    }
}
