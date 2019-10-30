// <copyright file="AuthzResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Model
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class AuthzResponse
    {
        [DataMember(Order =0)]
        public string version;

        [DataMember(Order = 1)]
        public int status;

        [DataMember(Order = 2)]
        public string message;

        [DataMember(Order = 3)]

        public List<string> roles { get; set; }

        [DataMember(Order = 4)]
        public List<string> mappings { get; set; }
    }

    [DataContract]
    public class Mappings
    {
        [DataMember]
        public List<string> map { get; set; }
    }
}