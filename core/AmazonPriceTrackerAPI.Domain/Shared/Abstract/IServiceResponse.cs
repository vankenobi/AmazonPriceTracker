using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Shared.Abstract
{
    public interface IServiceResponse
    {
        ResponseCode ResponseCode { get; }
        string Message { get; }
    }
}
