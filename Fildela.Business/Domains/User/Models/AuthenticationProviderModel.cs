namespace Fildela.Business.Domains.User.Models
{
    public class AuthenticationProviderModel
    {
        public int AuthenticationProviderID { get; set; }

        public string Name { get; set; }

        public string IconClass { get; set; }

        public string IconColor { get; set; }
    }
}
