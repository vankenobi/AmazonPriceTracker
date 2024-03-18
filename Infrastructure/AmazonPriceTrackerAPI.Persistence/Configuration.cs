using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AmazonPriceTrackerAPI.Persistence
{
    static public class Configuration
    {
        static public string ConnectionString
        {
            get
            {
                ConfigurationManager configurationManager = new();
                configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory()));
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker")
                {
                    return Environment.GetEnvironmentVariable("DB_CONNECTION");
                }
                else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    configurationManager.AddJsonFile("appsettings.Development.json");
                    return configurationManager.GetConnectionString("postgresql");
                }
                else
                {
                    configurationManager.AddJsonFile("appsettings.json");
                    return configurationManager.GetConnectionString("postgresql");
                }
            }
        }

        static public Serilog.LoggerConfiguration loggerConfiguration
        {
            get
            {
                string url = "";
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    url = "http://localhost:5341/";
                }
                else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Docker") 
                {
                    url = "http://" + Environment.GetEnvironmentVariable("SEQ_DNS") + ":" + Environment.GetEnvironmentVariable("SEQ_PORT") + "/";
                }
                Console.WriteLine(url);
                var loggerConfig = new LoggerConfiguration()
                    .WriteTo.File("all-daily-.logs", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {RequestId} {Message:lj}{NewLine}{Exception}")
                    .MinimumLevel.Debug()
                    .WriteTo.Seq(serverUrl: url)
                    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {RequestId} {Message:lj}{NewLine}{Exception}")
                    .Enrich.FromLogContext();
                return loggerConfig;
            }
        }

    }
}
