using AmazonPriceTrackerAPI.Domain.Shared.Abstract;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Shared.Concret
{
    public class Response<T> : IServiceResponse
    {
        public ResponseCode ResponseCode { get; }
        public string Message { get; }
        public T? Data { get; }

        public Response(ResponseCode responseCode)
        {
            ResponseCode = responseCode;
        }

        public Response(ResponseCode responseCode,string message)
        {
            Message = message;
            ResponseCode = responseCode;
        }

        public Response(ResponseCode responseCode , T data)
        {
            ResponseCode= responseCode;
            Data = data;
        }

        public Response(ResponseCode responseCode , string message , T data)
        {
            ResponseCode = responseCode ;
            Data = data;
            Message = message;
        }
    }

    public class Response : IServiceResponse
    {
        public ResponseCode ResponseCode { get; }

        public string Message { get; }

        public Response(ResponseCode responseCode)
        {
            ResponseCode = responseCode;
        }

        public Response(ResponseCode responseCode , string message)
        {
            Message= message;
            ResponseCode= responseCode;
        }
    }
}
