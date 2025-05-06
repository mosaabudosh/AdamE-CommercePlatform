


using Adam.Generic.Enum;

namespace E_Commerce.Domain.IHttpServices
{
    public interface IHttpAccessor
    {
        public int GetClientTimeZoneOffset();
        LanguageCode GetCurrentUserLanguage();
    }
}
