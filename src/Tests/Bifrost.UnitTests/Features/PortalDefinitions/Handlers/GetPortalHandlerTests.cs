using Bifrost.Data.Base;
using Bifrost.Features.PortalDefinitions.Handlers;
using Bifrost.Features.PortalDefinitions.Repositories;
using Bifrost.Models.Portals;
using Bifrost.Queries.Portals;

namespace Bifrost.Tests.Features.PortalDefinitions.Handlers;

[TestClass]
public class GetPortalHandlerTests
{
    [TestMethod]
    public async Task Get_Succeeds_ForValidInput()
    {
        // Arrange
        PortalDefinition portal = PortalFaker.ValidPortal.Generate();
        GetPortalQuery query = new("id-1");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(portal)
            .Verifiable(Times.Once);
        GetPortalHandler handler = new(repoMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Portal.Should().Be(portal);
        repoMock.Verify();
    }

    [TestMethod]
    public async Task Get_Fails_ForUnknownId()
    {
        // Arrange
        GetPortalQuery query = new("id-1");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException())
            .Verifiable(Times.Once);
        GetPortalHandler handler = new(repoMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data.Portal.Should().BeNull();
        result.Data.ErrorDetails.Should().NotBeNull().And.ContainKey("NotFound");
        repoMock.Verify();
    }
}
