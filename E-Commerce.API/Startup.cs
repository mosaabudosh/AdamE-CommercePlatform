using E_Commerce.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API
{
    public static class Startup
    {
        public static void Log(string methodName, Exception ex)
        {
            try
            {
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\_log.txt", true))
                {
                    outputFile.WriteLine($"{methodName}: {ex.Message}");
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        outputFile.WriteLine(ex.Message);
                    }
                    outputFile.WriteLine("*****************");
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

        public static void LogHttpRespose(string methodName, string response)
        {
            try
            {
                string x = DateTime.UtcNow.ToString().Replace("\\", "--").Replace("/", "--").Replace(":", "--");
                methodName = methodName.Replace("\\", "--");
                methodName = methodName.Replace("/", "--");

                string path = $"{System.AppDomain.CurrentDomain.BaseDirectory}\\HttpResponse";

                bool isExists = System.IO.Directory.Exists(path);
                if (!isExists)
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter($"{path}\\{x}-{methodName}.txt", true))
                {
                    outputFile.WriteLine(methodName);
                    outputFile.WriteLine(response);
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

        public static void UpdateDatabase(IServiceScope serviceScope)
        {
            try
            {
                using (E_CommerceContext context = serviceScope.ServiceProvider.GetService<E_CommerceContext>())
                {
                    context.Database.Migrate();
                    context.Database.EnsureCreated();
                }
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\_MigrationLog.txt", true))
                {
                    outputFile.WriteLine(ex.Message);
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        outputFile.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
