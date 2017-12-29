namespace Fildela.Business.Domains.User.Models
{
    public class AccountAuthenticationProviderModel
    {
        public int AccountID { get; set; }

        public int AuthenticationProviderID { get; set; }

        public string KeyHash { get; set; }

        public string KeySalt { get; set; }
    }
}
