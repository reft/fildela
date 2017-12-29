using Fildela.Web.Models.UserModels;
using System.Collections.Generic;

namespace Fildela.Web.Models.AccountModels
{
    public class SettingsViewModel : AccountViewModel
    {
        public int Tab { get; set; }

        public IEnumerable<AuthenticationProviderModel> UserAuthenticationProviders { get; set; }
    }
}