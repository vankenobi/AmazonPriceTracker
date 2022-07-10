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
                //configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../Presentation/Amazon Price Tracker"));
                configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory()));
                configurationManager.AddJsonFile("appsettings.json");
                return configurationManager.GetConnectionString("postgresql");
            }
        }
        
    }
}
