namespace ArabazonPlatform.Application.Model.AppSettings
{
    public class AppSettings
    {

        public Logging logging { get; set; }
        public Security security { get; set; }
        public TokenOptions tokenOptions { get; set; }
        public ConnectionStrings connectionStrings { get; set; }
        public string AllowedHosts { get; set; }
        public AdamLogging adamLogging { get; set; }
        public string origins { get; set; }


        public class Logging
        {
            public LogLevel LogLevel { get; set; }
        }

        public class LogLevel
        {
            public string Default { get; set; }
        }

        public class Security
        {
            public string Secret { get; set; }
        }

        public class TokenOptions
        {
            public string Audience { get; set; }
            public string Issuer { get; set; }
            public string AccessTokenExpiration { get; set; }
            public string RefreshTokenExpiration { get; set; }
        }

        public class ConnectionStrings
        {
            public string DefaultConnection { get; set; }
        }


        public class AdamLogging
        {
            public bool EnableLogging { get; set; }
            public bool EnableLogHttpResponse { get; set; }
            public int HttpResponseLength { get; set; }
            public bool EnableTracingLogging { get; set; }
            public int MinTracingTime { get; set; }
            public string LoggingProvider { get; set; }
            public string ConnectionString { get; set; }
            public bool EnableInformationLogging { get; set; }
            public bool EnableExceptionLogging { get; set; }
        }

    }
}
