// <copyright file="InitialMatchResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Model
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class InitialMatchResponse
    {
        [DataMember]
        [JsonProperty("value")]
        public List<ServiceObject> Value { get; set; }
    }
}
