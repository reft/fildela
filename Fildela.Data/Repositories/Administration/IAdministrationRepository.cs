using Fildela.Data.CustomModels.Administration;
using Fildela.Data.Storage.Models;

namespace Fildela.Data.Repositories.Administration
{
    public interface IAdministrationRepository
    {
        void InsertAdminLog(AdminLog adminLog);

        bool AdminLogExist(string ipAddress, string type);

        int AdminLogExistCount(string ipAddress, string type);

        bool AdminLogExistAuthenticated(string email, string type);

        AdministrationAccountsDTO GetUsersAdministration();

        AdministrationNewsDTO GetNewsAdministration();

        AdministrationEmailsDTO GetEmailAdministration();

        AdministrationLogsDTO GetLogsAdministration();

        string GetUserFullName(int userID);
    }
}
