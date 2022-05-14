using AmazonPriceTrackerAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AmazonPriceTrackerDbContext>
    {
        public AmazonPriceTrackerDbContext CreateDbContext(string[] args)
        {

            DbContextOptionsBuilder<AmazonPriceTrackerDbContext> dbContextOptionsBuilder = new();
            
            dbContextOptionsBuilder.UseNpgsql(Configuration.ConnectionString);
            return new AmazonPriceTrackerDbContext(dbContextOptionsBuilder.Options); 
        }
    }
}
