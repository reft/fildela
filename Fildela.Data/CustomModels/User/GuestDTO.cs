using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.User
{
    [DataContract]
    public class GuestDTO
    {
        [DataMember]
        public int UserID { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public DateTime DateRegistered { get; set; }

        [DataMember]
        public List<String> PermissionNames { get; set; }

        [DataMember]
        public DateTime DateStart { get; set; }

        [DataMember]
        public DateTime DateExpires { get; set; }
    }
}
