using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence
{
    static class Configuration
    {
        static public string ConnectionString 
        {
            get 
            {
                ConfigurationManager configurationManager = new();
                Console.WriteLine(Directory.GetCurrentDirectory());
                configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory()));
                configurationManager.AddJsonFile("appsettings.json");
                return configurationManager.GetConnectionString("postgresql");
            }
        }
        
    }
}
