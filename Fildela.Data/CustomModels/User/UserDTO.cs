using System;
using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.User
{
    [DataContract]
    public class UserDTO
    {
        [DataMember]
        public int UserID { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string UserRolesToString { get; set; }

        [DataMember]
        public bool IsPremium { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public DateTime DateRegistered { get; set; }

        [DataMember]
        public DateTime DateLastActive { get; set; }
    }
}
