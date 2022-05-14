using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence.Concretes.TrackedProductConcrets
{
    public class TrackedProductReadRepository : ReadRepository<TrackedProduct>, ITrackedProductReadRepository
    {
        public TrackedProductReadRepository(AmazonPriceTrackerDbContext context) : base(context)
        {
        }
    }
}
