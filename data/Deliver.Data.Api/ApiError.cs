using System;

namespace Deliver.Data.Api
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string StackTrace { get; set; }

        public ApiError()
        {
        }

        public ApiError(string code, string message = null, string type = "", string description = "", string stacktrace = "")
        {
            Code = code;
            Message = message;
            Type = type;
            Description = description;
            StackTrace = stacktrace;
        }


        public ApiError(Exception exception, string code = "null")
        {
            if(exception is ApiException apiException)
            {
                code = apiException.Code;
            }

            Code = code;
            Message = exception.Message;
            Type = exception.GetType().ToString();
            Description = exception.InnerException?.Message;
            StackTrace = exception.StackTrace;
        }
    }
}
