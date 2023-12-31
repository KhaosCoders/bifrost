﻿using Bifrost.Client.Utils.Validation;

namespace Bifrost.UnitTests.Utils.Validation;

public class ValidationRuleTests
{
    [Test]
    public void Name_ReturnsPropertyOfAccessor()
    {
        // Arrange
        var obj = new { DummyName = "test" };
        ValidationRule<string> rule = new(() => obj.DummyName, _ => true);

        // Act
        string? name = rule.Name;

        // Assert
        name.Should().Be(nameof(obj.DummyName));
    }

    [Test]
    public void Name_ThrowsArgumentException_WhenAccessorIsNotProperty()
    {
        // Arrange
        var obj = new { DummyName = "test" };

        // Act
        Action act = () => _ = new ValidationRule<string>(() => "", _ => true);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Validate_ReturnsNull_WhenValid()
    {
        // Arrange
        var obj = new { DummyName = "test" };
        ValidationRule<string> rule = new(() => obj.DummyName, _ => true);

        // Act
        var result = rule.Validate();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Validate_ReturnsValidationFault_WhenInvalid()
    {
        // Arrange
        var obj = new { DummyName = "test" };
        ValidationRule<string> rule = new(() => obj.DummyName, _ => false);

        // Act
        var result = rule.Validate();

        // Assert
        result.Should().NotBeNull();
        result!.Property.Should().Be(nameof(obj.DummyName));
        result.Message.Should().Be("Validation failed");
    }

    [Test]
    public void Validate_ReturnsValidationFault_WhenValidatorThrowsException()
    {
        // Arrange
        var obj = new { DummyName = "test" };
        ValidationRule<string> rule = new(() => obj.DummyName, _ => throw new Exception("test"));

        // Act
        var result = rule.Validate();

        // Assert
        result.Should().NotBeNull();
        result!.Property.Should().Be(nameof(obj.DummyName));
        result.Message.Should().Be("test");
    }
}
