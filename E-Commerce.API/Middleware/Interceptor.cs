using Adam.Logging.Interfaces;
using E_Commerce.Domain.IHttpServices;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using E_Commerce.Application.Model.AppSettings;

namespace E_Commerce.API.Middleware
{
    public class Interceptor
    {
        private readonly RequestDelegate _next;
        bool notModified = true;
        bool enableLogHttpResponse = false;
        double httpResponseLength = 10000;
        public readonly IUserAccessor _userAccessor;
        private readonly AppSettings _appSettings;
        public Interceptor(RequestDelegate next, IConfiguration Configuration, IUserAccessor userAccessor, AppSettings appSettings)
        {
            _next = next;
            _userAccessor = userAccessor;
            _appSettings = appSettings;
            if (appSettings.adamLogging.EnableLogHttpResponse)
            {
                enableLogHttpResponse = appSettings.adamLogging.EnableLogHttpResponse;
                httpResponseLength = appSettings.adamLogging.HttpResponseLength;
            }
        }

        public async Task Invoke(HttpContext context)
        {
            string url = context.Request.Path.ToString();
            try
            {
                int timeZoneOffset = 0;
                if (!string.IsNullOrEmpty(context.Request.Headers["TimeZone-Offset"]))
                {
                    timeZoneOffset = Convert.ToInt32(context.Request.Headers["TimeZone-Offset"][0]);
                }
                if (!context.Request.Method.Equals("GET") && timeZoneOffset != 0 && context.Request.ContentType.Contains("application/json"))
                {
                    if (context.Request.Body != null)
                    {
                        using (StreamReader streamReader = new StreamReader(context.Request.Body))
                        {
                            string originalContent = await streamReader.ReadToEndAsync();
                            if (!string.IsNullOrEmpty(originalContent))
                            {
                                JToken parameterObject = null;
                                try
                                {
                                    parameterObject = JToken.Parse(originalContent);
                                    EnumerateObjectProperties(parameterObject, timeZoneOffset);
                                    string requestBody = notModified ? originalContent : JsonConvert.SerializeObject(parameterObject);
                                    context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
                                }
                                catch (Exception ex)
                                {
                                    Startup.Log("Invoke: Request part:", ex);
                                }
                            }
                        }
                    }
                }
                if ((context.Request.Headers.ContainsKey("ResponeType") && context.Request.Headers["ResponeType"] == "report") || ((context.Request.ContentType ?? string.Empty) != "application/json"))
                {
                    await _next(context);
                }
                else
                {
                    Stream originalBodyStream = context.Response.Body;
                    using (var responseBody = new MemoryStream())
                    {
                        context.Response.Body = responseBody;
                        await _next(context);
                        responseBody.Seek(0, SeekOrigin.Begin);
                        string bodyText = await new StreamReader(responseBody).ReadToEndAsync();
                        if (enableLogHttpResponse && bodyText.Length >= httpResponseLength)
                        {
                            Startup.LogHttpRespose(context.Request.Path.ToString().Replace("\\", "--"), bodyText);
                        }

                        string newBodyText = bodyText;
                        JToken respondedObject = null;
                        try
                        {
                            if (context.Response.ContentType != null && context.Response.ContentType.Contains("application/json") && !string.IsNullOrEmpty(bodyText))
                            {
                                respondedObject = JsonConvert.DeserializeObject<JToken>(bodyText, new JsonSerializerSettings
                                {
                                    FloatParseHandling = FloatParseHandling.Decimal
                                });
                                EnumerateObjectProperties(respondedObject, -timeZoneOffset);
                                newBodyText = JsonConvert.SerializeObject(respondedObject);
                            }
                        }
                        catch (Exception ex)
                        {
                            newBodyText = bodyText;
                            Startup.Log("Interceptor, Invoke", ex);
                        }
                        context.Response.Body = originalBodyStream;
                        if (!string.IsNullOrEmpty(newBodyText))
                        {
                            await context.Response.WriteAsync(newBodyText);
                        }
                        originalBodyStream.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Startup.Log("Invoke", ex);
            }
            finally
            {
                //_tracingService.End(tracingObject);
            }
        }

        private void EnumerateObjectProperties(JToken token, int timeZoneOffset)
        {
            try
            {
                if (timeZoneOffset == 0)
                {
                    return;
                }

                switch (token)
                {
                    case JValue jValue:
                        ModifyValue(jValue, timeZoneOffset);
                        break;

                    case JProperty jProperty:
                        EnumerateObjectProperties(jProperty.Value, timeZoneOffset);
                        break;

                    case JObject jObject:
                        foreach (var property in jObject.Properties())
                        {
                            EnumerateObjectProperties(property, timeZoneOffset);
                        }
                        break;

                    case JArray jArray:
                        foreach (var item in jArray)
                        {
                            EnumerateObjectProperties(item, timeZoneOffset);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Startup.Log("EnumerateObjectProperties", ex);
            }
        }

        private void ModifyValue(JValue jValue, int timeZoneOffset)
        {
            if (jValue.Type == JTokenType.Date && jValue.Value is DateTime dateTime && dateTime != DateTime.MinValue)
            {
                jValue.Value = dateTime.AddMinutes(timeZoneOffset);
                notModified = false;
            }
        }
    }
}
