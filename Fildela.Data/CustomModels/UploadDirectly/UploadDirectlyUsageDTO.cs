using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.UploadDirectly
{
    [DataContract]
    public class UploadDirectlyUsageDTO
    {
        [DataMember]
        public int FileCount { get; set; }

        [DataMember]
        public long StoredBytes { get; set; }

        [DataMember]
        public int AllowedTotalFileCount { get; set; }

        [DataMember]
        public int AllowedTotalStoredBytes { get; set; }

        [DataMember]
        public int AllowedEmailCountPerFile { get; set; }

        [DataMember]
        public int FileLifetimeInHours { get; set; }
    }
}
