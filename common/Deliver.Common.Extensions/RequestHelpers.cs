using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Mvc
{
    public static class RequestHelpers
    {
        public static bool IsAjax(this HttpRequest httpRequest)
            => httpRequest.Headers.ContainsKey("X-Requested-With") && httpRequest.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}
