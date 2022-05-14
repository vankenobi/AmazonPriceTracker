using AmazonPriceTrackerAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Application.Repositories.WorkerRepo
{
    public interface IWorkerWriteRepository : IWriteRepository<TrackedProduct>
    {
    }
}
