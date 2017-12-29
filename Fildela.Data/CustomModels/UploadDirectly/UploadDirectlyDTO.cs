using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.UploadDirectly
{
    [DataContract]
    public class UploadDirectlyDTO
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public long FileSize { get; set; }

        [DataMember]
        public string BlobURL { get; set; }
    }
}
