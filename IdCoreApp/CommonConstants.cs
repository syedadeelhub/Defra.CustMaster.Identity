// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp
{
    internal class CommonConstants
    {
        // Application Setting File
        internal const string AppSettsFileName = "appsettings";
        internal const string AppSetts = "AppSettings";
        internal const string AppSettsApplicationInsightsLogger = "ApplicationInsightsLogger";
        internal const string AppSettsAppInsightsCustomerRoleName = "AppInsightsCustomerRoleName";
        internal const string AppSettsApplicationModePrefix = "ApplicationModePrefix";

        // Azure Key Vault Setting File
        internal const string AzureKeyVaultSettsFileName = "azurekeyvaultsettings";
        internal const string AzureKeyVaultSetts = "AzureKeyVaultSettings";
        internal const string AzureKeyVaultSettsUseKeyVault = "UseKeyVault";
        internal const string AzureKeyVaultSettsVaultName = "VaultName";
        internal const string AzureKeyVaultSettsVaultClientId = "VaultClientId";
        internal const string AzureKeyVaultSettsVaultSecret = "VaultSecret";
        internal const string AzureKeyVaultSettsKeyDefraCmS2SAppId = "VaultKeyDefraCmS2SAppId";
        internal const string AzureKeyVaultSettsKeyDefraCmS2SSecret = "VaultKeyDefraCmS2SSecret";
        internal const string AzureKeyVaultSettsKeyDefraCmCertPwd = "VaultKeyDefraCmCertPwd";
        internal const string AzureKeyVaultSettsDefraCmS2SCount = "VaultKeyDefraCmS2SCount";

      // Dynamics 365 Setting File
        internal const string D365SettsFileName = "d365settings";
        internal const string D36Setts = "D365Settings";
        internal const string D36SettsKeyPrefix = "D365KeyPrefix";
        internal const string D36SettsKeyPrefixValue = "defra-cm";
        internal const string D365SettsApiBaseAddress = "D365ApiBaseAddress";
        internal const string D365SettsApiClientId = "D365ApiClientId";
        internal const string D365SettsApiClientSecret = "D365ApiClientSecret";

        // Azure Key Vault Secrets
        internal const string AzureKeyVaultSecrectDefraCmS2SAppid = "defra-cm-s2s-appid";
        internal const string AzureKeyVaultSecrectDefraCmS2SSecret = "defra-cm-s2s-secret";

        // Messages
        internal const string MessageD365KeyPrefixCannotBeNull = "The setting D365KeyPrefix cannot be empty or null.";
        internal const string MessageD365ResourceUrlCannotBeNull = "The setting D365ApiBaseAddress cannot be empty or null.";
        internal const string MessageUseKeyVaultCannotBeNull = "The setting UseKeyVault cannot be empty or null.";
        internal const string MessageD365ClientIdCannotBeNull = "The setting D365ApiClientId cannot be empty or null when Key Vault is in action.";
        internal const string MessageD365ClientSecretCannotBeNull = "The setting D365ApiClientSecret cannot be empty or null when Key Vault is in action.";
        internal const string MessageAppInsightsCustomerRoleNameBeNull = "The setting AppInsightsCustomerRoleName cannot empty or null.";
        internal const string MessageKeyVaultNameCannotBeNull = "The Key Vault name setting cannot empty or null.";
        internal const string MessageKeyVaultClientIdCannotBeNull = "The Key Vault client id setting cannot empty or null.";
        internal const string MessageKeyVaultClientSecretCannotBeNull = "The setting The Key Vault client secret cannot empty or null.";
        internal const string MessageKeyVaultD365ClientIdCannotBeNull = "The Key Vault D365 client id setting with this prefix cannot empty or null.";
        internal const string MessageKeyVaultDefraCmS2SCountCannotBeNull = "The Key Vault D365 S2S Count setting with this prefix cannot empty or null.";
        internal const string MessageKeyVaultD365ClientSecretCannotBeNull = "The Key Vault D365 client secret setting with this prefix cannot empty or null.";
        internal const string MessageKeyVaultClientCertificateNotFound = "The Key Vault Client Certificate setting with this prefix cannot empty or null, please configure the Client Certificate appropriately.";
        internal const string MessageKeyVaultClientCertificateCannotBeNull = "The Key Vault Client Certificate setting with 'VaultKeyCertValSetsClientCertificate' cannot empty or null, please configure correctly.";
        internal const string MessageCertificateNoCertificatePassed = "Please pass a valid Certificate.";
        internal const string MessageCertificateCertificateDatesInvalid = "Please make sure the Client Certificate is valid, the dates are not valid.";
        internal const string MessageCertificateCertificateSubjectsInvalid = "Please make sure the Client Certificate is valid, the Subjects do not match.";
        internal const string MessageCertificateCertificateIssuerInvalid = "Please make sure the Client Certificate is valid, the Issuers do not match.";
        internal const string MessageCertificateCertificateThumbprintInvalid = "Please make sure the Client Certificate is valid, the Thumbprint do not match.";
        internal const string MessageCertificateCertificateInvalidTrustedChainInvalid = "Please make sure the Client Certificate is valid, the certificate chains to a Non-Trusted Root Authority.";
        internal const string MessageCertificateCertificateRawDataInvalid = "Please make sure the Client Certificate is valid, the Raw Data do not match.";

        // Certificate
        internal const string CertHeaderXARRClientCert = "X-ARR-ClientCert";
        internal const string CertValidationSettsFileName = "certValidationSettings";
        internal const string CertValidationSetts = "CertValidationSettings";
        internal const string CertValidationVaultKeyCertValSetsClientCertificate = "VaultKeyCertValSetsClientCertificate";
    }
}
