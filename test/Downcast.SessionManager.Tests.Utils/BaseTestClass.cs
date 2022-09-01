using Bogus;

using Downcast.SessionManager.SDK.Client;

using Refit;

namespace Downcast.SessionManager.Tests.Utils;

public class BaseTestClass
{
    public BaseTestClass()
    {
        HttpClient httpClient = new SessionManagerServerInstance().CreateClient();
        Client = RestService.For<ISessionManagerClient>(httpClient);
    }

    protected ISessionManagerClient Client { get; }
    protected Faker Faker { get; } = new();
}