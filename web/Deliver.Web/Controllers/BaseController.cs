using Deliver.Data.Api;
using Deliver.Data.Pagination;
using Deliver.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deliver.Web.Controllers
{
    public abstract class BaseController : ControllerBase, IHasPaging
    {
        public readonly ILogger Logger;
        public virtual Paging Paging { get; set; }
        public DeliverDbContext Db { get; }

        public BaseController(DeliverDbContext context, ILogger logger)
        {
            Db = context;
            Logger = logger;
        }

        public BaseController(DeliverDbContext context)
        {
            Db = context;
            Logger = null;
        }

        public BaseController(IHttpContextAccessor contextAccessor)
        {
            Db = (DeliverDbContext)contextAccessor.HttpContext.RequestServices.GetService(typeof(DeliverDbContext));
            Logger = (ILogger)contextAccessor.HttpContext.RequestServices.GetService(typeof(ILogger));
        }



        //[Obsolete("Errors will be handled globally - should you require to send error, simply type throw new Exception(your message)")]
        public ApiResponse<T> Error<T>(Exception ex)
        {
            if (ex is NullReferenceException)
            {
                return new ApiResponse<T>(default(T), new ApiError("404", "Item(s) not found", "The specified item(s) cannot be found", ex.StackTrace));
            }
            return new ApiResponse<T>(default(T), new ApiError("500", ex.Message));
        }

        //[Obsolete("Errors will be handled globally - should you require to send error, simply type throw new Exception(your message)")]
        public ApiResponse<T> Error<T>(ApiError error)
        {
            return new ApiResponse<T>(default(T), error);
        }

        //[Obsolete("Errors will be handled globally - should you require to send error, simply type throw new Exception(your message)")]
        public dynamic Error(Exception ex)
        {
            if (ex is NullReferenceException)
            {
                return new ApiResponse<dynamic>(null, new ApiError("404", "Item(s) not found", "The specified item(s) cannot be found", ex.StackTrace));
            }
            return new ApiResponse<dynamic>(null, new ApiError("500", ex.Message/*"Server Fault"*/));
        }

        public ApiResponse<T> Success<T>(T result)
        {
            Paging paging = (this.Paging != null && typeof(T).IsArray) ? this.Paging : null;
            //Response.ContentType = "application/json";
            return new ApiResponse<T>(result, paging: paging);
        }

        public ApiResponse<T> Success<T>(T result, Paging paging)
        {
            //Response.ContentType = "application/json";
            return new ApiResponse<T>(result, paging: paging);
        }
    }
}
