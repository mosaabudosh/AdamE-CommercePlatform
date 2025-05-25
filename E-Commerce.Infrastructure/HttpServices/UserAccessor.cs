using Adam.Generic.Constants;
using Adam.Generic.Enum;
using E_Commerce.Domain.IHttpServices;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static Adam.Generic.Constants.Constants;

namespace E_Commerce.Infrastructure.HttpServices
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUserName()
        {
            string userName = _httpContextAccessor.HttpContext.User?.Claims?.LastOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            return userName;
        }
        public Guid? GetCurrentUserId()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                string userId = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return null;
                }
                return new Guid(userId);
            }
            return null;
        }
        public string GetAccessToken()
        {
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                return token.ToString().Replace("Bearer ", string.Empty);
            }
            return string.Empty;
        }
        public int GetClientTimeZoneOffset()
        {
            string timezoneOffset = _httpContextAccessor.HttpContext?.Request?.Headers?["Timezone-Offset"];
            if (string.IsNullOrEmpty(timezoneOffset))
            {
                return 0;
            }
            return Convert.ToInt32(timezoneOffset);
        }
        public LanguageCode GetCurrentUserLanguage()
        {
            string languageCode = _httpContextAccessor.HttpContext?.Request?.Headers?["Accept-Language"];

            switch (languageCode)
            {
                case null:
                case "":
                case Languages.LanguageCodeEn:
                    return LanguageCode.enUS;
                case Languages.LanguageCodeAr:
                    return LanguageCode.arJO;
                default:
                    return LanguageCode.enUS;
            }
        }

    }
}
