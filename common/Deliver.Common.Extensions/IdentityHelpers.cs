using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Security.Claims
{
    public static class IdentityHelpers
    {
        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            int.TryParse(
                claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "sub")?.Value 
                ?? claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value,
                out var id);
            return id;
        }
    }
}
