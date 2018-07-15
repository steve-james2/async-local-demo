using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class Service1 : IService1
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IService2 _service2;

    public Service1(IHttpContextAccessor httpContextAccessor, IService2 service2)
    {
        _httpContextAccessor = httpContextAccessor;
        _service2 = service2;
    }

    public async Task<IEnumerable<string>> GetValues()
    {
        await Task.Delay(20);

        var list = new List<string>();
        list.Add(Principal.GetPrincipal(_httpContextAccessor).ToPrincipalInfo());

        list.AddRange(await _service2.GetValues());

        list.Add(Principal.GetPrincipal(_httpContextAccessor).ToPrincipalInfo());

        await Task.Delay(20);
        
        return list;
    }
}