using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adam.Generic.Constants
{
    public static class Constants
    {
        public static class ErrorCode
        {
            public const string Duplicate = "Duplicate";
            public const string RelatedData = "RelatedData";
            public const string AlreadyExsist = "AlreadyExsist";
            public const string InvalidUserNameOrPassword = "InvalidUserNameOrPassword";
            public const string InvalidRefreshToken = "InvalidRefreshToken";
            public const string InvalidUser = "InvalidUser";
        }

        public static class Languages
        {
            public const string LanguageCodeAr = "ar-JO";
            public const string LanguageCodeEn = "en-US";
            public const string UserLanguage = "UserLanguage";
            public const string LanguageCode = "LanguageCode";
        }
        public static class PlatformTheme
        {
            public const string DarkMode = "dark-mode";
            public const string NormalMode = "normal-mode";
        }
    }
}
