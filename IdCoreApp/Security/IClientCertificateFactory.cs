// <copyright file="IClientCertificateHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Security
{
    using System.Security.Cryptography.X509Certificates;

    public interface IClientCertificateFactory
    {
        string ValidateClientCertificate(X509Certificate2 certificate);
    }
}
