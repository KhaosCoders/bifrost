using Bifrost.Commands.Portals;

namespace Bifrost.Tests.Commands.Portals;

[TestClass]
public class DeletePortalCommandValidationTests
{
    private static DeletePortalCommand ValidCommand => new("id-1");

    private readonly DeletePortalCommandValidator Validator = new();

    [TestMethod]
    public void Validate_ShouldSucceed_ForValidCommand()
    {
        // Arrange
        var command = ValidCommand;

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        // Arrange
        var command = ValidCommand with { Id = string.Empty };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeletePortalCommand.Id));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenIdIsWhitespace()
    {
        // Arrange
        var command = ValidCommand with { Id = " " };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(DeletePortalCommand.Id));
    }
}
