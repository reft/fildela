using Fildela.Data.Repositories.Account;
using Fildela.Data.Repositories.Administration;
using Fildela.Data.Repositories.Category;
using Fildela.Data.Repositories.News;
using Fildela.Data.Repositories.UploadDirectly;
using Fildela.Data.Repositories.User;
using Ninject.Modules;

namespace Fildela.Business
{
    public class DomainInjectionModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAccountRepository>().To<AccountRepository>();
            Bind<IAdministrationRepository>().To<AdministrationRepository>();
            Bind<ICategoryRepository>().To<CategoryRepository>();
            Bind<INewsRepository>().To<NewsRepository>();
            Bind<IUploadDirectlyRepository>().To<UploadDirectlyRepository>();
            Bind<IUserRepository>().To<UserRepository>();
        }
    }
}
