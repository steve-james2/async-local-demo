using System.Security.Principal;
using System.Threading;

public static class Principal
{
    static AsyncLocal<IPrincipal> _principal = new AsyncLocal<IPrincipal>();

    public static IPrincipal GetPrincipal()
    {
        return _principal.Value;
    }

    public static void SetPrincipal(IPrincipal principal)
    {
        _principal.Value = principal;
    }
}