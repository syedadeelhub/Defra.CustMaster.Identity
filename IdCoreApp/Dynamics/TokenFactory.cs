// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Dynamics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Defra.CustMaster.Identity.CoreApp.Security;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed.")]
    public class TokenFactory : ITokenFactory
    {
        private DateTimeOffset tokenExpiry = DateTimeOffset.MinValue.LocalDateTime;
        private int tokenIdex = 1;

        public TokenFactory(Uri resource, string clientId, string clientSecret, ClientCertificateFactory clientCertificateFactory)
        {
            Resource = resource;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ConfiguredClientCertificateFactory = clientCertificateFactory;
        }

        public TokenFactory(Uri resource, IDictionary<int, D365ClientCredential> clientConfigDictionary, ClientCertificateFactory clientCertificateFactory)
        {
            Resource = resource;
            ClientConfigDictionary = clientConfigDictionary;
            ConfiguredClientCertificateFactory = clientCertificateFactory;
        }

        public string BearerToken { get; private set; }

        private string ClientId { get; }

        private string ClientSecret { get; }

        private IDictionary<int, D365ClientCredential> ClientConfigDictionary { get; }

        private Uri Resource { get; }

        private ClientCertificateFactory ConfiguredClientCertificateFactory { get; }

        public ClientCertificateFactory GetClientCertificateFactory() => ConfiguredClientCertificateFactory;

        public string GetToken(bool moveToken)
        {
            if (string.IsNullOrWhiteSpace(BearerToken)
                || tokenExpiry < DateTimeOffset.Now.LocalDateTime.AddMinutes(5)
                || moveToken)
            {
                // Rotate the token index, if it reaches the limit restart the token index counter to 1
                if (moveToken)
                {
                    tokenIdex = (tokenIdex++ <= ClientConfigDictionary.Count) ? tokenIdex : 1;
                }

                string appId = ClientConfigDictionary[tokenIdex].ClientId;
                string secret = ClientConfigDictionary[tokenIdex].Secret;

                // Get a new D365 bearer token
                AuthenticationParameters authenticationParameters = AuthenticationParameters.CreateFromResourceUrlAsync(resourceUrl: Resource).Result;
                AuthenticationContext authenticationContext = new AuthenticationContext(authority: authenticationParameters.Authority, validateAuthority: false);
                ClientCredential clientCredential = new ClientCredential(clientId: appId, clientSecret: secret);
                AuthenticationResult authenticationResult = authenticationContext.AcquireTokenAsync(resource: authenticationParameters.Resource, clientCredential: clientCredential).Result;

                tokenExpiry = authenticationResult.ExpiresOn.LocalDateTime;
                BearerToken = authenticationResult.AccessToken;
            }

            return BearerToken;
        }

        public void InvalidateToken()
        {
            tokenExpiry = DateTime.Now;
        }
    }
}