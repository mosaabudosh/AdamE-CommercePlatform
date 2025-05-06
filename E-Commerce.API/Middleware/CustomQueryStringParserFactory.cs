using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_Commerce.API.Middleware
{
    public class CustomQueryStringParserFactory : IValueProviderFactory
    {
        public CustomQueryStringParserFactory()
        {
        }

        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            try
            {
                context.ValueProviders.Insert(0, new CustomQueryStringParser(context.ActionContext.HttpContext.Request));
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            return Task.CompletedTask;
        }

        private void Log(Exception ex)
        {
            try
            {
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\_log.txt", true))
                {
                    outputFile.WriteLine($"CustomQueryStringParserFactory: {ex.Message}");
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
