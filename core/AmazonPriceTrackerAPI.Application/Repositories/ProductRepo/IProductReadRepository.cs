using AmazonPriceTrackerAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Application.Repositories
{
    public interface IProductReadRepository : IReadRepository<Product>
    {
        
    }
}
