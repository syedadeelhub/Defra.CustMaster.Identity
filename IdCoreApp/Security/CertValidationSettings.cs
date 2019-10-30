// <copyright file="CertValidationSettings.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Security
{
    public class CertValidationSettings
    {
        public string Subject { get; set; }

        public string IssuerCN { get; set; }

        public string Thumbprint { get; set; }

        public string StoreCustomerClientCert { get; set; }
    }
}
