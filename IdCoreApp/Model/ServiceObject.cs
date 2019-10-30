// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Model
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class ServiceObject
    {
        [DataMember]
        [JsonProperty("contactid")]
        public string ServiceUserID { get; set; }

        [DataMember]
        [JsonProperty("defra_uniquereference")]
        public string UniqueReferenceId { get; set; }

        [DataMember]
        [DefaultValue("200")]
        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }

        [DataMember]
        [JsonProperty("errorMsg")]
        public string ErrorMsg { get; set; }
    }
}