// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Dynamics
{
    using Defra.CustMaster.Identity.CoreApp.Security;

    public interface ITokenFactory
    {
        string GetToken(bool moveToken);

        ClientCertificateFactory GetClientCertificateFactory();

        void InvalidateToken();
    }
}
