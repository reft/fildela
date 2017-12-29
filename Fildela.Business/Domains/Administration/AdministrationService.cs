
using Fildela.Data.Repositories.Administration;

namespace Fildela.Business.Domains.Administration
{
    public class AdministrationService : IAdministrationService
    {
        private readonly IAdministrationRepository _administrationRepository;

        public AdministrationService(IAdministrationRepository administrationRepository)
        {
            _administrationRepository = administrationRepository;
        }

        public void InsertAdminLog(Models.AdminLogModel adminLog)
        {
            _administrationRepository.InsertAdminLog(adminLog.ToEntity());
        }

        public bool AdminLogExist(string ipAddress, string type)
        {
            return _administrationRepository.AdminLogExist(ipAddress, type);
        }

        public int AdminLogExistCount(string ipAddress, string type)
        {
            return _administrationRepository.AdminLogExistCount(ipAddress, type);
        }

        public bool AdminLogExistAuthenticated(string email, string type)
        {
            return _administrationRepository.AdminLogExistAuthenticated(email, type);
        }

        public Models.AdministrationAccountsDTOModel GetUsersAdministration()
        {
            return _administrationRepository.GetUsersAdministration().ToModel();
        }

        public Models.AdministrationNewsDTOModel GetNewsAdministration()
        {
            return _administrationRepository.GetNewsAdministration().ToModel();
        }

        public Models.AdministrationEmailsDTOModel GetEmailAdministration()
        {
            return _administrationRepository.GetEmailAdministration().ToModel();
        }

        public Models.AdministrationLogsDTOModel GetLogsAdministration()
        {
            return _administrationRepository.GetLogsAdministration().ToModel();
        }

        public string GetUserFullName(int userID)
        {
            return _administrationRepository.GetUserFullName(userID);
        }
    }
}
