

using ArabazonPlatform.Generic.Enum;
using ArabazonPlatform.Domain.IHttpServices;
using Microsoft.AspNetCore.Http;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Infrastructure.HttpServices
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
                case ArabazonPlatform.Generic.Constants.Constants.Languages.LanguageCodeEn:
                    return LanguageCode.enUS;
                case ArabazonPlatform.Generic.Constants.Constants.Languages.LanguageCodeAr:
                    return LanguageCode.arJO;
                default:
                    return LanguageCode.enUS;
            }
        }
    }
}
