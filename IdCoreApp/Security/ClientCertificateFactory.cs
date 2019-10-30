// <copyright file="ClientCertificateFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Security
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Cryptography.X509Certificates;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed.")]
    public class ClientCertificateFactory : IClientCertificateFactory
    {
        private readonly X509Certificate2 configuredClientCertificate;
        private const string MessageIsValidCertificateFunction = "Function IsValidClientCertificate, Error = ";

        public ClientCertificateFactory(X509Certificate2 configuredClientCert) => configuredClientCertificate = configuredClientCert;

        public string ValidateClientCertificate(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                return $"{MessageIsValidCertificateFunction}{CommonConstants.MessageCertificateNoCertificatePassed}";
            }

            // 1. Check time validity of certificate
            if (DateTime.Compare(DateTime.Now, certificate.NotBefore) < 0
                || DateTime.Compare(DateTime.Now, certificate.NotAfter) > 0)
            {
                return $"{MessageIsValidCertificateFunction}{CommonConstants.MessageCertificateCertificateDatesInvalid}";
            }

            // 2. Check subject name of certificate
            if (!configuredClientCertificate.Subject.Equals(value: certificate.Subject, comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                return $"{MessageIsValidCertificateFunction}{CommonConstants.MessageCertificateCertificateSubjectsInvalid}";
            }

            // 3. Check issuer name of certificate
            if (!configuredClientCertificate.Issuer.Equals(value: certificate.Issuer, comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                return $"{MessageIsValidCertificateFunction}{CommonConstants.MessageCertificateCertificateSubjectsInvalid}";
            }

            // 4. Check thumbprint of certificate
            if (!configuredClientCertificate.Thumbprint.Equals(value: certificate.Thumbprint, comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                return $"{MessageIsValidCertificateFunction}{CommonConstants.MessageCertificateCertificateThumbprintInvalid}";
            }

            /*
            // If you also want to test if the certificate chains to a Trusted Root Authority
            X509Chain certChain = new X509Chain();
            certChain.Build(certificate);
            bool isValidCertChain = true;
            foreach (X509ChainElement chElement in certChain.ChainElements)
            {
                if (!chElement.Certificate.Verify())
                {
                    isValidCertChain = false;
                    break;
                }
            }            

            if (!isValidCertChain)
            {
                return $"{MessageIsValidCertificateFunction}{CommonConstants.MessageCertificateCertificateInvalidTrustedChainInvalid}";
            }
            */

            // 5. Check RawData of certificate
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(x: configuredClientCertificate.RawData, y: certificate.RawData))
            {
                return $"{MessageIsValidCertificateFunction}{CommonConstants.MessageCertificateCertificateRawDataInvalid}";
            }

            return string.Empty;
        }
    }
}
