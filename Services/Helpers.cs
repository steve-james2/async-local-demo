using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

public static class Helpers
{
    public static string ToPrincipalInfo(this IPrincipal principal)
    {
        return $"Name={principal?.Identity?.Name} on thread {Thread.CurrentThread.ManagedThreadId} at {DateTime.Now.ToString("hh:MM:ss.fff")}.";
    }

    public static ClaimsPrincipal CreateClaimsPrincipalWithName(string name)
    {
        var identity = new ClaimsIdentity(new List<Claim>{new Claim("sub", name)}, null, "sub", null);
        return new ClaimsPrincipal(identity);
    }
}