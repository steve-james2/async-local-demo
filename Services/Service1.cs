using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Service1 : IService1
{
    private readonly IService2 _service2;

    public Service1(IService2 service2)
    {
        _service2 = service2;
    }

    public async Task<IEnumerable<string>> GetValues()
    {
        await Task.Delay(20);

        var list = new List<string>();
        list.Add(Principal.GetPrincipal().ToPrincipalInfo());

        list.AddRange(await _service2.GetValues());

        list.Add(Principal.GetPrincipal().ToPrincipalInfo());

        await Task.Delay(20);
        
        return list;
    }
}