using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities.Common;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence.Concretes
{
    public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
    {
        private readonly AmazonPriceTrackerDbContext _context;
        public WriteRepository(AmazonPriceTrackerDbContext context)
        {
            _context = context; 
        }

        public DbSet<T> Table => _context.Set<T>();

        public async Task<bool> AddAsync(T model)
        {
            EntityEntry entityEntry =  await Table.AddAsync(model);
            return entityEntry.State == EntityState.Added;
        }
        
        public async Task<bool> AddRangeAsync(List<T> models)
        {
            await Table.AddRangeAsync(models);
            return true;
        }

        public bool Remove(T model)
        {
            EntityEntry entityEntry =  Table.Remove(model);
            return entityEntry.State == EntityState.Deleted;
        }

        public async Task<bool> Remove(int id)
        {
            T model = await Table.FirstOrDefaultAsync(x => x.Id == id);
            return Remove(model);
        }

        public bool RemoveRange(List<T> models)
        {
            Table.RemoveRange(models);
            return true;
        }

        public int SaveChanges()
            => _context.SaveChanges();

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync(); 

        public bool Update(T model)
        {
            EntityEntry entityEntry = Table.Update(model);
            return entityEntry.State == EntityState.Modified;
        }
    }
}
