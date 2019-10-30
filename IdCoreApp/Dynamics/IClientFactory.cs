// <copyright file="IClientFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Dynamics
{
    using System.Net.Http;
    using Defra.CustMaster.Identity.CoreApp.Security;

    public interface IClientFactory
    {

        HttpClient GetHttpClient(bool moveToken = false);

        ClientCertificateFactory GetClientCertificateFactory();
    }
}