using Microsoft.EntityFrameworkCore;
using AmazonPriceTrackerAPI.Persistence.Concretes;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Persistence.Concretes.TrackedProductConcrets;
using Amazon_Price_Tracker.Worker;

namespace AmazonPriceTrackerAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services) 
        {
            services.AddHostedService<Worker>(); 

            services.AddDbContext<AmazonPriceTrackerDbContext>(options => options.UseNpgsql(Configuration.ConnectionString));
            services.AddScoped<IProductReadRepository,ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<ITrackedProductReadRepository,TrackedProductReadRepository>();
            services.AddTransient<ITrackedProductWriteRepository, TrackedProductWriteRepository>();    
            
        }
    }
}
