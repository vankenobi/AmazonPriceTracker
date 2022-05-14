﻿using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Entities.Dto;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AmazonPriceTrackerAPI.Application.Repositories
{
    public interface ITrackedProductWriteRepository : IWriteRepository<TrackedProduct>
    {
        Task<Response> AddProductTracking(AddProductTrackingDto addTrackingProductDto);

    }
}