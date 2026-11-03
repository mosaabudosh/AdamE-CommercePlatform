


using ArabazonPlatform.Generic.Enum;

namespace ArabazonPlatform.Domain.IHttpServices
{
    public interface IHttpAccessor
    {
        public int GetClientTimeZoneOffset();
        LanguageCode GetCurrentUserLanguage();
    }
}
