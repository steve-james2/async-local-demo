using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IService1 _service1;

        public ValuesController(IHttpContextAccessor httpContextAccessor, IService1 service1)
        {
            _httpContextAccessor = httpContextAccessor;
            _service1 = service1;
        }

        private void SetPrincipal()
        {
           Principal.SetPrincipal(_httpContextAccessor, Helpers.CreateClaimsPrincipalWithName(Guid.NewGuid().ToString()));
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            SetPrincipal();

            var list = new List<string>();
            list.Add(Principal.GetPrincipal(_httpContextAccessor).ToPrincipalInfo());

            list.AddRange((await _service1.GetValues()));

            list.Add(Principal.GetPrincipal(_httpContextAccessor).ToPrincipalInfo());

            return list;
        }

        // GET api/values/5
        [HttpGet()]
        [Route("multiple")]
        // public async Task<ActionResult<IEnumerable<string>>> GetMultiple(int iterations)
        public async Task<IEnumerable<IEnumerable<string>>> GetMultiple(int iterations)
        {
            iterations = Math.Max(1, iterations);

            var tasks = new Task<string>[iterations];

            for(int i = 0; i < iterations; i++)
                tasks[i] = Task.Run(async () => await GetData());

            Task.WaitAll(tasks);

            return PackageResults(tasks);
        }

        private async Task<string> GetData()
        {
            var uri = "http://localhost:5000/api/values";
            using(var client = new HttpClient())
            using(var response = await client.GetAsync(uri))
            using(var content = response.Content)
                return await content.ReadAsStringAsync();
        }

        private IEnumerable<IEnumerable<string>> PackageResults(Task<string>[] tasks)
        {
            return tasks
                .Select(t => ((JArray)JsonConvert.DeserializeObject(t.Result)).ToObject<List<string>>())
                .ToList();
        }
    }
}
