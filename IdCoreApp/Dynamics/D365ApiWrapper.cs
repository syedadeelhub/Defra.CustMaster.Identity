// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Dynamics
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Defra.CustMaster.Identity.CoreApp.Model;
    using Defra.CustMaster.Identity.CoreApp.Security;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed.")]
    public class D365ApiWrapper : ICrmApiWrapper
    {
        public D365ApiWrapper(IClientFactory iHttpClientFactory)
        {
            ClientFactory = iHttpClientFactory;
            D365Client = ClientFactory.GetHttpClient();
            CertFactory = ClientFactory.GetClientCertificateFactory();
        }

        private IClientFactory ClientFactory { get; }

        private HttpClient D365Client { get; set; }

        private ClientCertificateFactory CertFactory { get; }

        public ServiceObject InitialMatch(Guid b2cObjectId)
        {
            return InitialMatch(b2cObjectId.ToString());
        }

        public Enrolments Authz(Guid serviceID, Guid b2cObjectId)
        {
            return Authz(serviceID.ToString(), b2cObjectId.ToString());
        }

        public ContactModel CreateContact(ContactModel contact)
        {
            HttpRequestMessage request = new HttpRequestMessage(
                method: HttpMethod.Post,
                requestUri: D365Client.BaseAddress + "defra_CreateContact");

            JObject exeAction = new JObject();
            if (contact.b2cobjectid != null)
            {
                exeAction["defra_b2cobjectid"] = contact.b2cobjectid;
            }

            if (contact.firstname != null)
            {
                exeAction["firstname"] = contact.firstname;
            }

            if (contact.lastname != null)
            {
                exeAction["lastname"] = contact.lastname;
            }

            if (contact.emailaddress1 != null)
            {
                exeAction["emailaddress1"] = contact.emailaddress1;
            }

            string paramsContent;
            if (exeAction.GetType().Name.Equals(value: "JObject", comparisonType: System.StringComparison.OrdinalIgnoreCase))
            {
                paramsContent = exeAction.ToString();
            }
            else
            {
                paramsContent = JsonConvert.SerializeObject(exeAction, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });
            }

            request.Content = new StringContent(paramsContent);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var content = D365Client.SendAsync(request).Result;

            if (!content.IsSuccessStatusCode)
            {
                throw new WebFaultException(content.ReasonPhrase, (int)content.StatusCode);
            }

            ContactModel contactResponse = JsonConvert.DeserializeObject<ContactModel>(content.Content.ReadAsStringAsync().Result);
            return contactResponse;
        }

        public string ValidateClientCertificate(X509Certificate2 cert) => CertFactory.ValidateClientCertificate(cert);

        private ServiceObject InitialMatch(string b2cObjectId)
        {
            tryagain:

            HttpRequestMessage request = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: D365Client.BaseAddress + $"contacts?$select=defra_uniquereference&$filter=defra_b2cobjectid eq '{b2cObjectId}'");

            HttpResponseMessage content = D365Client.SendAsync(request).Result;

            if (!content.IsSuccessStatusCode)
            {
                if (content.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                    content.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    D365Client = ClientFactory.GetHttpClient(moveToken: true);

                    goto tryagain;
                }

                throw new WebFaultException(content.ReasonPhrase, (int)content.StatusCode);
            }

            InitialMatchResponse contactResponse = JsonConvert.DeserializeObject<InitialMatchResponse>(content.Content.ReadAsStringAsync().Result);

            ServiceObject returnObj = new ServiceObject();
            if (contactResponse?.Value?.Count > 0)
            {
                if (contactResponse.Value.Count == 1)
                {
                    returnObj.ServiceUserID = contactResponse.Value[0].ServiceUserID;
                    returnObj.UniqueReferenceId = contactResponse.Value[0].UniqueReferenceId;
                }
                else
                {
                    returnObj.ErrorCode = 412;
                    returnObj.ErrorMsg = "Multiple records found";
                }
            }
            else
            {
                returnObj.ErrorCode = 204;
                returnObj.ErrorMsg = "No Content";
            }

            return returnObj;
        }

        private Enrolments Authz(string serviceID, string b2cObjectId)
        {
            StringBuilder fetchXmlRequestStrBuilder = new StringBuilder();

            fetchXmlRequestStrBuilder
                .Append(D365Client.BaseAddress)
                .Append("defra_lobserviceuserlinks?fetchXml=<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'><entity name='defra_lobserviceuserlink'>")
                .Append("<attribute name='defra_lobserviceuserlinkid'/> <attribute name = 'defra_enrolmentstatus' /> ")
                .Append("<attribute name='createdon'/><attribute name='defra_serviceuser'/><attribute name='defra_servicerole'/>")
                .Append("<order attribute='createdon' descending='true'/><filter type='and'><condition attribute='statecode' operator='eq' value='0'/></filter>")
                .Append("<link-entity name='contact' from='contactid' to='defra_serviceuser' link-type='inner' alias='serviceLinkContact'>")
                .Append("<attribute name='fullname'/><filter type='and'><condition attribute='defra_b2cobjectid' operator='eq' value='")
                .Append(b2cObjectId)
                .Append("' /></filter>")
                .Append("</link-entity><link-entity name='defra_lobserivcerole' from='defra_lobserivceroleid' to='defra_servicerole' link-type='inner' alias='serviceLinkRole'>")
                .Append("<attribute name='defra_name'/><attribute name='defra_name'/><attribute name='defra_lobserivceroleid'/><filter type='and'>")
                .Append("<condition attribute='defra_lobservice' operator='eq' uitype='defra_lobservice' value='")
                .Append(serviceID)
                .Append("'/>")
                .Append("</filter></link-entity><link-entity name='account' from='accountid' to='defra_organisation' visible='false' link-type='outer' alias='serviceLinkOrganisation'>")
                .Append("<attribute name='name'/><attribute name='accountid'/></link-entity></entity></fetch>");

            tryagain:

            HttpRequestMessage request = new HttpRequestMessage(
                method: HttpMethod.Get,
                requestUri: fetchXmlRequestStrBuilder.ToString());

            HttpResponseMessage content = D365Client.SendAsync(request).Result;

            if (!content.IsSuccessStatusCode)
            {
                if (content.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                    content.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    D365Client = ClientFactory.GetHttpClient(moveToken: true);

                    goto tryagain;
                }

                throw new WebFaultException(content.ReasonPhrase, (int)content.StatusCode);
            }

            return JsonConvert.DeserializeObject<Enrolments>(value: content.Content.ReadAsStringAsync().Result);
        }
    }
}