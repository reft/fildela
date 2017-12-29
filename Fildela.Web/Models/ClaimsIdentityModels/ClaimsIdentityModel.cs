
namespace Fildela.Web.Models
{
    public class ClaimsIdentityModel
    {
        public int AccountID { get; set; }

        public string AccountEmail { get; set; }

        public int AccountOwnerID { get; set; }

        public string AccountOwnerEmail { get; set; }

        public string UserRoles { get; set; }

        public string UserType { get; set; }

        public string IpAddress { get; set; }
    }
}