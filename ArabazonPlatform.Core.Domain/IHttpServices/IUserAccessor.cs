

using ArabazonPlatform.Generic.Enum;

namespace ArabazonPlatform.Domain.IHttpServices
{
    public interface IUserAccessor
    {
        string GetCurrentUserName();
        Guid? GetCurrentUserId();
        string GetAccessToken();
        int GetClientTimeZoneOffset();
        LanguageCode GetCurrentUserLanguage();
    }
}
