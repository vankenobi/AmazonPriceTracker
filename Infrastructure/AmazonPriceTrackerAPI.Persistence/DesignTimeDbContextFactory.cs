using AmazonPriceTrackerAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


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
