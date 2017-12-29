using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.Account
{
    [DataContract]
    public class AccountLinkPermissionBoolDTO
    {
        [DataMember]
        public bool FileRead { get; set; }

        [DataMember]
        public bool FileWrite { get; set; }

        [DataMember]
        public bool FileEdit { get; set; }

        [DataMember]
        public bool LinkRead { get; set; }

        [DataMember]
        public bool LinkWrite { get; set; }

        [DataMember]
        public bool LinkEdit { get; set; }
    }
}
