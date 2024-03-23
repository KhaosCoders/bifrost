using Bifrost.Commands.Portals;
using Bifrost.Commands.Portals.Validation;

namespace Bifrost.Tests.Commands.Portals;

[TestClass]
public class UpdatePortalCommandValidationTests
{
    private static UpdatePortalCommand ValidCommand => new("name", 2, nameof(VpnTypes.OpenVPN), "<config>");

    private readonly UpdatePortalCommandValidator Validator = new();

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
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var command = ValidCommand with { Name = string.Empty };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePortalCommand.Name));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenNameIsWhitespace()
    {
        // Arrange
        var command = ValidCommand with { Name = " " };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePortalCommand.Name));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenNameIsTooLong()
    {
        // Arrange
        var command = ValidCommand with { Name = new string('a', 51) };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePortalCommand.Name));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenMaxInstanceCountIsEmpty()
    {
        // Arrange
        var command = ValidCommand with { MaxInstanceCount = 0 };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePortalCommand.MaxInstanceCount));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenMaxInstanceCountIsNegative()
    {
        // Arrange
        var command = ValidCommand with { MaxInstanceCount = -1 };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePortalCommand.MaxInstanceCount));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenVpnTypeIsEmpty()
    {
        // Arrange
        var command = ValidCommand with { VpnType = string.Empty };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePortalCommand.VpnType));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenVpnTypeIsInvalid()
    {
        // Arrange
        var command = ValidCommand with { VpnType = "invalid" };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePortalCommand.VpnType));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenVpnConfigIsEmpty()
    {
        // Arrange
        var command = ValidCommand with { VpnConfig = string.Empty };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePortalCommand.VpnConfig));
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenVpnConfigIsWhitespace()
    {
        // Arrange
        var command = ValidCommand with { VpnConfig = " " };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreatePortalCommand.VpnConfig));
    }
}
