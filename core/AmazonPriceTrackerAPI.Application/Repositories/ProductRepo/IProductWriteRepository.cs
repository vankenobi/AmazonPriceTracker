
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AmazonPriceTrackerAPI.Application.Repositories
{
    public interface IProductWriteRepository : IWriteRepository<Product>
    {
        Task<Response> AddNewProductWithUrlAsync(string url);
        Task<Response> DeleteProductAsync(int id);
    }
}
