using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Microsoft.AspNetCore.Http;

public static class Principal
{
    static AsyncLocal<IPrincipal> _principal = new AsyncLocal<IPrincipal>();

    public static IPrincipal GetPrincipal(IHttpContextAccessor httpContextAccessor) =>
        // return _principal.Value;
        httpContextAccessor.HttpContext.User;


    public static void SetPrincipal(IHttpContextAccessor httpContextAccessor, IPrincipal principal)
    {
        // _principal.Value = principal;
        httpContextAccessor.HttpContext.User = principal as ClaimsPrincipal;
    }
}