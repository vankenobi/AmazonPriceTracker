using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence.Contexts
{
    public class AmazonPriceTrackerDbContext : DbContext
    {
        public AmazonPriceTrackerDbContext(DbContextOptions options) : base(options)
        {}

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<TrackedProduct> TrackedProducts { get; set; } = null!;

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Interceptor
            var date = DateTime.Now;
            var datas = ChangeTracker
                        .Entries<BaseEntity>();
           
            foreach (var data in datas)
            {
                switch (data.State)
                {
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        data.Entity.CreatedDate.ToLocalTime();
                        data.Entity.UpdatedDate.ToLocalTime();
                        break;
                    case EntityState.Deleted:
                        break;
                    case EntityState.Modified:
                        data.Entity.UpdatedDate = DateTime.SpecifyKind(DateTime.Now,DateTimeKind.Utc);
                        data.Entity.CreatedDate = data.Entity.CreatedDate;
                        break;
                    case EntityState.Added:
                        data.Entity.CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                        break;
                    default:
                        break;
                }
            }
           
            return await base.SaveChangesAsync(cancellationToken);
        }



        
        
    }
}
