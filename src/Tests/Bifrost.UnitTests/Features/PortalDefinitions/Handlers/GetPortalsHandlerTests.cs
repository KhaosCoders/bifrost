using Bifrost.Features.PortalDefinitions.Handlers;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Models.Portals;
using Bifrost.Queries.Portals;

namespace Bifrost.Tests.Features.PortalDefinitions.Handlers;

[TestClass]
public class GetPortalsHandlerTests
{
    [TestMethod]
    public async Task Gets_Succeeds_ForValidInput()
    {
        // Arrange
        IList<PortalDefinition> portals =
            Enumerable.Range(1, 3)
                .Select(_ => PortalFaker.ValidPortal.Generate())
                .ToList();

        GetPortalsQuery query = new();
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.QueryAll())
            .Returns(portals.AsQueryable())
            .Verifiable(Times.Once);
        GetPortalsHandler handler = new(repoMock.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Portals.Should().NotBeEmpty().And.BeEquivalentTo(portals);
        repoMock.Verify();
    }
}
