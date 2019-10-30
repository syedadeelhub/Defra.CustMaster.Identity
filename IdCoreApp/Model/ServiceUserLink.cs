namespace Defra.CustMaster.Identity.CoreApp.Model
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class ServiceUserLink
    {
        [DataMember]
        [JsonProperty("serviceLinkContact.fullname")]
        public string ContactName { get; set; }

        [DataMember]
        [JsonProperty("serviceLinkRole.defra_lobserivceroleid")]
        public string RoleId { get; set; }

        [DataMember]
        [JsonProperty("serviceLinkRole.defra_name")]
        public string RoleName { get; set; }

        [DataMember]
        [JsonProperty("serviceLinkOrganisation.accountid")]
        public string OrganisationId { get; set; }

        [DataMember]
        [JsonProperty("serviceLinkOrganisation.name")]
        public string OrganisationName { get; set; }

        [DataMember]
        [JsonProperty("defra_enrolmentstatus")]
        public string EnrolmentStatus { get; set; }

        [DataMember]
        [JsonProperty("defra_enrolmentstatus@OData.Community.Display.V1.FormattedValue")]
        public string EnrolmentStatusText { get; set; }
    }
}