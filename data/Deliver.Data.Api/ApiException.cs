using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deliver.Data.Api
{
    /// <summary>
    /// Exception to explicitly deal with Api thrown Errors
    /// </summary>
    public class ApiException : Exception
    {
        private string _stackTrace = "";
        
        public override string StackTrace => _stackTrace;
        public string Code { get; set; }
        public string Description { get; set; }
        public ApiError[] ApiErrors { get; private set; }

        public ApiException(IEnumerable<ApiError> apiErrors) : base(
            message: apiErrors.Count() == 1
            ? apiErrors.First().Message ?? "An ApiError has occured."
            : string.Join(Environment.NewLine + "===================" + Environment.NewLine,
                "[Multiple Errors] Please See ApiErrors", apiErrors?.Select(x => x.Message)?.ToArray())
            )
        {
            if (!apiErrors?.Any() ?? true)
                throw new ArgumentNullException(nameof(apiErrors));
            if (apiErrors.Count() == 1)
            {
                var apiError = apiErrors.First();
                _stackTrace = apiError.StackTrace;
                Code = apiError.Code;
                Description = apiError.Description;
                ApiErrors = new ApiError[] { apiError };
            }
            else
            {
                _stackTrace = string.Join(Environment.NewLine + "===================" + Environment.NewLine,
                    "[Multiple Errors] Please See ApiErrors", apiErrors?.Select(x => x.StackTrace)?.ToArray());
                Code = string.Join(Environment.NewLine + "===================" + Environment.NewLine,
                    "[Multiple Errors] Please See ApiErrors", apiErrors?.Select(x => x.Code)?.ToArray());
                Description = string.Join(Environment.NewLine + "===================" + Environment.NewLine,
                    "[Multiple Errors] Please See ApiErrors", apiErrors?.Select(x => x.Description)?.ToArray());
                ApiErrors = apiErrors.ToArray();
            }
        }

        public ApiException(params ApiError[] apiErrors) : base(
            message: apiErrors.Length == 1
            ? apiErrors[0].Message ?? "An ApiError has occured."
            : string.Join(Environment.NewLine + "===================" + Environment.NewLine,
                "[Multiple Errors] Please See ApiErrors", apiErrors?.Select(x => x.Message)?.ToArray())
            )
        {
            if (!apiErrors?.Any() ?? true)
                throw new ArgumentNullException(nameof(apiErrors));
            if (apiErrors.Length == 1)
            {
                var apiError = apiErrors[0];
                _stackTrace = apiError.StackTrace;
                Code = apiError.Code;
                Description = apiError.Description;
                ApiErrors = new ApiError[] { apiError };
            }
            else
            {
                _stackTrace = string.Join(Environment.NewLine + "===================" + Environment.NewLine,
                    "[Multiple Errors] Please See ApiErrors", apiErrors?.Select(x => x.StackTrace)?.ToArray());
                Code = string.Join(Environment.NewLine + "===================" + Environment.NewLine,
                    "[Multiple Errors] Please See ApiErrors", apiErrors?.Select(x => x.Code)?.ToArray());
                Description = string.Join(Environment.NewLine + "===================" + Environment.NewLine,
                    "[Multiple Errors] Please See ApiErrors", apiErrors?.Select(x => x.Description)?.ToArray());
                ApiErrors = apiErrors.ToArray();
            }
        }
    }
}
