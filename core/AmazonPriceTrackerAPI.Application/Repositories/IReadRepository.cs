using AmazonPriceTrackerAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Application.Repositories
{
    public interface IReadRepository<T> : IRepository<T> where T : BaseEntity
    { 
        IQueryable<T> GetAll(bool tracking = false);
        Task<List<T>> GetAllAsync(bool tracking = false);
        IQueryable<T> GetWhere(Expression<Func<T,bool>> method, bool tracking = false);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = false);
        Task<T> GetByIdAsync(int id, bool tracking = false);
        Task SaveChangesAsync();
    }
}
