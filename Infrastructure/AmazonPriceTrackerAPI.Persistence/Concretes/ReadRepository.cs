using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities.Common;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence.Concretes
{
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        private readonly AmazonPriceTrackerDbContext _context;

        public ReadRepository(AmazonPriceTrackerDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public IQueryable<T> GetAll(bool tracking = false)
        {
            var query = Table.AsQueryable();
            if (tracking)
                return query;
            return query.AsNoTracking();
        }

        public async Task<List<T>> GetAllAsync(bool tracking = false)
        {
            if (tracking)
                return await Table.ToListAsync();
            return await Table.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, bool tracking = false)
        {
            var query = Table.AsQueryable();
            if (tracking)
                return await query.FirstOrDefaultAsync(x => x.Id == id);
            return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = false)
        {
            var query = Table.AsQueryable();
            if (tracking) 
                return await query.FirstOrDefaultAsync(method);
            return await query.AsNoTracking().FirstOrDefaultAsync(method);
        }


        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = false)
        {
            var query = Table.AsQueryable();
            if (tracking)
                return query.Where(method);
            return query.Where(method).AsNoTracking();
        }
    }
}
