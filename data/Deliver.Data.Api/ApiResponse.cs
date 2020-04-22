using Newtonsoft.Json;
using Deliver.Data.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Deliver.Data.Api
{
    public class ApiResponse
    {
        [JsonProperty("result")]
        public object Result { get; set; }
        [JsonProperty("errors")]
        public List<ApiError> Errors { get; set; }
        [JsonProperty("apiStatus")]
        public ApiStatus ApiStatus { get; set; }
        [JsonProperty("paging")]
        public Paging Paging { get; internal set; }
        [JsonProperty("isSuccess")]
        public bool IsSuccess => (!Errors?.Any()) ?? true;

        public ApiResponse()
        {
            ApiStatus = new ApiStatus();
            Errors = new List<ApiError>();
        }

        public ApiResponse(object result, ApiError error = null, HttpStatusCode code = HttpStatusCode.OK, Paging paging = null)
        {
            Errors = new List<ApiError>();
            Result = result;
            Paging = paging;
            if (error == null)
            {
                ApiStatus = new ApiStatus()
                {
                    HttpStatusCode = code,
                    // Message = success ?? "Success",
                    ResultCount = result == null ? 0 : result.GetType().IsMultiple() ? (Result as Array)?.Length ?? 0 : 1,
                };
            }
            else
            {
                Errors.Add(error);
                ApiStatus = new ApiStatus()
                {
                    HttpStatusCode = code,
                    Message = error?.Message ?? "An Error Occured.",
                };
            }
        }


        public ApiResponse(object result, HttpStatusCode code, ApiError[] errors, Paging paging = null)
        {
            Errors = new List<ApiError>();
            Result = result;
            Paging = paging;
            Errors.AddRange(errors);
            ApiStatus = new ApiStatus()
            {
                HttpStatusCode = code,
                Message = (Errors.Count > 1 ? "Multiple Errors Occured, " : "") + "See Errors for details.",
            };
        }

        [JsonIgnore]
        public static ApiResponse EmptySuccess => new ApiResponse();

        [JsonIgnore]
        public static ApiResponse EmptyFailure => new ApiResponse(null, new ApiError("400", "The operation failed.", "Failure", "The action could not be completed"), HttpStatusCode.BadRequest);
    }

    public class ApiResponse<T> : ApiResponse
    {
        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public new T Result { get => (T)base.Result; set => base.Result = value; }

        public ApiResponse()
            : base() { }

        public ApiResponse(T result, ApiError error = null, HttpStatusCode code = HttpStatusCode.OK, Paging paging = null)
            : base(result, error, code, paging) { }

        public ApiResponse(T result, HttpStatusCode code, ApiError[] errors, Paging paging = null)
        : base(result, code, errors, paging) { }

        [JsonIgnore]
        public static new ApiResponse<T> EmptySuccess => new ApiResponse<T>();

        [JsonIgnore]
        public static new ApiResponse<T> EmptyFailure => new ApiResponse<T>(default(T), new ApiError("400", "The operation failed.", "Failure", "The action could not be completed"), HttpStatusCode.BadRequest);
    }
}
