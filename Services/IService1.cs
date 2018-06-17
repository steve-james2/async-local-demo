using System.Collections.Generic;
using System.Threading.Tasks;

public interface IService1
{
    Task<IEnumerable<string>> GetValues();
}