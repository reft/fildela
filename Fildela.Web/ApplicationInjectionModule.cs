using Fildela.Business.Domains.Account;
using Fildela.Business.Domains.Administration;
using Fildela.Business.Domains.Category;
using Fildela.Business.Domains.News;
using Fildela.Business.Domains.UploadDirectly;
using Fildela.Business.Domains.User;
using Ninject.Modules;

namespace Fildela.Web
{
    public class ApplicationInjectionModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAccountService>().To<AccountService>();
            Bind<IAdministrationService>().To<AdministrationService>();
            Bind<ICategoryService>().To<CategoryService>();
            Bind<INewsService>().To<NewsService>();
            Bind<IUploadDirectlyService>().To<UploadDirectlyService>();
            Bind<IUserService>().To<UserService>();
        }
    }
}