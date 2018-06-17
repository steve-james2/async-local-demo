using System.Collections.Generic;
using System.Threading.Tasks;

public interface IService2
{
    Task<IEnumerable<string>> GetValues();
}