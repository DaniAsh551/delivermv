using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Deliver.Data.Api
{
    public class ApiStatus
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public long ResultCount { get; set; }

        public ApiStatus()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        public ApiStatus(HttpStatusCode code, string message = null, string description = null)
        {
            HttpStatusCode = code;
            Message = message;
            Description = description;
        }
    }
}
