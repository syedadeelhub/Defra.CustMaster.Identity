// <copyright file="AzureKeyVaultSetting.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.KeyVault
{
    public class AzureKeyVaultSettings
    {
        public bool UseKeyVault { get; set; }

        public string VaultName { get; set; }

        public string VaultClientId { get; set; }

        public string VaultSecret { get; set; }

        public string VaultKeyDefraCmS2SAppid { get; set; }

        public string VaultKeyDefraCmS2SSecret { get; set; }

        public string VaultKeyDefraCmCertPwd { get; set; }

        public string VaultKeyCertValSetsClientCertificate { get; set; }
    }
}
