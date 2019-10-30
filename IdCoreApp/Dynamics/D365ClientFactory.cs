// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Dynamics
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Defra.CustMaster.Identity.CoreApp.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed.")]
    public class D365ClientFactory : IClientFactory
    {
        private readonly HttpClient factoryHttpClient;

        public D365ClientFactory(IConfiguration iConfig, ITokenFactory tokenFactory, HttpClient httpClient)
        {
            factoryHttpClient = httpClient;
            Configuration = iConfig;
            TokenFactory = tokenFactory;
            BaseAddress = iConfig[$"{CommonConstants.D36Setts}:{CommonConstants.D365SettsApiBaseAddress}"];
        }

        private ITokenFactory TokenFactory { get;  }

        private IConfiguration Configuration { get; }

        private string BaseAddress { get;  }

        public ClientCertificateFactory GetClientCertificateFactory()

        {
            return TokenFactory.GetClientCertificateFactory();
        }

        public HttpClient GetHttpClient(bool moveToken = false)
        {
            factoryHttpClient.BaseAddress = new Uri(BaseAddress);
            factoryHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            factoryHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(moveToken));
            factoryHttpClient.Timeout = new TimeSpan(0, 2, 0);  // 2 minutes
            factoryHttpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            factoryHttpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");

            // This format annotation of 'FormattedValue' is required for the Enrolment Status option set.
            factoryHttpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");

            return factoryHttpClient;
        }

        private string GetToken(bool moveToken)
        {
            return TokenFactory.GetToken(moveToken);
        }
    }
}
