using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.Account
{
    [DataContract]
    public class UsageDTO
    {
        [DataMember]
        public int FileCount { get; set; }

        [DataMember]
        public int GuestAccountCount { get; set; }

        [DataMember]
        public int LinkCount { get; set; }

        [DataMember]
        public int LogCount { get; set; }

        [DataMember]
        public long StoredBytes { get; set; }

        [DataMember]
        public int AllowedFileCount { get; set; }

        [DataMember]
        public int AllowedGuestAccountCount { get; set; }

        [DataMember]
        public int AllowedLinkCount { get; set; }

        [DataMember]
        public long AllowedStoredBytes { get; set; }
    }
}
