namespace Defra.CustMaster.Identity.CoreApp.Model
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class Enrolments
    {
        [DataMember]
        [JsonProperty("value")]
        public List<ServiceUserLink> ServiceUserLinks { get; set; }
    }
}
