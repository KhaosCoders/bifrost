using Bifrost.Commands.Portals;
using Bifrost.Features.PortalDefinitions.Handlers;
using Bifrost.Features.PortalDefinitions.Repositories;
using Bifrost.Models.Portals;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bifrost.Tests.Features.PortalDefinitions.Handlers;

[TestClass]
public class CreatePortalHandlerTests
{
    private static IHttpContextAccessor MockHttpAccessor(string username)
    {
        ClaimsIdentity identity = new([new Claim(ClaimsIdentity.DefaultNameClaimType, username)]);
        ClaimsPrincipal principal = new(identity);
        Mock<HttpContext> httpContextMock = new();
        httpContextMock.Setup(x => x.User)
            .Returns(principal);
        Mock<IHttpContextAccessor> httpAccessorMock = new();
        httpAccessorMock.Setup(x => x.HttpContext)
            .Returns(httpContextMock.Object);

        return httpAccessorMock.Object;
    }

    [TestMethod]
    public async Task Create_Succeeds_ForValidInput()
    {
        // Arrange
        CreatePortalCommand cmd = new("portal1", 2, nameof(VpnTypes.OpenVPN), "<config>");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.CreateAsync(It.IsAny<PortalDefinition>(), false))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);
        var httpContext = MockHttpAccessor("user1");
        CreatePortalHandler handler = new(httpContext, repoMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.UnauthorizedRequest.Should().BeFalse();
        result.Data.PortalId.Should().NotBeNullOrEmpty();
        repoMock.Verify();
    }

    [TestMethod]
    public async Task Create_Fails_ForAnonymousUsers()
    {
        // Arrange
        CreatePortalCommand cmd = new("portal1", 2, nameof(VpnTypes.OpenVPN), "<config>");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.CreateAsync(It.IsAny<PortalDefinition>(), false))
            .Returns(Task.CompletedTask);
        var httpContext = MockHttpAccessor(string.Empty);
        CreatePortalHandler handler = new(httpContext, repoMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        result.Success.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data.UnauthorizedRequest.Should().BeTrue();
        result.Data.PortalId.Should().BeNullOrEmpty();
    }

    [TestMethod]
    public async Task Create_Fails_ForDuplicateName()
    {
        // Arrange
        CreatePortalCommand cmd = new("portal1", 2, nameof(VpnTypes.OpenVPN), "<config>");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.CreateAsync(It.IsAny<PortalDefinition>(), false))
            .Throws(new DbUpdateException("", new Exception("SQLite Error 19: 'UNIQUE constraint failed: PortalDefinition.Name'")));
        var httpContext = MockHttpAccessor("user1");
        CreatePortalHandler handler = new(httpContext, repoMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        result.Success.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data.UnauthorizedRequest.Should().BeFalse();
        result.Data.PortalId.Should().BeNullOrEmpty();
        result.Data.ErrorDetails.Should().NotBeNull().And.ContainKey("DuplicateEntry");
    }
}
