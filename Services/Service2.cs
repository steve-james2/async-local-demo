using System.Collections.Generic;
using System.Threading.Tasks;

public class Service2 : IService2
{
    public async Task<IEnumerable<string>> GetValues()
    {
        var list = new List<string>();
        list.Add(Principal.GetPrincipal().ToPrincipalInfo());

        await Task.Delay(50);

        list.Add(Principal.GetPrincipal().ToPrincipalInfo());

        return list;
    }
}