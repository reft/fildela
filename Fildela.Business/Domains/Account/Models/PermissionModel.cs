
namespace Fildela.Business.Domains.Account.Models
{
    public class PermissionModel
    {
        public int PermissionID { get; set; }

        public int PermissionTypeID { get; set; }

        public string Name { get; set; }

        public PermissionTypeModel PermissionType { get; set; }
    }
}
