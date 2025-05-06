using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace E_Commerce.API.Middleware
{
    public class CustomQueryStringParser : QueryStringValueProvider
    {
        int timeZoneOffset = 0;

        public CustomQueryStringParser(HttpRequest request) : base(BindingSource.Query, request.Query, CultureInfo.InvariantCulture)
        {
            if (!string.IsNullOrEmpty(request.Headers["TimeZone-Offset"]))
            {
                timeZoneOffset = int.Parse(request.Headers["TimeZone-Offset"][0]);
            }
        }

        public override ValueProviderResult GetValue(string key)
        {
            var value = base.GetValue(key);
            string queryParameter = value.ToString();
            try
            {
                if (!string.IsNullOrEmpty(queryParameter))
                {
                    DateTime date;
                    if (!queryParameter.Contains(".") && !queryParameter.Contains(",") && DateTime.TryParse(queryParameter, out date))
                    {
                        return new ValueProviderResult(new StringValues(date.AddMinutes(timeZoneOffset).ToString()), value.Culture);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            return value;
        }

        private void Log(Exception ex)
        {
            try
            {
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\_log.txt", true))
                {
                    outputFile.WriteLine($"CustomQueryStringParser: {ex.Message}");
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        outputFile.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception logEx)
            {
                while (logEx.InnerException != null)
                {
                    logEx = logEx.InnerException;
                }
                Console.WriteLine(logEx.Message);
            }
        }
    }
}
