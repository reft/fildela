using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.Account
{
    [DataContract]
    public class FileDTO
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long Size { get; set; }
    }
}
