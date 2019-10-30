// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Telemetry
{
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.Extensibility;

    /*
     * Custom TelemetryInitializer that overrides the default SDK
     * behavior of treating response codes >= 400 as failed requests
     *
     */
    public class CustomerTelemetryInitialiser : ITelemetryInitializer
    {
        internal string RoleName { get; set; }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = RoleName;
        }
    }
}
