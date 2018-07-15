using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class Service2 : IService2
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Service2(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<IEnumerable<string>> GetValues()
    {
        var list = new List<string>();
        list.Add(Principal.GetPrincipal(_httpContextAccessor).ToPrincipalInfo());

        await Task.Delay(50);

        list.Add(Principal.GetPrincipal(_httpContextAccessor).ToPrincipalInfo());

        return list;
    }
}