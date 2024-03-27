using Bifrost.Features.PortalDefinitions.Handlers;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Models.Portals;
using Bifrost.Queries.Portals;
using FluentValidation;

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
        Mock<IValidator<GetPortalsQuery>> validatorMock = new();
        validatorMock.Setup(x => x.Validate(query))
            .Returns(new FluentValidation.Results.ValidationResult());
        GetPortalsHandler handler = new(repoMock.Object, validatorMock.Object);

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
