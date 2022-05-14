using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes
{
    public enum ResponseCode
    {
        Success = 200,
        NoContent = 204,
        BadRequest = 400,
        NotFound = 404,
        Error = 500
    }
}
