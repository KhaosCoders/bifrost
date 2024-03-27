using Bifrost.Commands.Portals;
using Bifrost.Data.Base;
using Bifrost.Features.PortalDefinitions.Handlers;
using Bifrost.Features.PortalDefinitions.Services;
using FluentValidation;

namespace Bifrost.Tests.Features.PortalDefinitions.Handlers;

[TestClass]
public class DeletePortalHandlerTests
{
    [TestMethod]
    public async Task Delete_Succeeds_ForValidId()
    {
        // Arrange
        DeletePortalCommand cmd = new("id-1");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.DeleteAsync(cmd.Id, false))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);
        Mock<IValidator<DeletePortalCommand>> validatorMock = new();
        validatorMock.Setup(x => x.Validate(cmd))
               .Returns(new FluentValidation.Results.ValidationResult());
        DeletePortalHandler handler = new(repoMock.Object, validatorMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Deleted.Should().BeTrue();
        result.Data.NotFound.Should().BeFalse();
        result.Data.ErrorDetails.Should().BeNullOrEmpty();
        repoMock.Verify();
    }

    [TestMethod]
    public async Task Delete_Fails_ForUnknownId()
    {
        // Arrange
        DeletePortalCommand cmd = new("id-1");
        Mock<IPortalDefinitionRepository> repoMock = new();
        repoMock.Setup(x => x.DeleteAsync(cmd.Id, false))
            .Throws(new EntityNotFoundException());
        Mock<IValidator<DeletePortalCommand>> validatorMock = new();
        validatorMock.Setup(x => x.Validate(cmd))
               .Returns(new FluentValidation.Results.ValidationResult());
        DeletePortalHandler handler = new(repoMock.Object, validatorMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Data.Should().NotBeNull();
        result.Data.Deleted.Should().BeFalse();
        result.Data.NotFound.Should().BeTrue();
        result.Data.ErrorDetails.Should().NotBeNull().And.ContainKey("NotFound");
    }
}
