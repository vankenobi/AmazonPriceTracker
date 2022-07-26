using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using AmazonPriceTrackerAPI.Persistence.Concretes;
using AmazonPriceTrackerAPI.Application.Repositories;

namespace UnitTests.ProductFixture
{
    public class ProductReadFixture
    {
     

        [Fact]
        public void GetAllProductsAsyncTest()
        {
            var productReadRepository = new Mock<ReadRepository<Product>>();

            List<Product> inMemory = new List<Product> {
                new Product { Id = 1, Name = "Iphone 13 Pro" },
                new Product { Id = 2, Name = "Samsung S22 Ultra" },
                new Product { Id = 3, Name = "Macbook Pro" }
            };

            productReadRepository.Setup( x => x.GetAllAsync(false)).Returns(Task.FromResult(inMemory));
            /*
            ProductReadRepository _productReadRepository = new ProductReadRepository();

            var result = _productReadRepository.GetAllProductsAsync();
            
            Assert.Equal(result.Result.Data[0].Name, inMemory[0].Name);
            */
        }
    }
}
