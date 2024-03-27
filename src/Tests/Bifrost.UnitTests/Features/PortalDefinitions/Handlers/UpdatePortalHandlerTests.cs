using Bifrost.Commands.Portals;
using Bifrost.Data.Base;
using Bifrost.Features.PortalDefinitions.Handlers;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Models.Portals;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Tests.Features.PortalDefinitions.Handlers;

[TestClass]
public class UpdatePortalHandlerTests
{
    [TestMethod]
    public async Task Update_Succeeds_ForValidInput()
    {
        // Arrange
        PortalDefinition currentState = PortalFaker.ValidPortal.Generate();
        UpdatePortalCommand cmd = new("id-1", "portal1", 2, nameof(VpnTypes.OpenVPN), "<config>");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), false))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((PortalDefinition?)currentState))
            .Verifiable(Times.Once);
        UpdatePortalHandler handler = new(repoMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.PortalId.Should().NotBeNullOrEmpty();
        result.Data.ErrorDetails.Should().BeNullOrEmpty();
        repoMock.Verify();
    }

    [TestMethod]
    public async Task Update_Fails_ForDuplicateName()
    {
        // Arrange
        PortalDefinition currentState = PortalFaker.ValidPortal.Generate();
        UpdatePortalCommand cmd = new("id-1", "portal1", 2, nameof(VpnTypes.OpenVPN), "<config>");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.UpdateAsync(It.IsAny<PortalDefinition>(), false))
            .Throws(new DbUpdateException("", new Exception("SQLite Error 19: 'UNIQUE constraint failed: PortalDefinition.Name'")));
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .Returns(Task.FromResult((PortalDefinition?)currentState));
        UpdatePortalHandler handler = new(repoMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        result.Success.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data.PortalId.Should().Be(cmd.Id);
        result.Data.ErrorDetails.Should().NotBeNull().And.ContainKey("DuplicateEntry");
    }

    [TestMethod]
    public async Task Update_Fails_ForUnknownId()
    {
        // Arrange
        UpdatePortalCommand cmd = new("id-1", "portal1", 2, nameof(VpnTypes.OpenVPN), "<config>");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new EntityNotFoundException());
        UpdatePortalHandler handler = new(repoMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        result.Success.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data.PortalId.Should().Be(cmd.Id);
        result.Data.ErrorDetails.Should().NotBeNull().And.ContainKey("NotFound");
    }
}
