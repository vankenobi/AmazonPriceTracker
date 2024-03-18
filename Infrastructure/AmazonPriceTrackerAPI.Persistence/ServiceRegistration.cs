using Microsoft.EntityFrameworkCore;
using AmazonPriceTrackerAPI.Persistence.Concretes;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;
using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Persistence.Concretes.TrackedProductConcrets;
using AmazonPriceTrackerAPI.Persistence.Concretes.Worker;
using AmazonPriceTrackerAPI.Application.Repositories.MailRepo;
using AmazonPriceTrackerAPI.Infrastructure.Concrets;

namespace AmazonPriceTrackerAPI.Persistence
{
    public static class ServiceRegistration
    {

        public static void AddPersistenceServices(this IServiceCollection services) 
        {
            var loggerConfig = Configuration.loggerConfiguration;

            services.AddSingleton<Worker, Worker>();
            services.AddHostedService<Worker>();
            services.AddDbContext<AmazonPriceTrackerDbContext>(options => options.UseNpgsql(Configuration.ConnectionString),ServiceLifetime.Transient); 
            services.AddSingleton<Serilog.ILogger>(loggerConfig.CreateLogger());

            services.AddScoped<IProductReadRepository,ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<ITrackedProductReadRepository,TrackedProductReadRepository>();
            services.AddScoped<ITrackedProductWriteRepository, TrackedProductWriteRepository>();
            services.AddScoped<IMailRepository,MailRepository>();
        }
    }
}
