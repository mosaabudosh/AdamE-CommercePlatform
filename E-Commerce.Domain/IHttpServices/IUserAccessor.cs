

using Adam.Generic.Enum;

namespace E_Commerce.Domain.IHttpServices
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
