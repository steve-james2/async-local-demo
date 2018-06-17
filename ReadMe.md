# AsyncLocal

It turns out AsyncLocal seems to work quite well. Here is a simple non-thread-safe implementation:

```csharp
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
```

You could just as easily create an injectable or scoped ambient implementation.

Here is an example of a client writing and reading this value:

```csharp
        private void SetPrincipal()
        {
            Principal.SetPrincipal(Helpers.CreateClaimsPrincipalWithName(Guid.NewGuid().ToString()));
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            SetPrincipal();

            var list = new List<string>();
            list.Add(Principal.GetPrincipal().ToPrincipalInfo());

            list.AddRange((await _service1.GetValues()));

            list.Add(Principal.GetPrincipal().ToPrincipalInfo());

            return list;
        }
```
		
This client creates a principal and then awaits a call to a service which in turn awaits a call to another service. At various points in the controller and both services, the current principal is retrieved and details are written to a collection of strings which are eventually returned to the client. The output looks like this:

```json
[
    "Name=5db1dc1a-a4a9-4095-bf9f-7b9d7959592b on thread 64 at 07:06:30.002.",
    "Name=5db1dc1a-a4a9-4095-bf9f-7b9d7959592b on thread 55 at 07:06:30.033.",
    "Name=5db1dc1a-a4a9-4095-bf9f-7b9d7959592b on thread 55 at 07:06:30.033.",
    "Name=5db1dc1a-a4a9-4095-bf9f-7b9d7959592b on thread 67 at 07:06:30.089.",
    "Name=5db1dc1a-a4a9-4095-bf9f-7b9d7959592b on thread 67 at 07:06:30.089.",
    "Name=5db1dc1a-a4a9-4095-bf9f-7b9d7959592b on thread 79 at 07:06:30.125."
]
```

This shows the principal name remains the same through the entire async flow despite the awaits and the switching to different threads.

There is another endpoint that calls the previous endpoint multiple times simultaneously. The result is:

```json
[
    [
        "Name=da4ffc2b-b72f-4bfe-84e3-7d1c13c227b9 on thread 14 at 07:50:45.852.",
        "Name=da4ffc2b-b72f-4bfe-84e3-7d1c13c227b9 on thread 20 at 07:50:45.887.",
        "Name=da4ffc2b-b72f-4bfe-84e3-7d1c13c227b9 on thread 20 at 07:50:45.888.",
        "Name=da4ffc2b-b72f-4bfe-84e3-7d1c13c227b9 on thread 20 at 07:50:45.948.",
        "Name=da4ffc2b-b72f-4bfe-84e3-7d1c13c227b9 on thread 20 at 07:50:45.948.",
        "Name=da4ffc2b-b72f-4bfe-84e3-7d1c13c227b9 on thread 20 at 07:50:45.980."
    ],
    [
        "Name=9cd55e6e-d986-4479-b9d2-94723912b44a on thread 20 at 07:50:45.852.",
        "Name=9cd55e6e-d986-4479-b9d2-94723912b44a on thread 17 at 07:50:45.887.",
        "Name=9cd55e6e-d986-4479-b9d2-94723912b44a on thread 17 at 07:50:45.888.",
        "Name=9cd55e6e-d986-4479-b9d2-94723912b44a on thread 17 at 07:50:45.948.",
        "Name=9cd55e6e-d986-4479-b9d2-94723912b44a on thread 17 at 07:50:45.948.",
        "Name=9cd55e6e-d986-4479-b9d2-94723912b44a on thread 18 at 07:50:45.980."
    ],
    [
        "Name=26306437-9f56-4c33-abb6-7e7f6327558b on thread 17 at 07:50:45.852.",
        "Name=26306437-9f56-4c33-abb6-7e7f6327558b on thread 19 at 07:50:45.887.",
        "Name=26306437-9f56-4c33-abb6-7e7f6327558b on thread 19 at 07:50:45.888.",
        "Name=26306437-9f56-4c33-abb6-7e7f6327558b on thread 14 at 07:50:45.948.",
        "Name=26306437-9f56-4c33-abb6-7e7f6327558b on thread 14 at 07:50:45.948.",
        "Name=26306437-9f56-4c33-abb6-7e7f6327558b on thread 17 at 07:50:45.980."
    ]
]
```

This shows each request had a the same principal throught it's async flow but independent of all other requests. The timestamps show the requests were taking place at the same time.

I've attached the sample code.
