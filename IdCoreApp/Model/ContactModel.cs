// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract]
    public class ContactModel
    {
        [JsonProperty("contactid")]
        public string contactid;

        [JsonProperty("defra_uniquereference")]
        public string uniqueReference;

        [JsonProperty("firstname")]
        public string firstname;

        [JsonProperty("lastname")]
        public string lastname;

        [JsonProperty("emailaddress1")]
        public string emailaddress1;

        [JsonProperty("defra_b2cobjectid")]
        [Required(ErrorMessage= "defra_b2cobjectid can not be empty or null")]
        public string b2cobjectid;

        [JsonProperty("serviceid")]
        [Required(ErrorMessage = "serviceid can not be empty or null")]
        public string serviceid;

        [JsonProperty("Code")]
        public int Code;

        [JsonProperty("Message")]
        public string Message;

        [JsonProperty("MessageDetail")]
        public string MessageDetail;

        [JsonProperty("HttpStatusCode")]
        public HttpStatusCode HttpStatusCode;
    }   
}