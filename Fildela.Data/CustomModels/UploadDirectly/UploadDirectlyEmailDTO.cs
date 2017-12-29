using System.Runtime.Serialization;

namespace Fildela.Data.CustomModels.UploadDirectly
{
    [DataContract]
    public class UploadDirectlyEmailDTO
    {
        [DataMember]
        public int AllowedEmailCountPerFile { get; set; }

        [DataMember]
        public int EmailCountForCurrentFile { get; set; }
    }
}
