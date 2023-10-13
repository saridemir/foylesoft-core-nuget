using FoyleSoft.AzureCore.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    public class AzureADJwtBearerValidation : LoggerBaseService<IAzureADJwtBearerValidation>, IAzureADJwtBearerValidation
    {
        private readonly string _wellKnownEndpoint;
        private readonly IAzureConfigurationService _azureConfigurationService;
        
        public AzureADJwtBearerValidation(IAzureConfigurationService azureConfigurationService, ILogger<IAzureADJwtBearerValidation> log)
        :base(log)
        {
            _azureConfigurationService = azureConfigurationService;
            _wellKnownEndpoint = $"{_azureConfigurationService.AzureConfig.Instance}/{_azureConfigurationService.AzureConfig.TenantId}/v2.0/.well-known/openid-configuration?p={_azureConfigurationService.AzureConfig.SignInPolicyId}";
        }

        public Task<ClaimsPrincipal> ValidateTokenAsync(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return Task.FromResult<ClaimsPrincipal>(null);
            }
            //var accessToken = authorizationHeader.StartsWith("Bearer ")? authorizationHeader.Substring("Bearer ".Length): authorizationHeader;
            try
            {
                var oidcWellknownEndpoints = GetOIDCWellknownConfiguration();
                var tokenValidator = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidAudience = _azureConfigurationService.AzureConfig.ClientIdB2C,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    IssuerSigningKeys = oidcWellknownEndpoints.Result.SigningKeys,
                    ValidIssuer = oidcWellknownEndpoints.Result.Issuer
                };
                SecurityToken securityToken;
                var _claimsPrincipal = tokenValidator.ValidateToken(authorizationHeader, validationParameters, out securityToken);
                return Task.FromResult<ClaimsPrincipal>(_claimsPrincipal);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
            return Task.FromResult<ClaimsPrincipal>(null);
        }
        private async Task<OpenIdConnectConfiguration> GetOIDCWellknownConfiguration()
        {
            Logger.LogDebug($"Get OIDC well known endpoints {_wellKnownEndpoint}");
            var _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(_wellKnownEndpoint, new OpenIdConnectConfigurationRetriever());
            try
            {
                return await _configurationManager.GetConfigurationAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
