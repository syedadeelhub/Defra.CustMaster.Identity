// <copyright file="CustomerTelemetryInitialiser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Model
{
    using System;

    public class WebFaultException : Exception
    {
        public string ErrorMsg;

        public int HttpStatusCode;

        public WebFaultException(string errorMsg, int httpStatusCode)
        {
            ErrorMsg = errorMsg;
            HttpStatusCode = httpStatusCode;
        }
    }
}