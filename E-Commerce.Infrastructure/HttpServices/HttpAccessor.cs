

using Adam.Generic.Enum;
using E_Commerce.Domain.IHttpServices;
using Microsoft.AspNetCore.Http;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.HttpServices
{
    public class HttpAccessor : IHttpAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
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
                case Adam.Generic.Constants.Constants.Languages.LanguageCodeEn:
                    return LanguageCode.enUS;
                case Adam.Generic.Constants.Constants.Languages.LanguageCodeAr:
                    return LanguageCode.arJO;
                default:
                    return LanguageCode.enUS;
            }
        }
    }
}
