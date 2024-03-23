using Bifrost.Commands.Identity;
using Bifrost.Commands.Identity.Validation;

namespace Bifrost.Tests.Commands.Identity;

[TestClass]
public class LoginCommandValidationTests
{
    private static LoginCommand ValidCommand => new("user", "Pa$$-1", true, true);

    private readonly LoginCommandValidator Validator = new();

    [TestMethod]
    public void Validation_ShouldSucceed_ForValidCommand()
    {
        // Arrange
        var command = ValidCommand;

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public void Validation_ShouldFail_ForEmptyUsername()
    {
        // Arrange
        var command = ValidCommand with { Username = string.Empty };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(LoginCommand.Username));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForWhitespaceUsername()
    {
        // Arrange
        var command = ValidCommand with { Username = " " };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(LoginCommand.Username));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForEmptyPassword()
    {
        // Arrange
        var command = ValidCommand with { Password = string.Empty };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForWhitespacePassword()
    {
        // Arrange
        var command = ValidCommand with { Password = " " };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(LoginCommand.Password));
    }
}
