// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Dynamics
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Defra.CustMaster.Identity.CoreApp.Model;

    public interface ICrmApiWrapper
    {
        ContactModel CreateContact(ContactModel contact);

        ServiceObject InitialMatch(Guid b2cObjectId);

        Enrolments Authz(Guid serviceID, Guid b2cObjectId);

        string ValidateClientCertificate(X509Certificate2 cert);
    }
}
