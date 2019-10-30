// <copyright file="ContactController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Defra.CustMaster.Identity.CoreApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using Defra.CustMaster.Identity.CoreApp.Dynamics;
    using Defra.CustMaster.Identity.CoreApp.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [Produces("application/json")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed.")]
    public class ContactController : Controller
    {
        public ContactController(ICrmApiWrapper crmApiWrapper)
        {
            CrmWrapper = crmApiWrapper;
        }

        private ICrmApiWrapper CrmWrapper { get; }

        [HttpGet]
        [Route("InitialMatch")]
        public object InitialMatch([FromQuery] string b2cobjectid)
        {
            try
            {
                // Client Mutual Authentication
                string authenicationResult = AuthenticateCertificate();

                // If there are error for Client Certificate checks return 403
                if (!string.IsNullOrEmpty(authenicationResult))
                {
                    return BadRequest(new ServiceObject
                    {
                        ErrorCode = (int)HttpStatusCode.Forbidden,
                        ErrorMsg = $"Access Denied - {authenicationResult}"
                    });
                }

                if (!string.IsNullOrEmpty(b2cobjectid))
                {
                    if (Guid.TryParse(b2cobjectid, out Guid b2cobjectIdGuid))
                    {
                        // Make Initial Match request
                        return Ok(CrmWrapper.InitialMatch(b2cobjectIdGuid));
                    }

                    return BadRequest(new ServiceObject { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorMsg = "B2CObjectid is invalid, is not a valid Guid." });
                }

                return BadRequest(new ServiceObject { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorMsg = "B2CObjectid is invalid" });
            }
            catch (WebFaultException ex)
            {
                return BadRequest(new ServiceObject { ErrorCode = ex.HttpStatusCode, ErrorMsg = ex.ErrorMsg });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceObject { ErrorCode = (int)HttpStatusCode.InternalServerError, ErrorMsg = ex.Message });
            }
        }

        [HttpGet]
        [Route("Authz")]
        public AuthzResponse Authz([FromQuery] string serviceid, [FromQuery]string b2cobjectid)
        {
            List<string> rolesList = new List<string>();
            List<string> mappingsList = new List<string>();
            try
            {
                // Client Mutual Authentication
                string authenicationResult = AuthenticateCertificate();

                // If there are error for Client Certificate checks return 403
                if (!string.IsNullOrEmpty(authenicationResult))
                {
                    return new AuthzResponse
                    {
                        status = (int)HttpStatusCode.Forbidden,
                        message = $"Access Denied - {authenicationResult}",
                        version = ApiVersion.Default.ToString(),
                        roles = rolesList,
                        mappings = mappingsList,
                    };
                }

                // B2C Object ID validation
                if (string.IsNullOrEmpty(b2cobjectid)
                    || !Guid.TryParse(b2cobjectid, out Guid b2cobjectIdGuid))
                {
                    return new AuthzResponse
                    {
                        status = (int)HttpStatusCode.BadRequest,
                        message = "B2CObjectId is invalid, is not a valid Guid.",
                        version = ApiVersion.Default.ToString(),
                        roles = rolesList,
                        mappings = mappingsList,
                    };
                }

                // Service ID validation
                if (string.IsNullOrEmpty(serviceid)
                    || !Guid.TryParse(serviceid, out Guid serviceIdGuid))
                {
                    return new AuthzResponse
                    {
                        status = (int)HttpStatusCode.BadRequest,
                        message = "ServiceId is invalid, is not a valid Guid.",
                        version = ApiVersion.Default.ToString(),
                        roles = rolesList,
                        mappings = mappingsList,
                    };
                }

                // Make Authorisation request
                Enrolments enrolments = CrmWrapper.Authz(serviceIdGuid, b2cobjectIdGuid);

                // If no Enrolments return No Content
                if (enrolments?.ServiceUserLinks?.Count == 0)
                {
                    return new AuthzResponse
                    {
                        status = (int)HttpStatusCode.NoContent,
                        message = "No Content",
                        version = ApiVersion.Default.ToString(),
                        roles = rolesList,
                        mappings = mappingsList,
                    };
                }

                // Compile return JSON, used for loop because of performance
                for (int i = 0; i < enrolments.ServiceUserLinks.Count; i++)
                {
                    ServiceUserLink serviceUserLink = enrolments.ServiceUserLinks[i];
                    string roleListItem = $"{serviceUserLink.OrganisationId}:{serviceUserLink.RoleId}:{serviceUserLink.EnrolmentStatus}";
                    if (!rolesList.Contains(roleListItem))
                    {
                        rolesList.Add(roleListItem);
                    }

                    string mappingListOrgItem = $"{serviceUserLink.OrganisationId}:{serviceUserLink.OrganisationName}";
                    if (!mappingsList.Contains(mappingListOrgItem))
                    {
                        mappingsList.Add(mappingListOrgItem);
                    }

                    string mappingListRoleItem = $"{serviceUserLink.RoleId}:{serviceUserLink.RoleName}";
                    if (!mappingsList.Contains(mappingListRoleItem))
                    {
                        mappingsList.Add(mappingListRoleItem);
                    }

                    string mappingListStatus = $"{serviceUserLink.EnrolmentStatus}:{serviceUserLink.EnrolmentStatusText}";
                    if (!mappingsList.Contains(mappingListStatus))
                    {
                        mappingsList.Add(mappingListStatus);
                    }
                }
            }
            catch (WebFaultException ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                return new AuthzResponse
                {
                    status = ex.HttpStatusCode,
                    message = ex.ErrorMsg + " " + ex.InnerException?.Message,
                    version = ApiVersion.Default.ToString(),
                    roles = rolesList,
                    mappings = mappingsList
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                return new AuthzResponse
                {
                    status = (int)HttpStatusCode.InternalServerError,
                    message = ex.Message,
                    version = ApiVersion.Default.ToString(),
                    roles = rolesList,
                    mappings = mappingsList,
                };
            }

            return new AuthzResponse
            {
                status = (int)HttpStatusCode.OK,
                message = "Success!",
                version = ApiVersion.Default.ToString(),
                roles = rolesList,
                mappings = mappingsList,
            };
        }

        private string AuthenticateCertificate()
        {
            try
            {
                IHeaderDictionary headers = Request.Headers;
                string certHeader = headers[CommonConstants.CertHeaderXARRClientCert];

                if (string.IsNullOrEmpty(certHeader))
                {
                    return "No client certificate was provided, please pass a valid Client Certificate with the request.";
                }

                byte[] clientCertBytes = Convert.FromBase64String(certHeader);
                var certificate = new X509Certificate2(clientCertBytes);

                string certValidationResult = CrmWrapper.ValidateClientCertificate(certificate);

                if (!string.IsNullOrEmpty(certValidationResult))
                {
                    return certValidationResult;
                }
            }
            catch (Exception ex)
            {
                return $"Error while validating the Client Certificate. Exception Message: {ex.Message}, Exception Details: {ex}";
            }

            return string.Empty;
        }
    }
}