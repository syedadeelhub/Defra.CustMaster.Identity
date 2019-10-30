// <copyright file="Startup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using Defra.CustMaster.Identity.CoreApp.Dynamics;
    using Defra.CustMaster.Identity.CoreApp.KeyVault;
    using Defra.CustMaster.Identity.CoreApp.Security;
    using Defra.CustMaster.Identity.CoreApp.Telemetry;
    using Microsoft.ApplicationInsights.AspNetCore;
    using Microsoft.ApplicationInsights.AspNetCore.Logging;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.SnapshotCollector;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed.")]
    public class Startup
    {
        public Startup(IConfiguration config, IHostingEnvironment env, ILoggerFactory logFactory)
        {
            D365ClientConfigDictionary = new Dictionary<int, D365ClientCredential>();
            Environment = env ?? throw new System.ArgumentNullException(nameof(env));
            LoggerFactory = logFactory ?? throw new System.ArgumentNullException(nameof(logFactory));
            Configuration = config ?? throw new System.ArgumentNullException(nameof(config));

            var configBuilder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile($"{CommonConstants.AppSettsFileName}.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"{CommonConstants.AzureKeyVaultSettsFileName}.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"{CommonConstants.D365SettsFileName}.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();

            config = configBuilder.Build();

            // Valid the application settings
            ValidateApplicationConfiguration(ref config, configBuilder);

            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        public ILoggerFactory LoggerFactory { get; }

        public bool UseKeyVault { get; private set; }

        public Uri D365ResourceUrl { get; private set; }

        public IDictionary<int, D365ClientCredential> D365ClientConfigDictionary { get; }

        public string D365ClientId { get; private set; }

        public string D365ClientSecret { get; private set; }

        public X509Certificate2 ClientCertificate { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure SnapshotCollector from application settings
            services.Configure<SnapshotCollectorConfiguration>(
                config: Configuration.GetSection(nameof(SnapshotCollectorConfiguration)));

            if (UseKeyVault)
            {
                // Configure AzureKeyVaultSetting from application settings
                services.Configure<AzureKeyVaultSettings>(
                    config: Configuration.GetSection(nameof(AzureKeyVaultSettings)));
            }

            // Configure D365Setting from application settings
            services.Configure<D365Settings>(
                config: Configuration.GetSection(nameof(D365Settings)));

            // Add SnapshotCollector telemetry processor.
            services.AddSingleton<ITelemetryProcessorFactory>(sp => new SnapshotCollectorTelemetryProcessorFactory(sp));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddApiVersioning(
                setupAction: sact =>
                {
                    sact.AssumeDefaultVersionWhenUnspecified = true;
                    sact.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 1);
                });

            services.AddApplicationInsightsTelemetry();

            // Include EventId in logs
            services.AddOptions<ApplicationInsightsLoggerOptions>().Bind(
                config: Configuration.GetSection(CommonConstants.AppSettsApplicationInsightsLogger));

            // Singleton Services
            // Use this if MyCustomTelemetryInitializer constructor has parameters which are DIed.
            services.AddSingleton<ITelemetryInitializer>(new CustomerTelemetryInitialiser()
            {
                RoleName = Configuration[$"{CommonConstants.AppSetts}:{CommonConstants.AppSettsAppInsightsCustomerRoleName}"],
            });

            services.AddSingleton(Configuration);

            ClientCertificateFactory clientCertificateFactory = new ClientCertificateFactory(ClientCertificate);

            TokenFactory tokenFactory = new TokenFactory(
                                                    resource: D365ResourceUrl,
                                                    clientConfigDictionary: D365ClientConfigDictionary,
                                                    clientCertificateFactory: clientCertificateFactory);

            services.AddSingleton<ITokenFactory>(tokenFactory);

            // Scoped Services
            services.AddScoped<ICrmApiWrapper, D365ApiWrapper>();

            // Clients
            services.AddHttpClient<IClientFactory, D365ClientFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("Index.html");
            app.UseDefaultFiles(options);
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void ValidateApplicationConfiguration(ref IConfiguration config, IConfigurationBuilder configBuilder)
        {
            // Dynamics Resource URL
            string d365ResourceUrl = config[$"{CommonConstants.D36Setts}:{CommonConstants.D365SettsApiBaseAddress}"];
            if (string.IsNullOrEmpty(d365ResourceUrl))
            {
                throw new NullReferenceException(
                    message: CommonConstants.MessageD365ResourceUrlCannotBeNull);
            }

            D365ResourceUrl = new Uri(d365ResourceUrl);

            // Dynamics Key Prefix
            string d365KeyPrefix = config[$"{CommonConstants.D36Setts}:{CommonConstants.D36SettsKeyPrefix}"];
            if (string.IsNullOrEmpty(d365KeyPrefix))
            {
                throw new NullReferenceException(
                    message: CommonConstants.MessageD365KeyPrefixCannotBeNull);
            }

            // User KeyVault
            string useKeyVault = config[$"{CommonConstants.AzureKeyVaultSetts}:{CommonConstants.AzureKeyVaultSettsUseKeyVault}"];
            if (string.IsNullOrEmpty(useKeyVault))
            {
                throw new NullReferenceException(
                    message: CommonConstants.MessageUseKeyVaultCannotBeNull);
            }

            if (useKeyVault.Equals(value: bool.TrueString, comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                UseKeyVault = true;

                LoadAkvConfig(ref config, ref configBuilder);

                GetD365ClientDataFromLocalConfig(config);

                string settingS2SCount = config[$"{CommonConstants.AzureKeyVaultSetts}:{CommonConstants.AzureKeyVaultSettsDefraCmS2SCount}"];
                if (string.IsNullOrEmpty(settingS2SCount))
                {
                    throw new NullReferenceException(
                        message: CommonConstants.MessageKeyVaultDefraCmS2SCountCannotBeNull);
                }

                if (!int.TryParse(
                                config[$"{CommonConstants.D365SettsFileName}:{CommonConstants.D36Setts}:{settingS2SCount}"],
                                out int s2SCount)
                    && s2SCount <= 0)
                {
                    throw new NullReferenceException(
                        message: $"Please provide a valid value (number > 0) for the {CommonConstants.AzureKeyVaultSettsDefraCmS2SCount}.");
                }

                for (int iloop = 1; iloop <= s2SCount; iloop++)
                {
                    string clientId = config[$"{CommonConstants.D365SettsFileName}:{CommonConstants.D36Setts}:{D365ClientId}-{iloop}"];
                    if (string.IsNullOrEmpty(value: clientId))
                    {
                        throw new NullReferenceException(
                            message: $"{CommonConstants.MessageKeyVaultD365ClientIdCannotBeNull} for index = {iloop}");
                    }

                    string secret = config[$"{CommonConstants.D365SettsFileName}:{CommonConstants.D36Setts}:{D365ClientSecret}-{iloop}"];
                    if (string.IsNullOrEmpty(value: secret))
                    {
                        throw new NullReferenceException(
                            message: $"{CommonConstants.MessageKeyVaultD365ClientSecretCannotBeNull} for index = {iloop}");
                    }

                    D365ClientConfigDictionary.Add(iloop, new D365ClientCredential(clientId, secret));
                }

                // Certificate Key name
                string clientCertKeyName = config[$"{CommonConstants.AzureKeyVaultSetts}:{CommonConstants.CertValidationVaultKeyCertValSetsClientCertificate}"];
                if (string.IsNullOrEmpty(value: clientCertKeyName))
                {
                    throw new NullReferenceException(
                        message: CommonConstants.MessageKeyVaultClientCertificateCannotBeNull);
                }

                // Check the Azure Key Vault Certificates
                string clientCertificateRaw = config[$"{CommonConstants.CertValidationSettsFileName}:{CommonConstants.CertValidationSetts}:{clientCertKeyName}"];
                if (string.IsNullOrEmpty(value: clientCertificateRaw))
                {
                    throw new NullReferenceException(
                        message: CommonConstants.MessageKeyVaultClientCertificateNotFound);
                }

                try
                {
                    string passwordKey = config[$"{CommonConstants.AzureKeyVaultSetts}:{CommonConstants.AzureKeyVaultSettsKeyDefraCmCertPwd}"];
                    string password = config[$"{CommonConstants.D365SettsFileName}:{CommonConstants.D36Setts}:{passwordKey}"];

                    byte[] privateKeyBytes = Convert.FromBase64String(clientCertificateRaw);
                    ClientCertificate = new X509Certificate2(
                        rawData: privateKeyBytes,
                        password: password.Equals(value: passwordKey, comparisonType: StringComparison.OrdinalIgnoreCase) ? string.Empty : password,
                        keyStorageFlags: X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                                    message: "There was an error loading the Configured Client Certificate.",
                                    innerException: ex);
                }
            }
            else
            {
                UseKeyVault = false;

                // Dynamics client configuration
                GetD365ClientDataFromLocalConfig(config);
            }

            // Application Insights, Role Name
            if (string.IsNullOrEmpty(value: Configuration[$"{CommonConstants.AppSetts}:{CommonConstants.AppSettsAppInsightsCustomerRoleName}"]))
            {
                throw new NullReferenceException(
                    message: CommonConstants.MessageAppInsightsCustomerRoleNameBeNull);
            }
        }

        private void LoadAkvConfig(
            ref IConfiguration config,
            ref IConfigurationBuilder configBuilder)
        {
            string akvName = config[$"{CommonConstants.AzureKeyVaultSetts}:{CommonConstants.AzureKeyVaultSettsVaultName}"];
            if (string.IsNullOrEmpty(akvName))
            {
                throw new NullReferenceException(
                    message: CommonConstants.MessageKeyVaultNameCannotBeNull);
            }

            string akvClientId = config[$"{CommonConstants.AzureKeyVaultSetts}:{CommonConstants.AzureKeyVaultSettsVaultClientId}"];
            if (string.IsNullOrEmpty(akvClientId))
            {
                throw new NullReferenceException(
                    message: CommonConstants.MessageKeyVaultClientIdCannotBeNull);
            }

            string akvClientSecret = config[$"{CommonConstants.AzureKeyVaultSetts}:{CommonConstants.AzureKeyVaultSettsVaultSecret}"];
            if (string.IsNullOrEmpty(akvClientSecret))
            {
                throw new NullReferenceException(
                    message: CommonConstants.MessageKeyVaultClientSecretCannotBeNull);
            }

            // Add the Azure Key Vault
            configBuilder.AddAzureKeyVault(vault: $"https://{akvName}.vault.azure.net/", clientId: akvClientId, clientSecret: akvClientSecret);

            // Build the configuration
            config = configBuilder.Build();
        }

        private void GetD365ClientDataFromLocalConfig(IConfiguration config)
        {
            D365ClientId = config[$"{CommonConstants.D36Setts}:{CommonConstants.D365SettsApiClientId}"];
            if (string.IsNullOrEmpty(value: D365ClientId))
            {
                throw new NullReferenceException(
                    message: CommonConstants.MessageD365ClientIdCannotBeNull);
            }

            // Dynamics Client Secret
            D365ClientSecret = config[$"{CommonConstants.D36Setts}:{CommonConstants.D365SettsApiClientSecret}"];
            if (string.IsNullOrEmpty(value: D365ClientSecret))
            {
                throw new NullReferenceException(
                    message: CommonConstants.MessageD365ClientSecretCannotBeNull);
            }
        }
    }
}
