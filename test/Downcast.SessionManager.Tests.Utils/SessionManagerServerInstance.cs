using Downcast.SessionManager.API.Controllers;

using Microsoft.AspNetCore.Mvc.Testing;

namespace Downcast.SessionManager.Tests.Utils;

public class SessionManagerServerInstance : WebApplicationFactory<SessionController>
{
}