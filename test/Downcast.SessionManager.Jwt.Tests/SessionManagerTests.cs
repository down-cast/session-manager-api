using System.Net;

using Downcast.SessionManager.SDK.Authentication.Extensions;
using Downcast.SessionManager.SDK.Client.Model;
using Downcast.SessionManager.Tests.Utils;

using FluentAssertions.Specialized;

using Refit;

namespace Downcast.SessionManager.Jwt.Tests;

public class SessionManagerTests : BaseTestClass
{
    private readonly Dictionary<string, object> _claims;

    public SessionManagerTests()
    {
        _claims = new Dictionary<string, object>
        {
            { ClaimNames.Email, Faker.Internet.Email() },
            { ClaimNames.UserId, Faker.Random.Guid().ToString() },
            { ClaimNames.Role, new[] { RoleNames.Admin, RoleNames.Author } },
            { ClaimNames.DisplayName, Faker.Person.FullName },
            { ClaimNames.ProfilePictureUri, Faker.Person.Avatar }
        };
    }

    [Fact]
    public async Task CreateSession_Returns_OK()
    {
        TokenResult result = await Client.CreateSessionToken(_claims).ConfigureAwait(false);
        result.Token.Should().NotBeNullOrEmpty();
    }


    [Fact]
    public async Task Validate_Created_Session_Success()
    {
        TokenResult result = await Client.CreateSessionToken(_claims).ConfigureAwait(false);
        result.Token.Should().NotBeNullOrEmpty();

        await Client.Invoking(client => client.ValidateSessionToken(result.Token))
            .Should()
            .NotThrowAsync("Session should be valid")
            .ConfigureAwait(false);
    }


    [Fact]
    public async Task Validate_Invalid_Session_Throws_Exception()
    {
        TokenResult result = await Client.CreateSessionToken(_claims).ConfigureAwait(false);
        result.Token.Should().NotBeNullOrEmpty();

        ExceptionAssertions<ApiException>? exception = await Client
            .Invoking(client => client.ValidateSessionToken(result.Token + "invalid"))
            .Should()
            .ThrowExactlyAsync<ApiException>("Session is invalid")
            .ConfigureAwait(false);

        exception.And.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}