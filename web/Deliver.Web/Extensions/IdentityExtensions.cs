using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Deliver.Web
{
    public static class IdentityExtensions
    {
        /// <summary>
        /// Gets the currently logged in user Id.
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if(claimsPrincipal.Identity.IsAuthenticated)
                return claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;

            return null;
        }
    }
}
