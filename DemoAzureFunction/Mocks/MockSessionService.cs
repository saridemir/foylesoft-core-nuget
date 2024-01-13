using FoyleSoft.AzureCore.Interfaces;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace DemoAzureFunction.Mocks
{
    public class MockSessionService : IMockSessionService
    {
        private readonly IHttpContextAccessor _context;
        private readonly int currentUserId;
        private readonly int currentLicenseId;
        private readonly int currentProjectId;
        private readonly string currentName;
        private readonly string currentSurname;
        private readonly string currentImageUrl;
        private readonly string currentUserGuid;

        private readonly string token;
        private readonly IAzureConfigurationService _configurationService;
        public MockSessionService(IHttpContextAccessor context, MockContext MockContext, IAzureConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _context = context;
            currentUserId = 0;
            currentLicenseId = 0;
            currentProjectId = 0;
            currentName = string.Empty;
            currentSurname = string.Empty;
            currentImageUrl = string.Empty;
            token = string.Empty;



            if (context?.HttpContext != null)
            {
                var authHeader = context.HttpContext.Request.Headers.FirstOrDefault(c => c.Key == "Authorization");

                var bearerHeader = authHeader.Value.FirstOrDefault(h => h.StartsWith("Bearer "));
                if (bearerHeader != null)
                {
                    while (bearerHeader.StartsWith("Bearer "))
                    {
                        bearerHeader = bearerHeader.Substring(7);
                    }
                    token = bearerHeader;
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                if (!string.IsNullOrEmpty(token))
                {
                    var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                    currentUserGuid = securityToken.Claims.FirstOrDefault(c => c.Type == "oid")?.Value;

                    if (currentUserGuid != null)
                    {
                        var user = MockContext.Set<Tables.User>().FirstOrDefault(f => f.UserGuid == currentUserGuid);
                        if (user != null)
                        {
                            currentUserId = user.Id;
                            currentLicenseId = user.EvideLicenseId;
                            currentProjectId = user.EvideProjectId;
                            currentSurname = user.Surname;
                            currentName = user.Name;
                            currentImageUrl = user.ImageUrl;
                        }
                        else
                        {
                            currentUserId = 0;
                            currentLicenseId = 0;
                            currentProjectId = 0;
                            currentSurname = "";
                            currentName = "";
                            currentImageUrl = "";
                        }
                    }
                    else
                    {
                        currentUserId = 0;
                        currentLicenseId = 0;
                        currentProjectId = 0;
                        currentSurname = "";
                        currentName = "";
                        currentImageUrl = "";
                    }
                }
            }
        }
        public DateTimeOffset GetExpiryTimestamp()
        {
            try
            {
                var jwtSecurityToken = new JwtSecurityToken(token);
                return jwtSecurityToken.ValidTo.ToUniversalTime();
            }
            catch { return DateTimeOffset.MaxValue; }
        }
        public int CurrentUserId => currentUserId;
        public int CurrentLicenseId => currentLicenseId;

        public int CurrentProjectId => currentProjectId;
        public string CurrentName => currentName;
        public string CurrentSurname => currentSurname;
        public string CurrentImageUrl => currentImageUrl;
        public string CurrentUserGuid => currentUserGuid;

        public string Token => token;
        //public string ClassJson => classJson;
    }
}
