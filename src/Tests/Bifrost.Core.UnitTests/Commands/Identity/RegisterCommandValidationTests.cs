using Bifrost.Commands.Identity;
using Bifrost.Commands.Identity.Validation;

namespace Bifrost.Tests.Commands.Identity;

[TestClass]
public class RegisterCommandValidationTests
{
    private static RegisterCommand ValidCommand => new("user", "Pa$$-1", "mail@domain.xzy");

    private readonly RegisterCommandValidator Validator = new();

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
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.Username));
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
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.Username));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForTooShortUsername()
    {
        // Arrange
        var command = ValidCommand with { Username = "xy" };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.Username));
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

    [TestMethod]
    public void Validation_ShouldFail_ForTooShortPassword()
    {
        // Arrange
        var command = ValidCommand with { Password = "aB$1-" };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForPasswordWithoutLowercaseChar()
    {
        // Arrange
        var command = ValidCommand with { Password = "B$123-" };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForPasswordWithoutUppercaseChar()
    {
        // Arrange
        var command = ValidCommand with { Password = "a$123-" };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForPasswordWithoutSpecialChar()
    {
        // Arrange
        var command = ValidCommand with { Password = "abc123def" };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForPasswordWithoutNumberChar()
    {
        // Arrange
        var command = ValidCommand with { Password = "abc.-_def" };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(LoginCommand.Password));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForEmptyEmail()
    {
        // Arrange
        var command = ValidCommand with { Email = string.Empty };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.Email));
    }

    [TestMethod]
    public void Validation_ShouldFail_ForInvalidEmail()
    {
        // Arrange
        var command = ValidCommand with { Email = "invalid-email" };

        // Act
        var result = Validator.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(RegisterCommand.Email));
    }
}
