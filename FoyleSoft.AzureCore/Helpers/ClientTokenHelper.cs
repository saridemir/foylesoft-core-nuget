using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.AzureCore.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Helpers
{
    public class ClientTokenHelper
    {
        private readonly IAzureConfigurationService _configurationService;
        public ClientTokenHelper(IAzureConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public IBaseResponse<string> GenerateFormJWTTokenByClaims(Guid licenseGuid, string jsonString,string oid="")
        {
            try
            {
                var securityKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationService.ClientConfig.ClientSecretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var userRoles = _configurationService.ClientConfig.ClientRoleMappings.Split(',');

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, licenseGuid.ToString()),
                    new Claim("ClassJson", jsonString),
                    new Claim("UserId", "0"),
                    new Claim("UserName", "contact"),
                    new Claim("License", "0"),
                    new Claim("oid", oid),                    
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                claims.AddRange(userRoles.Select(n => new Claim(ClaimTypes.Role, n)));

                var token = new JwtSecurityToken(
                    issuer: _configurationService.ClientConfig.ClientValidIssuer,
                    audience: _configurationService.ClientConfig.ClientValidAudience,
                    claims: claims,
                    expires: DateTime.Now.AddYears(1),
                    signingCredentials: credentials
                );

                var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

                return new BaseResponse<string> { IsSuccess = true, Data = generatedToken };
            }
            catch (Exception ex)
            {
                return new BaseResponse<string> { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
        public IBaseResponse<string> GetClassJson(string jwtToken)
        {
            string classJson = string.Empty;
            var claimsPrincipalResponse = ValidateClientTokenInfo(jwtToken);
            bool isValid = claimsPrincipalResponse.IsSuccess;
            if (claimsPrincipalResponse.IsSuccess)
            {
                if (claimsPrincipalResponse.Data.HasClaim(n => n.Type == "ClassJson"))
                    classJson = claimsPrincipalResponse.Data.FindFirst("ClassJson").Value;
            }
            return new BaseResponse<string> { IsSuccess = !string.IsNullOrEmpty(classJson) && isValid, Data = classJson , ErrorMessage= claimsPrincipalResponse .ErrorMessage};
        }
        public IBaseResponse<ClaimsPrincipal> ValidateTokenInfo(string jwtToken)
        {
            try
            {
                SecurityToken validatedToken;
                TokenValidationParameters validationParameters = new TokenValidationParameters();

                validationParameters.ValidateLifetime = false;

                validationParameters.ValidAudience = _configurationService.ClientConfig.ClientValidAudience;
                validationParameters.ValidIssuer = _configurationService.ClientConfig.ClientValidIssuer;
                validationParameters.IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationService.ClientConfig.ClientSecretKey));

                ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

                return new BaseResponse<ClaimsPrincipal> { IsSuccess = true, Data = principal };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsPrincipal> { IsSuccess = false, ErrorMessage = "Validation Error" };
            }
        }

        public IBaseResponse<ClaimsPrincipal> ValidateClientTokenInfo(string jwtToken)
        {
            try
            {
                SecurityToken validatedToken;
                TokenValidationParameters validationParameters = new TokenValidationParameters();

                validationParameters.ValidateLifetime = false;

                validationParameters.ValidAudience = _configurationService.ClientConfig.ClientValidAudience;
                validationParameters.ValidIssuer = _configurationService.ClientConfig.ClientValidIssuer;
                validationParameters.IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationService.ClientConfig.ClientSecretKey));

                ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

                return new BaseResponse<ClaimsPrincipal> { IsSuccess = true, Data = principal };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsPrincipal> { IsSuccess = false, ErrorMessage = "Validation Error" };
            }
        }
    }
}
